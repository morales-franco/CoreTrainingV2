using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private IDutchRepository _repository;
        private ILogger<OrdersController> _logger;
        private IMapper _mapper;

        public OrdersController(IDutchRepository repository, ILogger<OrdersController> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Order>> Get(bool includeItems = true)
        {
            try
            {
                var orders = _repository.GetAllOrders(includeItems);
                return Ok(_mapper.Map<IEnumerable<Order>, IEnumerable<OrderVM>>(orders));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get orders: {ex}");
                return BadRequest("Failed to get orders");
            }
        }

        [HttpGet("{id:int}")]
        public ActionResult<OrderVM> Get(int id)
        {
            try
            {
                var order = _repository.GetOrderById(id);

                if (order == null)
                    return NotFound();

                return Ok(_mapper.Map<Order, OrderVM>(order));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get order: {ex}");
                return BadRequest("Failed to get order");
            }
        }

        [HttpPost]
        public ActionResult<Order> Post([FromBody] OrderVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Order entity = _mapper.Map<OrderVM, Order>(model);

                    if (entity.OrderDate == DateTime.MinValue)
                        entity.OrderDate = DateTime.Now;

                    _repository.AddEntity(entity);
                    if (_repository.SaveAll())
                    {
                        return Created($"api/orders/{model.OrderId}", _mapper.Map<Order,OrderVM>(entity));
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to save a new order: {ex}");
            }

            return BadRequest("Failed to save a new order");
        }
    }
}