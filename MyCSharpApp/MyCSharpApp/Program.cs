using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSharpApp
{
    class Program
    {
       static string ProjectsPath;
        static void Main(string[] args)
        {
            Console.WriteLine("Press 1 For Coding Any Other Key to Open Projects path");
            if (Console.ReadLine() == "1") 
            {
                PracticeCodes practiceCodes = new PracticeCodes();//Constructor will call other methods
            }
            ReadResourcFile();
            //string startupPath = System.IO.Directory.GetCurrentDirectory();
            var directories = Directory.GetDirectories(ProjectsPath);
            List<DirectoryModel> dirList = new List<DirectoryModel>();
            int intNum = 0;
            foreach (var dirPath in directories)
            {   
                DirectoryModel model = new DirectoryModel();
                model.ProjectName = dirPath.Substring(dirPath.LastIndexOf("\\") + 1);
                if (model.ProjectName.Contains("_"))
                { 
                    intNum = 0;
                    int.TryParse(model.ProjectName.Substring(0, 2), out intNum);
                    model.Id = intNum;
                    model.Path = dirPath;
                    model.ExePath = dirPath + @"\bin\Debug";
                    dirList.Add(model);
                }
            }
            Console.WriteLine("Id\tProject");
            foreach (var item in dirList)
            {
                Console.WriteLine(item.Id + "\t" + item.ProjectName);
            }
            intNum = 0;
            Console.WriteLine();
            Console.WriteLine("Enter Id for Opening EXE Path");
            int.TryParse(Console.ReadLine(), out intNum);
            if (intNum > 0 && intNum <= dirList.Count)
            {
                System.Diagnostics.Process.Start(dirList.FirstOrDefault(x=>x.Id==intNum).ExePath);
            }
            Console.ReadKey();
        }
        static void ReadResourcFile()
        {
            ProjectsPath = MyCSharpApp.Resources.ResourcesFile.RepositoryProjectsPath_USMUMSUBDEB3;
        }
    }
    class DirectoryModel
    {
        public string ProjectName { get; set; }
        public string Path { get; set; }
        public int Id { get; set; }
        public string ExePath { get; set; }
    }
}
