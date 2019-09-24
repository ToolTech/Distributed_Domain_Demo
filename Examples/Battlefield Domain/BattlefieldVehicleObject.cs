using System;
using GizmoSDK.GizmoDistribution;
using GizmoSDK.GizmoBase;

namespace Battlefield
{
    class BattlefieldVehicleObject : DistObject
    {
        // Let the constructor be private or internal so we dont expose this by mistake
        protected BattlefieldVehicleObject(IntPtr nativeReference) : base(nativeReference)
        {
        }

        // a factory design pattern for this class
        public override Reference Create(IntPtr nativeReference)
        {
            return new BattlefieldVehicleObject(nativeReference) as Reference;
        }
    }
}
