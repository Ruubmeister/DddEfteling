using System;
using System.Buffers.Text;
using DddEfteling.Shared.Entities;
using Geolocation;
using Newtonsoft.Json.Linq;

namespace DddEfteling.ParkTests.Entities
{
    public class LocationImpl: Location
    {
        public LocationImpl() : base(Guid.NewGuid(), LocationType.RIDE){}
        public LocationImpl(Coordinate coordinate) : base(Guid.NewGuid(), LocationType.RIDE)
        {
            this.Coordinates = coordinate;
        }
        
        public LocationImpl(JObject obj): base(Guid.NewGuid(), LocationType.RIDE) 
        {
            Coordinates = new Coordinate(obj["coordinates"]["lat"].ToObject<double>(),
                obj["coordinates"]["long"].ToObject<double>());
            Name = obj["name"].ToString();
        }
    }
}