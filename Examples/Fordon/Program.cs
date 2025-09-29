//****************************************************************************** 
// File         : Program.cs
// Module       : Distribution Examples
// Description  : Fordon-demo med typed objekt + events i session "fordon"
// Author       : Anders Modén
//******************************************************************************

using System;
using System.Collections.Generic;
using System.Threading;
using GizmoSDK.GizmoBase;
using GizmoSDK.GizmoDistribution;

namespace FordonDemo
{
    // --- Typed objekt ---
    public class VehicleObject : DistObject
    {
        protected VehicleObject(IntPtr nativeRef) : base(nativeRef) { }
        public override Reference Create(IntPtr nativeRef) => new VehicleObject(nativeRef);
    }

    // --- Typed event ---
    [DistPropertyAutoStore]
    [DistPropertyAutoRestore]
    public class VehicleEvent : DistEvent
    {
        protected VehicleEvent(IntPtr nativeRef) : base(nativeRef) { }
        public override Reference Create(IntPtr nativeRef) => new VehicleEvent(nativeRef);

        [DistProperty] public string EventType { get; set; }
        [DistProperty] public double Timestamp { get; set; }
        [DistProperty] public string Source { get; set; }
    }

    // --- Extension för typed SubscribeObjects ---
    static class DistClientExtensions
    {
        public static void SubscribeObjects<T>(this DistClient c, DistSession s, bool subscribe = true)
            where T : DistObject
            => c.SubscribeObjects(s, typeof(T).Name, subscribe);
    }

    class Program
    {
        static readonly LicenseManager lic = new LicenseManager();

        static void Main(string[] args)
        {
            // Verbos logg
            Message.OnMessage += (sender, lvl, msg) => Console.WriteLine(msg);
            Message.SetMessageLevel(MessageLevel.DEBUG);

            // Initiera plattformar
            GizmoSDK.GizmoBase.Platform.Initialize();
            GizmoSDK.GizmoDistribution.Platform.Initialize();

            // Manager + registrera typed klasser
            var manager = DistManager.GetManager(create: true);
            manager.RegisterObject<VehicleObject>();
            manager.RegisterEvent<VehicleEvent>();

            // Transport
            manager.Start(
                DistRemoteChannel.CreateDefaultSessionChannel(),
                DistRemoteChannel.CreateDefaultServerChannel());

            // Klient och session
            var client = new DistClient("VehicleClient", manager);
            client.Initialize();

            var session = client.GetSession("fordon", create: true, global: true);
            client.JoinSession(session);

            // Prenumerationer (objekt + events)
            client.SubscribeObjects<VehicleObject>(session, true);
            client.SubscribeEvents(session);

            // Debounce med update-counter (valfritt)
            var lastCounter = new Dictionary<string, ulong>();

            client.OnNewObject += (snd, o, s) =>
            {
                snd.SubscribeAttributes(o, true);
                Console.WriteLine("[NEW] " + o.ToJSON());
            };

            client.OnNewAttributes += (snd, notif, o, s) =>
            {
                snd.SubscribeAttributeValue(notif, o, true);
            };

            client.OnUpdateAttributes += (snd, notif, o, s) =>
            {
                var name = o.GetName().ToString();
                var ctr = o.GetUpdateCounter();
                if (!lastCounter.TryGetValue(name, out var prev) || ctr != prev)
                {
                    lastCounter[name] = ctr;
                    Console.WriteLine("[UPDATE] " + o.ToJSON());
                }
            };

            client.OnEvent += (snd, e) =>
            {
                // Visa även egna events om du vill: ta bort if-raden nedan
                if (e.GetSource() == snd.GetClientID().InstanceID) return;
                Console.WriteLine("[EVENT] " + e.ToJSON());
            };

            // --- Skapa cykel ---
            var bicycle = manager.GetObject<VehicleObject>("Bicycle1");
            client.AddObject(bicycle, session);
            bicycle = client.WaitForObject("Bicycle1", session) as VehicleObject;

            var txn = new DistTransaction();
            txn.NewTransaction();
            txn.SetAttributeValue("Type", "Bicycle");
            txn.SetAttributeValue("Color", "Red");
            txn.SetAttributeValue("Gears", 7);
            txn.SetAttributeValue("HasBasket", 1);
            txn.SetAttributeValue("LastUpdated", Time.SystemSeconds);
            client.UpdateObject(txn, bicycle);

            // --- Skapa bil ---
            var car = manager.GetObject<VehicleObject>("Car1");
            client.AddObject(car, session);
            car = client.WaitForObject("Car1", session) as VehicleObject;

            txn.NewTransaction();
            txn.SetAttributeValue("Type", "Car");
            txn.SetAttributeValue("Color", "Blue");
            txn.SetAttributeValue("Doors", 4);
            txn.SetAttributeValue("Engine", "Diesel");
            txn.SetAttributeValue("Horsepower", 120.0);
            txn.SetAttributeValue("LastUpdated", Time.SystemSeconds);
            client.UpdateObject(txn, car);

            // --- Periodiska uppdateringar + events ---
            var exit = new ManualResetEvent(false);
            Console.CancelKeyPress += (_, e) => { e.Cancel = true; exit.Set(); };

            Console.WriteLine("Kör. Tryck [Enter] eller Ctrl+C för att avsluta.");

            var updater = new System.Threading.Thread(() =>
            {
                int i = 0;
                while (!exit.WaitOne(0))
                {
                    // Uppdatera Bicycle
                    txn.NewTransaction();
                    txn.SetAttributeValue("LastUpdated", Time.SystemSeconds);
                    txn.SetAttributeValue("OdometerKm", i);
                    client.UpdateObject(txn, bicycle);

                    // Uppdatera Car
                    txn.NewTransaction();
                    txn.SetAttributeValue("LastUpdated", Time.SystemSeconds);
                    txn.SetAttributeValue("OdometerKm", i * 2);
                    client.UpdateObject(txn, car);

                    // Skicka event
                    var evt = manager.GetEvent<VehicleEvent>();
                    evt.EventType = "Heartbeat";
                    evt.Timestamp = Time.SystemSeconds;
                    evt.Source = "VehicleClient";
                    client.SendEvent(evt, session);

                    i++;
                    System.Threading.Thread.Sleep(2000);
                }
            })
            { IsBackground = true };

            updater.Start();

            if (Console.IsInputRedirected)
                exit.WaitOne();
            else
            {
                Console.ReadLine();
                exit.Set();
            }

            updater.Join();

            // Avsluta
            client.ResignSession(session);
            client.Uninitialize();
            manager.Shutdown();
        }
    }
}
