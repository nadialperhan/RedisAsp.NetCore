using Microsoft.AspNetCore.Mvc;
using RedisExampleApi.Repository;
using RedisExampleApi.Services;
using RedisExampleCache;
using StackExchange.Redis;

namespace RedisExampleApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _productService.GetAsync());
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _productService.GetByIdAsync(id));
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]Model.Product product)
        {
            return Created(string.Empty, await _productService.CreateAsync(product));
        }
        
    }
}
