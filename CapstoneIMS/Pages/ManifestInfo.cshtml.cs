using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CapstoneIMS.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CapstoneIMS.Web.Pages
{
    public class ManifestInfoModel : PageModel
    {
        private readonly MongoDBContext dbContext;
        public Shipment Shipment { get; set; }
        public List<Inventory> ItemList { get; set; }
        public List<Product> Products { get; set; }


        public ManifestInfoModel()
        {
            dbContext = new MongoDBContext();
            Shipment = new Shipment();

            Products = dbContext.Products.Find(m => true).ToList();
        }

        [Route("{id}")]
        public void OnGet(string id)
        {
            Shipment = dbContext.Shipments.Find(Builders<Shipment>.Filter.Eq("_id", ObjectId.Parse(id))).Single();
            ItemList = Shipment.Items.Where(m => m.Quantity > 0).ToList();
        }
    }
}