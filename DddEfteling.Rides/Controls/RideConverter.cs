﻿using DddEfteling.Rides.Entities;
using Geolocation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace DddEfteling.Rides.Controls
{
    public class RideConverter : JsonConverter
    {

        public RideConverter()
        {
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Ride));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);

            TimeSpan duration = new TimeSpan(0, int.Parse(obj["duration"]["minutes"].ToString()), 
                int.Parse(obj["duration"]["seconds"].ToString()));

            Coordinate coordinates = new Coordinate(obj["coordinates"]["lat"].ToObject<double>(),
                obj["coordinates"]["long"].ToObject<double>());

            return new Ride(
                RideStatus.Closed,
                coordinates,
                obj["name"].ToString(),
                int.Parse(obj["minimumAge"].ToString()),
                double.Parse(obj["minimumLength"].ToString()),
                duration,
                int.Parse(obj["maxPersons"].ToString())
                );
        }

        public override bool CanWrite
        {
            get { return false; }
        }
    }

}