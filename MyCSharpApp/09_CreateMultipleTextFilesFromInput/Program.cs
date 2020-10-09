using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
/*






*/


class Program
{
    public static string RepositoryProjectsPath = string.Empty;
    public const string InputFileRelativePath = @"09_CreateMultipleTextFilesFromInput\bin\Debug\InputOutput\InputFile.txt";
    public const string OutputFileRelativePath = @"09_CreateMultipleTextFilesFromInput\bin\Debug\InputOutput\OutputFile.txt";
    public const string DirectoryFileCreationRelativePath = @"09_CreateMultipleTextFilesFromInput\bin\Debug\InputOutput\FileCreationFolder\";
    static List<string> ListStrLineElements = new List<string>();
    static void Main(string[] args)
    {
        ReadResourcFile();
        ReadFromInputFile();
        ProocessAndWriteToOutputFile();
    }
    static void ReadResourcFile()
    {
        //Make sure the resourFile have access modifier as public and System.Forms.Dll is imported for ResXResourceReader to work
        var resourceFileRelativePath = @"MyCSharpApp\MyCSharpApp\MyCSharpApp\Resources\ResourcesFile.resx";
        var executingAssemblyPath = Assembly.GetExecutingAssembly().Location;
        var firstIndexOfMyCSharpApp = executingAssemblyPath.IndexOf("MyCSharpApp");
        string resourceFilePath = executingAssemblyPath.Substring(0, firstIndexOfMyCSharpApp) + resourceFileRelativePath;
        ResXResourceReader rsxr = new ResXResourceReader(resourceFilePath);
        foreach (DictionaryEntry de in rsxr)
        {
            if (de.Key.ToString() == "RepositoryProjectsPath")
            {
                RepositoryProjectsPath = de.Value.ToString();
            }
        }
        rsxr.Close();
    }
    static void ReadFromInputFile()
    {
        var fileStream = new FileStream(RepositoryProjectsPath + InputFileRelativePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
        {
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                ListStrLineElements.Add(line);
            }
            streamReader.Close();
        }
    }
    static void ProocessAndWriteToOutputFile()
    {
        StringBuilder sbQuery = new StringBuilder();
        sbQuery.AppendLine();
        sbQuery.AppendLine();
        sbQuery.AppendLine("---------------------------------------------------------------");
        sbQuery.AppendLine();
        sbQuery.AppendLine();
        sbQuery.AppendLine("---------------------------------------------------------------");
        sbQuery.AppendLine();
        sbQuery.AppendLine();
        sbQuery.AppendLine("---------------------------------------------------------------");
        sbQuery.AppendLine();
        sbQuery.AppendLine();
        sbQuery.AppendLine("---------------------------------------------------------------");
        sbQuery.AppendLine();
        sbQuery.AppendLine();
        //Assiming Each Input ine was in the format:- 3	Join our Online Learning Community
        foreach (var lines in ListStrLineElements)
        {
            var linesSplitArray = lines.Split('\t');
            var titleAfterRemovingSpecialChars = Regex.Replace(linesSplitArray[1], @"[^0-9a-zA-Z ]+", "");
            string fileName = RepositoryProjectsPath + DirectoryFileCreationRelativePath + linesSplitArray[0] + " " + titleAfterRemovingSpecialChars + ".txt";
            // Create a new file     
            using (FileStream fs = File.Create(fileName))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(sbQuery.ToString());
                fs.Write(info, 0, info.Length);
            }
        }
    }
}