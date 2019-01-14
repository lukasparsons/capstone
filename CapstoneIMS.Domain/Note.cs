

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CapstoneIMS.Domain
{
    public class Note
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("ShipmentID")]
        public ObjectId ShipmentId { get; set; }
        [BsonElement]
        public bool AsExpected { get; set; }
        [BsonElement]
        public string Notes { get; set; }

        //public Note(ObjectId shipmentID, bool asExpected, string notes)
        //{
        //    ShipmentId = shipmentID;
        //    AsExpected = asExpected;
        //    Notes = notes;
        //}
    }
}
