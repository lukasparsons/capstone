using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CapstoneIMS.Domain
{
    [BsonIgnoreExtraElements]
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
        [BsonElement("Type")]
        public string LocationType { get; set; }
        [BsonElement("Inventory")]
        public List<Inventory> Inventory { get; set; }
        
    }
}
