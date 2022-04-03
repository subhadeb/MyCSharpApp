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
 
    ReadFromInputFile(): Reads from the Input file InputFilePath(Ignores the first line if it contains ID, it populates DefectList based on the text file. 
    ReadDataFromOutputFile(): Gets the last serial count from o/p file(OutputFilePath) and make entries for all the BugIds to ExistingBugIdList common variable. It 
                              calls GetBugIdFromLineItem to get the BugId from OutputFilePath and push to ExistingBugIdList.
    GetBugIdFromLineItem(): Splits the string based on _ and returs the second element
    FormatOutputFileForTFSIE_EMail(): Not used mostly after 12_Outlook_ReadEmailForTriage is made, If the Input text contains 'Work item Changed' when we copy
                                      the bug from TFSIE email. It Formats Bug Based on that and calls WriteToOutputFile().
    WriteToOutputFile(): Based on the boolean flag writeToFile, it appends a new line for the bug into OutputFilePath and also creates a folder in OutputDirectoryPath
    FormatOutputFileForAssignedEMail(): Not used mostly after 12_Outlook_ReadEmailForTriage is made, If the Input text starts with 6digit numbers When we copy the
                                        the bug from Daily Triage Mail Assignment,  It Formats Bug Based on that and calls WriteToOutputFile().
    FormatOutputFileForTFS(): If the Input text does not contains 'Work item Changed' or does not start with 6 Digit numbers. Meaning if we copy it from VDI TFS 
                             Bug it calls this method. It Formats Bug Based on that and calls WriteToOutputFile().
     
    
     
     
*/


public class Program2
{
    //FOR DEV, IN THE METHOD WriteToOutputFile set writeToFile = false, for Prod it is true
    public static string RepositoryProjectsPath = string.Empty;
    public static string InputFilePath = @"00Input_Copy.txt";
    public static string OutputFilePath = @"00DefectList.txt";
    public static string OutputDirectoryPath = @"";


    //Static Common Variables
    static List<string> DefectList;
    static int LastSerialCount;
    static List<string> ExistingBugIdList;
    static List<string> InsertedSerialCount;

    public static void Main2(string[] args)
    {
        var currentExeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        InputFilePath = currentExeDirectory + @"\" + InputFilePath;
        OutputFilePath = currentExeDirectory + @"\" + OutputFilePath;
        OutputDirectoryPath = currentExeDirectory + @"\";
        ReadFromInputFile();
        ReadDataFromOutputFile();
        if (DefectList.Any() && LastSerialCount > 0)
        {
            InsertedSerialCount = new List<string>();
            int tempBugIdAssignedEmail = 0;
            int.TryParse(DefectList.FirstOrDefault().Substring(0, 6), out tempBugIdAssignedEmail);//For Email Bug Assignments/Triage, the first 6chars will be the Bug Id.

            if (DefectList.FirstOrDefault().Contains("Work item Changed"))//If it is Autmated TFSIE Email, it will contain the mentioned text
            {
                FormatOutputFileForTFSIE_EMail();
            }
            else if (tempBugIdAssignedEmail > 100000 && (DefectList.FirstOrDefault().Substring(6, 4) != "\tBug"))
            {
                FormatOutputFileForAssignedEMail();
            }
            else
            {
                FormatOutputFileForTFS();
            }

            if (InsertedSerialCount.Count > 0)
            {
                Console.WriteLine(InsertedSerialCount.Count + " Record(s) Inserted with Generated Serial Count: " + string.Join(", ", InsertedSerialCount));
            }
            else
            {
                Console.WriteLine("No Records Inserted. The Records might already exist");
            }
        }
        else
        {
            Console.WriteLine("No Records Inserted. Reverify the Input/Output File");
        }
        //var EXEFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
        //System.Diagnostics.Process.Start(EXEFilePath);
        Console.ReadLine();
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
    static void ReadDataFromOutputFile()
    {
        ExistingBugIdList = new List<string>();
        var fileStream = new FileStream(OutputFilePath, FileMode.OpenOrCreate, FileAccess.Read);
        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
        {
            string LastLine = "";
            while (!streamReader.EndOfStream)
            {
                var currentLineItem = streamReader.ReadLine();
                if (!string.IsNullOrEmpty(currentLineItem))
                {
                    var currentBugId = GetBugIdFromLineItem(currentLineItem);
                    ExistingBugIdList.Add(currentBugId);
                    LastLine = currentLineItem;
                }
            }
            if (!string.IsNullOrEmpty(LastLine) && LastLine.Split('_').Length > 1 && int.TryParse(LastLine.Split('_')[0],out LastSerialCount))
            {
                LastSerialCount = Convert.ToInt32(LastLine.Split('_')[0]);//The statement is of no use but still for the if to execute added it.
            }
            else
            {
                Console.WriteLine(OutputFilePath + " : This file does not have the expected data");
                Console.WriteLine("It should have data in the below format and example");
                Console.WriteLine("Format: ");
                Console.WriteLine("{Counter}_{BugId}_Bug_{Bug Title}");
                Console.WriteLine("Example: ");
                Console.WriteLine("1_999999_Bug_Inital Bug for set up");
            }
            
            streamReader.Close();
        }
    }
    static string GetBugIdFromLineItem(string LineItem)
    {
        var LineItemElementsArray = LineItem.Split('_');
        return LineItemElementsArray[1];//The Second Array Element is the Bug Id
    }
    static void FormatOutputFileForTFSIE_EMail()
    {
        var strLineItemFromInputFile = DefectList.FirstOrDefault();
        var indexOfBug = strLineItemFromInputFile.IndexOf("Bug");
        var startIndexOfBugId = indexOfBug + 4;//Will count till Bug[Space]: 4 Characters
        var strBugId = strLineItemFromInputFile.Substring(startIndexOfBugId, 6);//Bug is of 6 Digits
        if (!ExistingBugIdList.Contains(strBugId))
        {
            StringBuilder sbLineItem = new StringBuilder();
            var startIndexOfBugTitle = startIndexOfBugId + 9;// [ - ] is there after the Bug Id before Bug Title
            var strBugTitle = strLineItemFromInputFile.Substring(startIndexOfBugTitle);
            strBugTitle = Regex.Replace(strBugTitle, @"[^0-9a-zA-Z ]+", "");
            if (strBugTitle.Length > 100)
            {
                strBugTitle = strBugTitle.Substring(0, 100);
            }
            LastSerialCount++;
            sbLineItem.Append(LastSerialCount);
            sbLineItem.Append("_" + strBugId);
            sbLineItem.Append("_" + "Bug");
            sbLineItem.Append("_" + strBugTitle);
            string strLineItem = sbLineItem.ToString();
            WriteToOutputFile(strLineItem);

        }
    }
    static void WriteToOutputFile(string strLineItem)
    {
        bool writeToFile = true;//Make it true for Writing to File/During Deployment and false while debugging
        if (writeToFile)
        {
            InsertedSerialCount.Add(strLineItem.Split('_')[0]);
            File.AppendAllText(OutputFilePath, Environment.NewLine);
            File.AppendAllText(OutputFilePath, strLineItem);
            Directory.CreateDirectory(OutputDirectoryPath + strLineItem);
        }
    }



    static void FormatOutputFileForAssignedEMail()
    {
        foreach (var strLineItemFromInputFile in DefectList)
        {
            var elementsOfDefectArray = strLineItemFromInputFile.Split('\t');//It will have two elements, BugId and T
            var strBugId = elementsOfDefectArray[0];
            var strBugTitle = elementsOfDefectArray[1];
            if (!ExistingBugIdList.Contains(strBugId))
            {
                StringBuilder sbLineItem = new StringBuilder();
                strBugTitle = Regex.Replace(strBugTitle, @"[^0-9a-zA-Z ]+", "");
                if (strBugTitle.Length > 100)
                {
                    strBugTitle = strBugTitle.Substring(0, 100);
                }
                LastSerialCount++;
                sbLineItem.Append(LastSerialCount);
                sbLineItem.Append("_" + strBugId);
                sbLineItem.Append("_" + "Bug");
                sbLineItem.Append("_" + strBugTitle);
                string strLineItem = sbLineItem.ToString();
                WriteToOutputFile(strLineItem);

            }
        }
    }

    static void FormatOutputFileForTFS()
    {

        foreach (var strLineItemFromInputFile in DefectList)
        {
            var ElementsOfDefectArray = strLineItemFromInputFile.Split('\t');
            StringBuilder sbLineItem = new StringBuilder();
            if (!ExistingBugIdList.Contains(ElementsOfDefectArray[0]))
            {
                LastSerialCount++;
                sbLineItem.Append(LastSerialCount);
                sbLineItem.Append("_" + ElementsOfDefectArray[0]);//0th Element Contains the BugId
                sbLineItem.Append("_" + ElementsOfDefectArray[1]);//1st Elemeent Contains the String 'Bug'
                var DefectTitleRemovingSpecialChars = Regex.Replace(ElementsOfDefectArray[2], @"[^0-9a-zA-Z ]+", "");//2nd Element Contains the BugTitle
                DefectTitleRemovingSpecialChars = DefectTitleRemovingSpecialChars.Length > 100 ? DefectTitleRemovingSpecialChars.Substring(0, 100) : DefectTitleRemovingSpecialChars;
                sbLineItem.Append("_" + DefectTitleRemovingSpecialChars);
                string strLineItem = sbLineItem.ToString();
                WriteToOutputFile(strLineItem);
            }
        }
    }

}