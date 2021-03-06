﻿using DutchTreat.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DutchTreat.Data
{
    public class DutchRepository : IDutchRepository
    {
        private DutchContext _context;

        public DutchRepository(DutchContext context)
        {
            _context = context;
        }

        public void AddEntity(Order model)
        {
            _context.Orders.Add(model);
        }

        public IEnumerable<Order> GetAllOrders(bool includeItems)
        {
            if (includeItems)
                return _context.Orders
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product) //Equivalent at: Include("Items.Product")
                    .OrderBy(o => o.OrderDate)
                    .ToList();
            else
                return _context.Orders
                .OrderBy(o => o.OrderDate)
                .ToList();
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _context.Products
                .OrderBy(p => p.Title)
                .ToList();
        }

        public IEnumerable<Product> GetAllProductsByCategory(string category)
        {
            return _context.Products
                .Where(p => p.Category == category)
                .OrderBy(p => p.Title)
                .ToList();
        }

        public Order GetOrderById(string username, int id)
        {
            return _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.Id == id && o.User.UserName == username)
                .FirstOrDefault();
        }

        public bool SaveAll()
        {
            return _context.SaveChanges() > 0;
        }

        public IEnumerable<Order> GetAllOrdersByUser(bool includeItems, string username)
        {
            if (includeItems)
                return _context.Orders
                    .Where(o => o.User.UserName == username)
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product) //Equivalent at: Include("Items.Product")
                    .OrderBy(o => o.OrderDate)
                    .ToList();
            else
                return _context.Orders
                    .Where(o => o.User.UserName == username)
                    .OrderBy(o => o.OrderDate)
                    .ToList();
        }
    }
}
