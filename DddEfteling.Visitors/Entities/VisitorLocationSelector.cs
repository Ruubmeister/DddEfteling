using DddEfteling.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DddEfteling.Visitors.Entities
{
    public class VisitorLocationSelector
    {
        private readonly Random random;

        private readonly Dictionary<LocationType, int> locationNumbers = new ()
        {
            {LocationType.FAIRYTALE, 30 },
            {LocationType.RIDE, 60 },
            {LocationType.STAND, 10 }
        };

        public VisitorLocationSelector(Random random)
        {
            this.random = random;
        }

        public void ReduceAndBalance(LocationType type)
        {
            var reducer = 0;
            switch (type)
            {
                case LocationType.FAIRYTALE:
                    reducer = 5;
                    break;
                case LocationType.RIDE:
                    reducer = 10;
                    break;
                case LocationType.STAND:
                    reducer = 50;
                    break;
            }

            locationNumbers[type] -= reducer;
            if (locationNumbers[type] < 0)
            {
                locationNumbers[type] = 0;
            }

            foreach (var entry in locationNumbers.Where(entry => !entry.Key.Equals(type)).ToList())
            {
                int newValue = entry.Value + (reducer / 2);
                locationNumbers[entry.Key] = newValue;
            }
        }

        public LocationType GetLocation(LocationType? previousType)
        {
            var fairyEnd = locationNumbers[LocationType.FAIRYTALE];
            var rideEnd = fairyEnd + locationNumbers[LocationType.RIDE];
            var standEnd = rideEnd + locationNumbers[LocationType.STAND];

            if (!previousType.Equals(null))
            {
                if (previousType.Equals(LocationType.FAIRYTALE))
                {
                    fairyEnd = (int)Math.Ceiling(Math.Pow(fairyEnd, 1.7));
                }
                else if (previousType.Equals(LocationType.RIDE))
                {
                    rideEnd = (int)Math.Ceiling(Math.Pow(rideEnd, 1.7));
                }
                else
                {
                    standEnd = (int)Math.Ceiling(Math.Pow(standEnd, 1.7));
                }
            }

            var randomNumber = random.Next(1, standEnd);

            if (randomNumber <= fairyEnd)
            {
                return LocationType.FAIRYTALE;
            }

            return (randomNumber <= rideEnd) ? LocationType.RIDE : LocationType.STAND;
        }

    }
}
