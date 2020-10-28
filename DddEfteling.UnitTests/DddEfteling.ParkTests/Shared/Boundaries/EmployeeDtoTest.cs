using DddEfteling.Shared.Boundaries;
using System;
using System.Collections.Generic;
using Xunit;

namespace DddEfteling.ParkTests.Shared.Boundaries
{
    public class EmployeeDtoTest
    {
        [Fact]
        public void Constructors_ConstructDto_ExpectDto()
        {
            EmployeeDto employeeDto = new EmployeeDto(Guid.NewGuid(), "first name", "last name", new List<string>() { { "Skill1" }, { "Skill2" } });

            Assert.Equal("first name", employeeDto.FirstName);
            Assert.Equal("last name", employeeDto.LastName);
            Assert.NotEmpty(employeeDto.Guid.ToString());
            Assert.Equal(2, employeeDto.Skills.Count);
            Assert.Contains<string>("Skill1", employeeDto.Skills);
            Assert.Contains<string>("Skill2", employeeDto.Skills);
        }

        [Fact]
        public void Setters_ConstructAndUseSetters_ExpectDto()
        {
            EmployeeDto employeeDto = new EmployeeDto();
            Assert.Null(employeeDto.FirstName);
            Assert.Null(employeeDto.LastName);
            Assert.Null(employeeDto.Skills);

            employeeDto.FirstName = "first name";
            employeeDto.LastName = "last name";
            employeeDto.Guid = Guid.NewGuid();
            employeeDto.Skills = new List<string>() { { "Skill1" }, { "Skill2" } };

            Assert.Equal("first name", employeeDto.FirstName);
            Assert.Equal("last name", employeeDto.LastName);
            Assert.NotEmpty(employeeDto.Guid.ToString());
            Assert.Equal(2, employeeDto.Skills.Count);
            Assert.Contains<string>("Skill1", employeeDto.Skills);
            Assert.Contains<string>("Skill2", employeeDto.Skills);
        }
    }
}
