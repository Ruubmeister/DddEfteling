using DddEfteling.Park.Realms.Controls;
using DddEfteling.Park.Rides.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DddEfteling.Park.Realms.Entities
{
    [JsonConverter(typeof(RealmConverter))]
    public class Realm
    {
        public Realm(String name)
        {
            Name = name;
        }
        public String Name { get; }

        public List<Ride> Rides { get; set; }

    }
}
