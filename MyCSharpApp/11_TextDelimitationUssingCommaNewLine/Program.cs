using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
/*

    ReadResourceFile(): Reads the Resource FIle and populated the Project path which is later appended with InputFileRelativePath/OutputFileRelativePath
    ReadFromInputFile(): Reads the Contents form Input file store in the location InputFileRelativePath and push all the lines to ListStrLineElements
    ProocessAndWriteToOutputFile(): Processes the Lines of ListStrLineElements and writes the contents to OutputFileRelativePath
    


*/


class Program
{

    public static string RepositoryProjectsPath = string.Empty;
    public const string InputFileRelativePath = @"11_TextDelimitationUssingCommaNewLine\bin\Debug\InputOutput_TextDelim\InputFile.txt";
    public const string OutputFileRelativePath = @"11_TextDelimitationUssingCommaNewLine\bin\Debug\InputOutput_TextDelim\OutputFile.txt";

    static List<string> ListStrLineElements = new List<string>();
    static void Main(string[] args)
    {
        ReadResourceFile();
        ReadFromInputFile();
        if (ListStrLineElements.Count > 0)
        {
            ProocessAndWriteToOutputFile();
        }
        else
        {
            Console.WriteLine("No Data In Input File in the path: " + InputFileRelativePath);
        }
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
    static void ProocessAndWriteToOutputFile()
    {
        Console.WriteLine("Should have the Input and output file in the below paths");
        Console.WriteLine(InputFileRelativePath);
        Console.WriteLine(OutputFileRelativePath);
        Console.WriteLine();
        Console.WriteLine("For Processing, Ennter the Input of your choice [Eg. 2]");
        Console.WriteLine("1. New Line to Comma Separated(Input Text file should have all the elements Line by Line)");
        Console.WriteLine("2. Tabbed to Comma Separated(Input Text file should have all the elements Separated by Tabs- Same as Excel Columns)");
        Console.WriteLine("3. Comma Separated to New Line(Input Text file should have all the elements Separated by Comma)");
        Console.WriteLine("4. New Line to New Line With Comma Separated(Input Text file should have all the elements Line by Line)");
        string userInput = Console.ReadLine();
        StringBuilder sbText = new StringBuilder();
        string processedString = string.Empty;
        switch (userInput)
        {
            case "1":
                processedString = String.Join(",", ListStrLineElements);
                break;
            case "2":
                var elementsArray2 = ListStrLineElements[0].Split('\t');
                processedString = String.Join(",", elementsArray2);
                break;
            case "3":
                var elementsArray3 = ListStrLineElements[0].Split(',');
                processedString = String.Join("\n", elementsArray3);
                break;
            case "4":
                for (int i = 0; i < ListStrLineElements.Count; i++)
                {
                    if (i == 0)
                    {
                        processedString = ListStrLineElements[i] + '\n';
                    }
                    else
                    {
                        processedString = processedString + ',' + ListStrLineElements[i] + '\n';
                    }
                }
                break;
        }
        sbText.Append(processedString);
        Console.WriteLine(sbText);
        string strOutput = sbText.ToString();
        File.WriteAllText(RepositoryProjectsPath + OutputFileRelativePath, strOutput);
        Console.WriteLine("Output File Updated");
        Console.ReadKey();
    }
}