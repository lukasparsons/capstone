using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CapstoneIMS.Domain;
using MongoDB.Driver;
using MongoDB.Bson;

namespace CapstoneIMS.Web.Pages
{
    public class NotificationsModel : PageModel
    {
        private readonly MongoDBContext dBContext;
        public Location Location { get; set; }

        public List<Location> Locations { get; set; }
        public IEnumerable<Inventory> ProblemProducts { get; set; }

        public List<Notification> Notifications { get; set; }
        public NotificationsModel()
        {
            dBContext = new MongoDBContext();
            Locations = dBContext.Locations.Find(m => true).ToList();
            Notifications = dBContext.Notifications.Find(m => true).ToList();

        }

        public void OnGet()
        {
            
        }

        public void OnPostDelete(string idString)
        {
            var id = ObjectId.Parse(idString);
            var filter = Builders<Notification>.Filter.Eq("_id", id);
            dBContext.Notifications.DeleteOne(filter);
            OnGet();
            RedirectToPage();
        }

        
    }
}