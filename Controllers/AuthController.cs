using AutoMapper;
using JwtAuth.Dto;
using JwtAuth.Entities;
using JwtAuth.Helper;
using JwtAuth.Repository;
using JwtAuth.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuth.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly IJwtHelper helper;
        private readonly IMapper mapper;

        public AuthController(IUserRepository userRepository, IJwtHelper helper,IMapper mapper )
        {
            this.mapper = mapper;
            this.userRepository = userRepository;
            this.helper = helper;
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<ResponseDto>> Login([FromBody] LoginUserReqDto req)
        {
            User? user =await this.userRepository.GetUserByEmail(req.Email);
            ResponseDto res = new ResponseDto();

            if (user == null)
            {
                res.IsSuccessed = false;
                res.Message = "Invalid Credentials!";
                return BadRequest(res);
            }

            if (!BCrypt.Net.BCrypt.Verify(req.Password,user.Password))
            {
                res.IsSuccessed = false;
                res.Message = "Invalid Credentials!";
                return BadRequest(res);
            }
            var RefreshToken = helper.GenerateRefreshToken();

            user.RefreshToken = RefreshToken;
            user.RefreshTokenExpire=DateTime.Now.AddDays(2);

            await userRepository.UpdateUser(user);

            LoginUserResDto userDetail = new LoginUserResDto()
            {
                AccessToken = this.helper.GenerateJwtToken(user),
                RefreshToken=RefreshToken,
                userData=this.mapper.Map<UserDto>(user)
            };

            res.Data= userDetail;

            return Ok(res);
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<ResponseDto>> Register([FromBody] RegisterUserReqDto req)
        {
            User? user = await this.userRepository.GetUserByEmail(req.Email);
            ResponseDto res = new ResponseDto();
            if (user != null)
            {
                res.IsSuccessed = false;
                res.Message = $"User with email {req.Email} already exsist";
                return BadRequest(res);
            }

            User newUser = new User()
            {
                UserName = req.UserName,
                Email = req.Email,
                Address = req.Address,
                Role=UserRoles.USER.ToString(),
                Password = BCrypt.Net.BCrypt.HashPassword(req.Password),
                RefreshToken="",
                RefreshTokenExpire=DateTime.Now.AddDays(2)
            };
            bool result = await this.userRepository.AddUser(newUser);

            if (!result)
            {
                res.IsSuccessed = false;
                res.Message = $"Internal Server error";
                return BadRequest(res);
            }
            res.Message = "User registered successfully";
            return Ok(res);
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<ActionResult<ResponseDto>> RegisterAdmin([FromBody] RegisterUserReqDto req)
        {
            User? user = await this.userRepository.GetUserByEmail(req.Email);
            ResponseDto res = new ResponseDto();
            if (user != null)
            {
                res.IsSuccessed = false;
                res.Message = $"User with email {req.Email} already exsist";
                return BadRequest(res);
            }

            User newUser = new User()
            {
                UserName = req.UserName,
                Email = req.Email,
                Address = req.Address,
                Role=UserRoles.ADMIN.ToString(),
                Password = BCrypt.Net.BCrypt.HashPassword(req.Password)
            };
            bool result = await this.userRepository.AddUser(newUser);

            if (!result)
            {
                res.IsSuccessed = false;
                res.Message = $"Internal Server error";
                return BadRequest(res);
            }
            res.Message = "User registered successfully";
            return Ok(res);
        }
    
        [HttpPost]
        [Route("refresh")]
         public async Task<ActionResult<ResponseDto>> RefreshToken([FromBody] RefreshTokenDto req)
         {
             ResponseDto res = new ResponseDto();
             if(req.RefreshToken == null || req.AccessToken == null){
                res.IsSuccessed=false;
                res.Message = "Invalid Request";
                return BadRequest(res);
             }

             var refreshToken=req.RefreshToken;
             var accessToken=req.AccessToken;

             var principal = helper.GetPrincipalFromExpiredToken(accessToken);
             var email = principal.Identity.Name;

             User? user = await this.userRepository.GetUserByEmail(email);

             if(user==null || user.RefreshToken!=refreshToken || user.RefreshTokenExpire<=DateTime.Now){
                res.IsSuccessed=false;
                res.Message = "Invalid Request";
                return BadRequest(res);
             }
            
            refreshToken=helper.GenerateRefreshToken();
            accessToken=helper.GenerateJwtToken(user);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpire=DateTime.Now.AddDays(2);

            await userRepository.UpdateUser(user);
            
            RefreshTokenDto data = new RefreshTokenDto(){
                AccessToken=accessToken,
                RefreshToken=refreshToken
            };

            res.Data=data;

            return Ok(res);
         }
   
        [HttpPost,Authorize]
        [Route("revoke")]
         public async Task<ActionResult<ResponseDto>> Revoke()
         {
            ResponseDto res = new ResponseDto();
            var email = User.Identity.Name;
             User? user = await this.userRepository.GetUserByEmail(email);
             if(user==null){
                res.IsSuccessed=false;
                res.Message = "Invalid Request";
                return BadRequest(res);
             }
             user.RefreshToken = "";
             await this.userRepository.UpdateUser(user);
             
             res.Message = "User Successfully logout";
             return Ok(res);
         }
    }
}
