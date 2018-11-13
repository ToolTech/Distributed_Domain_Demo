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
