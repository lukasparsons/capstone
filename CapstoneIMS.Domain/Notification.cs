using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CapstoneIMS.Domain
{
    [BsonIgnoreExtraElements]
    public class Notification
    {
        //todo figure out datetime
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement]
        public string Subject { get; set; }
        [BsonElement]
        public string Message { get; set; }

        public List<Notification> Notifications { get; set; }

        public Notification(ObjectId id, string itemName, int quantity, string storeName)
        {
            var itemid = id.ToString();
            if (quantity > 30)
            {
                Subject = "Item " + itemName + " has too much inventory at " + storeName + ".";
                Message = "The quantity of the item " + itemName + " with id of " + itemid + " is too high. (" + quantity + ")." +
                    " Please create a shipment from " + storeName + " to Warehouse with excess stock soon.";
            }
            else if (quantity < 3)
            {
                Subject = "Item " + itemName + " is running out at " + storeName + ".";
                Message = "The quantity of the item " + itemName + " with id of " + itemid + " is running low (" + quantity + ")." +
                    " Please create a shipment to " + storeName + " with additional stock soon.";
            }
        }
    }
}
