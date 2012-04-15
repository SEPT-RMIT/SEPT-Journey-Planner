using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using HtmlAgilityPack;
using System.Web.Script.Serialization;
using System.Web.Helpers;
using JourneyPlanner.Data;
using JourneyPlanner.Models.GoogleModels;

namespace JourneyPlanner.Models.TimeTableModels
{
    /// <summary>
    /// Encapsulates all data scraped from a train timetable 
    /// </summary>
    public class TimeTable
    {
        public string Line { get; set; }
        public List<Station> Stations { get; set; }
        public GoogleGeoCodeResponse Geo { get; set; }
        public TimeTable() { this.Stations = new List<Station>(); }
    }

    /// <summary>
    /// Encapsulates all data about a station, including name, times that
    /// trains arrive and GPS information.
    /// </summary>
    public class Station
    {
        public string Name { get; set; }
        public Dictionary<int, string> Times { get; set; }
        public GoogleGeoCodeResponse Geo { get; set; }
        public Station() { this.Times = new Dictionary<int, string>(); }
    }

    public interface ITimeTableService
    {
        IEnumerable<line> GetTrainLines();
        IEnumerable<station> GetStationsByLineId(int Id);
        IEnumerable<time> GetTimesByStationId(int Id);
    }

    /// <summary>
    /// Services exposes to the Controller for information access
    /// </summary>
    public class TimeTableService : ITimeTableService
    {
        // Data access
        private ITrainRepository _trainRepository;

        // Constructors
        public TimeTableService() : this(new TrainRepository()) { }

        public TimeTableService(ITrainRepository TrainRepository)
        {
            this._trainRepository = TrainRepository;
        }

        /// <summary>
        /// Get all train lines
        /// </summary>
        /// <returns>All train lines</returns>
        public IEnumerable<line> GetTrainLines()
        {
            return _trainRepository.GetTrainLines();
        }

        /// <summary>
        /// Get all the stations belonging to a Line
        /// </summary>
        /// <param name="Id">Line Id</param>
        /// <returns>Stations</returns>
        public IEnumerable<station> GetStationsByLineId(int Id)
        {
            return _trainRepository.GetStationsByLineId(Id);
        }

        /// <summary>
        /// Get all the times belonging to a station. Also removes all the
        /// - and | characters that are unnecessary for the view.
        /// </summary>
        /// <param name="Id">Station Id</param>
        /// <returns>Times</returns>
        public IEnumerable<time> GetTimesByStationId(int Id)
        {
            var times = _trainRepository.GetTimesByStationId(Id).ToList();
            times.RemoveAll(x => x.time1 == "-" || x.time1 == "|");
            return times;
        }
    }
}