namespace _10.DepartmentsWithMoreThan5Employees
{
    using System;
    using System.Linq;
    using System.Text;
    using SoftUni.Data;
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var softUniContext = new SoftUniContext();
            var result = GetDepartmentsWithMoreThan5Employees(softUniContext);
            Console.WriteLine(result);
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                .Where(x => x.Employees.Count > 5)
                .OrderBy(x => x.Employees.Count)
                .ThenBy(x => x.Name)
                .Select(x => new
                {
                    x.Name,
                    x.Manager.FirstName,
                    x.Manager.LastName,
                    Employees = x.Employees.Select(e => new
                    {
                        e.FirstName,
                        e.LastName,
                        e.JobTitle
                    })
                    .OrderBy(x => x.FirstName)
                    .ThenBy(x => x.LastName)
                    .ToList()
                })
                .ToList();

            var sb = new StringBuilder();

            foreach (var depart in departments)
            {
                sb.AppendLine($"{depart.Name} – {depart.FirstName} {depart.LastName}");

                foreach (var emp in depart.Employees)
                {
                    sb.AppendLine($"{emp.FirstName} {emp.LastName} - {emp.JobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }
    }
}