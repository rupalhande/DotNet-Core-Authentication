using JwtAuth.Dto;
using JwtAuth.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuth.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        public UserController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        [HttpGet]
        [Route("getall")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            ResponseDto responseDto = new ResponseDto();
            responseDto.Data =await this.userRepository.GetAllUsers();
            return Ok(responseDto);
        }
    }
}
