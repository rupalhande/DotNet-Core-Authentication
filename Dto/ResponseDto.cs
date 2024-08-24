namespace JwtAuth.Dto
{
    public class ResponseDto
    {
        public string Message { get; set; } = "Success";
        public bool IsSuccessed { get; set; } = true;
        public object? Data { get; set; } = null;

    }
}
