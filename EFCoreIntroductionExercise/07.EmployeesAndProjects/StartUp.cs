namespace _07.EmployeesAndProjects
{
    using System;
    using SoftUni.Data;
    using System.Linq;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using System.Globalization;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            var softUniContext = new SoftUniContext();
            var result = GetEmployeesInPeriod(softUniContext);
            Console.WriteLine(result);
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.EmployeesProjects
                             .Any(ep => ep.Project.StartDate.Year >= 2001 &&
                                       ep.Project.StartDate.Year <= 2003))
                .Select(x => new
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    ManagerFirstName = x.Manager.FirstName,
                    ManagerLastName = x.Manager.LastName,
                    Project = x.EmployeesProjects.Select(ep => new
                    {
                        Name = ep.Project.Name,
                        StartDate = ep.Project
                                      .StartDate
                                      .ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                        EndDate = ep.Project
                                    .EndDate.HasValue ? ep.Project
                                                          .EndDate
                                                          .Value
                                                          .ToString("M/d/yyyy h:mm:ss tt",                                                     CultureInfo.InvariantCulture) : "not finished"
                    })
                })
                .Take(10)
                .ToList();

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - Manager: {employee.ManagerFirstName} {employee.ManagerLastName} ");

                foreach (var project in employee.Project)
                {
                    sb.AppendLine($"--{project.Name} - {project.StartDate} - {project.EndDate}");
                }
            }

            return sb.ToString().TrimEnd();
        }
    }
}