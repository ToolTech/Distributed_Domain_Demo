//******************************************************************************
// File			: Program.cs
// Module		: Distribution Examples
// Description	: Basic examples for C# distribution
// Author		: Anders Modén		
//		
// Copyright © 2003- Saab Training Systems AB, Sweden
//			
// NOTE:	GizmoBase is a platform abstraction utility layer for C++. It contains 
//			design patterns and C++ solutions for the advanced programmer.
//
//
// Revision History...							
//									
// Who	Date	Description						
//									
// AMO	190923	Created file 	
//
//******************************************************************************

using GizmoSDK.GizmoBase;
using GizmoSDK.GizmoDistribution;

namespace Create_Client
{
        
    class Program
    {
        static void Main(string[] args)
        {

            // Add a message receiver
            Message.OnMessage += Message_OnMessage;

            // Set message level to debug
            Message.SetMessageLevel(MessageLevel.DEBUG);

           
            // Initialize platforms for various used SDKs
            GizmoSDK.GizmoBase.Platform.Initialize();
            GizmoSDK.GizmoDistribution.Platform.Initialize();
                       


            // Create a manager. The manager controls it all
            DistManager manager = DistManager.GetManager(true);

               // Start the manager with settting for transport protocols
            manager.Start(DistRemoteChannel.CreateDefaultSessionChannel(), DistRemoteChannel.CreateDefaultServerChannel());

            //If we want to attach the DistMonitor debugger
            manager.EnableDebug(true);
            
            // Client set up. You are a client that sends and receives information
            DistClient client = new DistClient("Our Test Client", manager);

            // We need to tell the client how to initialize
            client.Initialize();

            // Now we can get a session. A kind of a meeting room that is used to exchange various "topics"
            DistSession session = client.GetSession("MessageSession", true, true);

            // Joint that session and subribe all events
            client.JoinSession(session);

            System.Threading.Thread.Sleep(10000);

            client.ResignSession(session);
   
            client.Uninitialize();

            // Some kind of graceful shutdown
            manager.Shutdown();


            // GC and platform uninit is managed by the system automatically
        }

        private static void Message_OnMessage(string sender, MessageLevel level, string message)
        {
            System.Console.WriteLine(message);
        }
    }
}
