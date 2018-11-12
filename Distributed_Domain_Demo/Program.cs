using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GizmoSDK.GizmoBase;
using GizmoSDK.GizmoDistribution;

namespace Distributed_Domain_Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            DistManager manager = DistManager.GetManager(true);
        }
    }
}
