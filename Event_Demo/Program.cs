using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GizmoSDK.GizmoBase;
using GizmoSDK.GizmoDistribution;

namespace Event_Demo
{
    class Program
    {
        static void Main(string[] args)
        {
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
            DistClient client = new DistClient("MessageClient", manager);

            // We need to tell the client how to initialize
            client.Initialize();

            // Now we can get a session. A kind of a meeting room that is used to exchange various "topics"
            DistSession session=client.GetSession("MessageSession", true, true);

            // Joint that session and subribe all events
            client.JoinSession(session);
            client.SubscribeEvents(session);

            // Create a delegete
            client.OnEvent += Client_OnEvent;


            // Now loops around some simple program to get strings from console and distribute them as a message app

            while(true)
            {
                string result = Console.ReadLine();

                if (result == "quit")
                    break;

                // get a new empty event from manager
                DistEvent e = manager.GetEvent();

                // set some attributes in the event to any kind of value
                e.SetAttributeValue("Message", result);

                // and send the event on the specific session
                client.SendEvent(e, session);
            }

            // Some kind of graceful shutdown
            manager.Shutdown();


            // GC and platform uninit is managed by the system automatically
        }
                

        private static void Client_OnEvent(DistClient sender, DistEvent e)
        {
            if(e.GetSource()!=sender.GetClientID().InstanceID)
                Console.WriteLine(e.GetAttributeValue("Message").GetString());
        }
    }
}
