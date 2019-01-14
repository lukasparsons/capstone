using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CapstoneIMS.Domain
{
    
    public class Inventory
    {
        public Inventory(ObjectId id, int quantity)
        {
            ProductID = id;
            Quantity = quantity;
        }

        [BsonElement("ProductID")]
        public ObjectId ProductID { get; set; }
        [BsonElement("Quantity")]
        public int Quantity { get; set; }
    }
}
