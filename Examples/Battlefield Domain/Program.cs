//******************************************************************************
// File			: Program.cs
// Module		: Battlefield Examples
// Description	: Basic examples for Saab Battlefield Domain Model (BFD)
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
using System;



namespace Battlefield
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

            // Register some factories
            manager.RegisterObject<BattlefieldSoldierObject>();
            manager.RegisterObject<BattlefieldVehicleObject>();
            manager.RegisterObject<BattlefieldTimeObject>();

            // Events
            manager.RegisterEvent<BattlefieldTimeSyncEvent>();




            // Start the manager with settting for transport protocols
            var MCastAddress = "234.2.3.100";

            //var networkInterface = "10.23.24.50"; // Update this to your interface ip. E.g. 192.168.100.100.
            ushort serverPort = 6667;
            ushort sessionPort = 6668;

            var serverChannel = DistRemoteChannel.CreateChannel(5000, DistTransportType.MULTICAST, MCastAddress, serverPort /*,networkInterface*/);
            var sessionChannel = DistRemoteChannel.CreateChannel(5000, DistTransportType.MULTICAST, MCastAddress, sessionPort /*,networkInterface*/);

            manager.Start(sessionChannel, serverChannel);

            //If we want to attach the DistMonitor debugger
            manager.EnableDebug(true);
            
            // Client set up. You are a client that sends and receives information
            DistClient client = new DistClient("Our Test Client", manager);

            // We need to tell the client how to initialize
            client.Initialize();

            // Now we can get a session. A kind of a meeting room that is used to exchange various "topics"
            DistSession session = client.GetSession("Battlefield", true, true);

            // Joint that session and subribe all events
            client.JoinSession(session);

            // Create a delegete
            client.OnNewObject += Client_OnNewObject;
            client.OnNewAttributes += Client_OnNewAttributes;
            client.OnUpdateAttributes += Client_OnUpdateAttributes;

            client.OnEvent += Client_OnEvent;

            // Subscribe all standard objects
            client.SubscribeObjects(session,null,true);

            // Subscribe all standard events
            client.SubscribeEvents(session);


            BattlefieldSoldierObject o = manager.GetObject<BattlefieldSoldierObject>("Soldier");

            client.AddObject(o, session);

            o = client.WaitForObject(o.GetName(), session) as BattlefieldSoldierObject;

            DistTransaction update = new DistTransaction();

            for (int i=0;i<100;i++)
            {
                update.NewTransaction();

                update.SetAttributeValue("Updater", client.GetClientID().InstanceID.ToString());

                client.UpdateObject(update, o);

                System.Threading.Thread.Sleep(1000);
            }

            
            client.ResignSession(session);
   
            client.Uninitialize();

            // Some kind of graceful shutdown
            manager.Shutdown();


            // GC and platform uninit is managed by the system automatically
        }

     
        private static void Client_OnNewAttributes(DistClient sender, DistNotificationSet notif, DistObject o, DistSession session)
        {
            sender.SubscribeAttributeValue(notif, o, true);
        }

        private static void Client_OnUpdateAttributes(DistClient sender, DistNotificationSet notif, DistObject o, DistSession session)
        {
            foreach(DistAttribute attr in notif)
            {
                System.Console.WriteLine(attr.ToString());
            }
        }

        private static void Client_OnNewObject(DistClient sender, DistObject o, DistSession session)
        {
            sender.SubscribeAttributes(o, true);
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
