﻿//*******************************************************************************************************
//  MessageBusService.cs - Gbtc
//
//  Tennessee Valley Authority, 2010
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  This software is made freely available under the TVA Open Source Agreement (see below).
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  10/06/2010 - Pinal C. Patel
//       Generated original version of source code.
//  10/26/2010 - Pinal C. Patel
//       Added management methods GetClients(), GetQueues() and GetTopics().
//  11/23/2010 - Pinal C. Patel
//       Added new BufferThreshold and ProcessingMode properties.
//       Enhanced thread synchronization using ReaderWriterLockSlim for better performance.
//
//*******************************************************************************************************

#region [ TVA Open Source Agreement ]
/*

 THIS OPEN SOURCE AGREEMENT ("AGREEMENT") DEFINES THE RIGHTS OF USE,REPRODUCTION, DISTRIBUTION,
 MODIFICATION AND REDISTRIBUTION OF CERTAIN COMPUTER SOFTWARE ORIGINALLY RELEASED BY THE
 TENNESSEE VALLEY AUTHORITY, A CORPORATE AGENCY AND INSTRUMENTALITY OF THE UNITED STATES GOVERNMENT
 ("GOVERNMENT AGENCY"). GOVERNMENT AGENCY IS AN INTENDED THIRD-PARTY BENEFICIARY OF ALL SUBSEQUENT
 DISTRIBUTIONS OR REDISTRIBUTIONS OF THE SUBJECT SOFTWARE. ANYONE WHO USES, REPRODUCES, DISTRIBUTES,
 MODIFIES OR REDISTRIBUTES THE SUBJECT SOFTWARE, AS DEFINED HEREIN, OR ANY PART THEREOF, IS, BY THAT
 ACTION, ACCEPTING IN FULL THE RESPONSIBILITIES AND OBLIGATIONS CONTAINED IN THIS AGREEMENT.

 Original Software Designation: openPDC
 Original Software Title: The TVA Open Source Phasor Data Concentrator
 User Registration Requested. Please Visit https://naspi.tva.com/Registration/
 Point of Contact for Original Software: J. Ritchie Carroll <mailto:jrcarrol@tva.gov>

 1. DEFINITIONS

 A. "Contributor" means Government Agency, as the developer of the Original Software, and any entity
 that makes a Modification.

 B. "Covered Patents" mean patent claims licensable by a Contributor that are necessarily infringed by
 the use or sale of its Modification alone or when combined with the Subject Software.

 C. "Display" means the showing of a copy of the Subject Software, either directly or by means of an
 image, or any other device.

 D. "Distribution" means conveyance or transfer of the Subject Software, regardless of means, to
 another.

 E. "Larger Work" means computer software that combines Subject Software, or portions thereof, with
 software separate from the Subject Software that is not governed by the terms of this Agreement.

 F. "Modification" means any alteration of, including addition to or deletion from, the substance or
 structure of either the Original Software or Subject Software, and includes derivative works, as that
 term is defined in the Copyright Statute, 17 USC § 101. However, the act of including Subject Software
 as part of a Larger Work does not in and of itself constitute a Modification.

 G. "Original Software" means the computer software first released under this Agreement by Government
 Agency entitled openPDC, including source code, object code and accompanying documentation, if any.

 H. "Recipient" means anyone who acquires the Subject Software under this Agreement, including all
 Contributors.

 I. "Redistribution" means Distribution of the Subject Software after a Modification has been made.

 J. "Reproduction" means the making of a counterpart, image or copy of the Subject Software.

 K. "Sale" means the exchange of the Subject Software for money or equivalent value.

 L. "Subject Software" means the Original Software, Modifications, or any respective parts thereof.

 M. "Use" means the application or employment of the Subject Software for any purpose.

 2. GRANT OF RIGHTS

 A. Under Non-Patent Rights: Subject to the terms and conditions of this Agreement, each Contributor,
 with respect to its own contribution to the Subject Software, hereby grants to each Recipient a
 non-exclusive, world-wide, royalty-free license to engage in the following activities pertaining to
 the Subject Software:

 1. Use

 2. Distribution

 3. Reproduction

 4. Modification

 5. Redistribution

 6. Display

 B. Under Patent Rights: Subject to the terms and conditions of this Agreement, each Contributor, with
 respect to its own contribution to the Subject Software, hereby grants to each Recipient under Covered
 Patents a non-exclusive, world-wide, royalty-free license to engage in the following activities
 pertaining to the Subject Software:

 1. Use

 2. Distribution

 3. Reproduction

 4. Sale

 5. Offer for Sale

 C. The rights granted under Paragraph B. also apply to the combination of a Contributor's Modification
 and the Subject Software if, at the time the Modification is added by the Contributor, the addition of
 such Modification causes the combination to be covered by the Covered Patents. It does not apply to
 any other combinations that include a Modification. 

 D. The rights granted in Paragraphs A. and B. allow the Recipient to sublicense those same rights.
 Such sublicense must be under the same terms and conditions of this Agreement.

 3. OBLIGATIONS OF RECIPIENT

 A. Distribution or Redistribution of the Subject Software must be made under this Agreement except for
 additions covered under paragraph 3H. 

 1. Whenever a Recipient distributes or redistributes the Subject Software, a copy of this Agreement
 must be included with each copy of the Subject Software; and

 2. If Recipient distributes or redistributes the Subject Software in any form other than source code,
 Recipient must also make the source code freely available, and must provide with each copy of the
 Subject Software information on how to obtain the source code in a reasonable manner on or through a
 medium customarily used for software exchange.

 B. Each Recipient must ensure that the following copyright notice appears prominently in the Subject
 Software:

          No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.

 C. Each Contributor must characterize its alteration of the Subject Software as a Modification and
 must identify itself as the originator of its Modification in a manner that reasonably allows
 subsequent Recipients to identify the originator of the Modification. In fulfillment of these
 requirements, Contributor must include a file (e.g., a change log file) that describes the alterations
 made and the date of the alterations, identifies Contributor as originator of the alterations, and
 consents to characterization of the alterations as a Modification, for example, by including a
 statement that the Modification is derived, directly or indirectly, from Original Software provided by
 Government Agency. Once consent is granted, it may not thereafter be revoked.

 D. A Contributor may add its own copyright notice to the Subject Software. Once a copyright notice has
 been added to the Subject Software, a Recipient may not remove it without the express permission of
 the Contributor who added the notice.

 E. A Recipient may not make any representation in the Subject Software or in any promotional,
 advertising or other material that may be construed as an endorsement by Government Agency or by any
 prior Recipient of any product or service provided by Recipient, or that may seek to obtain commercial
 advantage by the fact of Government Agency's or a prior Recipient's participation in this Agreement.

 F. In an effort to track usage and maintain accurate records of the Subject Software, each Recipient,
 upon receipt of the Subject Software, is requested to register with Government Agency by visiting the
 following website: https://naspi.tva.com/Registration/. Recipient's name and personal information
 shall be used for statistical purposes only. Once a Recipient makes a Modification available, it is
 requested that the Recipient inform Government Agency at the web site provided above how to access the
 Modification.

 G. Each Contributor represents that that its Modification does not violate any existing agreements,
 regulations, statutes or rules, and further that Contributor has sufficient rights to grant the rights
 conveyed by this Agreement.

 H. A Recipient may choose to offer, and to charge a fee for, warranty, support, indemnity and/or
 liability obligations to one or more other Recipients of the Subject Software. A Recipient may do so,
 however, only on its own behalf and not on behalf of Government Agency or any other Recipient. Such a
 Recipient must make it absolutely clear that any such warranty, support, indemnity and/or liability
 obligation is offered by that Recipient alone. Further, such Recipient agrees to indemnify Government
 Agency and every other Recipient for any liability incurred by them as a result of warranty, support,
 indemnity and/or liability offered by such Recipient.

 I. A Recipient may create a Larger Work by combining Subject Software with separate software not
 governed by the terms of this agreement and distribute the Larger Work as a single product. In such
 case, the Recipient must make sure Subject Software, or portions thereof, included in the Larger Work
 is subject to this Agreement.

 J. Notwithstanding any provisions contained herein, Recipient is hereby put on notice that export of
 any goods or technical data from the United States may require some form of export license from the
 U.S. Government. Failure to obtain necessary export licenses may result in criminal liability under
 U.S. laws. Government Agency neither represents that a license shall not be required nor that, if
 required, it shall be issued. Nothing granted herein provides any such export license.

 4. DISCLAIMER OF WARRANTIES AND LIABILITIES; WAIVER AND INDEMNIFICATION

 A. No Warranty: THE SUBJECT SOFTWARE IS PROVIDED "AS IS" WITHOUT ANY WARRANTY OF ANY KIND, EITHER
 EXPRESSED, IMPLIED, OR STATUTORY, INCLUDING, BUT NOT LIMITED TO, ANY WARRANTY THAT THE SUBJECT
 SOFTWARE WILL CONFORM TO SPECIFICATIONS, ANY IMPLIED WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
 PARTICULAR PURPOSE, OR FREEDOM FROM INFRINGEMENT, ANY WARRANTY THAT THE SUBJECT SOFTWARE WILL BE ERROR
 FREE, OR ANY WARRANTY THAT DOCUMENTATION, IF PROVIDED, WILL CONFORM TO THE SUBJECT SOFTWARE. THIS
 AGREEMENT DOES NOT, IN ANY MANNER, CONSTITUTE AN ENDORSEMENT BY GOVERNMENT AGENCY OR ANY PRIOR
 RECIPIENT OF ANY RESULTS, RESULTING DESIGNS, HARDWARE, SOFTWARE PRODUCTS OR ANY OTHER APPLICATIONS
 RESULTING FROM USE OF THE SUBJECT SOFTWARE. FURTHER, GOVERNMENT AGENCY DISCLAIMS ALL WARRANTIES AND
 LIABILITIES REGARDING THIRD-PARTY SOFTWARE, IF PRESENT IN THE ORIGINAL SOFTWARE, AND DISTRIBUTES IT
 "AS IS."

 B. Waiver and Indemnity: RECIPIENT AGREES TO WAIVE ANY AND ALL CLAIMS AGAINST GOVERNMENT AGENCY, ITS
 AGENTS, EMPLOYEES, CONTRACTORS AND SUBCONTRACTORS, AS WELL AS ANY PRIOR RECIPIENT. IF RECIPIENT'S USE
 OF THE SUBJECT SOFTWARE RESULTS IN ANY LIABILITIES, DEMANDS, DAMAGES, EXPENSES OR LOSSES ARISING FROM
 SUCH USE, INCLUDING ANY DAMAGES FROM PRODUCTS BASED ON, OR RESULTING FROM, RECIPIENT'S USE OF THE
 SUBJECT SOFTWARE, RECIPIENT SHALL INDEMNIFY AND HOLD HARMLESS  GOVERNMENT AGENCY, ITS AGENTS,
 EMPLOYEES, CONTRACTORS AND SUBCONTRACTORS, AS WELL AS ANY PRIOR RECIPIENT, TO THE EXTENT PERMITTED BY
 LAW.  THE FOREGOING RELEASE AND INDEMNIFICATION SHALL APPLY EVEN IF THE LIABILITIES, DEMANDS, DAMAGES,
 EXPENSES OR LOSSES ARE CAUSED, OCCASIONED, OR CONTRIBUTED TO BY THE NEGLIGENCE, SOLE OR CONCURRENT, OF
 GOVERNMENT AGENCY OR ANY PRIOR RECIPIENT.  RECIPIENT'S SOLE REMEDY FOR ANY SUCH MATTER SHALL BE THE
 IMMEDIATE, UNILATERAL TERMINATION OF THIS AGREEMENT.

 5. GENERAL TERMS

 A. Termination: This Agreement and the rights granted hereunder will terminate automatically if a
 Recipient fails to comply with these terms and conditions, and fails to cure such noncompliance within
 thirty (30) days of becoming aware of such noncompliance. Upon termination, a Recipient agrees to
 immediately cease use and distribution of the Subject Software. All sublicenses to the Subject
 Software properly granted by the breaching Recipient shall survive any such termination of this
 Agreement.

 B. Severability: If any provision of this Agreement is invalid or unenforceable under applicable law,
 it shall not affect the validity or enforceability of the remainder of the terms of this Agreement.

 C. Applicable Law: This Agreement shall be subject to United States federal law only for all purposes,
 including, but not limited to, determining the validity of this Agreement, the meaning of its
 provisions and the rights, obligations and remedies of the parties.

 D. Entire Understanding: This Agreement constitutes the entire understanding and agreement of the
 parties relating to release of the Subject Software and may not be superseded, modified or amended
 except by further written agreement duly executed by the parties.

 E. Binding Authority: By accepting and using the Subject Software under this Agreement, a Recipient
 affirms its authority to bind the Recipient to all terms and conditions of this Agreement and that
 Recipient hereby agrees to all terms and conditions herein.

 F. Point of Contact: Any Recipient contact with Government Agency is to be directed to the designated
 representative as follows: J. Ritchie Carroll <mailto:jrcarrol@tva.gov>.

*/
#endregion

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Threading;
using TVA.Collections;
using TVA.Configuration;

namespace TVA.Web.Services.Messaging
{
    #region [ Enumerations ]

    /// <summary>
    /// Indicates how the distribution of <see cref="Message"/>s is processed by the <see cref="MessageBusService"/>.
    /// </summary>
    public enum MessageProcessingMode
    {
        /// <summary>
        /// <see cref="Message"/> distribution is processed in parallel for increased distribution performance.
        /// </summary>
        Parallel,
        /// <summary>
        /// <see cref="Message"/> distribution is processed sequentially to preserve <see cref="Message"/> ordering.
        /// </summary>
        Sequential
    }

    #endregion

    /// <summary>
    /// A message bus for event-based messaging between disjoint systems.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class MessageBusService : SelfHostingService, IMessageBusService
    {
        #region [ Members ]

        // Nested Types

        private class PublishContext
        {
            public PublishContext(Message message, RegistrationInfo registration)
            {
                Message = message;
                Registration = registration;
            }

            public Message Message;

            public RegistrationInfo Registration;
        }

        // Constants

        /// <summary>
        /// Specifies the default value for the <see cref="BufferThreshold"/> property.
        /// </summary>
        public const int DefaultBufferThreshold = -1;

        /// <summary>
        /// Specifies the default value for the <see cref="ProcessingMode"/> property.
        /// </summary>
        public const MessageProcessingMode DefaultProcessingMode = MessageProcessingMode.Sequential;

        // Fields
        private int m_bufferThreshold;
        private MessageProcessingMode m_processingMode;
        private Dictionary<string, ClientInfo> m_clients;
        private Dictionary<string, RegistrationInfo> m_queues;
        private Dictionary<string, RegistrationInfo> m_topics;
        private ReaderWriterLockSlim m_clientsLock;
        private ReaderWriterLockSlim m_queuesLock;
        private ReaderWriterLockSlim m_topicsLock;
        private ProcessQueue<PublishContext> m_publishQueue;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBusService"/> class.
        /// </summary>
        public MessageBusService()
            : base()
        {
            // Override base class settings.
            Singleton = true;
            PublishMetadata = true;
            PersistSettings = true;

            // Initialize member variables.
            m_bufferThreshold = DefaultBufferThreshold;
            m_processingMode = DefaultProcessingMode;
            m_clients = new Dictionary<string, ClientInfo>(StringComparer.CurrentCultureIgnoreCase);
            m_queues = new Dictionary<string, RegistrationInfo>(StringComparer.CurrentCultureIgnoreCase);
            m_topics = new Dictionary<string, RegistrationInfo>(StringComparer.CurrentCultureIgnoreCase);
            m_clientsLock = new ReaderWriterLockSlim();
            m_queuesLock = new ReaderWriterLockSlim();
            m_topicsLock = new ReaderWriterLockSlim();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets a boolean value that indicates whether the <see cref="MessageBusService"/> is currently enabled.
        /// </summary>
        public override bool Enabled
        {
            get
            {
                return (m_publishQueue != null && m_publishQueue.Enabled);
            }
            set
            {
                if (value && m_publishQueue == null)
                    Initialize();

                m_publishQueue.Enabled = value;
            }
        }

        /// <summary>
        /// Gets the descriptive status of the <see cref="MessageBusService"/>.
        /// </summary>
        public override string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();
                status.Append(base.Status);
                if (m_publishQueue != null)
                    status.Append(m_publishQueue.Status);

                return status.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of <see cref="Message"/>s that can be buffered for distribution by the <see cref="MessageBusService"/> before the 
        /// the oldest buffered <see cref="Message"/>s are discarded to keep memory consumption in check by avoiding <see cref="Message"/> flooding.
        /// </summary>
        /// <remarks>Set <see cref="BufferThreshold"/> to -1 to disable discarding of <see cref="Message"/>s.</remarks>
        public int BufferThreshold
        {
            get
            {
                return m_bufferThreshold;
            }
            set
            {
                if (value < 0)
                    value = -1;

                m_bufferThreshold = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="MessageProcessingMode"/> used by the <see cref="MessageBusService"/> for processing <see cref="Message"/> distribution.
        /// </summary>
        public MessageProcessingMode ProcessingMode
        {
            get
            {
                return m_processingMode;
            }
            set
            {
                m_processingMode = value;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes the <see cref="MessageBusService"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">The specified <see cref="ProcessingMode"/> is not supported.</exception>
        public override void Initialize()
        {
            base.Initialize();
            if (m_publishQueue == null)
            {
                // Instantiate the process queue.
                if (m_processingMode == MessageProcessingMode.Parallel)
                    m_publishQueue = ProcessQueue<PublishContext>.CreateAsynchronousQueue(PublishMessages);
                else if (m_processingMode == MessageProcessingMode.Sequential)
                    m_publishQueue = ProcessQueue<PublishContext>.CreateRealTimeQueue(PublishMessages);
                else
                    throw new NotSupportedException(string.Format("Processing mode '{0}' is not supported", m_processingMode));

                // Start the process queue.
                m_publishQueue.Start();
            }
        }

        /// <summary>
        /// Saves <see cref="MessageBusService"/> settings to the config file if the <see cref="TVA.Adapters.Adapter.PersistSettings"/> property is set to true.
        /// </summary>
        public override void SaveSettings()
        {
            base.SaveSettings();
            if (PersistSettings)
            {
                // Save settings under the specified category.
                ConfigurationFile config = ConfigurationFile.Current;
                CategorizedSettingsElementCollection settings = config.Settings[SettingsCategory];
                settings["BufferThreshold", true].Update(m_bufferThreshold);
                settings["ProcessingMode", true].Update(m_processingMode);
                config.Save();
            }
        }

        /// <summary>
        /// Loads saved <see cref="MessageBusService"/> settings from the config file if the <see cref="TVA.Adapters.Adapter.PersistSettings"/> property is set to true.
        /// </summary>
        public override void LoadSettings()
        {
            base.LoadSettings();
            if (PersistSettings)
            {
                // Load settings from the specified category.
                ConfigurationFile config = ConfigurationFile.Current;
                CategorizedSettingsElementCollection settings = config.Settings[SettingsCategory];
                settings.Add("BufferThreshold", m_bufferThreshold, "Maximum number of messages that can be queued for distribution before the oldest ones are discarded.");
                settings.Add("ProcessingMode", m_processingMode, "Processing mode (Parallel; Sequential) to be used for the distribution of messages.");
                BufferThreshold = settings["BufferThreshold"].ValueAs(m_bufferThreshold);
                ProcessingMode = settings["ProcessingMode"].ValueAs(m_processingMode);
            }
        }

        /// <summary>
        /// Registers with the <see cref="MessageBusService"/> to produce or consume <see cref="Message"/>s.
        /// </summary>
        /// <param name="request">An <see cref="RegistrationRequest"/> containing registration data.</param>
        public virtual void Register(RegistrationRequest request)
        {
            // Save client information if not already present.
            ClientInfo client;
            m_clientsLock.EnterUpgradeableReadLock();
            try
            {
                if (!m_clients.TryGetValue(OperationContext.Current.SessionId, out client))
                {
                    m_clientsLock.EnterWriteLock();
                    try
                    {
                        client = new ClientInfo(OperationContext.Current);
                        m_clients.Add(client.SessionId, client);
                        client.OperationContext.Channel.Faulted += OnChannelFaulted;
                        client.OperationContext.Channel.Closing += OnChannelClosing;
                    }
                    finally
                    {
                        m_clientsLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                m_clientsLock.ExitUpgradeableReadLock();
            }

            // Retrieve registration information.
            RegistrationInfo registration;
            if (request.MessageType == MessageType.Queue)
            {
                // Queue
                m_queuesLock.EnterUpgradeableReadLock();
                try
                {
                    if (!m_queues.TryGetValue(request.MessageName, out registration))
                    {
                        m_queuesLock.EnterWriteLock();
                        try
                        {
                            registration = new RegistrationInfo(request);
                            m_queues.Add(request.MessageName, registration);
                        }
                        finally
                        {
                            m_queuesLock.ExitWriteLock();
                        }
                    }
                }
                finally
                {
                    m_queuesLock.ExitUpgradeableReadLock();
                }
            }
            else if (request.MessageType == MessageType.Topic)
            {
                // Topic
                m_topicsLock.EnterUpgradeableReadLock();
                try
                {
                    if (!m_topics.TryGetValue(request.MessageName, out registration))
                    {
                        m_topicsLock.EnterWriteLock();
                        try
                        {
                            registration = new RegistrationInfo(request);
                            m_topics.Add(request.MessageName, registration);
                        }
                        finally
                        {
                            m_topicsLock.ExitWriteLock();
                        }
                    }
                }
                finally
                {
                    m_topicsLock.ExitUpgradeableReadLock();
                }
            }
            else
            {
                // Unsupported
                throw new NotSupportedException(string.Format("Message type '{0}' is not supported", request.MessageType));
            }

            // Update registration information.
            if (registration != null)
            {
                List<ClientInfo> clients = (request.RegistrationType == RegistrationType.Produce ? registration.Producers : registration.Consumers);
                lock (clients)
                {
                    if (!clients.Contains(client))
                        clients.Add(client);
                }
            }
        }

        /// <summary>
        /// Unregisters a previous registration with the <see cref="MessageBusService"/> to produce or consume <see cref="Message"/>s
        /// </summary>
        /// <param name="request">The original <see cref="RegistrationRequest"/> used when registering.</param>
        public virtual void Unregister(RegistrationRequest request)
        {
            // Retrieve registration information.
            RegistrationInfo registration;
            if (request.MessageType == MessageType.Queue)
            {
                // Queue
                m_queuesLock.EnterReadLock();
                try
                {
                    m_queues.TryGetValue(request.MessageName, out registration);
                }
                finally
                {
                    m_queuesLock.ExitReadLock();
                }
            }
            else if (request.MessageType == MessageType.Topic)
            {
                // Topic
                m_topicsLock.EnterReadLock();
                try
                {
                    m_topics.TryGetValue(request.MessageName, out registration);
                }
                finally
                {
                    m_topicsLock.ExitReadLock();
                }
            }
            else
            {
                // Unsupported
                throw new NotSupportedException(string.Format("Message type '{0}' is not supported", request.MessageType));
            }

            // Update registration information.
            if (registration != null)
            {
                List<ClientInfo> clients = (request.RegistrationType == RegistrationType.Produce ? registration.Producers : registration.Consumers);
                lock (clients)
                {
                    clients.RemoveAt(clients.FindIndex(client => client.SessionId == OperationContext.Current.SessionId));
                }
            }
        }

        /// <summary>
        /// Sends the <paramref name="message"/> to the <see cref="MessageBusService"/> for distribution amongst its registered consumers.
        /// </summary>
        /// <param name="message">The <see cref="Message"/> that is to be distributed.</param>
        public virtual void Publish(Message message)
        {
            // Retrieve publisher information.
            ClientInfo client;
            m_clientsLock.EnterReadLock();
            try
            {
                // Update statistics data.
                if (m_clients.TryGetValue(OperationContext.Current.SessionId, out client))
                    Interlocked.Increment(ref client.MessagesProduced);
            }
            finally
            {
                m_clientsLock.ExitReadLock();
            }

            // Retrieve registration information.
            RegistrationInfo registration;
            if (message.Type == MessageType.Queue)
            {
                // Queue
                m_queuesLock.EnterReadLock();
                try
                {
                    m_queues.TryGetValue(message.Name, out registration);
                }
                finally
                {
                    m_queuesLock.ExitReadLock();
                }
            }
            else if (message.Type == MessageType.Topic)
            {
                // Topic
                m_topicsLock.EnterReadLock();
                try
                {
                    m_topics.TryGetValue(message.Name, out registration);
                }
                finally
                {
                    m_topicsLock.ExitReadLock();
                }
            }
            else
            {
                // Unsupported
                throw new NotSupportedException(string.Format("Message type '{0}' is not supported", message.Type));
            }

            // Queue message for distribution.
            if (registration != null && m_publishQueue != null)
            {
                Interlocked.Increment(ref registration.MessagesReceived);
                m_publishQueue.Add(new PublishContext(message, registration));
            }
        }

        /// <summary>
        /// Gets a list of all clients connected to the <see cref="MessageBusService"/>.
        /// </summary>
        /// <returns>An <see cref="ICollection{T}"/> of <see cref="ClientInfo"/> objects.</returns>
        public ICollection<ClientInfo> GetClients()
        {
            return m_clients.Values;
        }

        /// <summary>
        /// Gets a list of all <see cref="MessageType.Queue"/>s registered on the <see cref="MessageBusService"/>.
        /// </summary>
        /// <returns>An <see cref="ICollection{T}"/> of <see cref="RegistrationInfo"/> objects.</returns>
        public ICollection<RegistrationInfo> GetQueues()
        {
            return m_queues.Values;
        }

        /// <summary>
        /// Gets a list of all <see cref="MessageType.Topic"/>s registered on the <see cref="MessageBusService"/>.
        /// </summary>
        /// <returns>An <see cref="ICollection{T}"/> of <see cref="RegistrationInfo"/> objects.</returns>
        public ICollection<RegistrationInfo> GetTopics()
        {
            return m_topics.Values;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="MessageBusService"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.
                    if (disposing)
                    {
                        // This will be done only when the object is disposed by calling Dispose().

                        // Disconnect all clients.
                        if (m_clients != null)
                        {
                            List<string> clientIds;
                            m_clientsLock.EnterReadLock();
                            try
                            {
                                clientIds = new List<string>(m_clients.Keys);
                            }
                            finally
                            {
                                m_clientsLock.ExitReadLock();
                            }

                            foreach (string clientId in clientIds)
                            {
                                DisconnectClient(clientId);
                            }
                        }

                        // Remove queue registrations.
                        if (m_queues != null)
                        {
                            m_queuesLock.EnterWriteLock();
                            try
                            {
                                foreach (RegistrationInfo registration in m_queues.Values)
                                {
                                    registration.Dispose();
                                }
                                m_queues.Clear();
                            }
                            finally
                            {
                                m_queuesLock.ExitWriteLock();
                            }
                        }

                        // Remove topic registrations.
                        if (m_topics != null)
                        {
                            m_topicsLock.EnterWriteLock();
                            try
                            {
                                foreach (RegistrationInfo registration in m_topics.Values)
                                {
                                    registration.Dispose();
                                }
                                m_topics.Clear();
                            }
                            finally
                            {
                                m_topicsLock.ExitWriteLock();
                            }
                        }

                        if (m_publishQueue != null)
                            m_publishQueue.Dispose();

                        if (m_clientsLock != null)
                            m_clientsLock.Dispose();

                        if (m_queuesLock != null)
                            m_queuesLock.Dispose();

                        if (m_topicsLock != null)
                            m_topicsLock.Dispose();
                    }
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                    base.Dispose(disposing);    // Call base class Dispose().
                }
            }
        }

        private void PublishMessages(PublishContext[] contexts)
        {
            // Process distribution of all the messages.
            foreach (PublishContext context in contexts)
            {
                // Distribute message to all subscribed clients.
                lock (context.Registration.Consumers)
                {
                    foreach (ClientInfo client in context.Registration.Consumers)
                    {
                        try
                        {
                            client.OperationContext.GetCallbackChannel<IMessageBusServiceCallback>().ProcessMessage(context.Message);
                            Interlocked.Increment(ref client.MessagesConsumed);

                            if (context.Message.Type == MessageType.Queue)
                                break;
                        }
                        catch
                        {
                            // Disconnect the subscriber if an error is encountered during transmission.
                            try
                            {
                                if (client.OperationContext.Channel.State == CommunicationState.Opened)
                                    client.OperationContext.Channel.Close();
                            }
                            catch { }
                        }
                    }
                }
                Interlocked.Increment(ref context.Registration.MessagesProcessed);
            }

            // Keep message buffer in check if specified.
            if (m_bufferThreshold > 0 && m_publishQueue.Count > m_bufferThreshold)
                m_publishQueue.RemoveRange(0, m_publishQueue.Count - m_bufferThreshold);
        }

        private void DisconnectClient(string clientId)
        {
            // Retrieve client information.
            ClientInfo client;
            m_clientsLock.EnterUpgradeableReadLock();
            try
            {
                if (m_clients.TryGetValue(clientId, out client))
                {
                    // Remove client.
                    m_clientsLock.EnterWriteLock();
                    try
                    {
                        m_clients.Remove(clientId);
                        client.OperationContext.Channel.Faulted -= OnChannelFaulted;
                        client.OperationContext.Channel.Closing -= OnChannelClosing;
                    }
                    finally
                    {
                        m_clientsLock.ExitWriteLock();
                    }

                    // Close channel.
                    if (client.OperationContext.Channel.State == CommunicationState.Opened)
                        client.OperationContext.Channel.Close();
                }
            }
            finally
            {
                m_clientsLock.ExitUpgradeableReadLock();
            }

            // Remove client registrations.
            if (client != null)
            {
                // Remove any queue registrations.
                m_queuesLock.EnterReadLock();
                try
                {
                    foreach (RegistrationInfo registration in m_queues.Values)
                    {
                        lock (registration.Producers)
                        {
                            registration.Producers.Remove(client);
                        }
                        lock (registration.Consumers)
                        {
                            registration.Consumers.Remove(client);
                        }
                    }
                }
                finally
                {
                    m_queuesLock.ExitReadLock();
                }

                // Remove any topic registrations.
                m_topicsLock.EnterReadLock();
                try
                {
                    foreach (RegistrationInfo registration in m_topics.Values)
                    {
                        lock (registration.Producers)
                        {
                            registration.Producers.Remove(client);
                        }
                        lock (registration.Consumers)
                        {
                            registration.Consumers.Remove(client);
                        }
                    }
                }
                finally
                {
                    m_topicsLock.ExitReadLock();
                }
            }
        }

        private void OnChannelClosing(object sender, EventArgs e)
        {
            DisconnectClient(((IContextChannel)sender).SessionId);
        }

        private void OnChannelFaulted(object sender, EventArgs e)
        {
            DisconnectClient(((IContextChannel)sender).SessionId);
        }

        #endregion
    }
}
