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

using System;

using GizmoSDK.GizmoBase;
using GizmoSDK.GizmoDistribution;

namespace Object_Performance
{
    
    class Program
    {
        const int COUNT = 1000;

        const int OBJECTS = 100;

        static readonly LicenseManager lic = new LicenseManager();
       
        static void Main(string[] args)
        {
            // Initialize platforms for various used SDKs
            GizmoSDK.GizmoBase.Platform.Initialize();
            GizmoSDK.GizmoDistribution.Platform.Initialize();

            GizmoSDK.GizmoBase.Message.SetMessageLevel(MessageLevel.DEBUG);

            // Create a manager. The manager controls it all
            DistManager manager = DistManager.GetManager(true);

            DistTransportType protocol = DistTransportType.MULTICAST;

           // string iface =  "127.0.0.1";

            string iface = "192.168.137.1";

            // Start the manager with settting for transport protocols
            manager.Start(DistRemoteChannel.CreateDefaultSessionChannel(false,protocol, iface), DistRemoteChannel.CreateDefaultServerChannel(false,protocol, iface));

            // Client set up. You are a client that sends and receives information
            DistClient client = new DistClient("PerfClient", manager);

            // We need to tell the client how to initialize
            client.Initialize(0,0,true);

            // Now we can get a session. A kind of a meeting room that is used to exchange various "topics"
            DistSession session = client.GetSession("PerfSession", true, true);

            // Joint that session and subribe all events
            client.JoinSession(session);
            client.SubscribeObjects(session); // Subscribe All Objects
            

            // Create a delegete
            client.OnNewObject += Client_OnNewObject;
            client.OnNewAttributes += Client_OnNewAttributes;
            client.OnUpdateAttributes += Client_OnUpdateAttributes;

            Console.WriteLine($"Press <RETURN> to start sending");

            // Now loops around some simple program to get strings from console and distribute them as a message app

            while (true)
            {
                string result = Console.ReadLine();

                if (result == "quit")
                    break;

                Timer timer = new Timer();

                DistObject [] objects = new DistObject[OBJECTS];

                for (int i = 0; i < OBJECTS; i++)
                {
                    objects[i] = manager.GetObject($"Object {i}");

                    client.AddObject(objects[i], session);

                    objects[i] = client.WaitForObject($"Object {i}", session);
                }

                Console.WriteLine($"Added {OBJECTS} objects in {timer.GetTime()} seconds -> Frequency: {timer.GetFrequency(OBJECTS)}");

                // make sure all objects are updated once

                DistTransaction transaction = new DistTransaction();

                transaction.SetAttributeValue("Test", 0.0);

                for (int j = 0; j < OBJECTS; j++)
                {
                    if (!client.UpdateObject(transaction, objects[j]))
                        Console.WriteLine("Bajs");
                }
            
                System.Threading.Thread.Sleep(10);

                // Send COUNT updates on OBJECTS objects

                timer = new Timer();

                for (int i=0;i< COUNT; i++)
                {
                    transaction.SetAttributeValue("Test", timer.GetTime());

                    for (int j = 0; j < OBJECTS; j++)
                    {
                        if (!client.UpdateObject(transaction, objects[j]))
                            Console.WriteLine("Bajs");
                    }
                }

                Console.WriteLine($"Updated {OBJECTS} objects with {COUNT} updates in {timer.GetTime()} seconds -> Frequency: {timer.GetFrequency(COUNT*OBJECTS)}");
            }

            while (manager.HasPendingData())
                System.Threading.Thread.Sleep(10);

            client.Uninitialize(true);
       
            // Some kind of graceful shutdown
            manager.Shutdown();


            // GC and platform uninit is managed by the system automatically
        }

        static int update_counter = 0;

        static Timer update_timer = null;

        private static void Client_OnUpdateAttributes(DistClient sender, DistNotificationSet notif, DistObject o, DistSession session)
        {
            if (update_counter == OBJECTS)
            {
                update_timer = new Timer();
            }

            update_counter++;

            if (update_counter == OBJECTS*(COUNT+1))
            {
                Console.WriteLine($"Got {OBJECTS * COUNT} objects updates in {update_timer.GetTime()} seconds -> Frequency: {update_timer.GetFrequency(OBJECTS * COUNT)} ");
                update_counter = 0;
            }
        }

        private static void Client_OnNewAttributes(DistClient sender, DistNotificationSet notif, DistObject o, DistSession session)
        {
            sender.SubscribeAttributeValue(notif, o, true);
        }

        static int object_counter = 0;

        static Timer new_object_timer = null;

        private static void Client_OnNewObject(DistClient sender, DistObject o, DistSession session)
        {
            if (object_counter == 0)
            {
                new_object_timer = new Timer();
            }

            object_counter++;

            sender.SubscribeAttributes(o, true);

            if (object_counter == OBJECTS)
            {
                Console.WriteLine($"Got {OBJECTS} new objects in {new_object_timer.GetTime()} seconds -> Frequency: {new_object_timer.GetFrequency(OBJECTS)} ");
                object_counter = 0;
            }
        }



    }
}
