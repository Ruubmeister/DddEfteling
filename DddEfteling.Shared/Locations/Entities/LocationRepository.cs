using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DddEfteling.Shared.Controls;
using Newtonsoft.Json;

namespace DddEfteling.Shared.Entities
{
    public class LocationRepository<T> where T: Location
    {
        private readonly ConcurrentBag<T> locations;
        private readonly JsonConverter converter;
        private readonly ILocationService locationService;

        public LocationRepository(ILocationService locationService, JsonConverter converter)
        {
            this.converter = converter;
            this.locationService = locationService;
            this.locations = LoadLocations();
        }
        
        private ConcurrentBag<T> LoadLocations()
        {
            using StreamReader r = new StreamReader($"resources/{typeof(T).Name.ToLower()}.json");
            string json = r.ReadToEnd();
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(converter);
            return new ConcurrentBag<T>(JsonConvert.DeserializeObject<List<T>>(json, settings));
        }
        
        public T FindByName(string name)
        {
            return locations.FirstOrDefault(location => location.Name.Equals(name));
        }

        public ConcurrentBag<T> All()
        {
            return locations;
        }

        public List<T> AllAsList()
        {
            return locations.ToList();
        }
        
        public T FindByGuid(Guid guid)
        {
            return locations.First(location => location.Guid.Equals(guid));
        }

        public T NearestLocation(Guid locationGuid, List<Guid> exclusionList)
        {
            T location = this.locations.First(location => location.Guid.Equals(locationGuid));
            return locationService.NearestLocation(location, locations, exclusionList);
        }

        public T NextLocation(Guid locationGuid, List<Guid> exclusionList)
        {
            T location = this.locations.First(location => location.Guid.Equals(locationGuid));
            return locationService.NextLocation(location, locations, exclusionList) ?? this.GetRandom();
        }
        
        public T GetRandom()
        {
            return this.locations.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
        }
    }
}