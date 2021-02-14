using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

class Program
{

    //Configurable Paths and FileName Constants.
    public const string InputFilePath = @"C:\Users\subdeb\Documents\ProjectWP\DefectsList\00Input_Copy.txt";
    public const string DailyStatusInputFolderRelativePath = @"07_ExcelInteropDailyStatus\bin\Debug\InputOutput_DailyStatus\TriageEmailOutputFiles\";

    //Application Level Variables
    static List<string> DefectList;
    public static string RepositoryProjectsPath = string.Empty;
    static List<BugAssignmentModel> BugAssignmentModelList;

    static void Main(string[] args)
    {
        ReadResourceFile();
        ReadFromInputFile();
        PopulateBugModel();
        DailyStatusOperations();

        Console.ReadKey();
    }
    static void ReadResourceFile()
    {
        //Make sure the resourFile have access modifier as public and System.Forms.Dll is imported for ResXResourceReader to work
        var resourceFileRelativePath = @"MyCSharpApp\MyCSharpApp\MyCSharpApp\Resources\ResourcesFile.resx";
        var executingAssemblyPath = Assembly.GetExecutingAssembly().Location;
        var firstIndexOfMyCSharpApp = executingAssemblyPath.IndexOf("MyCSharpApp");
        string resourceFilePath = executingAssemblyPath.Substring(0, firstIndexOfMyCSharpApp) + resourceFileRelativePath;
        ResXResourceReader rsxr = new ResXResourceReader(resourceFilePath);
        foreach (DictionaryEntry de in rsxr)
        {
            if (de.Key.ToString() == "RepositoryProjectsPath_" + Environment.MachineName)
            {
                RepositoryProjectsPath = de.Value.ToString();
            }
        }
        rsxr.Close();
    }
    static void ReadFromInputFile()
    {
        DefectList = new List<string>();
        var fileStream = new FileStream(InputFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
        {
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                if (line.Substring(0, 2) != "ID")//Ignore the first line of Header in the Inputfile
                {
                    DefectList.Add(line);
                }

            }
            streamReader.Close();
        }
    }
    static void PopulateBugModel()
    {
        BugAssignmentModelList = new List<BugAssignmentModel>();
        foreach (var defectLine in DefectList)
        {
            BugAssignmentModel model = new BugAssignmentModel();
            var defectLineArray = defectLine.Split('\t');
            model.BugId = defectLineArray[0];
            model.Title = defectLineArray[1];
            model.BugCategory = "Triage";
            BugAssignmentModelList.Add(model);
        }
    }
    static void DailyStatusOperations()
    {
        if (BugAssignmentModelList != null && BugAssignmentModelList.Any())
        {
            string fileNameTimeStamp = DateTime.Now.ToString("yyyyMMdd_dddd_HHmmss");
            string fileName = "TriageEmail_" + " " + fileNameTimeStamp + ".txt";
            string filePath = RepositoryProjectsPath + DailyStatusInputFolderRelativePath + fileName;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var model in BugAssignmentModelList)
            {
                var bugTitle = model.Title;
                if (bugTitle.Length > 40)
                {
                    bugTitle = "..." + bugTitle.Substring(bugTitle.Length - 40);
                }
                stringBuilder.AppendLine("------------------------------------------------------------------");
                stringBuilder.AppendLine("Triage Bug ID[Eg. 101010]: " + model.BugId);
                stringBuilder.AppendLine("Module [eg. Data Collectiion]: <Module>");
                stringBuilder.AppendLine("Activity Details [Eg. Sign and Submit Bug]: " + bugTitle);
                stringBuilder.AppendLine("Complete? Give [Y/N] (Y- Complete; N- In-Progress): Y");
            }
            stringBuilder.AppendLine("------------------------------------------------------------------");
            using (FileStream fs = File.Create(filePath))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(stringBuilder.ToString());
                fs.Write(info, 0, info.Length);
                Console.WriteLine("File: {0} is Created Successfully", fileName);
            }
        }

    }
    class BugAssignmentModel
    {
        public string BugId { get; set; }
        public string Title { get; set; }
        public string BugCategory { get; set; }
    }
}