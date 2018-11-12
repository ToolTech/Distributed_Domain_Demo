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

    namespace Distributed_Domain_Demo
    {
      class Program
      {
       

        static void Main(string[] args)
        {
            GizmoSDK.GizmoBase.Platform.Initialize();

            GizmoSDK.GizmoDistribution.Platform.Initialize();

            DistManager manager = DistManager.GetManager(true);

            
            manager.Start(DistRemoteChannel.CreateDefaultSessionChannel(), DistRemoteChannel.CreateDefaultServerChannel());

            DistClient client = new DistClient("MessageClient", manager);

            client.Initialize();

            DistSession session=client.GetSession("MessageSession", true, true);

            client.JoinSession(session);

            client.SubscribeEvents(session);

            client.OnEvent += Client_OnEvent;

            while(true)
            {
                string result = Console.ReadLine();

                if (result == "quit")
                    break;

                DistEvent e = manager.GetEvent();

                e.SetAttributeValue("Message", result);

                client.SendEvent(e, session);
            }


            manager.Shutdown();
        }

        private static void Client_OnEvent(DistClient sender, DistEvent e)
        {
            if(e.GetSource()!=sender.GetClientID().InstanceID)
                Console.WriteLine(e.GetAttributeValue("Message").GetString());
        }
      }
    }
