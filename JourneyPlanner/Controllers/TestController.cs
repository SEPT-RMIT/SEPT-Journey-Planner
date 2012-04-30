using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JourneyPlanner.Models.TimeTableModels;

namespace JourneyPlanner.Controllers
{
    public class TestController : Controller
    {
        // Services this controller utilises are defined here
        private ITimeTableService _timeTableService;

        // Constructors
        public TestController()
            : this(new TimeTableService()) { }

        public TestController(ITimeTableService TimeTableService)
        {
            this._timeTableService = TimeTableService;
        }


        //
        // GET: /Test/

        public ActionResult Index()
        {
            ViewBag.BFS_Result = _timeTableService.CreateGraph();
            return View();
        }

    }
}
