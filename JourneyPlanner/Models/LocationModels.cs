using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JourneyPlanner.Models.GoogleModels;
using System.Web.Script.Serialization;
using JourneyPlanner.Models.TimeTableModels;

namespace JourneyPlanner.Models.LocationModels
{
    /// <summary>
    /// GeoCoding responsibilities
    /// </summary>
    public class GeoCode
    {
        /// <summary>
        /// Given a station it will optimise the station name and encode it to a url format.
        /// It then uses the Google geocode service to grab the address and gps coordinates.
        /// </summary>
        /// <param name="Station">Station</param>
        /// <returns>GeoCoding result</returns>
        public GoogleGeoCodeResponse GeoCodeStation(Station Station)
        {
            // Trim the station name so only relevant key words are geocoded
            int index = Station.Name.ToLower().LastIndexOf("station");
            index += 7;
            var name = Station.Name.Substring(0, index);
            name += " Victoria";

            // Build a Url formatted string and send to the Google geocode service
            var query = "http://maps.googleapis.com/maps/api/geocode/json?address=" + HttpUtility.UrlEncode(name) + "&sensor=false";
            var result = new System.Net.WebClient().DownloadString(query);

            // Deserialize the Json reponse into the appropriate classes in GoogleModels
            JavaScriptSerializer js = new JavaScriptSerializer();
            return (GoogleGeoCodeResponse)js.Deserialize<GoogleGeoCodeResponse>(result);
        }
    }
}