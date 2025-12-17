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
// File         : gizmochat.cpp
// Module       : GizmoChat
// Description  : A simple network chat program. (example application)
// Author       : Christian Andersson
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
// CAN  040513  Created file
// AMO  251217  Modified and updated example
//
//*****************************************************************************

// Include all necessary files.
#include "gzDistributionLibrary.h"

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



// --------------------- The chat client class declaration ---------------------

class ChatClient : public gzDistClientInterface
{
public:
    // Constructor declaration.
    ChatClient();

    // Destructor declaration.
    virtual ~ChatClient();

    // The run method.
    gzVoid run();

protected:

    // Virtual callback reimplemented from gzDistClientInterface.
    gzVoid onEvent(gzDistEvent* event) override;
};


// --------------------- The code ---------------------

// Constructor
ChatClient::ChatClient() : gzDistClientInterface("ChatClient")
{
}

// Destructor
ChatClient::~ChatClient()
{
}

gzVoid ChatClient::run()
{
    // Create the distribution manager.
    gzDistManagerPtr manager = gzDistManager::getManager(TRUE);

    // Start the manager and use default channels.
#if !defined USE_LOCAL_HOST
    manager->start(gzDistCreateDefaultSessionChannel(), gzDistCreateDefaultServerChannel());
#else
    manager->start(gzDistCreateDefaultSessionChannel(FALSE, GZ_DIST_MULTICAST, gzHostAddress("127.0.0.1")), gzDistCreateDefaultServerChannel(FALSE, GZ_DIST_MULTICAST, gzHostAddress("127.0.0.1")));
#endif

    // Initialize my distribution interface.
    initialize();

    // Create a global session where all chat messages will be sent.
    gzDistSessionPtr myChatSession = getSession("ChatSession", TRUE, TRUE);

    // Join the session.
    joinSession(myChatSession);

    // Subscribe all events on myChatSession.
    subscribeEvents(gzDistEvent::getClassType(), myChatSession);

    // Alloc a buffer for alias.
    gzString alias;

    // Tell the user to enter his/her alias.
    printf("Enter your alias:");

    // Get user alias.
    readLine(alias);

    // Print some information and a '>' at cursor position.
    printf("Write your messages here. Type 'exit' to exit.\r\n>");

    // Alloc a message buffer.
    gzString message;

    // Repeat until 'exit' is entered.
    while (readLine(message))
    {
        // Quit if the user type 'exit'.
        if (message == "exit")
        {
            // Break the 'while loop'.
            break;
        }

        // Don't send an empty message.
        if (!message)
        {
            // Empty message. Put a cursor at cursor position and wait for new input.
            printf(">");
            continue;
        }

        // Put a cursor at cursor position.
        printf(">");

        // Create a new event.
        gzDistEvent* event = new gzDistEvent;

        // Put the alias in the event.
        event->setAttributeValue("Sender", alias);

        // Put the message in the event.
        event->setAttributeValue("Message", message);

        // Put an id for current process in the event.
        event->setAttributeValue("Id", gzDistGetCurrentProcessID());

        // Send my message on myChatSession.
        sendEvent(event, myChatSession);

        // Clear message
        message.clear();
    }

    // Release my reference to the chat session.
    myChatSession = NULL;

    // Uninitialize my distribution interface.
    uninitialize();

    // Shut down the manager.
    manager->shutDown();
}

gzVoid ChatClient::onEvent(gzDistEvent* event)
{
    gzDouble id;

    // Get the id from the event
    if (event->getAttributeNumber("Id", id))
    {
        // Don't receive my own messages.
        if (id == gzDistGetCurrentProcessID())
        {
            // I'm the sender -> Don't display.
            return;
        }
    }

    // Get the sender name
    gzString sender = event->getAttributeString("Sender");

    // Get the message
    gzString message = event->getAttributeString("Message");

    // Display the message.
#ifdef GZ_WIN32
    // Change this line to make the example use
    // message box output on other platforms than Win32.
    MessageBox(NULL, message, sender, MB_OK);
#else
    // Default output to console.
    printf("%s: %s\n", (const char*)sender, (const char*)message);
#endif

}


// --------------------- The main routine. ---------------------

int main(int argc, char* argv[])
{
    // Create the chat client.
    ChatClient chatClient;

    // Run it!
    chatClient.run();

    return 0;
}