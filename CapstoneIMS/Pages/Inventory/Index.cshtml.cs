using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CapstoneIMS.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CapstoneIMS.Web.Pages
{
    public class InventoryModel : PageModel
    {
        static readonly MongoDBContext dbContext = new MongoDBContext();
        public List<Product> productList = dbContext.Products.Find(m => true).Sort("{Name: 1}").ToList();


        public void OnGet()
        {
            
        }

        public void OnPostDeleteItem(string id)
        {
            var itemID = ObjectId.Parse(id);
            var filter = Builders<Product>.Filter.Eq("_id", itemID);
            dbContext.Products.DeleteOne(filter);

            var locations = dbContext.Locations.Find(m => true).ToList();

            foreach(var loc in locations)
            {
                if (loc.Inventory.Any(inv => inv.ProductID == itemID))
                {
                    var inv = loc.Inventory.FirstOrDefault(i => i.ProductID == itemID);
                    loc.Inventory.Remove(inv);

                    var locationfilter = Builders<Location>.Filter.Eq("_id", loc.Id);

                    dbContext.Locations.ReplaceOne(locationfilter, loc);
                }
            }
        }
    }
}