using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Script.Serialization;
using HtmlAgilityPack;
using JourneyPlanner.Models.GoogleModels;
using JourneyPlanner.Data;
using JourneyPlanner.Models.LocationModels;
using JourneyPlanner.Models.TimeTableModels;
using JourneyPlanner.Models.GraphModels;

namespace JourneyPlanner.Models.Scraper
{
    /// <summary>
    /// Service that exposes web scraping functionality to the controller.
    /// </summary>
    public class ScrapingService
    {
        // Data access
        private ITrainRepository _trainRepository;
        private TimeTable timeTable;
        
        // Constructors
        public ScrapingService() : this(new TrainRepository()) { }

        public ScrapingService(ITrainRepository TrainRepository)
        {
            this._trainRepository = TrainRepository;
        }

        /// <summary>
        /// Calls a series of methods that Reads, Scrapes and Inserts into DB
        /// </summary>
        /// <param name="Url">Url to scrape</param>
        public void ScrapeTimeTable(string Url)
        {
            this.timeTable = new Parser().ParseTrainTimeTable(new Reader().Read(Url));
            this.InsertIntoDatabase();
        }

        /// <summary>
        /// Inserts all information collected into the working database
        /// </summary>
        private void InsertIntoDatabase()
        {
            // Insert Train line and get the id
            var lineId = _trainRepository.InsertLine(this.timeTable.Line);
            int stationId;

            // Loop through the stations
            foreach (var station in this.timeTable.Stations)
            {
                stationId = _trainRepository.InsertStation(lineId, station);
                foreach (KeyValuePair<int, string> pair in station.Times)
                    _trainRepository.InsertTime(stationId, pair.Key, pair.Value);
                _trainRepository.InsertAddress(stationId, station.Geo);
            }
        }
    }

    /// <summary>
    /// Class that captures downloading web page functionality
    /// </summary>
    public class Reader
    {
        /// <summary>
        /// Read in the Url and download the Web page returning that pages source code
        /// </summary>
        /// <param name="Url">Url of Timetable from PTA</param>
        /// <returns>string of Html source code</returns>
        public string Read(string Url)
        {
            string source;

            using (WebClient client = new WebClient())
            {
                source = client.DownloadString(Url);
            }

            return source;
        }
    }

    /// <summary>
    /// Class that captures web scraping functionality
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// Reads the html source code of the PTA train time table and extracts
        /// all relevant data, including, lines, stations times and gps coordinates.
        /// </summary>
        /// <param name="Source"></param>
        /// <returns>Returns a TimeTable object that encapsulates all info extracted</returns>
        public TimeTable ParseTrainTimeTable(string Source)
        {
            // Set up the 
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(Source);

            // Access required information by targeting div elements with unique ids
            var stations = htmlDocument.DocumentNode.SelectNodes("//div[@id='ttMargin']");
            var times = htmlDocument.DocumentNode.SelectNodes("//div[@id='ttBody']");
            var line = htmlDocument.DocumentNode.SelectNodes("//div[@id='ttHeadline']");

            TimeTable timeTable = new TimeTable();
            GeoCode geoCoder = new GeoCode();

            // Set Line name
            timeTable.Line = line.First().FirstChild.FirstChild.InnerText;
            Station station;

            // Loop through all the ChildNodes (i.e. All the stations)
            for (int i = 0; i < stations.First().ChildNodes.Count; i++)
            {
                // Create a new station and add it to the list
                timeTable.Stations.Add(station = new Station
                {
                    Name = stations.First().ChildNodes[i].ChildNodes[1].FirstChild.InnerText,
                });

                // Set location information
                station.Geo = geoCoder.GeoCodeStation(station);

                // Loop through all the ChildNodes (i.e. All the times) and add the time to the station
                for (int j = 0; j < times.First().ChildNodes[i + 1].ChildNodes.Count; j++)
                    station.Times.Add(j,
                        times.First().ChildNodes[i + 1].ChildNodes[j].FirstChild.InnerText);
            }

            return timeTable;
        }
    }
}