//*****************************************************************************
//
// Copyright (C) SAAB AB
//
// All rights, including the copyright, to the computer program(s)
// herein belong to Saab AB. The program(s) may be used and/or
// copied only with the written permission of Saab AB, or in
// accordance with the terms and conditions stipulated in the
// agreement/contract under which the program(s) have been
// supplied.
//
//
// Information Class:   COMPANY UNCLASSIFIED
// Defence Secrecy:     NOT CLASSIFIED
// Export Control:      NOT EXPORT CONTROLLED
//
//
// File         : main.cpp
// Module       : main
// Description  : main (example application)
// Author       : Anders Sandblad       
// Product      : GizmoDistribution
//      
//
//          
// NOTE:    GizmoDistribution is a toolkit used for implementing distributed 
//          objects and events in C++
//
//
// Revision History...                          
//                                  
// Who  Date    Description                     
//                                  
// XAA  040423  Created file    
// AMO  251217  Modified and updated example
//
//*****************************************************************************

// Includes
#include "gzBaseLibrary.h"
#include "gzDistLibrary.h"
#include "gzDistRemoteLibrary.h"


#define GLOBAL          // Use this define to enable global distribution

#define USE_LOCAL_HOST  // Defie this to use local host if you have firewall issues


// An instance of the license manager that will load the gz_license_server.dat file
class LicenseManager : public gzLicenseManager
{
public:

    LicenseManager()
    {
        // Run and wait for manager
        run(TRUE);
    }

} lic_manager;

// Application entry point
int main(int argc, char* argv[])
{
    // Setup default message receiver
    gzMessage::setMessageLevel(GZ_MESSAGE_NOTICE);

#ifndef GLOBAL

    // Create the manager and start local distribution
    gzDistManagerPtr manager = gzDistManager::getManager(TRUE);
    manager->start();

    GZMESSAGE(GZ_MESSAGE_NOTICE, "Hello Local GizmoDistribution !!!");

#else

    // To enable global distribution you should
    // provide two channels (with different transports).
    // One channel is used for data distribution and
    // the other is used for server control messages.

    // Create the manager
    gzDistManagerPtr manager = gzDistManager::getManager(TRUE);

    // This is the simplest way to do it. It uses default parameters.
    // See GizmoDistribution online documentation for details.
#if !defined USE_LOCAL_HOST
    manager->start(gzDistCreateDefaultSessionChannel(), gzDistCreateDefaultServerChannel());
#else
    manager->start(gzDistCreateDefaultSessionChannel(FALSE, GZ_DIST_MULTICAST, gzHostAddress("127.0.0.1")), gzDistCreateDefaultServerChannel(FALSE, GZ_DIST_MULTICAST, gzHostAddress("127.0.0.1")));
#endif

    /*  *** Note: If you want to specify IP and ports, do as below: ***

        // Setup session channel
        gzDistRemoteChannel* sessionChannel = new gzDistRemoteChannel;
        gzDistTransportUDP* sessionTransport = new gzDistTransportUDP;

    !   // For multicast...                                                                     \
    !   sessionTransport->createMulticast(gzSocketAddress(gzHostAddress(234,56,78,90), 2345));   |
    !                                                                                            |
    !   // ...or for broadcast                                                                   | NOTE: Use only one of these!
    !   sessionTransport->createBroadcast(2345);                                                 |
                                                                                                /
        sessionChannel->setTransport(sessionTransport);

        // Setup server channel
        gzDistRemoteChannel* serverChannel = new gzDistRemoteChannel;
        gzDistTransportUDP* serverTransport = new gzDistTransportUDP;

    !   // For multicast...                                                                     \
    !   serverTransport->createMulticast(gzSocketAddress(gzHostAddress(234,56,78,90), 5432));    |
    !                                                                                            |
    !   // ...or for broadcast:                                                                  | NOTE: Use only one of these!
    !   serverTransport->createBroadcast(5432);                                                  |
                                                                                                /
        serverChannel->setTransport(serverTransport);

        // Start manager for global distribution
        manager->start(sessionChannel, serverChannel);
    */
    GZMESSAGE(GZ_MESSAGE_NOTICE, "Hello Global GizmoDistribution !!!");

#endif

    // run a while
    gzSleep(2 * GZ_SLEEP_SECOND);

    // shut down and release manager
    manager->shutDown();
    manager = NULL;

    gzSleep(GZ_SLEEP_SECOND);

    return 0;

}