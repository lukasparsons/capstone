using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CapstoneIMS.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;

namespace CapstoneIMS.Web.Pages
{
    public class AddInventoryModel : PageModel
    {
        

        public void OnGet()
        {
            
        }

        public async Task OnPostAsync()
        {
            var client = new MongoClient();
            var db = client.GetDatabase("imsDB");
            var collection = db.GetCollection<Product>("Products");

            var doc = new Product
            {
                ProductName = Request.Form["itemName"],
                ItemNumber = Request.Form["sku"],
                ProductType = Request.Form["productType"],
                DefaultPrice = decimal.Parse(Request.Form["msrp"])
            };

            await collection.InsertOneAsync(doc);
            Response.Redirect("Index");
        }
        
    }
}