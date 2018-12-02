using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CapstoneIMS.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;

namespace CapstoneIMS.Pages
{
    public class DashboardModel : PageModel
    {
        //Location location = new Location
        //{

        //};

        public IEnumerable<Location> Locations;
        private readonly IMongoClient _mongoclient;

        public DashboardModel(IMongoClient client)
        {
            _mongoclient = client;
            Locations = new List<Location>();
            var collection = _mongoclient.GetDatabase("imsDB").GetCollection<Location>("Locations");
        }

        public void OnGet()
        {
            //Locations = GetLocations();
            

        }
        //**Causing exceptions**
        //private IEnumerable<Location> GetLocations()
        //{
        //    return _mongoclient
        //                    .GetDatabase("imsDB")
        //                    .GetCollection<Location>("Locations")
        //                    .Find(FilterDefinition<Location>.Empty)
        //                    .ToList();
        //}

    }
}