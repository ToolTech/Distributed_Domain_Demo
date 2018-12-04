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
        const int COUNT = 10000;

        static void Main(string[] args)
        {
            // Initialize platforms for various used SDKs
            GizmoSDK.GizmoBase.Platform.Initialize();
            GizmoSDK.GizmoDistribution.Platform.Initialize();

            // Create a manager. The manager controls it all
            DistManager manager = DistManager.GetManager(true);

            // Start the manager with settting for transport protocols
            manager.Start(DistRemoteChannel.CreateDefaultSessionChannel(false,DistTransportType.BROADCAST,"127.0.0.1"), DistRemoteChannel.CreateDefaultServerChannel(false,DistTransportType.BROADCAST,"127.0.0.1"));

            // Client set up. You are a client that sends and receives information
            DistClient client = new DistClient("PerfClient", manager);

            // We need to tell the client how to initialize
            client.Initialize(0,0,true);

            // Now we can get a session. A kind of a meeting room that is used to exchange various "topics"
            DistSession session = client.GetSession("PerfSession", true, true);

            // Joint that session and subribe all events
            client.JoinSession(session);
            client.SubscribeEvents(session); // Subscribe All Events

            // Create a delegete
            client.OnEvent += Client_OnEvent;


            // Now loops around some simple program to get strings from console and distribute them as a message app

            while (true)
            {
                string result = Console.ReadLine();

                if (result == "quit")
                    break;

                // Create 1000 test event 

                Timer timer = new Timer();

                DistEvent [] e_arr=new DistEvent[COUNT];

                for (int i = 0; i < COUNT; i++)
                {
                    e_arr[i] = new DistEvent();
                    e_arr[i].SetAttributeValue("Counter", i);
                }

                Console.WriteLine($"Created {COUNT} events in {timer.GetTime()} seconds -> Frequency: { timer.GetFrequency(COUNT)}");

                // Send 1000 events

                timer =new Timer();

                for(int i=0;i< COUNT; i++)
                    client.SendEvent(e_arr[i], session);

                Console.WriteLine($"Sent {COUNT} events in {timer.GetTime()} seconds -> Frequency: {timer.GetFrequency(COUNT)}");
            }

            while (manager.HasPendingData())
                System.Threading.Thread.Sleep(10);

            // Some kind of graceful shutdown
            manager.Shutdown();


            // GC and platform uninit is managed by the system automatically
        }

        static int counter = 0;

        static Timer recv_timer=null;

        private static void Client_OnEvent(DistClient sender, DistEvent e)
        {
            // Check if message is from us
            if (e.GetSource() == sender.GetClientID().InstanceID)
                return;

            if(counter==0)
            {
                recv_timer = new Timer();
            }

            if (e.GetAttributeValue("Counter") != counter)
                Console.WriteLine("Error");

            counter++;

            if(counter== COUNT)
            {
                Console.WriteLine($"Received {COUNT} events in {recv_timer.GetTime()} seconds -> Frequency: {recv_timer.GetFrequency(COUNT)} ");
                counter = 0;
            }

            
        }
    }
}
