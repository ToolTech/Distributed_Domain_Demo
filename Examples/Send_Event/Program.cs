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

namespace Send_Event
{
        
    class Program
    {
        static readonly LicenseManager lic = new LicenseManager();

        static void Main(string[] args)
        {

            // Add a message receiver
            Message.OnMessage += Message_OnMessage;

            // Set message level to debug
            Message.SetMessageLevel(MessageLevel.DEBUG|MessageLevel.INTERNAL);

           
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

            // Join that session and subribe all events
            client.JoinSession(session);

            // Subscribe standard events
            client.SubscribeEvents(session);

            // Create a delegete
            client.OnEvent += Client_OnEvent;

            for(int i=0;i<100;i++)
            {
                DistEvent e = manager.GetEvent();

                e.SetAttributeValue("Message", "Helluuuu");

                e.SetAttributeValue("Count", i);

                client.SendEvent(e, session);

                System.Threading.Thread.Sleep(1000);
            }

            

            client.ResignSession(session);
   
            client.Uninitialize();

            // Some kind of graceful shutdown
            manager.Shutdown();


            // GC and platform uninit is managed by the system automatically
        }

        private static void Client_OnEvent(DistClient sender, DistEvent e)
        {
            // Check if message is from us
            if (e.GetSource() == sender.GetClientID().InstanceID)
                return;

            System.Console.WriteLine(e.ToString());
        }

        private static void Message_OnMessage(string sender, MessageLevel level, string message)
        {
            System.Console.WriteLine(message);
        }
    }
}
