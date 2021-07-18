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
    ReadFromInputFile(): Reads the Contents from two Input files stored in the location InputFileRelativePath and push all the lines to PanelMembersLineInput1 and PanelMembersLineInput2
    FilterDuplicatesInBothLineInputs(): Finds Duplicate Emails of both the Inputs to create PanelMembersLineInput1Unique and PanelMembersLineInput2Unique and updates sbTextToWriteInOutput for the same.
    FindCommonAndDifferentPanelMembers(): Updates to the three important Lists: PanelMembersCommonInBoth,PanelMembersInput2NotInInput1 and PanelMembersInput1NotInInput2 and updates sbTextToWriteInOutput.


    Assume InputFile1 is the existing Sharepoint list, InputFile2 is the new one to be Added to the portal
    InputFile1 Format: It should have: Id  [/t]  Email   [/t]   Level
    InputFile2 Format: It should have: Id  [/t]  Email   [/t]   Level (If Id is not present keep it -1 or -something)



*/


class Program
{
    public static string RepositoryProjectsPath = string.Empty;
    public const string InputFile1RelativePath = @"10_PanelMembersOperationsCount\bin\Debug\InputOutput_PanelMemberOperations\InputFile1.txt";
    public const string InputFile2RelativePath = @"10_PanelMembersOperationsCount\bin\Debug\InputOutput_PanelMemberOperations\InputFile2.txt";
    public const string OutputFileRelativePath = @"10_PanelMembersOperationsCount\bin\Debug\InputOutput_PanelMemberOperations\OutputFile.txt";



    static List<PanelMemberModel> PanelMembersLineInput1 = new List<PanelMemberModel>();
    static List<PanelMemberModel> PanelMembersLineInput2 = new List<PanelMemberModel>();
    static List<PanelMemberModel> PanelMembersLineInput1Unique = new List<PanelMemberModel>();
    static List<PanelMemberModel> PanelMembersLineInput2Unique = new List<PanelMemberModel>();
    static List<PanelMemberModel> PanelMembersCommonInBoth = new List<PanelMemberModel>();
    static List<PanelMemberModel> PanelMembersInput2NotInInput1 = new List<PanelMemberModel>();
    static List<PanelMemberModel> PanelMembersInput1NotInInput2 = new List<PanelMemberModel>();

    static StringBuilder sbTextToWriteInOutput = new StringBuilder();

    static void Main(string[] args)
    {
        ReadResourceFile();
        ReadFromInputFile();
        bool isWriteToFileFilterDuplicates = true;
        FilterDuplicatesInBothLineInputs(isWriteToFileFilterDuplicates);
        bool isWriteToFileFindCommonDifferent = true;
        FindCommonAndDifferentPanelMembers(isWriteToFileFindCommonDifferent);
        if (sbTextToWriteInOutput.Length > 0)
        {
            WriteToFile();
        }
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
        var fileStream1 = new FileStream(RepositoryProjectsPath + InputFile1RelativePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        using (var streamReader = new StreamReader(fileStream1, Encoding.UTF8))
        {
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                var splittedArray = line.Split('\t');
                PanelMemberModel el = new PanelMemberModel() { Id = splittedArray[0], Email = splittedArray[1].ToLower(), Level = splittedArray[2] };
                PanelMembersLineInput1.Add(el);
            }
            streamReader.Close();
        }
        var fileStream2 = new FileStream(RepositoryProjectsPath + InputFile2RelativePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        using (var streamReader = new StreamReader(fileStream2, Encoding.UTF8))
        {
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                var splittedArray = line.Split('\t');
                PanelMemberModel el = new PanelMemberModel() { Id = splittedArray[0], Email = splittedArray[1].ToLower(), Level = splittedArray[2] };
                PanelMembersLineInput2.Add(el);
            }
            streamReader.Close();
        }
    }

    public static void FilterDuplicatesInBothLineInputs(bool isWriteToFile)
    {
        var dupes1 = PanelMembersLineInput1.GroupBy(x => new { x.Email })
                   .Where(x => x.Skip(1).Any()).ToList();


        var dupes2 = PanelMembersLineInput2.GroupBy(x => new { x.Email })
                   .Where(x => x.Skip(1).Any()).ToList();

        foreach (var data in PanelMembersLineInput1)
        {
            if (!PanelMembersLineInput1Unique.Any(x => x.Email == data.Email))
            {
                PanelMembersLineInput1Unique.Add(data);
            }
        }

        foreach (var data in PanelMembersLineInput2)
        {
            if (!PanelMembersLineInput2Unique.Any(x => x.Email == data.Email))
            {
                PanelMembersLineInput2Unique.Add(data);
            }
        }
       

        if (isWriteToFile)
        {
            sbTextToWriteInOutput.AppendLine("Total PanelMembers in Input1: " + PanelMembersLineInput1.Count());
            sbTextToWriteInOutput.AppendLine("Total PanelMembers in Input2: " + PanelMembersLineInput2.Count());

            sbTextToWriteInOutput.AppendLine();
            sbTextToWriteInOutput.AppendLine("Duplicates in InputFile1: " + dupes1.Count());
            sbTextToWriteInOutput.AppendLine("Duplicates in InputFile2: " + dupes2.Count());

            if (dupes1.Count() > 0)
            {
                sbTextToWriteInOutput.AppendLine();
                sbTextToWriteInOutput.AppendLine("Below are the Duplicate Emails in InputFile1");
                foreach (var pMember in dupes1)
                {
                    sbTextToWriteInOutput.AppendLine(pMember.Key.Email);
                }
                sbTextToWriteInOutput.AppendLine();
            }
            if (dupes2.Count() > 0)
            {
                sbTextToWriteInOutput.AppendLine();
                sbTextToWriteInOutput.AppendLine("Below are the Duplicate Emails in InputFile2");
                foreach (var pMember in dupes2)
                {
                    sbTextToWriteInOutput.AppendLine(pMember.Key.Email);
                }
                sbTextToWriteInOutput.AppendLine();
            }
            sbTextToWriteInOutput.AppendLine("------------------------------------------------------------------");
        }
    }
    static void FindCommonAndDifferentPanelMembers(bool isWriteToFile)
    {
        foreach (var data in PanelMembersLineInput1Unique)
        {
            if (PanelMembersLineInput2Unique.Any(x => x.Email == data.Email))
            {
                PanelMembersCommonInBoth.Add(data);
            }
            else
            {
                PanelMembersInput1NotInInput2.Add(data);
            }
        }
        foreach (var data in PanelMembersLineInput2Unique)
        {
            if (!PanelMembersLineInput1Unique.Any(x => x.Email == data.Email))
            {
                PanelMembersInput2NotInInput1.Add(data);
            }
        }


        if (isWriteToFile)
        {
            sbTextToWriteInOutput.AppendLine();
            sbTextToWriteInOutput.AppendLine("Distinct PanelMembers in Input1: " + PanelMembersLineInput1Unique.Count());
            sbTextToWriteInOutput.AppendLine("Distinct PanelMembers in Input2: " + PanelMembersLineInput2Unique.Count());

            sbTextToWriteInOutput.AppendLine();
            sbTextToWriteInOutput.AppendLine("PanelMembers Common In Both Count: " + PanelMembersCommonInBoth.Count());
            sbTextToWriteInOutput.AppendLine("PanelMembers in Input1 Not In Input2 Count: " + PanelMembersInput1NotInInput2.Count());
            sbTextToWriteInOutput.AppendLine("PanelMembers in Input2 Not In Input1 Count: " + PanelMembersInput2NotInInput1.Count());
            sbTextToWriteInOutput.AppendLine("------------------------------------------------------------------");
            if (PanelMembersCommonInBoth.Any())
            {
                sbTextToWriteInOutput.AppendLine();
                sbTextToWriteInOutput.AppendLine("PanelMembers Common In Both");
                foreach (var data in PanelMembersCommonInBoth)
                {
                    sbTextToWriteInOutput.AppendLine(data.Id + '\t' + data.Email + '\t' + data.Level);
                }
                sbTextToWriteInOutput.AppendLine("------------------------------------------------------------------");
            }

            if (PanelMembersInput1NotInInput2.Any())
            {
                sbTextToWriteInOutput.AppendLine();
                sbTextToWriteInOutput.AppendLine("PanelMembers in Input1, not in Input2");
                foreach (var data in PanelMembersInput1NotInInput2)
                {
                    sbTextToWriteInOutput.AppendLine(data.Id + '\t' + data.Email + '\t' + data.Level);
                }
                sbTextToWriteInOutput.AppendLine("------------------------------------------------------------------");
            }

            if (PanelMembersInput2NotInInput1.Any())
            {
                sbTextToWriteInOutput.AppendLine();
                sbTextToWriteInOutput.AppendLine("PanelMembers in Input2, not in Input1");
                foreach (var data in PanelMembersInput2NotInInput1)
                {
                    sbTextToWriteInOutput.AppendLine(data.Id + '\t' + data.Email + '\t' + data.Level);
                }
                sbTextToWriteInOutput.AppendLine("------------------------------------------------------------------");
            }

            sbTextToWriteInOutput.AppendLine("------------------------------------------------------------------");
        }
        
    }

    public static void WriteToFile()
    {
        string strOutput = sbTextToWriteInOutput.ToString();
        File.WriteAllText(RepositoryProjectsPath + OutputFileRelativePath, strOutput);
        Console.WriteLine("Output File Updated");
    }
}


class PanelMemberModel
{
    public string Id { get; set; }
    public string Level { get; set; }
    public string Email { get; set; }

}