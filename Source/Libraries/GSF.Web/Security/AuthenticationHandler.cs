﻿//******************************************************************************************************
//  AuthenticationHandler.cs - Gbtc
//
//  Copyright © 2017, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  08/25/2017 - Stephen C. Wills
//       Generated original version of source code.
//  08/26/2017 - J. Ritchie Carroll
//       Updated handling for anonymous requests and added principal lookup function for a session ID
//
//******************************************************************************************************

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using GSF.Reflection;
using GSF.Security;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;

namespace GSF.Web.Security
{
    /// <summary>
    /// Handles authentication using the configured <see cref="ISecurityProvider"/> implementation in the Owin pipeline.
    /// </summary>
    public class AuthenticationHandler : AuthenticationHandler<AuthenticationOptions>
    {
        #region [ Properties ]

        // Reads the authorization header value from the request
        private AuthenticationHeaderValue AuthorizationHeader
        {
            get
            {
                string[] authorization = Request.Headers["Authorization"]?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if ((object)authorization == null || authorization.Length < 2)
                    return null;

                return new AuthenticationHeaderValue(authorization[0], authorization[1]);
            }
        }

        // Gets a principal that represents an unauthenticated anonymous user
        private IPrincipal AnonymousPrincipal
        {
            get
            {
                IIdentity anonymousIdentity = new GenericIdentity("anonymous");
                return new GenericPrincipal(anonymousIdentity, new string[0]);
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// The core authentication logic which must be provided by the handler. Will be invoked at most
        /// once per request. Do not call directly, call the wrapping Authenticate method instead.
        /// </summary>
        /// <returns>The ticket data provided by the authentication logic</returns>
        protected override Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            return Task.Run(() =>
            {
                SecurityPrincipal securityPrincipal;

                // Track original principal
                Context.Environment["OriginalPrincipal"] = Request.User;

                // No authentication required for anonymous resources
                if (Options.IsAnonymousResource(Request.Path.Value))
                    return null;

                // Attempt to read the session ID from the HTTP cookies
                Guid sessionID = SessionHandler.GetSessionIDFromCookie(Request, Options.SessionToken);
                AuthenticationHeaderValue authorization = AuthorizationHeader;

                // Attempt to retrieve the user's credentials that were cached to the user's session
                if (s_authorizationCache.TryGetValue(sessionID, out securityPrincipal))
                {
                    bool useCachedCredentials =
                        (object)Request.User == null ||
                        Request.User.Identity.Name.Equals(securityPrincipal.Identity.Name, StringComparison.OrdinalIgnoreCase) ||
                        authorization?.Scheme != "Basic";

                    if (!useCachedCredentials)
                    {
                        // Explicit login attempts as a different user
                        // cause credentials to be flushed from the session
                        s_authorizationCache.TryRemove(sessionID, out securityPrincipal);
                        securityPrincipal = null;
                    }
                }

                if ((object)securityPrincipal == null)
                {
                    // Pick the appropriate authentication logic based
                    // on the authorization type in the HTTP headers
                    if (authorization?.Scheme == "Basic")
                        securityPrincipal = AuthenticateBasic(authorization?.Parameter);
                    else
                        securityPrincipal = AuthenticatePassthrough();

                    // Attempt to cache the security principal to the session
                    if (sessionID != Guid.Empty && securityPrincipal?.Identity.IsAuthenticated == true)
                        s_authorizationCache[sessionID] = securityPrincipal;
                }

                // Set the principal of the IOwinRequest so that it
                // can be propagated through the Owin pipeline
                Request.User = securityPrincipal ?? AnonymousPrincipal;

                return (AuthenticationTicket)null;
            });
        }

        /// <summary>
        /// Called once by common code after initialization. If an authentication middle-ware
        /// responds directly to specifically known paths it must override this virtual,
        /// compare the request path to it's known paths, provide any response information
        /// as appropriate, and true to stop further processing.
        /// </summary>
        /// <returns>
        /// Returning false will cause the common code to call the next middle-ware in line.
        /// Returning true will cause the common code to begin the async completion journey
        /// without calling the rest of the middle-ware pipeline.
        /// </returns>
        public override Task<bool> InvokeAsync()
        {
            return Task.Run(() =>
            {
                SecurityPrincipal securityPrincipal = Request.User as SecurityPrincipal;
                string urlPath = Request.Path.Value;

                // If request is for an anonymous resource or user is properly authenticated, allow
                // request to propagate through the Owin pipeline; otherwise, adjust the HTTP response
                if (Options.IsAnonymousResource(urlPath) || securityPrincipal?.Identity.IsAuthenticated == true)
                    return false;

                // Check configured options that define if a resource should redirect to the login
                // page upon authentication failure or simply return an unauthorized (401) response
                if (Options.IsAuthFailureRedirectResource(urlPath))
                {
                    Response.Redirect(Options.LoginPage);                    
                }
                else
                {
                    string currentIdentity = securityPrincipal?.Identity?.Name ?? "anonymous";
                    object value;

                    if (Context.Environment.TryGetValue("OriginalPrincipal", out value))
                    {
                        IPrincipal originalPrincpal = value as IPrincipal;

                        if ((object)originalPrincpal != null && (object)originalPrincpal.Identity != null)
                            currentIdentity = originalPrincpal.Identity.Name;
                    }

                    Response.Headers.Add("CurrentIdentity", new[] { currentIdentity });
                    Response.StatusCode = (int)HttpStatusCode.Unauthorized;                    
                }
                Response.ReasonPhrase = SecurityPrincipal.GetFailureReasonPhrase(securityPrincipal, AuthorizationHeader?.Scheme, AssemblyInfo.EntryAssembly.Debuggable);

                return true;
            });
        }

        // Applies authentication for requests where credentials are passed directly in the HTTP headers.
        private SecurityPrincipal AuthenticateBasic(string credentials)
        {
            string username, password;

            // Get the user's credentials from the HTTP headers
            if (!TryParseCredentials(credentials, out username, out password))
                return null;

            // Create the security provider that will authenticate the user's credentials
            ISecurityProvider securityProvider = SecurityProviderCache.CreateProvider(username);
            securityProvider.Password = password;
            securityProvider.Authenticate();

            // Return the security principal that will be used for role-based authorization
            SecurityIdentity securityIdentity = new SecurityIdentity(securityProvider);
            return new SecurityPrincipal(securityIdentity);
        }

        // Applies authentication for requests using Windows pass-through authentication.
        private SecurityPrincipal AuthenticatePassthrough()
        {
            string username = Request.User?.Identity.Name;

            if ((object)username == null)
                return null;

            // Get the principal used for verifying the user's pass-through authentication
            IPrincipal passthroughPrincipal = Request.User;

            // Create the security provider that will verify the user's pass-through authentication
            ISecurityProvider securityProvider = SecurityProviderCache.CreateProvider(username);
            securityProvider.PassthroughPrincipal = passthroughPrincipal;
            securityProvider.Authenticate();

            // Return the security principal that will be used for role-based authorization
            SecurityIdentity securityIdentity = new SecurityIdentity(securityProvider);
            return new SecurityPrincipal(securityIdentity);
        }

        #endregion

        #region [ Static ]

        // Static Fields
        private static readonly ConcurrentDictionary<Guid, SecurityPrincipal> s_authorizationCache;

        // Static Constructor
        static AuthenticationHandler()
        {
            s_authorizationCache = new ConcurrentDictionary<Guid, SecurityPrincipal>();

            // Attach to razor view session expiration event so any cached authorizations can also be cleared
            Model.RazorView.SessionExpired += (sender, e) => ClearAuthorizationCache(e.Argument1);
        }

        // Static Methods

        /// <summary>
        /// Attempt to get current security principal for specified <paramref name="sessionID"/>.
        /// </summary>
        /// <param name="sessionID">Session ID to user.</param>
        /// <param name="securityPrincipal">Principal of user with specified <paramref name="sessionID"/>, if found.</param>
        /// <returns><c>true</c> if principal was found for specified <paramref name="sessionID"/>; otherwise, <c>false</c>.</returns>
        public static bool TryGetPrincipal(Guid sessionID, out SecurityPrincipal securityPrincipal)
        {
            return s_authorizationCache.TryGetValue(sessionID, out securityPrincipal);
        }

        /// <summary>
        /// Clears any cached authorizations for the specified <paramref name="sessionID"/>.
        /// </summary>
        /// <param name="sessionID">Identifier of session authorization to clear.</param>
        /// <returns><c>true</c> if session authorization was found and cleared; otherwise, <c>false</c>.</returns>
        public static bool ClearAuthorizationCache(Guid sessionID)
        {
            SecurityPrincipal securityPrincipal;
            return s_authorizationCache.TryRemove(sessionID, out securityPrincipal);
        }

        private static bool TryParseCredentials(string authorizationParameter, out string userName, out string password)
        {
            byte[] credentialBytes;

            userName = null;
            password = null;

            try
            {
                credentialBytes = Convert.FromBase64String(authorizationParameter);
            }
            catch (FormatException)
            {
                return false;
            }

            // The currently approved HTTP 1.1 specification says characters here are ISO-8859-1.
            // However, the current draft updated specification for HTTP 1.1 indicates this
            // encoding is infrequently used in practice and defines behavior only for ASCII.

            // Make a writable copy of the ASCII encoding to enable setting the decoder fall-back
            Encoding encoding = Encoding.ASCII.Clone() as Encoding;

            if ((object)encoding == null)
                return false;

            // Fail on invalid bytes rather than silently replacing and continuing
            encoding.DecoderFallback = DecoderFallback.ExceptionFallback;

            string credentials;

            try
            {
                credentials = encoding.GetString(credentialBytes);
            }
            catch (DecoderFallbackException)
            {
                return false;
            }

            if (string.IsNullOrEmpty(credentials))
                return false;

            int index = credentials.IndexOf(':');

            if (index == -1)
                return false;

            userName = credentials.Substring(0, index);
            password = credentials.Substring(index + 1);

            return true;
        }

        #endregion
    }
}
