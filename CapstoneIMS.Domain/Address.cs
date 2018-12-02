using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CapstoneIMS.Domain
{
    public class Address
    {
        public Address(string streetAddress, string city, string state, string zip)
        {
            StreetAddress = streetAddress;
            City = city;
            State = state;
            Zip = zip;
        }
        [BsonElement("StreetAddress")]
        public string StreetAddress { get; }
        [BsonElement("City")]
        public string City { get; }
        [BsonElement("State")]
        public string State { get; }
        [BsonElement("Zip")]
        public string Zip { get; }

        public override string ToString()
        {
            return $"{StreetAddress}, {City}, {State} {Zip}";
        }

    }
}
