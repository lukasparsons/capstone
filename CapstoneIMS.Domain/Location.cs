using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CapstoneIMS.Domain
{
    public class Location
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("LocationId")]
        public int LocationId { get; set; }
        [BsonElement("StoreName")]
        public string StoreName { get; set; }
        [BsonElement("Address")]
        public Address Address { get; set; }
        [BsonElement("Inventory")]
        public IDictionary<ObjectId, int> Inventory { get; set; }
    }
}
