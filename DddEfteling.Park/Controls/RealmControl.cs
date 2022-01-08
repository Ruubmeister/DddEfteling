using DddEfteling.Park.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DddEfteling.Park.Controls
{
    public class RealmControl : IRealmControl
    {

        private readonly List<Realm> realms;

        public RealmControl()
        {
            realms = LoadRealms();
        }

        public Realm FindRealmByName(string name)
        {
            return realms.FirstOrDefault(realm => realm.Name.Equals(name));
        }

        private List<Realm> LoadRealms()
        {
            using var r = new StreamReader("resources/realms.json");
            var json = r.ReadToEnd();
            return JsonConvert.DeserializeObject<List<Realm>>(json);
        }
    }

    public interface IRealmControl
    {
        Realm FindRealmByName(string name);
    }
}
