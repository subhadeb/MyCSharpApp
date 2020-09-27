﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


class Program
{
    //FOR DEV, IN THE METHOD WriteToOutputFile set writeToFile = false, for Prod it is true
    public const string InputFilePath = @"C:\Users\subdeb\Documents\ProjectWP\DefectsList\00Input_Copy.txt";
    public const string OutputFilePath = @"C:\Users\subdeb\Documents\ProjectWP\DefectsList\00DefectListSIT.txt";
    public const string OutputDirectoryPath = @"C:\Users\subdeb\Documents\ProjectWP\DefectsList\";


    //Static Common Variables
    static List<string> DefectList;
    static int LastSerialCount;
    static List<string> ExistingBugIdList;
    static List<string> InsertedSerialCount;

    static void Main(string[] args)
    {
        ReadFromInputFile();//This Method reads the input file and make entries to the DefectList common variable
        ReadDataFromOutputFile();//This method gets the last serial count from o/p file and make entries for all the BugIds to ExistingBugIdList common variable
        if (DefectList.Any())
        {
            InsertedSerialCount = new List<string>();
            int tempBugIdAssignedEmail = 0;
            int.TryParse(DefectList.FirstOrDefault().Substring(0, 6), out tempBugIdAssignedEmail);//For Email Bug Assignments/Triage, the first 6chars will be the Bug Id.

            if (DefectList.FirstOrDefault().Contains("Work item Changed"))//If it is Autmated TFSIE Email, it will contain the mentioned text
            {
                FormatOutputFileForTFSIE_EMail();
            }
            else if (tempBugIdAssignedEmail > 100000 && (DefectList.FirstOrDefault().Substring(6, 4) != "\tBug"))//Currently All BugIds are of 6 Digits greater than 100000
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
            Console.WriteLine("No Records Inserted. Reverify the Input File");
        }
        //var EXEFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
        //System.Diagnostics.Process.Start(EXEFilePath);
        Console.ReadLine();
    }
    static void WriteToOutputFile(string strLineItem)
    {
        bool writeToFile = true;//Make it true for Writing to File/During Deployment and false while debugging
        if (writeToFile)
        {
            InsertedSerialCount.Add(strLineItem.Substring(0, 3));
            File.AppendAllText(OutputFilePath, Environment.NewLine);
            File.AppendAllText(OutputFilePath, strLineItem);
            Directory.CreateDirectory(OutputDirectoryPath + strLineItem);
        }
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
            LastSerialCount = Convert.ToInt32(LastLine.Substring(0, 3));
            streamReader.Close();
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
    static string GetBugIdFromLineItem(string LineItem)
    {
        var LineItemElementsArray = LineItem.Split('_');
        return LineItemElementsArray[1];//The Second Array Element is the Bug Id
    }
}