using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Adhesive.AppInfoCenter;
using Adhesive.Test.Mvc.Models;

namespace Adhesive.Test.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private static string[] stringfilterpool = new string[] { "Trading", "Consignment", "Escort", "Admin" };
        private static Random rnd = new Random();
        private ISuckService suckservice;

        public HomeController(ISuckService service)
        {
            this.suckservice = service;
        }

        public ViewResult Fluent()
        {
            var model = new PersonEditModel
            {
                Person = new Person
                {
                    Gender = "M",
                    Name = "Jeremy",
                    Roles = new List<int> { 1, 2 },
                    Father = new Parent { Name = "Jim" },
                    Mother = new Parent { Name = "Joan" }
                },
                Genders = new Dictionary<string, string> { { "M", "Male" }, { "F", "Female" } },
                Roles = new List<Role> { new Role(0, "Administrator"), new Role(1, "Developer"), new Role(2, "User") },
                Companies = new SelectList(new List<Company>
                {
                    new Company { Id = Guid.NewGuid(), Name = "Acme Inc"},
                    new Company { Id = Guid.NewGuid(), Name = "MicorSoft" }
                }, "Id", "Name"),
                Shifts = new[] { "Day", "Night", "Graveyard" }
            };
            return View(model);
        }

        public ActionResult Index()
        {
            Stopwatch sw = Stopwatch.StartNew();

            var extraInfo = new ExtraInfo
            {
                DisplayItems = new Dictionary<string, string>()
                {
                    { "DisplayItem1", "DisplayItem1" },
                    { "DisplayItem2", "DisplayItem2" }
                },
                DropDownListFilterItem1 = stringfilterpool[rnd.Next(4)],
                DropDownListFilterItem2 = stringfilterpool[rnd.Next(4)],
                CheckBoxListFilterItem1 = stringfilterpool[rnd.Next(4)],
                CheckBoxListFilterItem2 = stringfilterpool[rnd.Next(4)],
            };

            AppInfoCenterService.LoggingService.Error("测试日志", extraInfo);

            new Exception("错误" + Guid.NewGuid().ToString(), new Exception("内部错误")).Handle(extraInfo: extraInfo);

            AppInfoCenterService.PerformanceService.StartPerformanceMeasure("性能测试1");
            aa();
            AppInfoCenterService.PerformanceService.SetPerformanceMeasurePoint("性能测试1", "性能测试1aa");
            cc();
            AppInfoCenterService.PerformanceService.SetPerformanceMeasurePoint("性能测试1", "性能测试1cc");
            dd();
            AppInfoCenterService.PerformanceService.SetPerformanceMeasurePoint("性能测试1", "性能测试1dd");

            //try
            //{
            //    ViewBag.Message = WcfService.WcfServiceLocator.GetService<IFuckService>().YouWannaFuckWho("you", 200);
            //}
            //catch (Exception ex)
            //{
            //    ex.Handle();
            //    ViewBag.Message = ex.Message;
            //}

            //ViewBag.Time = sw.ElapsedMilliseconds;

            //throw new Exception("assadad");
            //ViewBag.Message = suckservice.Suck();
            return View();
        }

        public void aa()
        {
            for (int i = 0; i < 2; i++)
            {
                bb();
                AppInfoCenterService.PerformanceService.SetPerformanceMeasurePoint("性能测试1", "性能测试1bb");
            }
        }

        public void bb()
        {
            Thread.Sleep(100);
        }

        public void cc()
        {
            Thread.Sleep(200);
        }

        public void dd()
        {
            for (long i = 0; i < 100000; i++)
            {
                Random r = new Random();
                double j = r.NextDouble();
            }
        }

        public ActionResult About()
        {
            AppInfoCenterService.PerformanceService.StartPerformanceMeasure("性能测试2");
            aa();
            AppInfoCenterService.PerformanceService.SetPerformanceMeasurePoint("性能测试2", "性能测试2aa");
            cc();
            AppInfoCenterService.PerformanceService.SetPerformanceMeasurePoint("性能测试2", "性能测试2cc");

            Parallel.For(0, 1000, i =>
            {
                AppInfoCenterService.LoggingService.Error("测试日志" + Guid.NewGuid().ToString());
            });

            return View();
        }
    }
}
