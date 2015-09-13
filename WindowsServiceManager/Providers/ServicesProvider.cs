using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;

namespace WindowsServiceManager.Providers
{
    public class ServicesProvider
    {
        public List<ServiceController> GetServices(string machineName, string nameContains)
        {
            return ServiceController.GetServices(machineName).Where(controller => controller.ServiceName.Contains(nameContains)).ToList();
        } 
    }
}