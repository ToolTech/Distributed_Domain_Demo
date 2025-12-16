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
