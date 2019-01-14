using CapstoneIMS.Domain;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CapstoneIMS.Web.ViewModels
{
    public class InventoryItemViewModel
    {
        public string Id { get; set; }
        public string ItemNumber { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public decimal DefaultPrice { get; set; }
        public int Quantity { get; set; }

        public static implicit operator InventoryItemViewModel(Product prod)
        {
            return new InventoryItemViewModel
            {
                Id = prod.Id.ToString(),
                ProductName = prod.ProductName,
                ProductType = prod.ProductType,
                DefaultPrice = prod.DefaultPrice

            };
        }

        public static implicit operator Product(InventoryItemViewModel vm)
        {
            return new Product
            {
                Id = ObjectId.Parse(vm.Id),
                DefaultPrice = vm.DefaultPrice,
                ProductName = vm.ProductName,
                ProductType = vm.ProductType,

            };
        }
    }
}
