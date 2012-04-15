using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JourneyPlanner.Models;
using JourneyPlanner.Models.Scraper;

namespace JourneyPlanner.Controllers
{
    public class ScrapeController : Controller
    {
        /// <summary>
        /// GET: /Scrape/Scraper
        /// Public access to the scraping class that will scrape transport details
        /// from the given PTA url. At the moment, only scrapes Train timetables.
        /// </summary>
        /// <param name="url">The PTA url that you wish to scrape</param>
        [HttpGet]
        public void Scraper(string Url)
        {
            new ScrapingService().ScrapeTimeTable(Url);
        }
    }
}
