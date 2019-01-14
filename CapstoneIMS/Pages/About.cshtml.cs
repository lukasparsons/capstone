using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CapstoneIMS.Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CapstoneIMS.Pages
{
    public class AboutModel : PageModel
    {
        private readonly MongoDBContext dbContext;

        public AboutModel()
        {
            dbContext = new MongoDBContext();

        }

        public async Task OnPostAsync()
        {

            var doc = new User
            {
                Email = Request.Form["email"],
                FirstName = Request.Form["firstName"],
                LastName = Request.Form["lastName"],
                Password = Request.Form["password"]
            };

            await dbContext.Users.InsertOneAsync(doc);
            Response.Redirect("Dashboard");
        }
    }
}
