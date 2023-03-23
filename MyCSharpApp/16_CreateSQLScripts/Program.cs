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

    NOTE: For running the project initially, Keep/copy the folder InputOutput_SQLScripts near exe file under Debug.
    
    ReadFromInputFile(): Reads the Contents form Input file store in the location InputFileRelativePath and push all the lines to ListStrLineElements
    processAndCreateScript(): Creates the SQL Script for Create Script(Based on Console Input) and Insert Script in the StringBuilder SBQueryToWrite.
    WriteToOutputFile(): If writeToFile is true, it writes SBQueryToWrite to OutputFileRelativePath


    Input Format: The Input will start from Line Five starting with the column names. Table Name should be there in the first line.



 */


class Program
{
    //Configurable Paths and FileName Constants.
    public static string RepositoryProjectsPath = string.Empty;
    public const string InputFileRelativePath = @"InputOutput_SQLScripts\InputFile.txt";
    public const string OutputFileRelativePath = @"InputOutput_SQLScripts\OutputFile.sql";
    


    //Application Level Variables
    static List<string> ListStrLineElements = new List<string>();
    static StringBuilder SBQueryToWrite = new StringBuilder();
    public static string TableName;
    public static string InputFilePath;
    public static string OutputFilePath;

    static void Main(string[] args)
    {
        PopulateInputOutputFilePath();
        ReadFromInputFile();
        if (ListStrLineElements.Count > 0)
        {
            processAndCreateScript();
            WriteToOutputFile();
        }
        Console.WriteLine("End of Application");
        Console.ReadKey();

    }
    static void PopulateInputOutputFilePath()
    {
        var currentExeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        InputFilePath = currentExeDirectory + @"\" + InputFileRelativePath;
        OutputFilePath = currentExeDirectory + @"\" + OutputFileRelativePath;
    }

    static void ReadFromInputFile()
    {
        var fileStream = new FileStream(InputFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
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
        Console.WriteLine("Press 'C' for Create+Insert table statement; Press 'I' for Insert(only) statement; Press 'U' for Update statement");
        string inputLowerCase = Console.ReadLine().ToLower();
        if (inputLowerCase == "c")
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

        if (inputLowerCase == "c" || inputLowerCase == "i") 
        {
            var insertStatementLine1 = "INSERT INTO " + TableName + "\n(";
            var insertStatementLine2 = string.Join(",", columnNamesArray) + "\n) VALUES";

            SBQueryToWrite.AppendLine(insertStatementLine1);
            SBQueryToWrite.AppendLine(insertStatementLine2);
            //Assiming Each Input Line was in the format(Tab or \t after 3): 3 Join our Online Learning Community
            if (ListStrLineElements != null && ListStrLineElements.Any())
            {
                foreach (var lineItem in ListStrLineElements)
                {
                    var dataArray = lineItem.Split('\t');
                    var dataListWithQute = new List<string>();
                    foreach (var data in dataArray)
                    {
                        var dataAfterRemovingSpecialChars = Regex.Replace(data, @"[^0-9a-zA-Z ,./:()-_@&]+", "");
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
                    if (ListStrLineElements.IndexOf(lineItem) > 0 && ListStrLineElements.IndexOf(lineItem) % 900 == 0)
                    {
                        //Insert only allows upto 1000 Elements at a time
                        if (SBQueryToWrite[SBQueryToWrite.Length - 3] == ',')
                        {
                            SBQueryToWrite.Remove(SBQueryToWrite.Length - 3, 1);
                        }
                        SBQueryToWrite.AppendLine();
                        SBQueryToWrite.AppendLine();
                        SBQueryToWrite.AppendLine(insertStatementLine1);
                        SBQueryToWrite.AppendLine(insertStatementLine2);
                    }
                }
                if (SBQueryToWrite[SBQueryToWrite.Length - 3] == ',')
                {
                    SBQueryToWrite.Remove(SBQueryToWrite.Length - 3, 1);
                }
            }
        }
        if (inputLowerCase == "u")
        {
            Console.WriteLine();
            Console.WriteLine("Is First Column '" + columnNamesArray[0] + "' the Primary-Key/Where-Clause-Colum? Press 'Y' for Yes any other key for No");
            var isFirstColumnPrimaryKey = Console.ReadLine().ToLower() == "y";

            if (ListStrLineElements != null && ListStrLineElements.Any())
            {
                string firstColumnVal = null;
                foreach (var lineItem in ListStrLineElements)
                {
                    var dataArray = lineItem.Split('\t');
                    if (string.IsNullOrEmpty(dataArray[0]))//If the value is blank/"" we need to loop with invalid entry
                    {
                        continue;
                    }
                    SBQueryToWrite.AppendLine("UPDATE " + TableName);
                    SBQueryToWrite.AppendLine("SET");

                    for (int i = 0; i < dataArray.Length; i++)
                    {

                        var data = dataArray[i];
                        var columnName = columnNamesArray[i];

                        var dataAfterRemovingSpecialChars = Regex.Replace(data, @"[^0-9a-zA-Z ,./:()-_@&]+", "");
                        if (dataAfterRemovingSpecialChars != "NULL")
                        {
                            dataAfterRemovingSpecialChars = "'" + dataAfterRemovingSpecialChars + "'";
                        }
                        if (i == 0)
                        {
                            firstColumnVal = dataAfterRemovingSpecialChars;
                        }
                        if (i > 0 || (i == 0 && !isFirstColumnPrimaryKey))
                        {
                            SBQueryToWrite.AppendLine(columnName + " = " + dataAfterRemovingSpecialChars + ",");
                        }
                    }
                    if (SBQueryToWrite[SBQueryToWrite.Length - 3] == ',')
                    {
                        SBQueryToWrite.Remove(SBQueryToWrite.Length - 3, 1);
                    }
                    if (isFirstColumnPrimaryKey && !string.IsNullOrEmpty(firstColumnVal))
                    {
                        SBQueryToWrite.AppendLine("WHERE " + columnNamesArray[0] + " = " + firstColumnVal + "");
                    }
                    else
                    {
                        SBQueryToWrite.AppendLine("WHERE 1 = 0");
                    }
                    SBQueryToWrite.AppendLine("---------------------------------------------------------------");
                    SBQueryToWrite.AppendLine();

                }

            }
        }
        else
        {
            Console.WriteLine("Invalid Input");
            Console.ReadKey();
            Environment.Exit(0);
        }

    }
    static void WriteToOutputFile()
    {
        bool writeToFile = true;//Make it true for Writing to File/During Deployment and false while debugging
        if (writeToFile)
        {
            File.WriteAllText(OutputFilePath, SBQueryToWrite.ToString());
            Console.WriteLine("Output File Updated");
        }
    }
}
