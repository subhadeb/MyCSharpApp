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

    ReadResourceFile(): Reads the Resource Files and Populates RepositoryProjectsPath which is later combined with InputFileRelativePath to Read the Input file
    ReadFromInputFile(): Reads from InputFileRelativePath and populates ListStrLineElements
    processOutput(): Takes user input to find if Multiple Files needs to be created or Multiple Folders
    ProocessAndWriteToOutputFile(): Creates Contents of Input File and Creats Multiple Input Files based on ListStrLineElements
    ProcessAndCreateFolders(): Create Multiple Folders with the Same Name as the Input Lines in ListStrLineElements


*/


class Program
{
    public static string RepositoryProjectsPath = string.Empty;
    public const string InputFileRelativePath = @"09_CreateMultipleTextFilesFromInput\bin\Debug\InputOutput_CreateMultipleFiles\InputFile.txt";
    public const string OutputFileRelativePath = @"09_CreateMultipleTextFilesFromInput\bin\Debug\InputOutput_CreateMultipleFiles\OutputFile.txt";
    public const string DirectoryFileCreationRelativePath = @"09_CreateMultipleTextFilesFromInput\bin\Debug\InputOutput_CreateMultipleFiles\FileCreationFolder\";
    static List<string> ListStrLineElements = new List<string>();
    static void Main(string[] args)
    {
        ReadResourceFile();
        ReadFromInputFile();
        processOutput();
        Console.WriteLine("End Of Application");
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

    static void processOutput()
    {
        Console.WriteLine("Press:- ");
        Console.WriteLine("1 to Create Multiple Text Files");
        Console.WriteLine("2 to create Multiple Folders");
        Console.WriteLine("3 To See Input Format");
        Console.WriteLine("Any other key to exit");
        string input = Console.ReadLine();
        if (input == "1")
        {
            ProocessAndWriteToOutputFile();
        }
        else if (input == "2")
        {
            ProcessAndCreateFolders();
        }
        else if (input == "3")
        {
            DisplayInputFormat();
        }
        else
        {
            Environment.Exit(0);
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
    static void ProcessAndCreateFolders()
    {
        foreach (var line in ListStrLineElements)
        {
            var lineAfterRemovingSpecialChars = Regex.Replace(line, @"[^0-9a-zA-Z ]+", "");
            Directory.CreateDirectory(RepositoryProjectsPath + DirectoryFileCreationRelativePath + lineAfterRemovingSpecialChars);
        }
            
    }

    static void DisplayInputFormat()
    {
        Console.WriteLine("The Input should contain a Tab");
        Console.WriteLine("The First Part before tab can contain Charecters as well as numbers");
        Console.WriteLine("The First Part may contain Special Chars(Eg stop(.)) as well");
        Console.WriteLine();
        Console.WriteLine("Example 1 Below:");
        Console.WriteLine("UDM23	Welocme to This Course");
        Console.WriteLine("UDM24	What is Microsoft");

        Console.WriteLine();
        Console.WriteLine("Example 2 Below:");
        Console.WriteLine("1	Welocme to This Course");
        Console.WriteLine("2	What is Microsoft");


    }
}