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
