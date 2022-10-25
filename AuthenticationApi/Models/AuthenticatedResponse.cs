namespace AuthenticationApi.Models
{
    public class AuthenticatedResponse
    {
        public string Token { get; set; }
        public bool ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
        public string RefreshToken { get; set; }

    }
}
