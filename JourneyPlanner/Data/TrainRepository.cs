using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JourneyPlanner.Models.TimeTableModels;
using JourneyPlanner.Models.GoogleModels;

namespace JourneyPlanner.Data
{
    public interface ITrainRepository
    {
        IEnumerable<line> GetTrainLines();
        IEnumerable<station> GetStationsByLineId(int Id);
        IEnumerable<time> GetTimesByStationId(int Id);
        int InsertLine(string Name);
        int InsertStation(int Id, Station Station);
        void InsertTime(int Id, int Key, string Time);
        void InsertAddress(int Id, GoogleGeoCodeResponse GeoCode);
    }

    /// <summary>
    /// Data access to all things Train related is done
    /// </summary>
    public class TrainRepository : ITrainRepository
    {
        // Data dase context
        private TimeTableEntities _ctx;

        // Constructors
        public TrainRepository() : this(new TimeTableEntities()) { }

        public TrainRepository(TimeTableEntities ctx)
        {
            this._ctx = ctx;
        }

        // Get Methods
        public IEnumerable<line> GetTrainLines()
        {
            return _ctx.lines.OrderBy(x => x.name);
        }

        public IEnumerable<station> GetStationsByLineId(int Id)
        {
            return _ctx.stations.Where(x => x.line_id == Id);
        }

        public IEnumerable<time> GetTimesByStationId(int Id)
        {
            return _ctx.times.Where(x => x.station_id == Id);
        }

        // Insert Methods
        public int InsertLine(string Name)
        {
            line line = new line { name = Name };

            _ctx.AddTolines(line);
            _ctx.SaveChanges();

            return line.id;
        }

        public int InsertStation(int Id, Station Station)
        {
            station station = new station
            {
                line_id = Id,
                name = Station.Name
            };

            _ctx.AddTostations(station);
            _ctx.SaveChanges();

            return station.id;
        }

        public void InsertTime(int Id, int Key, string Time)
        {
            _ctx.AddTotimes(new time
            {
                station_id = Id,
                time1 = Time,
                number = Key
            });
            _ctx.SaveChanges();
        }

        public void InsertAddress(int Id, GoogleGeoCodeResponse GeoCode)
        {
            if (GeoCode.results.Count() > 0)
            {
                _ctx.AddToaddresses(new address
                {
                    station_id = Id,
                    address1 = GeoCode.results.First().formatted_address,
                    longitude = GeoCode.results.First().geometry.location.lng,
                    latitude = GeoCode.results.First().geometry.location.lat
                });
            }
            else
            {
                _ctx.AddToaddresses(new address
                {
                    station_id = Id,
                    address1 = "no result",
                    longitude = "0",
                    latitude = "0"
                });

            }
            _ctx.SaveChanges();
        }
    }
}