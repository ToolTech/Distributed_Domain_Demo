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
using System;
using GizmoSDK.GizmoDistribution;
using GizmoSDK.GizmoBase;


namespace Battlefield
{

    [DistPropertyAutoStore]         // We will reflect our dist property attributes at send event
    [DistPropertyAutoRestore]       // we will reflect our dist property attributes at OnEvent
    class BattlefieldTimeSyncEvent : DistEvent
    {
        // Let the constructor be private or internal so we dont expose this by mistake
        protected BattlefieldTimeSyncEvent(IntPtr nativeReference) : base(nativeReference)
        {
        }

        // a factory design pattern for this class
        public override Reference Create(IntPtr nativeReference)
        {
            return new BattlefieldTimeSyncEvent(nativeReference) as Reference;
        }

        [DistProperty]
        public double SystemTime=0.0;

        [DistProperty]
        public double Time=0.0;

        [DistProperty]
        public bool ReSync=false;

    }
}
