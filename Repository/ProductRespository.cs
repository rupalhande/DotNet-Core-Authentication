using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JwtAuth.Data;
using JwtAuth.Entities;
using Microsoft.EntityFrameworkCore;

namespace JwtAuth.Repository
{
    public class ProductRespository : IProductRepository
    {
        private readonly AuthContex authContex;
        public ProductRespository(AuthContex authContex)
        {
            this.authContex = authContex;
            
        }
        public async Task<bool> AddProduct(Product product)
        {
            await authContex.products.AddAsync(product);
            return await authContex.SaveChangesAsync()>0?true:false;
        }

        public async Task<bool> DeleteProductById(int productId)
        {
           return (await authContex.products
                        .Where(p=>p.ProductId == productId)
                        .ExecuteDeleteAsync()
                  )>0?true:false;
        }

        public async Task<List<Product>> GetAllProduts()
        {
            return await authContex.products.ToListAsync();
        }
    }
}