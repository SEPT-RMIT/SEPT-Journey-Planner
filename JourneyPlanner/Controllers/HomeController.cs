using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using JourneyPlanner.Models;
using JourneyPlanner.Models.TimeTableModels;

namespace JourneyPlanner.Controllers
{
    public class HomeController : Controller
    {
        // Services this controller utilises are defined here
        private ITimeTableService _timeTableService;

        // Constructors
        public HomeController()
            : this(new TimeTableService()) { }

        public HomeController(ITimeTableService TimeTableService)
        {
            this._timeTableService = TimeTableService;
        }

        /// <summary>
        /// GET: /Home/Index
        /// Returns the Index of the page. There is some data access that 
        /// collects the lines from the database. This access is exposed via
        /// the TimeTable Repository instanciated in the controller constructor 
        /// </summary>
        /// <returns>View of Index</returns>
        [HttpGet]
        public ActionResult Index()
        {
            // Drop down list that is rendered in the view
            ViewBag.lines = _timeTableService.GetTrainLines();

            return View();
        }

        /// <summary>
        /// GET: /Home/GetStations
        /// Asychronous call that returns all stations that belong to
        /// the line in question.
        /// </summary>
        /// <param name="Id">Train Line Id</param>
        /// <returns>
        /// Html encoded string that builds a select list of stations
        /// belonging to the line that is requested by the Id parameter.
        /// </returns>
        [HttpGet]
        public HtmlString GetStations(int Id)
        {
            if (!Request.IsAjaxRequest())
            {
                // TODO: Throw error
            }
            var stations = "<select id='stations-list' class='transport-list'>" +
                "<option>-- Select a Line --</option>";
            foreach (var item in _timeTableService.GetStationsByLineId(Id))
                stations += "<option value='" + item.id + "'>" + item.name + "</option>";
            stations += "</select>";

            return new HtmlString(stations);
        }

        /// <summary>
        /// GET: /Home/GetTimes
        /// </summary>
        /// <param name="Id">Station Id</param>
        /// <returns>
        /// Html encoded string that builds a select list of times
        /// belonging to the station that is requested by the Id parameter.
        /// </returns>
        [HttpGet]
        public HtmlString GetTimes(int Id)
        {
            if (!Request.IsAjaxRequest())
            {
                // TODO: Throw Error
            }

            var times = "<select id='times-list' class='transport-list'>" +
                "<option>-- Select a Time --</option>";
            foreach (var item in _timeTableService.GetTimesByStationId(Id))
                times += "<option value='" + item.id + "'>" + item.time1 + "</option>";
            times += "</select>";

            return new HtmlString(times);
        }

        /// <summary>
        /// GET: /Home/About
        /// </summary>
        /// <returns>View of About</returns>
        [HttpGet]
        public ActionResult About()
        {
            return View();
        }

        /// <summary>
        /// GET: /Home/Contact
        /// </summary>
        /// <returns>View of Contact</returns>
        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }

        /// <summary>
        /// GET: /Home/Scraper
        /// Returns the view Scraper, which details the information about how
        /// scraped our data from the PTA website.
        /// </summary>
        /// <returns>View of Scraper</returns>
        [HttpGet]
        public ActionResult Scraper()
        {
            return View();
        }
    }
}
