namespace PhiThanh.Core
{
    public class AppSettings
    {
        public static AppSettings Instance { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string TokenSecretKey { get; set; }
        public string BaseUrl { get; set; }
        public Persistence Persistence { get; set; }
        public List<RootAccount> RootAccounts { get; set; } = [];
    }

    public class RootAccount
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string DisplayName { get; set; }
    }

    public class Persistence
    {
        public string? CoreDataContext { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Build { get; set; }
    }
}
