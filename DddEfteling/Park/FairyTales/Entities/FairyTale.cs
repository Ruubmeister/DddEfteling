using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.Realms.Entities;
using System;

namespace DddEfteling.Park.FairyTales.Entities
{
    public class FairyTale
    {
        public FairyTale(String name, Realm realm, Coordinates coordinates)
        {
            this.Name = name;
            this.Realm = realm;
            this.Coordinates = coordinates;
        }

        public string Name { get; }

        public Realm Realm { get;  }

        public Coordinates Coordinates { get; }
    }
}
