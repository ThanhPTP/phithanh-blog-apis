using Humanizer;
using Newtonsoft.Json;

namespace PhiThanh.ConsoleTools
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(@"Usage: 
    --m: module names
    exp: dotnet run --m Account");
            }

            string moduleName = string.Empty;
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == "--m")
                {
                    moduleName = args[i + 1];
                }
            }

            if (!string.IsNullOrEmpty(moduleName))
            {
                ProcessInternal(moduleName);
                Console.WriteLine($"Generated done: {moduleName}");
            }
            else
            {
                Console.WriteLine("No module to generated");
            }
        }

        private static void ProcessInternal(string moduleName)
        {
            var projectConfig = JsonConvert.DeserializeObject<ProjectConfig>(File.ReadAllText("projectConfig.json"))!;

            CreateEndpoints(projectConfig.NameSpaces, projectConfig.Api, moduleName);
            CreateModules(projectConfig.NameSpaces, projectConfig.Module, moduleName);
        }

        private static void CreateEndpoints(string namespaces, string apiProject, string moduleName)
        {
            string moduleNameUpper = moduleName.ToUpperCase();
            var filePath = Path.Combine(apiProject, "Endpoints", $"{moduleNameUpper}Endpoints.cs");
            ReplaceFileTemplate(namespaces, moduleName, filePath, "templates/endpoint.txt");
        }

        private static void CreateModules(string namespaces, string moduleProject, string moduleName)
        {
            string moduleNameUpper = moduleName.ToUpperCase();
            var fileCreateOrUpdatePath = Path.Combine(moduleProject, $"{moduleNameUpper}Module", $"CreateOrUpdate{moduleNameUpper}Handler.cs");
            ReplaceFileTemplate(namespaces, moduleName, fileCreateOrUpdatePath, "templates/create-or-update.txt");

            var fileBatchDeletePath = Path.Combine(moduleProject, $"{moduleNameUpper}Module", $"DeleteBatch{moduleNameUpper.Pluralize()}Handler.cs");
            ReplaceFileTemplate(namespaces, moduleName, fileBatchDeletePath, "templates/batch-delete.txt");

            var fileDeletePath = Path.Combine(moduleProject, $"{moduleNameUpper}Module", $"Delete{moduleNameUpper}Handler.cs");
            ReplaceFileTemplate(namespaces, moduleName, fileDeletePath, "templates/delete.txt");

            var fileGetListPath = Path.Combine(moduleProject, $"{moduleNameUpper}Module", $"GetList{moduleNameUpper.Pluralize()}Handler.cs");
            ReplaceFileTemplate(namespaces, moduleName, fileGetListPath, "templates/get-list.txt");

            var fileGetDetailPath = Path.Combine(moduleProject, $"{moduleNameUpper}Module", $"GetDetail{moduleNameUpper}Handler.cs");
            ReplaceFileTemplate(namespaces, moduleName, fileGetDetailPath, "templates/get-detail.txt");
        }

        private static void ReplaceFileTemplate(string namespaces, string moduleName,
            string relativeFilePath, string templatePath)
        {
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)!.FullName;

            string moduleNameUpper = moduleName.ToUpperCase();
            string moduleNameLower = moduleName.ToLower();
            string moduleNameUpperPlural = moduleNameUpper.Pluralize();
            string moduleNameLowerPlural = moduleNameLower.Pluralize();

            var filePath = Path.Combine(projectDirectory, relativeFilePath);
            Console.WriteLine($"Generated: {filePath}");
            if (File.Exists(filePath))
            {
                Console.WriteLine("Endpoint already exists !");
                return;
            }
            else
            {
                string dir = Directory.GetParent(filePath)!.FullName;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }

            var endpointTemplate = File.ReadAllText(templatePath)!
                .Replace("[endpointUpper]", moduleNameUpper)
                .Replace("[endpointLower]", moduleNameLower)
                .Replace("[endpointUpperPlural]", moduleNameUpperPlural)
                .Replace("[endpointLowerPlural]", moduleNameLowerPlural)
                .Replace("[namespaces]", namespaces);

            using StreamWriter sw = File.CreateText(filePath);
            sw.Write(endpointTemplate);
        }

        private static string ToUpperCase(this string str)
        {
            return char.ToUpper(str[0]) + str[1..];
        }
    }
}
