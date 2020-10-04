﻿using DddEfteling.Stands.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DddEfteling.Stands.Controls
{
    public class StandControl : IStandControl
    {
        private readonly List<Stand> stands;


        public StandControl()
        {
            this.stands = this.LoadStands();
        }
        
        private List<Stand> LoadStands()
        {
            using StreamReader r = new StreamReader("resources/stands.json");
            string json = r.ReadToEnd();
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new StandConverter());
            return JsonConvert.DeserializeObject<List<Stand>>(json, settings);
        }

        public Stand FindStandByName(string name)
        {
            return this.stands.FirstOrDefault(stand => stand.Name.Equals(name));
        }

        public List<Stand> All()
        {
            return stands;
        }
    }

    public interface IStandControl
    {
        public Stand FindStandByName(string name);
        public List<Stand> All();
    }
}