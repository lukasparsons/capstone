using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CapstoneIMS.Domain
{
    [BsonIgnoreExtraElements]
    public class Shipment
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement]
        public string FromLocation { get; set; }
        [BsonElement]
        public string ToLocation { get; set; }
        [BsonElement]
        public List<Inventory> Items { get; set; }
        [BsonElement]
        public bool IsShipped { get; set; }
        [BsonElement]
        public bool IsReceived { get; set; }
        [BsonElement]
        public DateTime DateTime { get; set; }

        public Shipment()
        {
            DateTime = DateTime.Now;
        }
    }
}
