using DddEfteling.Visitors.Entities;

namespace DddEfteling.Visitors.Controls
{
    public interface IVisitorLocationStrategy
    {
        public void StartLocationActivity(Visitor visitor);

        public void SetNewLocation(Visitor visitor);
    }
}