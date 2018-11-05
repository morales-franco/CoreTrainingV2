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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Controllers.Api
{
    /*
     * For test
     * 1) Generate TOken
     * http://localhost:8888/account/CreateToken
     * Body:{
	 * "username" : "admin",
	 * "password" : "Pa$$Word123456"
     * }
     * 
     * Then copy the TOKEN_GENERATED.
     * 
     * 2) Use the token for request
     * http://localhost:8888/api/orders
     * Headers:
     * Key: Authorization
     * Value: Bearer TOKEN_GENERATED
     * 
     * Note: If you don't use the access token for get resources the operation will fail (401 unauthorized).
     */

    [Route("api/[controller]")]
    [ApiController]
    //Especificamos que este Api controller NO va a usar auth por cookies (Realmente inseguro para Api's) sino que va a usar JWT
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersController : ControllerBase
    {
        private IDutchRepository _repository;
        private ILogger<OrdersController> _logger;
        private IMapper _mapper;
        private UserManager<StoreUser> _userManager;

        public OrdersController(IDutchRepository repository, 
            ILogger<OrdersController> logger, 
            IMapper mapper,
            UserManager<StoreUser> userManager)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Order>> Get(bool includeItems = true)
        {
            try
            {
                var username = User.Identity.Name;

                var orders = _repository.GetAllOrdersByUser(includeItems, username);
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
                var order = _repository.GetOrderById(User.Identity.Name,id);

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
        public async Task<ActionResult<Order>> Post([FromBody] OrderVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Order entity = _mapper.Map<OrderVM, Order>(model);

                    if (entity.OrderDate == DateTime.MinValue)
                        entity.OrderDate = DateTime.Now;

                    //User es una representación de claims del user--> NO el storeUser de la BD

                    var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
                    entity.User = currentUser;

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