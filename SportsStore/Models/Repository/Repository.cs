using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SportsStore.Models.Repository
{
    public class Repository
    {
        private readonly EfDbContext _context = new EfDbContext();
        public IEnumerable<Product> Products
        {
            get { return _context.Products; }
        }

        public IEnumerable<Order> Orders
        {
            get
            {
                return _context.Orders
                    .Include(o => o.OrderLines
                        .Select(ol => ol.Product));
            }
        }

        public void SaveOrder(Order order)
        {
            if (order.OrderId == 0)
            {
                order = _context.Orders.Add(order);
                foreach (OrderLine line in order.OrderLines)
                {
                    _context.Entry(line.Product).State
                        = EntityState.Modified;
                }
            }
            else
            {
                Order dbOrder = _context.Orders.Find(order.OrderId);
                if (dbOrder != null)
                {
                    dbOrder.Name = order.Name;
                    dbOrder.Line1 = order.Line1;
                    dbOrder.Line2 = order.Line2;
                    dbOrder.Line3 = order.Line3;
                    dbOrder.City = order.City;
                    dbOrder.State = order.State;
                    dbOrder.GiftWrap = order.GiftWrap;
                    dbOrder.Dispatched = order.Dispatched;
                }
            }
            _context.SaveChanges();
        }

        public void SaveProduct(Product product)
        {
            if (product.ProductId == 0)
            {
                product = _context.Products.Add(product);
            }
            else
            {
                Product dbProduct = _context.Products.Find(product.ProductId);
                if (dbProduct != null)
                {
                    dbProduct.Name = product.Name;
                    dbProduct.Description = product.Description;
                    dbProduct.Price = product.Price;
                    dbProduct.Category = product.Category;
                }
            }
            _context.SaveChanges();
        }
        public void DeleteProduct(Product product)
        {
            IEnumerable<Order> orders = _context.Orders
                .Include(o => o.OrderLines.Select(ol => ol.Product))
                .Where(o => o.OrderLines.Count(ol => ol.Product
                                                         .ProductId == product.ProductId) > 0).ToArray();
            foreach (Order order in orders)
            {
                _context.Orders.Remove(order);
            }
            _context.Products.Remove(product);
            _context.SaveChanges();
        }
    }
}