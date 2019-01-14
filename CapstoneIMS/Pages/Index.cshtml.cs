using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CapstoneIMS.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;

namespace CapstoneIMS.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }

        private readonly MongoDBContext dbContext;
      
        public IndexModel()
        {
            dbContext = new MongoDBContext();
            
        }

        public void OnGet()
        {
            var locationList = dbContext.Locations.Find(m => true).ToList();
            foreach (var location in locationList)
            {
                if (location.LocationType != "Warehouse")
                {
                    var instance = new InventoryCheck();
                    instance.checkInventory(location.Id.ToString());
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(Email == null || Password == null)
            {
                return Page();
            }
            var usersCol = dbContext.Users;

            //TODO: Create Users collection, with username and password for each user

            //TODO: compare credentials of users in database to credentials provided by form
            //if there is a user that matches the credentials
            var count = usersCol.Find(d => d.Email == Email).CountDocuments();
            if (count <= 0)
            {
                return Page();
            }
            else
            {
                if (usersCol.Find(d => d.Password == Password) == null)
                {
                    return Page();
                }
                else
                {
                    var user = usersCol.Find(d => d.Email == Email).First();
                    if (user.Password == Password)
                    {
                        const string issuer = "lukasparsons.com";
                        var claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.Name, Email, ClaimValueTypes.String, issuer),
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                        return RedirectToPage("Dashboard");
                    }
                    else
                    {
                        return Page();
                    }
                }
            }
            
        }
    }
}
