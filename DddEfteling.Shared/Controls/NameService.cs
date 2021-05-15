using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DddEfteling.Shared.Controls
{
    public class NameService : INameService
    {
        private readonly Random rnd = new Random();
        private List<String> FirstNames { get; }
        private List<String> LastNames { get; }

        public NameService()
        {
            FirstNames = ReadJsonFile("resources/first-names.json");
            LastNames = ReadJsonFile("resources/last-names.json");
        }

        private static List<string> ReadJsonFile(string file)
        {
            using StreamReader r = new StreamReader(file);
            string json = r.ReadToEnd();
            return JsonConvert.DeserializeObject<List<string>>(json);
        }


        public string RandomFirstName()
        {
            return FirstNames[rnd.Next(FirstNames.Count)];
        }

        public string RandomLastName()
        {
            return LastNames[rnd.Next(LastNames.Count)];
        }

    }

    public interface INameService
    {
        string RandomFirstName();
        string RandomLastName();
    }
}
