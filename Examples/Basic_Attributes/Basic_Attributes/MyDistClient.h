//******************************************************************************
// File			: MyDistClient.h
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
#pragma once

#include "gzDistLibrary.h"

class MyDistClient : public gzDistClientInterface, public gzReference
{
public:

	
	MyDistClient(const gzString& name = "MyDistClient");

	virtual gzVoid onNewObject(gzDistObject* object, gzDistSession* session) override;

	virtual gzVoid onUpdateAttributes(const gzDistNotificationSet& attributes, gzDistObject* object, gzDistSession* session) override;

	virtual gzVoid onNewAttributes(const gzDistNotificationSet& attributes, gzDistObject* object, gzDistSession* session) override;

};

// Create a smartPointer ref type
GZ_DECLARE_REFPTR(MyDistClient);
