using JwtAuth.Dto;
using JwtAuth.Entities;
using JwtAuth.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuth.Controllers
{
    [ApiController]
    [Route("api/product")]
    
    public class ProductController : ControllerBase
    {
        public IProductRepository ProductRepository { get; }
        public ProductController(IProductRepository productRepository)
        {
            this.ProductRepository = productRepository;

        }

        [HttpGet]
        [Authorize(Roles ="ADMIN,USER")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            ResponseDto response = new ResponseDto();
            response.Data = await ProductRepository.GetAllProduts();
            return response;
        }

        [HttpPost]
        [Authorize(Roles ="ADMIN")]
        public async Task<ActionResult<ResponseDto>> Add([FromBody] ProductDto req)
        {
            ResponseDto response = new ResponseDto();
            Product product = new Product()
            {
                ProductName = req.ProductName,
                Price = req.Price
            };

            bool result = await ProductRepository.AddProduct(product);

            if (!result)
            {
                response.IsSuccessed = false;
                response.Message = "Internal Error";
                return BadRequest(response);
            }

            response.Message = "Product Added successfully";

            return Ok(response);
        }

        [HttpDelete("{ProductId}")]
        [Authorize(Roles ="ADMIN")]
        public async Task<ActionResult<ResponseDto>> Delete(int ProductId)
        {
            ResponseDto response = new ResponseDto();
            if (!await ProductRepository.DeleteProductById(ProductId))
            {
                response.IsSuccessed = false;
                response.Message = "Wrong Product Id";
            }
            return response;
        }
    }
}