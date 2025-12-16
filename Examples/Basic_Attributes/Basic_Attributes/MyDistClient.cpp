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
