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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GizmoSDK.GizmoBase;
using GizmoSDK.GizmoDistribution;

namespace Event_Performance
{

    class Program
    {
        const int COUNT = 100000;

        static void Main(string[] args)
        {
            // Initialize platforms for various used SDKs
            GizmoSDK.GizmoBase.Platform.Initialize();
            GizmoSDK.GizmoDistribution.Platform.Initialize();


            Message.OnMessage += Message_OnMessage;

            Message.SetMessageLevel(MessageLevel.DEBUG);
              
            // Create a manager. The manager controls it all
            DistManager manager = DistManager.GetManager(true);

            // Start the manager with settting for transport protocols
            manager.Start();

            DistSession session = manager.GetSession("Perfa", true);

            // First Client -------------------------------------------------------------------

            // Client set up. You are a client that only sends information
            DistClient client = new DistClient("PerfClient", manager);

            // We need to tell the client how to initialize
            client.Initialize(0, 0, true);

                                    

            // Second Client -------------------------------------------------------------------

            // Client set up. You are a client that only receives information
            DistClient client2 = new DistClient("PerfClient2", manager);

            // We need to tell the client how to initialize
            client2.Initialize(0, 0, true);
            
            // Joint that session and subribe all events
            client2.JoinSession(session);
            client2.SubscribeEvents(session); // Subscribe All Events

            // Create a delegete
            client2.OnEvent += Client2_OnEvent;

            System.Threading.Thread.Sleep(100);

            // Create COUNT test event 

            Timer timer = new Timer();

            DistEvent[] e_arr = new DistEvent[COUNT];

            for (int i = 0; i < COUNT; i++)
            {
                e_arr[i] = new DistEvent();
                e_arr[i].SetAttributeValue("Cnt", i);

                // Set some additional data e.g. two vec3
                e_arr[i].SetAttributeValue("vec1", new Vec3(1, 2, 3));
                e_arr[i].SetAttributeValue("vec2", new Vec3(4, 5, 6));
            }

            Console.WriteLine($"Created {COUNT} events in {timer.GetTime()} seconds -> Frequency: { timer.GetFrequency(COUNT)}");

            // Send COUNT events

            timer = new Timer();

            for (int i = 0; i < COUNT; i++)
                client.SendEvent(e_arr[i], session);

            Console.WriteLine($"Sent {COUNT} events in {timer.GetTime()} seconds -> Frequency: {timer.GetFrequency(COUNT)}");
        
            while (manager.HasPendingData() || !stopper)
                System.Threading.Thread.Sleep(10);

            client.Uninitialize(true);
            client2.Uninitialize(true);

            // Some kind of graceful shutdown
            manager.Shutdown();

            // GC and platform uninit is managed by the system automatically
        }

        private static void Message_OnMessage(string sender, MessageLevel level, string message)
        {
            Console.WriteLine(message);
        }

        static int counter = 0;

        static Timer recv_timer = null;

        static bool stopper = false;

       
        private static void Client2_OnEvent(DistClient sender, DistEvent e)
        {
            if (counter == 0)
            {
                recv_timer = new Timer();
            }

            if (e.GetAttributeValue("Cnt") != counter)
                Console.WriteLine("Error");

            counter++;

            if (counter == COUNT)
            {
                Console.WriteLine($"Received {COUNT} events in {recv_timer.GetTime()} seconds -> Frequency: {recv_timer.GetFrequency(COUNT)} ");
                counter = 0;
                stopper = true;
            }


        }
    }
}
