using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JwtAuth.Entities;

namespace JwtAuth.Repository
{
    public interface IProductRepository
    {
       Task<List<Product>> GetAllProduts();
       Task<bool> AddProduct(Product product);
       Task<bool> DeleteProductById(int productId);
    }
}