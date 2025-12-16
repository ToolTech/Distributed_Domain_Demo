//******************************************************************************
// File			: main.cpp
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
// AMO	251215	Created file 	
//
//******************************************************************************

#include "MyDistClient.h"
#include "BasicType.h"

#include "gzDistRemoteLibrary.h"

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

	
	//gzMessage::setMessageLevel(GZ_MESSAGE_DEBUG /*| GZ_MESSAGE_API_INTERNAL*/);

	// Create a manager. The manager controls it all
	gzDistManagerPtr manager = gzDistManager::getManager(TRUE);

	// Register a factory object
	BasicType::registerFactory(manager);

	// Start the manager and use default channels. 
	manager->start(gzDistCreateDefaultSessionChannel(FALSE,GZ_DIST_MULTICAST,gzHostAddress("127.0.0.1")), gzDistCreateDefaultServerChannel(FALSE,GZ_DIST_MULTICAST, gzHostAddress("127.0.0.1")));
	//manager->start(gzDistCreateDefaultSessionChannel(), gzDistCreateDefaultServerChannel());


	manager->enableDebug(TRUE);

	// Initalize my client
	MyDistClientPtr client = new MyDistClient;
	client->initialize();

	// Create a global session where all chat messages will be sent. 
	gzDistSessionPtr session = manager->getSession("test", TRUE, TRUE);


	client->joinSession(session);

	client->subscribeObjects(BasicType::getClassType(), session, TRUE);



	BasicTypePtr object = new BasicType(gzString::formatString("Pelle:%s",gzGUID::generateGUID().asString()));

	object->setAttributeValue("Identity Of Client", client->getID().asString());

	client->addObject(object.get(), session);

	object = gzDynamic_Cast<BasicType>(client->waitForObject(object->getName(),session));

	try
	{
		while (!gz_kbhit())
		{
			gzSleep(5000);

			gzDistTransactionPtr trans = new gzDistTransaction();

			trans->setAttributeValue("Update", gzTime::now().asString());

			client->updateObject(trans, object.get());
		}

	}
	catch (gzBaseError& e)
	{
		e.reportError();
	}



	// Shut down all

	client->uninitialize();
	client = nullptr;

	manager->shutDown();

}