namespace _11.FindLatest10Projects
{
    using System;
    using System.Linq;
    using SoftUni.Data;
    using System.Text;
    using System.Globalization;
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var softUniContext = new SoftUniContext();
            var result = GetLatestProjects(softUniContext);
            Console.WriteLine(result);
        }

        static string GetLatestProjects(SoftUniContext context)
        {
            //Write a program that return information about the last 10 started projects. Sort them by name lexicographically and return their name, description and start date, each on a new row.Format of the output

           var projects = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .Select(x => new
                {
                    Name = x.Name,
                    Description = x.Description,
                    StartDate = x.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                })
                .OrderBy(x => x.Name)
                .ToList();

            var sb = new StringBuilder();

            foreach (var project in projects)
            {
                sb.AppendLine(project.Name);
                sb.AppendLine(project.Description);
                sb.AppendLine(project.StartDate);
            }

            return sb.ToString().TrimEnd();
        }
    }
}