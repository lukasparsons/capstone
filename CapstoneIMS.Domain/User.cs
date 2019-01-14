using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CapstoneIMS.Domain
{
    public class User   
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("email")]
        public string Email { get; set; }
        [BsonElement("password")]
        public string Password { get; set; }
        [BsonElement("firstName")]
        public string FirstName { get; set; }
        [BsonElement("lastName")]
        public string LastName { get; set; }

    }
}
