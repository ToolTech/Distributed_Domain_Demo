//******************************************************************************
// File			: LicenseManager.cs
// Module		: GizmoBase C#
// Description	: C# Bridge to gzLicenseManager class
// Author		: Anders Modén		
// Product		: GizmoBase 2.10.4
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
// AMO	191015	Created file 	
//
//******************************************************************************

using System.Runtime.InteropServices;
using System;

namespace GizmoSDK
{
    namespace GizmoBase
    {
        public class LicenseManager : Reference
        {
            public LicenseManager() : base(LicenseManager_create()) { }


            #region -------------- Native calls ------------------
        
            [DllImport(Platform.BRIDGE, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            private static extern IntPtr LicenseManager_create();

            #endregion
        }

    }
}

