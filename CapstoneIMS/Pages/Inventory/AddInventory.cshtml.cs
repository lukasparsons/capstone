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
        private readonly MongoDBContext dbContext;

        public AddInventoryModel()
        {
            dbContext = new MongoDBContext();

        }

        public async Task OnPostAsync()
        {

            var doc = new Product
            {
                ProductName = Request.Form["itemName"],
                ProductType = Request.Form["productType"],
                DefaultPrice = decimal.Parse(Request.Form["msrp"])
            };

            await dbContext.Products.InsertOneAsync(doc);
            Response.Redirect("Index");
        }
        
    }
}