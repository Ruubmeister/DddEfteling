using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using System;
using System.Collections.Generic;

namespace DddEfteling.Park.Entities
{
    public class Employee
    {

        public Employee(string firstName, string lastName, List<WorkplaceSkill> skills)
        {
            FirstName = firstName;
            LastName = lastName;
            Guid = Guid.NewGuid();
            Skills = skills;
        }

        public Guid Guid { get; }
        public string FirstName { get; }
        public string LastName { get; }

        public WorkplaceDto ActiveWorkplace { get; private set; }

        public List<WorkplaceSkill> Skills { get; }

        public WorkplaceSkill? ActiveSkill { get; private set; }

        public void GoToWork(WorkplaceDto workspace, WorkplaceSkill skill)
        {
            ActiveWorkplace = workspace;
            ActiveSkill = skill;
        }

        public void StopWork()
        {
            if (ActiveWorkplace == null)
            {
                return;
            }
            ActiveWorkplace = null;
            ActiveSkill = null;

            //Also send event
        }
    }
}
