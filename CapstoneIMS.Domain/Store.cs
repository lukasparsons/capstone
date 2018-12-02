using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CapstoneIMS.Domain
{
    public class Store
    {
        public Store()
        {
            Inventory = new Dictionary<ObjectId, int>();
            ProductCatalog = new List<Product>();
        }

        public int Id { get; set; }
        public Address Address { get; set; }
        public Dictionary<ObjectId, int> Inventory { get; private set; }
        public ICollection<Product> ProductCatalog { get; private set; }

        public void PrintStoreInfo()
        {
            
        }

        public void AddStock(Product product)
        {
            if (!ProductCatalog.Any(prod => prod.Id == product.Id))
            {
                ProductCatalog.Add(product);
            }
            if (!Inventory.Keys.Contains(product.Id))
            {
                Inventory.Add(product.Id, 1);
            }
            else
            {
                Inventory[product.Id]++;
            }
        }

        public void RemoveStock(Product product)
        {
            // TODO: Check if value is already 0, dont want to go below zero.
            // TODO: Check that product exists in the inventory anyways.
            Inventory[product.Id]--;
        }

        public Product GetProduct(ObjectId id)
        {
            return ProductCatalog.FirstOrDefault(pr => pr.Id == id);
        }

        //public bool GetIdGreaterThan10(IPerson person) => person.Id > 10;

        //public void PrintWorkers()
        //{
        //    foreach (var worker in Workers)
        //    {
        //        Console.WriteLine(worker.FullName);
        //    }

        //}
    }
}
