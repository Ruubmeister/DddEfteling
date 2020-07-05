using DddEfteling.Park.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DddEfteling.Visitors.Entities
{
    public class VisitorLocationSelector
    {
        Random random;

        private Dictionary<LocationType, int> locationNumbers = new Dictionary<LocationType, int>()
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
            int reducer = 0;
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

            this.locationNumbers[type] -= reducer;
            if (this.locationNumbers[type] < 0)
            {
                this.locationNumbers[type] = 0;
            }

            foreach (KeyValuePair<LocationType, int> entry in locationNumbers.Where(entry => !entry.Key.Equals(type)))
            {
                int newValue = entry.Value + (reducer / 2);
                locationNumbers[entry.Key] = newValue;
            }
        }

        public LocationType GetLocation(LocationType? previousType)
        {
            int fairyEnd = locationNumbers[LocationType.FAIRYTALE];
            int rideEnd = fairyEnd + locationNumbers[LocationType.RIDE];
            int standEnd = rideEnd + locationNumbers[LocationType.STAND];

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

            int randomNumber = random.Next(1, standEnd);

            if (randomNumber <= fairyEnd)
            {
                return LocationType.FAIRYTALE;
            }
            else if (randomNumber <= rideEnd)
            {
                return LocationType.RIDE;
            }
            else
            {
                return LocationType.STAND;
            }
        }

    }
}
