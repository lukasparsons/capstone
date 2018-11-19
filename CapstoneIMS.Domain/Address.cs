using System;
using System.Collections.Generic;
using System.Text;

namespace CapstoneIMS.Domain
{
    class Address
    {
        public Address(string streetAddress, string city, string state, string zip)
        {
            StreetAddress = streetAddress;
            City = city;
            State = state;
            Zip = zip;
        }

        public string StreetAddress { get; }
        public string City { get; }
        public string State { get; }
        public string Zip { get; }

        public override string ToString()
        {
            return $"{StreetAddress}, {City}, {State} {Zip}";
        }

    }
}
