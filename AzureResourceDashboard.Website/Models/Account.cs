namespace AzureResourceDashboard.Website.Models
{
    public class Account
    {
        public static readonly Account Anonymous = new Account { IsAuthenticated = false };

        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }
    }
}