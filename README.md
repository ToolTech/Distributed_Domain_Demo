Distributed Domain Demo
==================

<B>A Saab Dynamics demo of the GizmoDistribution API SDK</B>

This GIT repository will allow you to take a look at a Saab demo, written by the Open Saab Development community using the GizmoSDK code base from Saab Dynamics, Training & Simulation. 

The open source and open documentation in here are to be considered as GPL code and can be used in your own projects.

The binaries are licensed by Saab Dynamics. If you are interested in using this for commersial use, please contact 

anders.moden@saabgroup.com



Running the demo
================

Just open the solution in VisualStudio and press play
Good Luck !


Technology Info
===============

The demo is based on GizmoDistribution SDK that is part of the GizmoSDK. It will allow a HLA/DIS/CORBA alike API for C#

You can take a look at http://www.gizmosdk.se/html/distrdoc.htm for more info! It is also structured to be used in Unity3D.

Simplicty of code
=================

We tried to make an easy to use API but also very high performance

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
