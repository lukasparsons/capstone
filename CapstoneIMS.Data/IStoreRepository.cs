using CapstoneIMS.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace CapstoneIMS.Data
{
    public interface IStoreRepository
    {
        ICollection<Store> GetAll();
        Store Get(int id);
        void Add(Store store);
    }
}
