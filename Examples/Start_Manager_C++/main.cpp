//******************************************************************************
// File			: main.cpp
// Module		: Distribution Examples
// Description	: Basic examples for C++ distribution
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
// AMO	251215	Created file 	
//
//******************************************************************************
#include "gzDistLibrary.h"

// An instance of the license manager that will load the gz_license_server.dat file
class LicenseManager : public gzLicenseManager
{
public:

	LicenseManager()
	{
		// Run and wait for manager
		run(TRUE);
	}

} lic_manager;

int main(int argc, char* argv[])
{
	// Create a manager. The manager controls it all
	gzDistManagerPtr manager = gzDistManager::getManager(TRUE);

	manager->start();

	//! Enable debug. manager must be started
	manager->enableDebug(TRUE);

	// --------------------------
	while (!gz_kbhit())
		gzYield();


	manager->shutDown();

	return 0;
}