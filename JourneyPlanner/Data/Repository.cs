using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JourneyPlanner.Data;

namespace JourneyPlanner.Models
{
    public class Repository
    {
        private TimeTableEntities ctx = new TimeTableEntities();

        public line GetLineById(int id)
        {
            return ctx.lines.Where(x => x.id == id).SingleOrDefault();        }

        public IQueryable<line> GetLines()
        {
            return ctx.lines;
        }

        public line GetLineByName(string name)
        {
            return ctx.lines.Where(x => x.name.ToLower().Contains(name.ToLower())).FirstOrDefault();
        }

        public station GetStationById(int id)
        {
            return ctx.stations.Where(x => x.id == id).SingleOrDefault();
        }

        public IQueryable<station> GetStations()
        {
            return ctx.stations;
        }

        public station GetStationByName(string name)
        {
            return ctx.stations.Where(x => x.name.ToLower().Contains(name.ToLower())).FirstOrDefault();
        }


        public IQueryable<station> GetStationsByLine(int id)
        {
            return ctx.stations.Where(x => x.line_id == id);
        }

        public List<time> GetTimesByStationId(int id)
        {
            var times = ctx.times.Where(x => x.station_id == id).ToList();
            times.RemoveAll(x => x.time1 == "-" || x.time1 == "|");
            return times;
        }

        public time GetTimesById(int id)
        {
            return ctx.times.Where(x => x.id == id).SingleOrDefault();
        }

        public void Save()
        {
            ctx.SaveChanges();
        }

        public address GetAddressById(int id)
        {
            return ctx.addresses.Where(x => x.station_id == id).SingleOrDefault();
        }
    }
}