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

namespace Event_Demo
{
    [DynamicTypePropertyAutoRestore]
    [DynamicTypePropertyAutoStore]

    class ComplexType :DynamicTypeContainer
    {
        [DynamicTypeProperty]
        public string SenderID;

        [DynamicTypeProperty]
        public float Speed;

    }

    [DistPropertyAutoStore]         // We will reflect our dist property attributes at send event
    [DistPropertyAutoRestore]       // we will reflect our dist property attributes at OnEvent

    class MessageEvent : DistEvent
    {
        public enum Fault : UInt64
        {
            BZ=0,
        }
        // Let the constructor be private or internal so we dont expose this by mistake
        protected  MessageEvent(IntPtr nativeReference) : base(nativeReference)
        {
        }

        // a factory design pattern for this class
        public override Reference Create(IntPtr nativeReference)
        {
            return new MessageEvent(nativeReference) as Reference;
        }

        [DistProperty]
        public ComplexType [] Compis;

        [DistProperty]
        public string Message;
        
        [DistProperty]
        public double Time;

        [DistProperty]
        public Fault EnumTest;

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
                Array.Resize<ComplexType>(ref e.Compis, 1);

                e.Compis[0] = new ComplexType { SenderID = "Aloha", Speed = 27 };

                e.Message=result;

                e.Time = Time.SystemSeconds;

                e.EnumTest = MessageEvent.Fault.BZ;

                // and send the event on the specific session
                client.SendEvent(e, session);
            }

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
           
            MessageEvent mess = e as MessageEvent;
            
            if(mess!=null)
                Console.WriteLine(mess.Message);
        }
    }
}
