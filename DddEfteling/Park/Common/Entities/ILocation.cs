using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace DddEfteling.Park.Common.Entities
{
    public interface ILocation
    {
        public string Name { get; }
        public ImmutableSortedDictionary<string, double> DistanceToOthers { get; set; }
    }
}
