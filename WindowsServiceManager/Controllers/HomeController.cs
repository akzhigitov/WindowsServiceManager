using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Web.Mvc;
using SimpleImpersonation;

namespace WindowsServiceManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly IList<string> _serviceControllers;
        readonly string _machineName;

        public HomeController()
        {
            _machineName = ConfigurationManager.AppSettings["machineName"];
            _serviceControllers = ConfigurationManager.AppSettings["services"].Split(';');
        }

        public ActionResult Index()
        {
            var serviceViewModels = _serviceControllers.Select(
                serviceName => new ServiceViewModel
                {
                    ServiceName = serviceName,
                    Status = GetStatus(serviceName)
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

        private string GetStatus(string serviceName)
        {
            using (LogonUser())
            {
                var serviceController = new ServiceController(serviceName, _machineName);
                var status = serviceController.Status.ToString();

                return status;
            }
        }

        private static Impersonation LogonUser()
        {
            var domain = ConfigurationManager.AppSettings["domain"];
            var username = ConfigurationManager.AppSettings["username"];
            var password = ConfigurationManager.AppSettings["password"];
            return Impersonation.LogonUser(domain, username, password, LogonType.NewCredentials);
        }
    }

    public class ServiceViewModel
    {
        public string ServiceName { get; set; }
        public string Status { get; set; }
    }
}