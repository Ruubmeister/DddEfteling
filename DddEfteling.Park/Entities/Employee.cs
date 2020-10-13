using DddEfteling.Shared.Boundary;
using DddEfteling.Shared.Entities;
using System;
using System.Collections.Generic;

namespace DddEfteling.Park.Entities
{
    public class Employee
    {

        public Employee(String firstName, String lastName,  List<WorkplaceSkill> skills) {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Guid = Guid.NewGuid();
            this.Skills = skills;
        }

        public Guid Guid { get; }
        public String FirstName { get; }
        public String LastName { get; }

        public WorkplaceDto ActiveWorkplace { get; private set; }

        public List<WorkplaceSkill> Skills { get; }

        public WorkplaceSkill? ActiveSkill {  get; private set; }

        public void GoToWork(WorkplaceDto workspace, WorkplaceSkill skill)
        {
            this.ActiveWorkplace = workspace;
            this.ActiveSkill = skill;
        }

        public void StopWork()
        {
            if (this.ActiveWorkplace != null){
                this.ActiveWorkplace = null;
                this.ActiveSkill = null;

                //Also send event
            }
        }
    }
}
