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
    CompareDataOfCommonPanelMembers(): will compare the common data of InputFile1 and InputFile2 based on different parameters.

    Assume InputFile1 is the existing Sharepoint list, InputFile2 is the new one to be Added to the portal
    InputFile1 and InputFile2 format below directly copied from Excel (If Id is not present keep it -1 or -[something])
    //FirstName	LastName	PhoneNumber	Address	Designation	Level	Location	EmailId	Capability	GenderCode	IsCertified	OfferingPortfolio   Id

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
        //DerriveFirstNameLastName();
        bool isWriteToFileFilterDuplicates = true;
        FilterDuplicatesInBothLineInputs(isWriteToFileFilterDuplicates);
        bool isWriteToFileFindCommonDifferent = true;
        FindCommonAndDifferentPanelMembers(isWriteToFileFindCommonDifferent);
        CompareDataOfCommonPanelMembers();
        if (sbTextToWriteInOutput.Length > 0)
        {
            WriteToFile();
            Console.WriteLine("Output File updated");
            Console.ReadKey();
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
                PanelMemberModel el;
                if (splittedArray.Length > 5)
                {
                    el = new PanelMemberModel();
                    el.FirstName = splittedArray[0];
                    el.LastName = splittedArray[1];
                    el.PhoneNumber = splittedArray[2];
                    el.Address = splittedArray[3];
                    el.Designation = splittedArray[4];
                    el.Level = splittedArray[5];
                    el.Location = splittedArray[6];
                    el.EmailId = splittedArray[7];
                    el.Capability = splittedArray[8];
                    el.GenderCode = splittedArray[9];
                    el.IsCertified = splittedArray[10];
                    el.OfferingPortfolio = splittedArray[11];
                    el.Id = splittedArray[12];
                }
                else if (splittedArray.Length >= 3) { //Only if Id, Email and Level were provided.
                    el = new PanelMemberModel() { Id=splittedArray[0],EmailId = splittedArray[1],Level = splittedArray[2] };
                }
                else
                {
                    el = new PanelMemberModel() { EmailId = splittedArray[0] };
                }

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
                PanelMemberModel el;
                if (splittedArray.Length > 5)
                {
                    el = new PanelMemberModel();
                    el.FirstName = splittedArray[0];
                    el.LastName = splittedArray[1];
                    el.PhoneNumber = splittedArray[2];
                    el.Address = splittedArray[3];
                    el.Designation = splittedArray[4];
                    el.Level = splittedArray[5];
                    el.Location = splittedArray[6];
                    el.EmailId = splittedArray[7];
                    el.Capability = splittedArray[8];
                    el.GenderCode = splittedArray[9];
                    el.IsCertified = splittedArray[10];
                    el.OfferingPortfolio = splittedArray[11];
                    el.Id = splittedArray[12];
                }
                else if (splittedArray.Length >= 3)
                { //Only if Id, Email and Level were provided.
                    el = new PanelMemberModel() { Id = splittedArray[0], EmailId = splittedArray[1], Level = splittedArray[2] };
                }
                else
                {
                    el = new PanelMemberModel() { EmailId = splittedArray[0] };
                }
                PanelMembersLineInput2.Add(el);
            }
            streamReader.Close();
        }
    }
    public static void DerriveFirstNameLastName()
    {
        foreach (var data in PanelMembersLineInput1)
        {
            var splitArrayFnLn = data.EmailId.Split(',');
            if (splitArrayFnLn.Length > 2 || splitArrayFnLn.Length == 1)
            {
                sbTextToWriteInOutput.AppendLine("EXCEPTION for the Panel Member: " + data.EmailId);
            }
            else
            {
                sbTextToWriteInOutput.AppendLine(splitArrayFnLn[1].Trim() + "\t" + splitArrayFnLn[0].Trim());
            }

        }
    }

    public static void FilterDuplicatesInBothLineInputs(bool isWriteToFile)
    {
        var dupes1 = PanelMembersLineInput1.GroupBy(x => new { x.EmailId })
                   .Where(x => x.Skip(1).Any()).ToList();


        var dupes2 = PanelMembersLineInput2.GroupBy(x => new { x.EmailId })
                   .Where(x => x.Skip(1).Any()).ToList();

        foreach (var data in PanelMembersLineInput1)
        {
            if (!PanelMembersLineInput1Unique.Any(x => x.EmailId.ToLower() == data.EmailId.ToLower()))
            {
                PanelMembersLineInput1Unique.Add(data);
            }
        }

        foreach (var data in PanelMembersLineInput2)
        {
            if (!PanelMembersLineInput2Unique.Any(x => x.EmailId.ToLower() == data.EmailId.ToLower()))
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
                    sbTextToWriteInOutput.AppendLine(pMember.Key.EmailId);
                }
                sbTextToWriteInOutput.AppendLine();
            }
            if (dupes2.Count() > 0)
            {
                sbTextToWriteInOutput.AppendLine();
                sbTextToWriteInOutput.AppendLine("Below are the Duplicate Emails in InputFile2");
                foreach (var pMember in dupes2)
                {
                    sbTextToWriteInOutput.AppendLine(pMember.Key.EmailId);
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
            if (PanelMembersLineInput2Unique.Any(x => x.EmailId.ToLower() == data.EmailId.ToLower()))
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
            if (!PanelMembersLineInput1Unique.Any(x => x.EmailId.ToLower() == data.EmailId.ToLower()))
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
                    sbTextToWriteInOutput.AppendLine(data.Id + '\t' + data.EmailId + '\t' + data.Level);
                }
                sbTextToWriteInOutput.AppendLine("------------------------------------------------------------------");
            }

            if (PanelMembersInput1NotInInput2.Any())
            {
                sbTextToWriteInOutput.AppendLine();
                sbTextToWriteInOutput.AppendLine("PanelMembers in Input1, not in Input2");
                foreach (var data in PanelMembersInput1NotInInput2)
                {
                    sbTextToWriteInOutput.AppendLine(data.Id + '\t' + data.EmailId + '\t' + data.Level);
                }
                sbTextToWriteInOutput.AppendLine("------------------------------------------------------------------");
            }

            if (PanelMembersInput2NotInInput1.Any())
            {
                sbTextToWriteInOutput.AppendLine();
                sbTextToWriteInOutput.AppendLine("PanelMembers in Input2, not in Input1");
                foreach (var data in PanelMembersInput2NotInInput1)
                {
                    ////FirstName	LastName	PhoneNumber	Address	Designation	Level	Location	EmailId	Capability	GenderCode	IsCertified	OfferingPortfolio   Id
                    string outputStr = data.FirstName + '\t' + data.LastName + '\t' + data.PhoneNumber + '\t' + data.Address + '\t' + data.Designation + '\t' + data.Level + '\t' + data.Location;
                    outputStr += '\t' + data.EmailId + '\t' + data.Capability + '\t' + data.GenderCode + '\t' + data.IsCertified + '\t' + data.OfferingPortfolio + '\t' + data.Id;
                    sbTextToWriteInOutput.AppendLine(outputStr);
                }
                sbTextToWriteInOutput.AppendLine("------------------------------------------------------------------");
            }

        }

    }
    static void CompareDataOfCommonPanelMembers()
    {
        if (PanelMembersCommonInBoth.Any())
        {
            sbTextToWriteInOutput.AppendLine();
            sbTextToWriteInOutput.AppendLine("The Below Common PanelMembers have the Location/Level Mismatch");
            foreach (var pmData in PanelMembersCommonInBoth)
            {
                var input1pmData = PanelMembersLineInput1Unique.FirstOrDefault(x => x.EmailId.ToLower() == pmData.EmailId.ToLower());
                var input2pmData = PanelMembersLineInput2Unique.FirstOrDefault(x => x.EmailId.ToLower() == pmData.EmailId.ToLower());
                if (input1pmData.Level != input2pmData.Level || input1pmData.Location != input2pmData.Location)
                {
                    sbTextToWriteInOutput.AppendLine(input1pmData.Id + '\t' + input2pmData.EmailId);
                }
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
    //FirstName	LastName	PhoneNumber	Address	Designation	Level	Location	EmailId	Capability	GenderCode	IsCertified	OfferingPortfolio   Id

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string Designation { get; set; }
    public string Level { get; set; }

    public string Location { get; set; }
    public string EmailId { get; set; }
    public string Capability { get; set; }

    public string GenderCode { get; set; }
    public string IsCertified { get; set; }
    public string OfferingPortfolio { get; set; }
    public string Id { get; set; }


}