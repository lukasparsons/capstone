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
    public class ShippingModel : PageModel
    {
        private readonly MongoDBContext dbContext;
        public Shipment Shipment { get; set; }
        public List<Shipment> Shipments { get; set; }
        public List<Inventory> ShipmentItems { get; set; }
        public List<Location> Locations { get; set; }
        public List<Product> Products { get; set; }
        public ObjectId ItemID { get; set; }
        public const string Message = nameof(Message);
        public const string Error = nameof(Error);
        public const string MessageReceiving = nameof(MessageReceiving);
        public const string ErrorReceiving = nameof(ErrorReceiving);

        public ShippingModel()
        {
            dbContext = new MongoDBContext();
            Locations = dbContext.Locations.Find(m => true).ToList();
            Products = dbContext.Products.Find(m => true).ToList();
            Shipments = dbContext.Shipments.Find(m => m.IsReceived == false).ToList();
            Shipment = new Shipment();
        }

        public void OnGet()
        {

        }

        public ActionResult OnPostAddToManifest()
        {
            var shippingIDstring = Request.Form["shippingID"];
            var itemIDString = Request.Form["itemID"];
            //ensure itemID from form is in correct format and return error if it is not.
            if (!ObjectId.TryParse(itemIDString, out ObjectId ItemID))
            {
                TempData[Error] = "Item ID was not a valid ID";
                return RedirectToPage();
            }
            

            var quantity = Convert.ToInt32(Request.Form["quantity"]);
            var toStore = Request.Form["ToLocation"];
            var fromStore = Request.Form["FromLocation"];

            //if the shipping idstring is empty, create a new manifest or shipment.
            if (string.IsNullOrEmpty(shippingIDstring))
            {
                //create the new document to be inserted.
                var doc = new Shipment
                {
                    FromLocation = fromStore,
                    ToLocation = toStore,
                    IsReceived = false,
                    IsShipped = false,
                    Items = new List<Inventory>()
                };
                //insert the document
                dbContext.Shipments.InsertOne(doc);

                //create the page that will be the inventory
                var page = new Inventory(ItemID, quantity)
                {
                    ProductID = ItemID,
                    Quantity = quantity
                };
                //find the document we just inserted
                var filter = Builders<Shipment>.Filter.Eq("_id", doc.Id);
                //add our item to the "items"
                var update = Builders<Shipment>.Update.Push("Items", page);
                //update
                dbContext.Shipments.FindOneAndUpdate(filter, update);


                TempData[Message] = "Shipment Created! shipment ID is " + doc.Id.ToString() + ". Please use this to add items to shipment";
                return RedirectToPage();
            }
            //if the Shipping ID Box is not empty:
            else
            {
                //check to make sure it is in the correct format
                if (!ObjectId.TryParse(shippingIDstring, out ObjectId ShippingId))
                {
                    //if not, redirect
                    TempData[Error] = "Shipping ID is not in correct format or does not exist.";
                    return RedirectToPage();

                }
                //find our shipment from shipment ID
                var document = dbContext.Shipments
                    .Find(s => s.Id == ShippingId)
                    .Single();
                //check if the item we are adding to the shipment is already there
                var count = document.Items.Where(i => i.ProductID == ItemID).Count();
                //if it is:
                if (count > 0)
                {
                    // find the page of the Items where the ID = the input itemID
                    var page = document.Items.Single(i => i.ProductID == ItemID);
                    //call updateshipment method which adds the inventory
                    if (page.Quantity < 0)
                    {
                        page.Quantity = 0;
                    }
                    else if (page.Quantity == 0)
                    {
                        page.Quantity = quantity;
                    }
                    else
                    {
                        page.Quantity = page.Quantity + quantity;
                    }
                    UpdateShipment(document, page, quantity, ShippingId);
                    TempData[Message] = "Items added to shipment successfully.";
                    return RedirectToPage();
                }
                else
                {
                    //create a new page in "Items" and add it
                    var page = new Inventory(ItemID, quantity)
                    {
                        ProductID = ItemID,
                        Quantity = quantity
                    };
                    UpdateShipment(document, page, quantity, ShippingId);
                    TempData[Message] = "Items added to shipment successfully.";
                    return RedirectToPage();
                }

                
            }

            

        }

        public ActionResult OnPostCreateShipment()
        {
            var shipmentIdstring = Request.Form["shipmentId2"];
            
            

            if (string.IsNullOrEmpty(Request.Form["shipmentId2"]))
            {
                TempData[Error] = "Shipment ID Box was Blank. What shipment should be shipped?";
                return RedirectToPage();
            }

            if(!ObjectId.TryParse(Request.Form["shipmentId2"], out ObjectId ShippingId))
            {
                return RedirectToPage();
            }

            var doc = dbContext.Shipments.Find(Builders<Shipment>.Filter.Eq("_id", ShippingId)).Single();
            var fromStore = doc.FromLocation;
            var toStore = doc.ToLocation;

            CreateShipment(doc, fromStore, toStore);
            return RedirectToPage();


        }

        public ActionResult OnPostReceiveShipment()
        {
            bool asExpected;
            if (Request.Form["asExpected"] == "Yes")
            {
                asExpected = true;
            }
            else
            {
                asExpected = false;
            }
            if (!ObjectId.TryParse(Request.Form["shipmentID"], out ObjectId shipmentId))
            {
                TempData[ErrorReceiving] = "Shipment ID is not valid";
                return RedirectToPage();
            }
            if(dbContext.Shipments.Find(Builders<Shipment>.Filter.Eq("_id", shipmentId)).CountDocuments() < 1)
            {
                TempData[ErrorReceiving] = "Shipment ID does not exist for a current shipment.";
                return RedirectToPage();
            }

            var shipment = dbContext.Shipments.Find(Builders<Shipment>.Filter.Eq("_id", shipmentId)).Single();
            var manifest = shipment.Items.Where(m => m.Quantity > 0);
            var updateLocation = dbContext.Locations.Find(Builders<Location>.Filter.Eq("StoreName", shipment.ToLocation)).Single();
            

            foreach (var item in manifest)
            {
                var itemId = item.ProductID;
                var quantity = item.Quantity;
                var count = updateLocation.Inventory.Where(p => p.ProductID == itemId).Count();
                if (count > 0)
                {
                    var page = updateLocation.Inventory.Single(p => p.ProductID == itemId);

                    page.Quantity = page.Quantity + quantity;
                    dbContext.Locations.ReplaceOne(d => d.Id == updateLocation.Id, updateLocation);
                }
                else
                {
                    var page = new Inventory(itemId, quantity)
                    {
                        ProductID = itemId,
                        Quantity = quantity

                    };
                    var filter = Builders<Location>.Filter.Eq("_id", updateLocation.Id);
                    var update = Builders<Location>.Update.Push("Inventory", page);

                    dbContext.Locations.FindOneAndUpdate(filter, update);
                }
            }

            var doc = new Note
            {
                AsExpected = asExpected,
                ShipmentId = shipmentId,
                Notes = Request.Form["Notes"]
            };
            dbContext.Notes.InsertOne(doc);
            TempData[MessageReceiving] = "Shipment Received. Note saved to database.";

            shipment.IsReceived = true;
            dbContext.Shipments.ReplaceOne(d => d.Id == shipment.Id, shipment);
            return RedirectToPage();


        }


        public void UpdateShipment(Shipment document, Inventory page, int quantity, ObjectId shippingId)
        {
            var filter = Builders<Shipment>.Filter.Eq("_id", shippingId);
            var update = Builders<Shipment>.Update.Push("Items", page);
            dbContext.Shipments.FindOneAndUpdate(filter, update);
        }

        public void CreateShipment(Shipment doc, string fromStore, string toStore)
        {
            doc.IsShipped = true;

            var fromlocation = dbContext.Locations.Find(Builders<Location>.Filter.Eq("StoreName", fromStore)).Single();

            var itemList = doc.Items.Where(m => m.Quantity > 0).ToList();
            //remove stock from fromstore
            foreach (var item in itemList)
            {
                var itemId = item.ProductID;
                var quantityChange = item.Quantity;
                var itemToUpdate = fromlocation.Inventory.Where(m => m.ProductID == itemId).First();
                itemToUpdate.Quantity = itemToUpdate.Quantity - quantityChange;
                dbContext.Locations.ReplaceOne(d => d.Id == fromlocation.Id, fromlocation);
                //var filter = Builders<Location>.Filter.Eq("_id", fromlocation.Id);
                //var update = Builders<Location>.Update.Push("Inventory", itemToUpdate);
                //dbContext.Locations.FindOneAndUpdate(filter, update);
            }
            var docFilter = Builders<Shipment>.Filter.Eq("_id", doc.Id);
            dbContext.Shipments.ReplaceOne(docFilter, doc);
            TempData[Message] = "Shipment Created Successfully. Items have been removed from inventory at " + fromStore + " please send shipment to " + toStore + " ASAP. Shipment ID: " + doc.Id;
        }

    }
}