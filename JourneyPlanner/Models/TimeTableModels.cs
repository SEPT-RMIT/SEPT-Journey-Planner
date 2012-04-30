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
using JourneyPlanner.Models.GraphModels;

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
        List<string> CreateGraph(); // creates a Graph, populates it with 2 lines, and performs a BFS and returns the result
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

        public List<string> CreateGraph()
        {
            Graph graph = new Graph();
            List<station> HurstbridgeLine = _trainRepository.GetStationsByLineId(2).ToList();
            List<station> EppingLine = _trainRepository.GetStationsByLineId(1).ToList();
            graph.AddLineToGraph(HurstbridgeLine, "HB");
            graph.AddLineToGraph(EppingLine, "EP");
            station station1 = HurstbridgeLine[0];
            station station2 = EppingLine[3];
            return graph.BFS(station1, station2);
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