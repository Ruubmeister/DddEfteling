using System.Collections.Generic;
using DddEfteling.Shared.Entities;

namespace DddEfteling.Visitors.Controls
{
    public class LocationTypeStrategy: ILocationTypeStrategy
    {
        private readonly Dictionary<LocationType, IVisitorLocationStrategy> registry = new ();

        public void Register(LocationType type, IVisitorLocationStrategy strategy)
        {
            this.registry.TryAdd(type, strategy);
        }

        public IVisitorLocationStrategy GetStrategy(LocationType type)
        {
            this.registry.TryGetValue(type, out IVisitorLocationStrategy result);

            return result;
        }
    }

    public interface ILocationTypeStrategy
    {
        void Register(LocationType type, IVisitorLocationStrategy strategy);
        IVisitorLocationStrategy GetStrategy(LocationType type);
    }
}