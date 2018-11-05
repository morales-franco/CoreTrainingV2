using DutchTreat.Data.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DutchTreat.Data
{
    public class DutchSeeder
    {
        private DutchContext _context;
        private IHostingEnvironment _hosting;
        private UserManager<StoreUser> _userManager;

        public DutchSeeder(DutchContext context, 
                           IHostingEnvironment hostingEnvironment,
                           UserManager<StoreUser> userManager)
        {
            _context = context;
            _hosting = hostingEnvironment;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            //Verificar si existe BD
            _context.Database.EnsureCreated();

            //Verificar existe admin user

            StoreUser user = await _userManager.FindByEmailAsync("admin@ironskysolutions.com");

            if(user == null)
            {
                user = new StoreUser()
                {
                    FirstName = "Admin",
                    LastName = "Iron",
                    Email = "admin@ironskysolutions.com",
                    UserName = "admin"
                };

                var result = await _userManager.CreateAsync(user, "Pa$$Word123456");

                if (result != IdentityResult.Success)
                    throw new InvalidOperationException("Could not create new user in seeder");
                
            }


            if (!_context.Products.Any())
            {
                //Need to create sample data
                var filePath = Path.Combine(_hosting.ContentRootPath, "Data/art.json");
                var json = File.ReadAllText(filePath);
                var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(json);
                _context.Products.AddRange(products);

                var order = _context.Orders.Where(o => o.Id == 1).FirstOrDefault();

                if(order != null)
                {
                    order.User = user;
                    order.Items = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Product = products.First(),
                            Quantity = 5,
                            UnitPrice = products.First().Price
                        }
                    };
                }

                _context.SaveChanges();
            }

        }
    }
}
