using DddEfteling.Park.Controls;
using DddEfteling.Shared.Boundaries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DddEfteling.Park.Entities
{
    [JsonConverter(typeof(RealmConverter))]
    public class Realm
    {
        public Realm(string name)
        {
            Name = name;
        }
        public string Name { get; }

        public List<RideDto> Rides { get; set; }

    }
}
