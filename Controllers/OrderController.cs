using System;
using CasestudyAPI.DAL;
using CasestudyAPI.DAL.DAO;
using CasestudyAPI.DAL.DomainClasses;
using CasestudyAPI.APIHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CasestudyAPI.Helpers;
using System.Collections.Generic;
namespace CasestudyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        AppDbContext _ctx;
        public OrderController(AppDbContext context) // injected here
        {
            _ctx = context;
        }
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<string>> Index(OrderHelper helper)
        {
            string retVal = "";
            try
            {
                CustomerDAO uDao = new CustomerDAO(_ctx);
                Customer trayOwner = await uDao.GetByEmail(helper.email);
                OrderDAO tDao = new OrderDAO(_ctx);
                int trayId = await tDao.AddOrder(trayOwner.Id, helper.selections);
                if (trayId > 0)
                {
                    retVal = "Order " + trayId + " saved!";
                }
                else
                {
                    retVal = "Order not saved";
                }
            }
            catch (Exception ex)
            {
                retVal = "Order not saved " + ex.Message;
            }
            return retVal;
        }
        [Route("{email}")]
        public async Task<ActionResult<List<Order>>> List(string email)
        {
            List<Order> orders = new List<Order>();
            CustomerDAO cDao = new CustomerDAO(_ctx);
            Customer orderOwner = await cDao.GetByEmail(email);
            OrderDAO oDao = new OrderDAO(_ctx);
            orders = await oDao.GetAll(orderOwner.Id);
            return orders;
        }
        [Route("{orderid}/{email}")]
        public async Task<ActionResult<List<OrderDetailsHelper>>> GetOrderDetails(int orderid, string email)
        {
            OrderDAO dao = new OrderDAO(_ctx);
            return await dao.GetOrderDetails(orderid, email);
        }
    }
}