using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapstoneIMS.Domain
{
    public class InventoryCheck
    {
        private readonly MongoDBContext dBContext;
        public Location Location { get; set; }
        public List<Product> Products { get; set; }
        public List<Location> Locations { get; set; }
        public IEnumerable<Inventory> ProblemProducts { get; set; }

        public List<Notification> Notifications { get; set; }
        public InventoryCheck()
        {
            dBContext = new MongoDBContext();
            Locations = dBContext.Locations.Find(m => true).ToList();
            Notifications = dBContext.Notifications.Find(m => true).ToList();
            Products = dBContext.Products.Find(m => true).ToList();
        }

        public void checkInventory(string id)
        {
            //todo find a way to not repeatedly create notifications over and over. 
            var storeId = ObjectId.Parse(id);
            Location = dBContext.Locations.Find(Builders<Location>.Filter.Eq("_id", storeId)).First();
            ProblemProducts = Location.Inventory.Where(p => p.Quantity > 30 || p.Quantity < 3);

            foreach (var prod in ProblemProducts)
            {
                var quantity = prod.Quantity;
                var prodId = prod.ProductID;
                var storeName = Location.StoreName;
                var count = Notifications.Where(p => p.Message.Contains(prodId.ToString()) && p.Message.Contains(quantity.ToString())).Count();

                if (count == 0 && Location.LocationType != "Warehouse")
                {
                    var product = Products.Find(m => m.Id == prod.ProductID);
                    var doc = new Notification(prod.ProductID,product.ProductName, prod.Quantity, Location.StoreName);
                    dBContext.Notifications.InsertOne(doc);
                }

            }
        }
    }
}
