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

    
    ReadFromInputFile(): Reads the Contents form Input file store in the location InputFileRelativePath and push all the lines to ListStrLineElements
    processAndCreateScript(): Creates the SQL Script for Create Script(Based on Console Input) and Insert Script in the StringBuilder SBQueryToWrite.
    WriteToOutputFile(): If writeToFile is true, it writes SBQueryToWrite to OutputFileRelativePath


    Input Format: The Input will start from Line Five starting with the column names. Table Name should be there in the first line.



 */


class Program
{
    //Configurable Paths and FileName Constants.
    public static string RepositoryProjectsPath = string.Empty;
    public const string InputFileRelativePath = @"16_CreateSQLScripts\bin\Debug\\InputOutput_SQLScripts\InputFile.txt";
    public const string OutputFileRelativePath = @"16_CreateSQLScripts\bin\Debug\\InputOutput_SQLScripts\OutputFile.sql";

    //Application Level Variables
    static List<string> ListStrLineElements = new List<string>();
    static StringBuilder SBQueryToWrite = new StringBuilder();
    public static string TableName;

    static void Main(string[] args)
    {
        ReadResourceFile();
        ReadFromInputFile();
        if (ListStrLineElements.Count > 0)
        {
            processAndCreateScript();
            WriteToOutputFile();
        }
        Console.WriteLine("End of Application");
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
            if (de.Key.ToString() == "RepositoryProjectsPath_"+ Environment.MachineName)
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
    static void processAndCreateScript()
    {
        string firstLineTableName = ListStrLineElements.First();//First Line will have the table name
        TableName = firstLineTableName.Substring(firstLineTableName.IndexOf(':') + 1).Trim();
        var columnNamesArray = ListStrLineElements[4].Split('\t');//Fourth Line will contain the column names
        ListStrLineElements.RemoveRange(0, 5);//Remove 4 Elements(Lines) starting at position 0(Till last -------...)
        //The Table Data will start from the fifth Line.

        SBQueryToWrite = new StringBuilder();
        int input = 0;
        Console.WriteLine("Table Name: " + TableName);
        Console.WriteLine("Do you want Create table Statement Press Y For Yes any other Key For No");
        if (Console.ReadLine().ToLower() == "y")
        {

            Console.WriteLine("Datatype Input:[1: INT] [2: VARCHAR(50)] [3: VARCHAR(150)] [4 DATETIME]");
            SBQueryToWrite.AppendLine("CREATE TABLE " + TableName + "\n(");
            foreach (var column in columnNamesArray)
            {
                Console.WriteLine("Enter Datatype for Column: ["+ column + "] as Per above Datatype Input");
                //var isValid = false;
                input = 0;
                while (input <= 0 || input > 4)
                {
                    int.TryParse(Console.ReadLine(), out input);
                    if (input <= 0 || input > 4)
                    {
                        Console.WriteLine("Invalid Input Enter Data Type Again Based on Above Datatype Input");
                        Console.WriteLine();
                    }
                    else
                    {
                        switch (input)
                        {
                            case 1:
                                SBQueryToWrite.AppendLine(column + " INT,");
                                break;
                            case 2:
                                SBQueryToWrite.AppendLine(column + " VARCHAR(50),");
                                break;
                            case 3:
                                SBQueryToWrite.AppendLine(column + " VARCHAR(150),");
                                break;
                            case 4:
                                SBQueryToWrite.AppendLine(column + " DATETIME,");
                                break;
                            default:
                                break;

                        }
                    }
                }
            }
            SBQueryToWrite.Remove(SBQueryToWrite.Length - 3, 1);//Remove the last ','
            SBQueryToWrite.AppendLine(")");
            SBQueryToWrite.AppendLine();
        }



        SBQueryToWrite.AppendLine("INSERT INTO " + TableName + "\n(");
        SBQueryToWrite.AppendLine(string.Join(",", columnNamesArray) + "\n) VALUES");
        //Assiming Each Input Line was in the format(Tab or \t after 3): 3 Join our Online Learning Community
        if (ListStrLineElements != null && ListStrLineElements.Any())
        {
            foreach (var lineItem in ListStrLineElements)
            {
                var dataArray = lineItem.Split('\t');
                var dataListWithQute = new List<string>();
                foreach (var data in dataArray)
                {
                    var dataAfterRemovingSpecialChars = Regex.Replace(data, @"[^0-9a-zA-Z ,./:()-]+", "");
                    if (dataAfterRemovingSpecialChars != "NULL")
                    {
                        dataListWithQute.Add("'" + dataAfterRemovingSpecialChars + "'");
                    }
                    else
                    {
                        //For NULL, we dont need Quotation.
                        dataListWithQute.Add(dataAfterRemovingSpecialChars);
                    }
                }
                SBQueryToWrite.AppendLine("(" + string.Join(",", dataListWithQute) + "),");
            }
            if (SBQueryToWrite[SBQueryToWrite.Length - 3] == ',')
            {
                SBQueryToWrite.Remove(SBQueryToWrite.Length - 3, 1);
            }
        }
    }
    static void WriteToOutputFile()
    {
        bool writeToFile = true;//Make it true for Writing to File/During Deployment and false while debugging
        if (writeToFile)
        {
            File.WriteAllText(RepositoryProjectsPath + OutputFileRelativePath, SBQueryToWrite.ToString());
            Console.WriteLine("Output File Updated");
        }
    }
}
