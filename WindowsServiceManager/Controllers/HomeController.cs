using System;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Web.Mvc;
using WindowsServiceManager.Models;
using WindowsServiceManager.Providers;
using SimpleImpersonation;

namespace WindowsServiceManager.Controllers
{
    public class HomeController : Controller
    {
        readonly string _machineName;
        readonly string _filter;

        public HomeController()
        {
            _machineName = ConfigurationManager.AppSettings["machineName"];
            _filter = ConfigurationManager.AppSettings["filter"];
        }

        public ActionResult Index()
        {
            var servicesProvider = new ServicesProvider();
            var serviceViewModels = servicesProvider.GetServices(_machineName, _filter).Select(
                service =>
                {
                    return new ServiceViewModel
                    {
                        ServiceName = service.ServiceName,
                        Status = service.Status.ToString(),
                        CanStop = service.CanStop
                    };
                }).ToList();

            return View(serviceViewModels);
        }

        [HttpPost]
        public ActionResult Stop(string serviceName)
        {
            using (LogonUser())
            {
                var serviceController = new ServiceController(serviceName, _machineName);
                serviceController.Stop();
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
            }
            return View("Index");
        }

        [HttpPost]
        public ActionResult Start(string serviceName)
        {
            using (LogonUser())
            {
                var serviceController = new ServiceController(serviceName, _machineName);
                serviceController.Start();
            }

            return View("Index");
        }

        private static Impersonation LogonUser()
        {
            var domain = ConfigurationManager.AppSettings["domain"];
            var username = ConfigurationManager.AppSettings["username"];
            var password = ConfigurationManager.AppSettings["password"];
            return Impersonation.LogonUser(domain, username, password, LogonType.NewCredentials);
        }
    }
}