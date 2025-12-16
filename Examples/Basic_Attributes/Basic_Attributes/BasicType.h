#pragma once
#include "gzDistLibrary.h"

class BasicType : public gzDistObject
{
public:

	// Declare interface (used for subscription etc) 
	GZ_DECLARE_TYPE_INTERFACE;

	// Construction and destruction 
	BasicType(const gzString& name);
	virtual ~BasicType();

	// Clone interface 
	virtual gzReference* clone() const override;

	// Register factory object
	static gzVoid registerFactory(gzDistManager* manager);
};

GZ_DECLARE_REFPTR(BasicType);

