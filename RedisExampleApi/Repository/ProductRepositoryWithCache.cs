using RedisExampleApi.Model;
using RedisExampleCache;
using StackExchange.Redis;
using System.Text.Json;

namespace RedisExampleApi.Repository
{
    public class ProductRepositoryWithCache : IProductRepository
    {
        private readonly IProductRepository _repository;
        private readonly RedisService _redisService;
        private readonly IDatabase _cacheRepo;
        private const string productKey = "productCaches";
        public ProductRepositoryWithCache(IProductRepository repository, RedisService redisService)
        {
            _repository = repository;
            _redisService = redisService;
            _cacheRepo = _redisService.GetDb(0);
        }
        public async Task<Product> CreateAsync(Product product)
        {
            var newproduct=await _repository.CreateAsync(product);
            if (await _cacheRepo.KeyExistsAsync(productKey))
            {
                await _cacheRepo.HashSetAsync(productKey, product.Id, JsonSerializer.Serialize(newproduct));

            }
            return newproduct;
        }

        public async Task<List<Product>> GetAsync()
        {
            if (!await _cacheRepo.KeyExistsAsync(productKey))
            {
                return await LoadToCacheFromDb();
            }
            else
            {
                var products = new List<Product>();
                var cacherepos=await _cacheRepo.HashGetAllAsync(productKey);
                foreach (var item in cacherepos.ToList())
                {
                    var product = JsonSerializer.Deserialize<Product>(item.Value);
                    products.Add(product);
                }
                return products;
            }
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            if (await _cacheRepo.KeyExistsAsync(productKey))
            {
                var product = await _cacheRepo.HashGetAsync(productKey, id);
                return product.HasValue ? JsonSerializer.Deserialize<Product>(product) : null;

            }
            else
            {
                var product =await LoadToCacheFromDb();
                return product.FirstOrDefault(x => x.Id == id);
            }
        }
        private async Task<List<Product>> LoadToCacheFromDb()
        {
            var products = await _repository.GetAsync();

            products.ForEach(product =>
            {
                _cacheRepo.HashSetAsync(productKey, product.Id, JsonSerializer.Serialize(product));
            });
            return products;
        }
    }
}
