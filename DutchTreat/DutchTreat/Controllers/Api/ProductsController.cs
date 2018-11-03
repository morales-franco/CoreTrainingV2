using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Controllers.Api
{
    /*
     * Usamos caracteristicas de core 2.1 para documentar api's
     * [ApiController], [Produces("application/json")], [ProducesResponseType(200)]
     * y también lo que retornan los action methods ActionResult<IEnumerable<Product>> (esto es más descriptivo que decir
     * que solo se retorna un ActionResult)
     * Esto hace que la documentación de la API sea más facil inclusive si usamos Swagger.
     * Para utilziación de esto necesitamos heredar de ControllerBase
     */

    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private IDutchRepository _repository;
        private ILogger<ProductsController> _logger;

        public ProductsController(IDutchRepository repository, ILogger<ProductsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult<IEnumerable<Product>> Get()
        {
            /*
             * Al heredar de controller base NO es necesario el Ok(_repository.GetAllProducts())
             * deberia bastar el return _repository.GetAllProducts();
             * pero esto es una limitación de c# solo se pueden devolver clase implicitas (No interfaces):
             * public static implicit operator ActionResult<TValue>(TValue value);
             * 
             * Entonces 
             * 1ra opción
             * ActionResult<IEnumerable<Product>> Get()
             * Funciona con Ok(_repository.GetAllProducts())
             * 
             * 2da opción (No usamos interface):
             * ActionResult<List<Product>> Get()
             * return _repository.GetAllProducts().ToList();
             * 
             * Si utilizamos clase implicita esto andaria, el problema es la interface para la definición de ActionResult
             * ActionResult<Product> Get()
             * return _repository.Get(1); //Devuelve un Product
             */
            try
            {
                return Ok(_repository.GetAllProducts());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get products: {ex}");
                return BadRequest("Failed to get products");
            }
        }
    }
}