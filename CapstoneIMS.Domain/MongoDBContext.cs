using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace CapstoneIMS.Domain
{
    public class MongoDBContext
    {
        private IMongoDatabase _database { get; }

        public MongoDBContext()
        {
            try
            {
                var mongoClient = new MongoClient("mongodb+srv://lukas:martin95@imsdb-5mp2s.azure.mongodb.net/test?retryWrites=true");
                _database = mongoClient.GetDatabase("imsDB");
            }
            catch (Exception ex)
            {
                throw new Exception("Can not access db server.", ex);
            }
        }

        public IMongoCollection<Product> Products
        {
            get
            {
                return _database.GetCollection<Product>("Products");
            }
        }

        public IMongoCollection<Location> Locations
        {
            get
            {
                return _database.GetCollection<Location>("Locations");
            }
        }

        public IMongoCollection<User> Users
        {
            get
            {
                return _database.GetCollection<User>("users");
            }
        }

        public IMongoCollection<Notification> Notifications
        {
            get
            {
                return _database.GetCollection<Notification>("Notifications");
            }
        }

        public IMongoCollection<Shipment> Shipments
        {
            get
            {
                return _database.GetCollection<Shipment>("Shipments");
            }
        }

        public IMongoCollection<Note> Notes
        {
            get
            {
                return _database.GetCollection<Note>("ReceivedNotes");
            }
        }
    }
}
