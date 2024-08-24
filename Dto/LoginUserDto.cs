namespace JwtAuth.Dto
{
    public class LoginUserReqDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginUserResDto
    {
        public string AccessToken { get; set; }

         public string RefreshToken { get; set; }

         public UserDto userData { get; set; }
    }
}
