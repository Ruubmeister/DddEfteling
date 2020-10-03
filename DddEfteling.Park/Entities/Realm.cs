using DddEfteling.Park.Controls;
using DddEfteling.Shared.Boundary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DddEfteling.Park.Entities
{
    [JsonConverter(typeof(RealmConverter))]
    public class Realm
    {
        public Realm(String name)
        {
            Name = name;
        }
        public String Name { get; }

        public List<RideDto> Rides { get; set; }

    }
}
