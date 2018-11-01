using DutchTreat.Data.Entities;
using Microsoft.AspNetCore.Hosting;
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

        public DutchSeeder(DutchContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hosting = hostingEnvironment;
        }

        public void Seed()
        {
            //Verificar si existe BD
            _context.Database.EnsureCreated();

            if (!_context.Products.Any())
            {
                //Need to create sample data
                var filePath = Path.Combine(_hosting.ContentRootPath, "Data/art.json");
                var json = File.ReadAllText(filePath);
                var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(json);
                _context.AddRange(products);

                var order = _context.Orders.Where(o => o.Id == 1).FirstOrDefault();

                if(order != null)
                {
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
