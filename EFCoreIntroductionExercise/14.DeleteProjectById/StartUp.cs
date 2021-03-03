namespace _14.DeleteProjectById
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
            var result = DeleteProjectById(softUniContext);
            Console.WriteLine(result);
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            var project = context.Projects
                .FirstOrDefault(p => p.ProjectId == 2);

            var employeeProjects = context.EmployeesProjects
                .Where(x => x.ProjectId == 2) 
                .ToList();

            foreach (var emProject in employeeProjects)
            {
                context.EmployeesProjects.Remove(emProject);
            }

            context.Projects.Remove(project);

            context.SaveChanges();

            var projects = context.Projects
                .Select(p => p.Name)
                .Take(10)
                .ToList();

            var sb = new StringBuilder();

            foreach (var currProject in projects)
            {
                sb.AppendLine(currProject);
            }

            return sb.ToString().TrimEnd();
        }
    }
}