using DddEfteling.Park.Employees.Entities;
using DddEfteling.Park.Realms.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DddEfteling.Park.Common.Entities
{
    public abstract class Workspace
    {
        protected Workspace()
        {
            foreach (Skill skill in Enum.GetValues(typeof(Skill)))
            {
                skillRequiredEmployeeMap.Add(skill, 0);
            }
        }

        public Realm Realm { get; set; }

        private readonly Dictionary<Skill, int> skillRequiredEmployeeMap = new Dictionary<Skill, int>();

        public void setEmployeeSkillRequirement(Skill skill, int requirement)
        {
            this.skillRequiredEmployeeMap.Add(skill, requirement);
        }

        private Boolean isSkillUnderstaffed(List<Employee> rideEmployees, Skill skill)
        {
            return rideEmployees.Count(employee => employee.ActiveSkill.Equals(skill)) < skillRequiredEmployeeMap.GetValueOrDefault(skill);
        }
    }
}
