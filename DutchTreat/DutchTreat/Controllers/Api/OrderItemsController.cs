using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Controllers.Api
{
    /*
     * Implementamos subcontroller que depende de Orders, 
     * Lo hacemos para manejar los items de una forma más clara
     */
    [Route("api/orders/{orderid}/items")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrderItemsController : ControllerBase
    {
        private IDutchRepository _repository;
        private ILogger<OrderItemsController> _logger;
        private IMapper _mapper;

        public OrderItemsController(IDutchRepository repository, 
                                    ILogger<OrderItemsController> logger,
                                    IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        //http://localhost:8888/api/orders/1/items
        [HttpGet]
        public IActionResult Get(int orderId)
        {
            var order = _repository.GetOrderById(User.Identity.Name, orderId);

            if (order != null)
                return Ok(_mapper.Map<IEnumerable<OrderItem>, IEnumerable<OrderItemVM>>(order.Items));

            return NotFound();
        }

        //http://localhost:8888/api/orders/1/items/1
        [HttpGet("{orderItemId}")]
        public IActionResult Get(int orderId, int orderItemId)
        {
            var order = _repository.GetOrderById(User.Identity.Name, orderId);

            if (order != null)
            {
                var item = order.Items.FirstOrDefault(i => i.Id == orderItemId);
                return Ok(_mapper.Map<OrderItem, OrderItemVM>(item));
            }
            return NotFound();
        }
    }
}