namespace SoftwareTokenProvider
{
    public class AuthData
    {
        public string Scheme { get; set; }
        public string Host { get; set; }
        public string Application { get; set; }
        public string User { get; set; }
        public string Issuer { get; set; }
        public string Secret { get; set; }
        public int Digits { get; set; }
    }
}