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
