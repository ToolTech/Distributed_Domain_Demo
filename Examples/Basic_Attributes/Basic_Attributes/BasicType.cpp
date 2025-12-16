//******************************************************************************
// File			: BasicType.cpp
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
#include "BasicType.h"


// Declare interface (used for subscription etc) 
GZ_DECLARE_TYPE_CHILD(gzDistObject, BasicType, "BasicType");

// Construction and destruction 
BasicType::BasicType(const gzString& name) :gzDistObject(name)
{

}

BasicType::~BasicType()
{

}

// Clone interface 
gzReference* BasicType::clone() const
{
	return new BasicType(*this);
}

// Factory
gzVoid BasicType::registerFactory(gzDistManager* manager)
{
	manager->registerFactory(new BasicType(GZ_EMPTY_STRING));
}
