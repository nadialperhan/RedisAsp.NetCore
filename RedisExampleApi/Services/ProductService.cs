using RedisExampleApi.Model;
using RedisExampleApi.Repository;

namespace RedisExampleApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productrepo;

        public ProductService(IProductRepository productrepo)
        {
            _productrepo = productrepo;
        }

        public async Task<Product> CreateAsync(Product product)
        {
           var por =await _productrepo.CreateAsync(product);
            return por;
        }

        public async Task<List<Product>> GetAsync()
        {
            return await _productrepo.GetAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _productrepo.GetByIdAsync(id);
        }
    }
}
