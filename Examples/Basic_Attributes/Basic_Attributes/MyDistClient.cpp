//******************************************************************************
// File			: MyDistClient.cpp
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
#include "MyDistClient.h"

MyDistClient::MyDistClient(const gzString& name) :gzDistClientInterface(name)
{

}

gzVoid MyDistClient::onNewObject(gzDistObject* object, gzDistSession* session)
{
	GZMESSAGE(GZ_MESSAGE_NOTICE, "New object : %s", object->asJSON());
	
	subscribeAttributes(object, TRUE);
}

gzVoid MyDistClient::onUpdateAttributes(const gzDistNotificationSet& attributes, gzDistObject* object, gzDistSession* session)
{
	GZMESSAGE(GZ_MESSAGE_NOTICE, "Updated object : %s", object->asJSON());
}

gzVoid MyDistClient::onNewAttributes(const gzDistNotificationSet& attributes, gzDistObject* object, gzDistSession* session)
{
	subscribeAttributeValue(attributes, object, TRUE);
}
