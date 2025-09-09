﻿//******************************************************************************
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
using GizmoSDK.GizmoBase;
using GizmoSDK.GizmoDistribution;

namespace Start_Manager_Example
{
    class Program
    {
        // Start a license manager internally
        static readonly LicenseManager _lic = new LicenseManager();

        static void Main(string[] args)
        {
            // Initialize platforms for various used SDKs
            GizmoSDK.GizmoBase.Platform.Initialize();
            GizmoSDK.GizmoDistribution.Platform.Initialize();

            // Create a manager. The manager controls it all
            DistManager manager = DistManager.GetManager(true);

            // Start the manager with settting for transport protocols
            //manager.Start(DistRemoteChannel.CreateDefaultSessionChannel(true,DistTransportType.MULTICAST,"127.0.0.1"), DistRemoteChannel.CreateDefaultServerChannel(true,DistTransportType.MULTICAST,"127.0.0.1"));

            // Just plain. Above opens up on 127.0.0.1 (localhost)
            manager.Start(DistRemoteChannel.CreateDefaultSessionChannel(), DistRemoteChannel.CreateDefaultServerChannel());

            //If we want to attach the DistMonitor debugger
            manager.EnableDebug(true);

            System.Threading.Thread.Sleep(100000);

        }
    }       
}
