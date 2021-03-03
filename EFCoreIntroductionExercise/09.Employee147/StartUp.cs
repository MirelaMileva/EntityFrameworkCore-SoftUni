namespace _09.Employee147
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
            var result = GetEmployee147(softUniContext);
            Console.WriteLine(result);
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            var employee = context.Employees
                 .Select(x => new
                 {
                     x.EmployeeId,
                     x.FirstName,
                     x.LastName,
                     x.JobTitle,
                     Projects = x.EmployeesProjects.OrderBy(x => x.Project.Name).Select(p => new
                     {
                         ProjectName = p.Project.Name
                     })
                 })
                 .FirstOrDefault(x => x.EmployeeId == 147);

            var sb = new StringBuilder();

            sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

            foreach (var project in employee.Projects)
            {
                sb.AppendLine($"{project.ProjectName}");
            }

            return sb.ToString().TrimEnd();
        }
        }
}