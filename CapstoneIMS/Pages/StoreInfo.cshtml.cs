using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CapstoneIMS.Domain;
using CapstoneIMS.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CapstoneIMS.Pages
{
    public class StoreInfoModel : PageModel
    {
        private readonly MongoDBContext dbContext;
        public Location Location { get; set; }
        public List<Product> Products { get; set; }
        public List<InventoryItemViewModel> StoreInventory { get; set; }
        [BindProperty(SupportsGet = true)]
        public string itemIDString { get; set; }
        public decimal total { get; set; }
        public const string Feedback = nameof(Feedback);
        public const string Success = nameof(Success);

        public StoreInfoModel()
        {
            dbContext = new MongoDBContext();

            StoreInventory = new List<InventoryItemViewModel>();
            Location = new Location();
            Products = dbContext.Products.Find(m => true).ToList();
            
        }


        [Route("{id}")]
        public void OnGet(string id)
        {
            // Go ahead and load all the products in memory
            var allProducts = dbContext.Products.Find(m => true).ToList();

            // Get only the location that we want
            var locationfilter = Builders<Location>.Filter.Eq("_id", ObjectId.Parse(id));
            Location = dbContext.Locations.Find(locationfilter).FirstOrDefault();
            // Get a list of just the product ids from the inventory for the store
            var productIds = Location.Inventory.Select(item => item.ProductID);

            // Filter all the products into a new list with only the products that are in our inventory
            StoreInventory = allProducts
                .Where(prod => productIds.Contains(prod.Id))
                .Select(prod => new InventoryItemViewModel
                {
                    Id = prod.Id.ToString(),
                    ItemNumber = prod.ItemNumber,
                    ProductName = prod.ProductName,
                    ProductType = prod.ProductType,
                    DefaultPrice = prod.DefaultPrice,
                    Quantity = Location.Inventory.FirstOrDefault(inv => inv.ProductID == prod.Id)?.Quantity ?? 0
                })
                    .ToList();

        }

        public IActionResult OnPostAsync(string id)
        {
            var itemNameIn = Request.Form["itemName"].ToString().Trim();
            var updateQuantity = int.Parse(Request.Form["Quantity"].ToString().Trim());
            var allProducts = dbContext.Products.Find(m => true).ToList();
            var productNames = Products.Select(item => item.ProductName).ToList();
            if (string.IsNullOrWhiteSpace(Request.Form["Quantity"]))
            {
                TempData[Feedback] = "Quantity cannot be blank!";
                return RedirectToPage("StoreInfo", id);
            }
            else
            {
                ObjectId itemID;
                if (string.IsNullOrWhiteSpace(Request.Form["itemID"]))
                {
                    if (!dbContext.Products.Find(m => m.ProductName == itemNameIn).Any())
                    {

                        TempData[Feedback] = "No products found! Please check your spelling of the Item Name.";
                        return RedirectToPage("StoreInfo", id);
                    }

                    var productbyName = dbContext.Products.Find(m => m.ProductName == itemNameIn).First();
                    itemID = productbyName.Id;

                }
                else if(ObjectId.TryParse(Request.Form["itemID"].ToString().Trim(), out ObjectId objectId))
                {
                    itemID = objectId;
                }
                else
                {
                    TempData[Feedback] = "Item ID was invalid. Please try again.";
                    return RedirectToPage("StoreInfo", id);
                }
                var locationfilter = Builders<Location>.Filter.Eq("_id", ObjectId.Parse(id));
                Location = dbContext.Locations.Find(locationfilter).FirstOrDefault();




                if (!dbContext.Products.Find(Builders<Product>.Filter.Eq("_id", itemID)).Any())
                {
                    TempData[Feedback] = "No product was found with that item ID. Please Check your ID and try again.";
                    return RedirectToPage("StoreInfo", id);
                }
                var document = dbContext.Locations
                    .Find(d => d.Id == ObjectId.Parse(id))
                    .Single();
                var count = document.Inventory.Where(p => p.ProductID == itemID).Count();
                if (count > 0)
                {
                    var page = document.Inventory.Single(p => p.ProductID == itemID);

                    page.Quantity = page.Quantity + updateQuantity;
                    dbContext.Locations.ReplaceOneAsync(d => d.Id == document.Id, document);
                    TempData[Success] = "Added " + updateQuantity + " items.";
                }
                else
                {
                    var page = new Inventory(itemID, updateQuantity)
                    {
                        ProductID = itemID,
                        Quantity = updateQuantity

                    };
                    var filter = locationfilter;
                    var update = Builders<Location>.Update.Push("Inventory", page);
                    dbContext.Locations.FindOneAndUpdateAsync(filter, update);
                    TempData[Success] = "Created new stock of " + itemID + " at " + Location.StoreName + ".";
                }

                return RedirectToPage("StoreInfo", id);

            }
        }

    }
}