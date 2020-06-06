using DddEfteling.Park.FairyTales.Controls;
using DddEfteling.Park.FairyTales.Entities;
using DddEfteling.Park.Realms.Controls;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DddEfteling.Park.FairyTales.Controls
{
    public class FairyTaleControl : IFairyTaleControl
    {
        private readonly List<FairyTale> fairyTales;
        private readonly IRealmControl realmControl;
        private readonly ILogger logger;

        public FairyTaleControl(IRealmControl realmControl, ILogger<FairyTaleControl> logger)
        {
            this.realmControl = realmControl;
            this.fairyTales = LoadFairyTales();
            this.logger = logger;

            this.logger.LogInformation($"Loaded fairy tales count: {fairyTales.Count}");
        }

        private List<FairyTale> LoadFairyTales()
        {
            using (StreamReader r = new StreamReader("resources/fairy-tales.json"))
            {
                string json = r.ReadToEnd();
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Converters.Add(new FairyTaleConverter(realmControl));
                return JsonConvert.DeserializeObject<List<FairyTale>>(json, settings);
            }
        }

        public FairyTale FindFairyTaleByName(string name)
        {
            return fairyTales.FirstOrDefault(tale => tale.Name.Equals(name));
        }
    }

    interface IFairyTaleControl
    {
        FairyTale FindFairyTaleByName(string name);
    }
}
