namespace WindowsServiceManager.Models
{
    public class ServiceViewModel
    {
        public string ServiceName { get; set; }

        public string Status { get; set; }

        public bool CanStop { get; set; }
    }
}