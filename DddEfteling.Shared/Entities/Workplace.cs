using DddEfteling.Shared.Boundaries;
using System;
using System.Collections.Generic;

namespace DddEfteling.Shared.Entities
{
    public abstract class Workplace
    {
        public Guid Guid { get; set; }
        public LocationType LocationType { get; set; }

        protected Workplace()
        {
            this.Guid = Guid.NewGuid();
            foreach (WorkplaceSkill skill in Enum.GetValues(typeof(WorkplaceSkill)))
            {
                skillRequiredEmployeeMap.Add(skill, 0);
            }
        }

        public readonly Dictionary<WorkplaceSkill, int> skillRequiredEmployeeMap = new Dictionary<WorkplaceSkill, int>();

        public void setEmployeeSkillRequirement(WorkplaceSkill skill, int requirement)
        {
            this.skillRequiredEmployeeMap[skill] = requirement;
        }

        public int requiredEmployeesForSkill(List<EmployeeDto> rideEmployees, WorkplaceSkill skill)
        {
            //Todo: Fix
            return 0;
        }

        public WorkplaceDto ToWorkplaceDto()
        {
            return new WorkplaceDto(this.Guid, this.LocationType);
        }
    }
}
