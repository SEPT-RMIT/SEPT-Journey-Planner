using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JourneyPlanner.Models;

namespace JourneyPlanner.Controllers
{
    public class DataServiceController : Controller
    {
        //
        // GET: /DataService/

        public JsonResult GetStations()
        {
            Repository repo = new Repository();
            var stations = new List<JsonStation>();

            foreach (var item in repo.GetStations())
            {
                stations.Add(new JsonStation()
                {
                    name = item.name,
                    latitude = item.addresses.First().latitude,
                    longitude = item.addresses.First().longitude
                });
            }

            return Json(stations, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStation(string query)
        {
            Repository repo = new Repository();
            if (repo.GetStationByName(query) == null)
                return null;
            var x = repo.GetStationById(repo.GetStationByName(query).id);

            return Json(
                new JsonStation()
                {
                    name = x.name,
                    latitude = x.addresses.First().latitude,
                    longitude = x.addresses.First().longitude
                }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStationsByLine(string query)
        {
            Repository repo = new Repository();
            if (repo.GetLineByName(query) == null)
                return null;
            var stations = new List<JsonStation>();

            foreach (var item in repo.GetStationsByLine(repo.GetLineByName(query).id))
            {
                stations.Add(new JsonStation()
                {
                    name = item.name,
                    latitude = item.addresses.First().latitude,
                    longitude = item.addresses.First().longitude
                });
            }

            return Json(stations, JsonRequestBehavior.AllowGet);
        }

        public class JsonStation
        {
            public string name { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
        }

        [HttpGet]
        public string GetLongitude(int id)
        {
            if (!Request.IsAjaxRequest())
            {
                // TODO: Throw Error
            }
            Repository repo = new Repository();
            return repo.GetAddressById(id).longitude;
        }

        [HttpGet]
        public string GetLatitude(int id)
        {
            if (!Request.IsAjaxRequest())
            {
                //Throw error
            }
            Repository repo = new Repository();
            return repo.GetAddressById(id).latitude;
        }
    }
}
