Distributed Domain Demo
=======================

<B>A Saab Dynamics demo of the GizmoDistribution API SDK</B>

This GIT repository will allow you to take a look at a Saab demo, written by the Open Saab Development community using the GizmoSDK code base from Saab Dynamics, Training & Simulation. 

The open source and open documentation in here are to be considered as GPL code and can be used in your own projects.

The binaries are licensed by Saab Dynamics. If you are interested in using this for commersial use, please contact 

anders.moden@saabgroup.com

Generate License
================
Run the "licApp.exe" application in the LicApp directory and press "Copy Request" and send clipboard content to email adress above

Running the demo
================

Just open the solution in VisualStudio and press play
Good Luck !


Technology Info
===============

The demo is based on GizmoDistribution SDK that is part of the GizmoSDK. It will allow a HLA/DIS/CORBA alike API for C# but at the same time compatible with C++ native API, enabling a transparent data exchange between native C++ and C# in high speed.

You can take a look at https://gizmosdk.blob.core.windows.net/html/html/distrdoc.htm for more info! It is also structured to be used in Unity3D.

Simplicty of code
=================

We tried to make an easy to use API but also very high performance

```csharp    
    using System;

    using GizmoSDK.GizmoBase;
    using GizmoSDK.GizmoDistribution;

    namespace Event_Demo
    {
        class MessageEvent : DistEvent
        {
            // Let the constructor be private or internal so we dont expose this by mistake
            internal  MessageEvent(IntPtr nativeReference) : base(nativeReference)
            {
            }

            // a factory design pattern for this class
            public override Reference Create(IntPtr nativeReference)
            {
                return new MessageEvent(nativeReference) as Reference;
            }

            public string Message
            {
                get { return GetAttributeValue(nameof(Message)); }
                set { SetAttributeValue(nameof(Message), value); }
            }

            public double Time
            {
                get { return GetAttributeValue(nameof(Time)); }
                set { SetAttributeValue(nameof(Time), value); }
            }
        }

        class Program
        {
            static void Main(string[] args)
            {
                // Initialize platforms for various used SDKs
                GizmoSDK.GizmoBase.Platform.Initialize();
                GizmoSDK.GizmoDistribution.Platform.Initialize();

                // Create a manager. The manager controls it all
                DistManager manager = DistManager.GetManager(true);

                // Let the manager know about our special event
                manager.RegisterEvent<MessageEvent>();

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
                client.SubscribeEvents< MessageEvent>(session); // Subscribe MessageEvent as base type

                // Create a delegete
                client.OnEvent += Client_OnEvent;


                // Now loops around some simple program to get strings from console and distribute them as a message app

                while(true)
                {
                    string result = Console.ReadLine();

                    if (result == "quit")
                        break;

                    // get a new empty event from manager
                    MessageEvent e = manager.GetEvent<MessageEvent>();

                    // set some attributes in the event to any kind of value
                    e.Message=result;
                    e.Time = Time.SystemSeconds;

                    // and send the event on the specific session
                    client.SendEvent(e, session);
                }

                // Some kind of graceful shutdown
                manager.Shutdown();


                // GC and platform uninit is managed by the system automatically
            }


            private static void Client_OnEvent(DistClient sender, DistEvent e)
            {
                // Check if message is from us
                if (e.GetSource() == sender.GetClientID().InstanceID)
                    return;

                MessageEvent mess = e as MessageEvent;

                if(mess!=null)
                    Console.WriteLine(mess.Message);
            }
        }
    }
```
