using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CapstoneIMS.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CapstoneIMS.Pages
{
    public class DashboardModel : PageModel
    {
        static readonly MongoDBContext dbContext = new MongoDBContext();
        public List<Location> locationList = dbContext.Locations.Find(m => true).ToList();
        public List<Notification> notifications = dbContext.Notifications.Find(m => true).ToList();
        public Location Location { get; set; }
        public const string MessageKey = nameof(MessageKey);
        public const string ErrorMessage = nameof(ErrorMessage);

        public void OnGet()
        {
            foreach (var location in locationList)
            {
                if (location.StoreName != "Warehouse")
                {
                    var instance = new InventoryCheck();
                    instance.checkInventory(location.Id.ToString());
                }
            }
        }

        public async Task<IActionResult> OnPostAuditAsync()
        {
            var storeInput = Request.Form["storeBeingAudited"];
            var storeFromInput = Request.Form["auditedFrom"];
            var rawId = Request.Form["id"].ToString().Trim();
            int quantity;
            if (!ObjectId.TryParse(rawId, out ObjectId id))
            {
                TempData[ErrorMessage] = "ID is not in correct format or was empty.";
                return RedirectToPage();
            }
            if (string.IsNullOrEmpty(Request.Form["quantity"]))
            {
                TempData[ErrorMessage] = "Quantity cannot be Empty.";
                return RedirectToPage();
            }
            quantity = Convert.ToInt32(Request.Form["quantity"]);

            if (quantity < 0)
            {
                TempData[ErrorMessage] = "Quantity cannot be a negative number!";
                return RedirectToPage();
            }

            //remove stock from store
            if (storeFromInput != "none")
            {
                var doc = dbContext.Locations
                    .Find(d => d.StoreName == storeFromInput)
                    .Single();

                var currentQuantity = doc.Inventory.Where(p => p.ProductID == id).First().Quantity;
                if (currentQuantity < quantity)
                {
                    TempData[ErrorMessage] = "Insufficient stock at " + storeFromInput;
                    return RedirectToPage();
                }
                else
                {
                    var fromPage = doc.Inventory.Single(p => p.ProductID == id);

                    fromPage.Quantity = fromPage.Quantity - quantity;
                    await dbContext.Locations.ReplaceOneAsync(dbContext => dbContext.Id == doc.Id, doc);
                    TempData[MessageKey] = "Audit was Successful!";
                }
            }

            //Add Stock to store
            var document = dbContext.Locations
                .Find(d => d.StoreName == storeInput)
                .Single();
            var count = document.Inventory.Where(p => p.ProductID == id).Count();
            if (count > 0)
            {
                var page = document.Inventory.Single(p => p.ProductID == id);

                page.Quantity = page.Quantity + quantity;
                await dbContext.Locations.ReplaceOneAsync(d => d.Id == document.Id, document);
                TempData[MessageKey] = "Audit was Successful!";
            }
            else
            {
                var page = new Inventory(id, quantity)
                {
                    ProductID = id,
                    Quantity = quantity

                };
                var filter = Builders<Location>.Filter.Eq("StoreName", storeInput);
                var update = Builders<Location>.Update.Push("Inventory", page);

                await dbContext.Locations.FindOneAndUpdateAsync(filter, update);
                TempData[MessageKey] = "Audit was Successful!";
            }

            return RedirectToPage();


        }

    }
}