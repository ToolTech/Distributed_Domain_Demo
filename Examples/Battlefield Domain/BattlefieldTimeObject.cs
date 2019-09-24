using System;
using GizmoSDK.GizmoDistribution;
using GizmoSDK.GizmoBase;


namespace Battlefield
{
    class BattlefieldTimeObject : DistObject
    {
        // Let the constructor be private or internal so we dont expose this by mistake
        protected BattlefieldTimeObject(IntPtr nativeReference) : base(nativeReference)
        {
        }

        // a factory design pattern for this class
        public override Reference Create(IntPtr nativeReference)
        {
            return new BattlefieldTimeObject(nativeReference) as Reference;
        }
    }
}
