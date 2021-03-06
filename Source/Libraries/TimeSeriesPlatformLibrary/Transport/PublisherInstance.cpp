//******************************************************************************************************
//  PublisherInstance.cpp - Gbtc
//
//  Copyright � 2018, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  12/05/2018 - Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

#include <iostream>

#include "PublisherInstance.h"
#include "../Common/Convert.h"

using namespace std;
using namespace pugi;
using namespace GSF::TimeSeries;
using namespace GSF::TimeSeries::Transport;

PublisherInstance::PublisherInstance(uint16_t port, bool ipV6) :
    m_port(port),
    m_isIPV6(ipV6),
    m_publisher(port, ipV6),
    m_initialized(false)
{
    // Reference this PublisherInstance in DataPublisher user data
    m_publisher.SetUserData(this);
}

PublisherInstance::~PublisherInstance() = default;

void PublisherInstance::HandleStatusMessage(DataPublisher* source, const string& message)
{
    PublisherInstance* instance = static_cast<PublisherInstance*>(source->GetUserData());
    instance->StatusMessage(message);
}

void PublisherInstance::HandleErrorMessage(DataPublisher* source, const string& message)
{
    PublisherInstance* instance = static_cast<PublisherInstance*>(source->GetUserData());
    instance->ErrorMessage(message);
}

void PublisherInstance::HandleClientConnected(DataPublisher* source, const Guid& clientID, const string& connectionInfo, const string& subscriberInfo)
{
    PublisherInstance* instance = static_cast<PublisherInstance*>(source->GetUserData());
    instance->ClientConnected(clientID, connectionInfo, subscriberInfo);;
}

void PublisherInstance::StatusMessage(const string& message)
{
    cout << message << endl << endl;
}

void PublisherInstance::ErrorMessage(const string& message)
{
    cerr << message << endl << endl;
}

void PublisherInstance::ClientConnected(const Guid& clientID, const string& connectionInfo, const string& subscriberInfo)
{
    cout << "Client \"" << subscriberInfo << "\" with ID " << ToString(clientID) << " connected..." << endl;
    cout << "Connection Info:" << endl;
    cout << connectionInfo << endl << endl;
}

void PublisherInstance::Initialize()
{
    m_initialized = true;
}

void PublisherInstance::DefineMetadata(const vector<DeviceMetadataPtr>& deviceMetadata, const vector<MeasurementMetadataPtr>& measurementMetadata, const vector<PhasorMetadataPtr>& phasorMetadata)
{
    m_publisher.DefineMetadata(deviceMetadata, measurementMetadata, phasorMetadata);
}

void PublisherInstance::DefineMetadata(const vector<ConfigurationFramePtr>& devices, const MeasurementMetadataPtr& qualityFlags)
{
    m_publisher.DefineMetadata(devices, qualityFlags);
}

void PublisherInstance::DefineMetadata(const xml_document& metadata)
{
    m_publisher.DefineMetadata(metadata);
}

void PublisherInstance::PublishMeasurements(const vector<Measurement>& measurements)
{
    if (!m_initialized)
        throw PublisherException("Operation failed, publisher is not initialized.");

    m_publisher.PublishMeasurements(measurements);
}

void PublisherInstance::PublishMeasurements(const vector<MeasurementPtr>& measurements)
{
    if (!m_initialized)
        throw PublisherException("Operation failed, publisher is not initialized.");

    m_publisher.PublishMeasurements(measurements);
}

bool PublisherInstance::IsMetadataRefreshAllowed() const
{
    return m_publisher.IsMetadataRefreshAllowed();
}

void PublisherInstance::SetMetadataRefreshAllowed(bool allowed)
{
    m_publisher.SetMetadataRefreshAllowed(allowed);
}

bool PublisherInstance::IsNaNValueFilterAllowed() const
{
    return m_publisher.IsNaNValueFilterAllowed();
}

void PublisherInstance::SetNaNValueFilterAllowed(bool allowed)
{
    m_publisher.SetNaNValueFilterAllowed(allowed);
}

bool PublisherInstance::IsNaNValueFilterForced() const
{
    return m_publisher.IsNaNValueFilterForced();
}

void PublisherInstance::SetNaNValueFilterForced(bool forced)
{
    m_publisher.SetNaNValueFilterForced(forced);
}

uint32_t PublisherInstance::GetCipherKeyRotationPeriod() const
{
    return m_publisher.GetCipherKeyRotationPeriod();
}

void PublisherInstance::SetCipherKeyRotationPeriod(uint32_t period)
{
    m_publisher.SetCipherKeyRotationPeriod(period);
}

uint16_t PublisherInstance::GetPort() const
{
    return m_port;
}

bool PublisherInstance::IsIPv6() const
{
    return m_isIPV6;
}

uint64_t PublisherInstance::GetTotalCommandChannelBytesSent() const
{
    return m_publisher.GetTotalCommandChannelBytesSent();
}

uint64_t PublisherInstance::GetTotalDataChannelBytesSent() const
{
    return m_publisher.GetTotalDataChannelBytesSent();
}

uint64_t PublisherInstance::GetTotalMeasurementsSent() const
{
    return m_publisher.GetTotalMeasurementsSent();
}

bool PublisherInstance::IsConnected() const
{
    return m_publisher.IsConnected();
}

bool PublisherInstance::IsInitialized() const
{
    return m_initialized;
}
