using CasestudyAPI.APIHelpers;
using CasestudyAPI.DAL.DomainClasses;
using CasestudyAPI.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CasestudyAPI.DAL.DAO
{
    public class OrderDAO
    {
        private AppDbContext _db;
        public OrderDAO(AppDbContext ctx)
        {
            _db = ctx;
        }
        public async Task<int> AddOrder(int userid, OrderSelectionHelper[] selections)
        {
            int orderId = -1;
            using (_db)
            {
                // we need a transaction as multiple entities involved
                using (var _trans = await _db.Database.BeginTransactionAsync())
                {
                    try
                    {
                        Order order = new Order();
                        order.UserId = userid;
                        order.OrderDate = System.DateTime.Now;
                        order.OrderAmount = 0;
                        // calculate the totals and then add the tray row to the table
                        foreach (OrderSelectionHelper selection in selections)
                        {
                            order.OrderAmount += selection.Qty * (decimal) selection.item.MSRP;
                        }
                        await _db.Orders.AddAsync(order);
                        await _db.SaveChangesAsync();
                        // then add each item to the trayitems table
                        foreach (OrderSelectionHelper selection in selections)
                        {
                            OrderLineItem tItem = new OrderLineItem();
                            Product product = selection.item;
                            if(selection.Qty <= selection.item.QtyOnHand)
                            {
                                tItem.QtyOrdered = selection.Qty;
                                tItem.QtySold = selection.Qty;
                                product.QtyOnHand -= selection.Qty;
                                tItem.QtyBackOrdered = 0;
                                tItem.ProductId = selection.item.Id;
                            }
                            else 
                            {
                                tItem.QtyOrdered = selection.Qty;
                                tItem.QtySold = selection.item.QtyOnHand;
                                product.QtyOnHand = 0;
                                product.QtyOnBackOrder += selection.Qty - selection.item.QtyOnHand;
                                tItem.QtyBackOrdered = selection.Qty - selection.item.QtyOnHand;
                                
                            }
                          
                            tItem.OrderId = order.Id;
                            tItem.SellingPrice = (decimal)product.MSRP;
                            tItem.ProductId = product.Id;
                      
                            await _db.OrderLineItems.AddAsync(tItem);
                            _db.Products.Update(product);
                            await _db.SaveChangesAsync();
                        }
                        // test trans by uncommenting out these 3 lines
                        //int x = 1;
                        //int y = 0;
                        //x = x / y; 
                        await _trans.CommitAsync();
                        orderId = order.Id;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        await _trans.RollbackAsync();
                    }
                }
            }
            return orderId;
        }
        public async Task<List<Order>> GetAll(int id)
        {
            return await _db.Orders.Where(order => order.UserId == id).ToListAsync<Order>();
        }
        public async Task<List<OrderDetailsHelper>> GetOrderDetails(int oid, string email)
        {
            Customer customer = _db.Customers.FirstOrDefault(customer => customer.Email == email);
            List<OrderDetailsHelper> allDetails = new List<OrderDetailsHelper>();
            // LINQ way of doing INNER JOINS
            var results = from t in _db.Orders
                          join ti in _db.OrderLineItems on t.Id equals ti.OrderId
                          join mi in _db.Products on ti.ProductId equals mi.Id
                          where (t.UserId == customer.Id && t.Id == oid)
                          select new OrderDetailsHelper
                          {
                              OrderId = t.Id,
                              SubTotal = t.OrderAmount,
                              UserId = customer.Id,
                              QtyOrdered = ti.QtyOrdered,
                              QtySold = ti.QtySold,
                              QtyB = ti.QtyBackOrdered,
                              Extend = ti.SellingPrice * ti.QtySold,
                              Description = mi.Description,
                              ProductId = ti.ProductId,
                              Qty = ti.QtyOrdered,
                              DateCreated = t.OrderDate.ToString("yyyy/MM/dd - hh:mm tt")
                          };
            allDetails = await results.ToListAsync<OrderDetailsHelper>();
            return allDetails;
        }

    }
}