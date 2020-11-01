using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;
/*


    MailOperations(): Connects to Office Outlook, Populates emailModels and calls TriageMailOperations/TFSIEMailOperations based on User Input It also writes to the o/p file.
    TriageMailOperations(): Reads the Triage Assignment Mail, filters the Name from the list and populates BugAssignmentModelList and then crreats the StringBuilder for writing.
    TFSIEMailOperations(): Reads all the mails from TFSIE and based on the User Input, it creates the Bug Details for pushig to OutputFilePath   
    DailyStatusOperations(): Based on the common BugAssignmentModelList, I am pushing the Daily Assignment to DailyStatusInputFolderPath by creating a new file.


    Next Improvements:
    For Triage Email make it like the other two, that is filter from the list based on the the Name and sender.

*/



class Program
{
    public const string OutputFilePath = @"C:\Users\subdeb\Documents\ProjectWP\\DefectsList\00Input_Copy.txt";
    public const string OutputFileRunEXEPath = @"C:\Users\subdeb\Documents\ProjectWP\DefectsList\2_DefectFormatterApp.exe";
    public const string DailyStatusInputFolderPath = @"C:\Users\subdeb\source\repos\MyCSharpApp\MyCSharpApp\07_ExcelInteropDailyStatus\bin\Debug\InputOutput\";

    static List<BugAssignmentModel> BugAssignmentModelList;

    static void Main(string[] args)
    {
        MailOperations();
        Console.WriteLine("-----End of Application-----");
        Console.ReadKey();
    }
    static void MailOperations()
    {
        Application outlookApplication = new Application();
        NameSpace outlookNamespace = outlookApplication.GetNamespace("MAPI");
        MAPIFolder inboxFolder = outlookNamespace.GetDefaultFolder(OlDefaultFolders.olFolderInbox);

        //inboxFolder.Items.Restrict("[ReceivedTime] > '" + dt.ToString("MM/dd/yyyy HH:mm") + "'");

        StringBuilder stringBuilder = new StringBuilder();
        Items mailItems = inboxFolder.Items;
        Console.WriteLine("Enter Days Like \t 1- Today \t 2- Yesterday \t 3-Two Days Ago \t 4-Three Days Ago");
        int numInput = 0;
        int.TryParse(Console.ReadLine(), out numInput);
        if (numInput >= 1 && numInput <= 4)
        {
            numInput--;
        }
        else
        {
            return;
        }
        DateTime dt = DateTime.Now.AddDays(-numInput).Date;
        string strRestrictFilter = "[ReceivedTime] > '" + dt.ToString("MM/dd/yyyy HH:mm") + "'";
        mailItems = mailItems.Restrict(strRestrictFilter);
        mailItems.Sort("[ReceivedTime]", true);
        List<EmailModel> emailModels = new List<EmailModel>();
        int idCounter = 1;
        foreach (Object item in mailItems)
        {
            try
            {
                MailItem mailItem = (MailItem)item;
                emailModels.Add(new EmailModel()
                {
                    Id = idCounter,
                    SenderName = mailItem.SenderName,
                    To = mailItem.To,
                    Subject = mailItem.Subject,
                    ReceivedTime = mailItem.ReceivedTime,
                    Body = mailItem.Body,
                });
                idCounter++;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Got Exception but continue");
            }


        }
        Console.WriteLine("----------------------------------------------------------------------------------------------------------");
        Console.WriteLine("Id" + "\t" + "Received DateTime" + "\t" + "Sender" + "\t\t" + "Subject");
        Console.WriteLine("----------------------------------------------------------------------------------------------------------");
        foreach (var emailModel in emailModels)
        {
            var subject = emailModel.Subject;
            var senderName = emailModel.SenderName;
            if (emailModel.SenderName.Length > 15)
            {
                senderName = emailModel.SenderName.Substring(0, 15);
            }

            if (emailModel.Subject.Length > 60)
            {
                subject = emailModel.Subject.Substring(0, 60);
            }
            Console.WriteLine(emailModel.Id + "\t" + emailModel.ReceivedTime + "\t" + senderName + "\t" + subject);
        }

        Console.WriteLine();
        Console.WriteLine();

        Console.WriteLine("Enter Operation Like \t 1.Triage Assignment Mail Operation \t 2. TFSIE Mail Operation \t 3. Peer Test Mail Operation");
        numInput = 0;
        int.TryParse(Console.ReadLine(), out numInput);
        if (numInput > 0 & numInput <= 3)
        {
            string strToWrite = string.Empty;
            switch (numInput)
            {
                case 1:
                    strToWrite = TriageMailOperations(emailModels);
                    break;
                case 2:
                    strToWrite = TFSIEMailOperations(emailModels);
                    break;
                case 3:
                    PeerTestMailOperations(emailModels);
                    break;

            }
            if (string.IsNullOrEmpty(strToWrite))
                return;

            var outputFilePathSplitArray = OutputFilePath.Split('\\');
            var outputFilePathDisplay = outputFilePathSplitArray[outputFilePathSplitArray.Length - 2] + '/' + outputFilePathSplitArray[outputFilePathSplitArray.Length - 1];
            Console.WriteLine("Press 1 to push the above text to the Folder/File: " + outputFilePathDisplay);
            var outputFileRunEXEPathSplitArray = OutputFileRunEXEPath.Split('\\');
            var outputFileRunEXEPathDisplay = outputFileRunEXEPathSplitArray[outputFileRunEXEPathSplitArray.Length - 2] + '/' + outputFileRunEXEPathSplitArray[outputFileRunEXEPathSplitArray.Length - 1];
            Console.WriteLine("Press 2 to push the above text to the Above Folder/File and Also Run " + outputFileRunEXEPathDisplay);
            Console.WriteLine("Press 3 or any other Key to Exit");
            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    File.WriteAllText(OutputFilePath, strToWrite);
                    Console.WriteLine("Done!!");
                    break;
                case "2":
                    File.WriteAllText(OutputFilePath, strToWrite);
                    Process myProcess = new Process();
                    try
                    {
                        myProcess.StartInfo.UseShellExecute = false;
                        myProcess.StartInfo.FileName = OutputFileRunEXEPath;
                        myProcess.StartInfo.CreateNoWindow = true;
                        myProcess.Start();
                        Console.WriteLine("Done!!");
                    }
                    catch (System.Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                default:
                    break;
            }
            DailyStatusOperations();
        }
        else
        {
            Console.WriteLine("Invalid Input");
        }
    }
    static string TriageMailOperations(List<EmailModel> emailModels)
    {
        string strToWrite = string.Empty;
        Console.WriteLine("Enter Id of Triage Email[Eg. 12]");
        int numInput = 0;
        int.TryParse(Console.ReadLine(), out numInput);
        if (numInput > 0 && numInput <= emailModels.Count)
        {
            var emailModel = emailModels.FirstOrDefault(x => x.Id == numInput);
            Console.WriteLine("Below Are the Assignments");
            Console.WriteLine();
            //Console.WriteLine(emailModel.Body);
            emailModel.Body = emailModel.Body.Replace("\r", "");
            var linesSplit = emailModel.Body.Split('\n');
            BugAssignmentModelList = new List<BugAssignmentModel>();
            for (int i = 0; i < linesSplit.Length; i++)
            {
                if (linesSplit[i].ToUpper().Contains("SUBHA"))
                {
                    BugAssignmentModelList.Add(new BugAssignmentModel()
                    {
                        BugId = linesSplit[i - 6],
                        Title = linesSplit[i - 4],
                        BugCategory = Constants.BUGCATEGORY_TRIAGE
                    });
                }
            }
            StringBuilder sb = new StringBuilder();
            foreach (var item in BugAssignmentModelList)
            {
                sb.AppendLine(item.BugId + "\t" + item.Title);
            }
            Console.WriteLine(sb.ToString());
            strToWrite = sb.ToString();
        }
        else
        {
            Console.WriteLine("Invalid Input");
        }
        return strToWrite;
    }
    static string TFSIEMailOperations(List<EmailModel> emailModels)
    {
        string strToWrite = string.Empty;
        string senderTFSIE = "TFSIE@dhsoha.state.or.us";
        Console.WriteLine();
        Console.WriteLine("Below are the Bug Email From " + senderTFSIE);
        var tfsieEmailModels = emailModels.Where(x => x.SenderName == senderTFSIE);
        Console.WriteLine("----------------------------------------------------------------------------------------------------------");
        Console.WriteLine("Id" + "\t" + "Received DateTime" + "\t" + "Bug");
        Console.WriteLine("----------------------------------------------------------------------------------------------------------");
        foreach (var emailModel in tfsieEmailModels)
        {
            var subjectBug = emailModel.Subject;
            var senderName = emailModel.SenderName;
            if (emailModel.SenderName.Length > 15)
            {
                senderName = emailModel.SenderName.Substring(0, 15);
            }
            subjectBug = subjectBug.Replace("[EXT] IE Work Item Changed: ", "");
            if (subjectBug.Length > 80)
            {
                subjectBug = subjectBug.Substring(0, 80);
            }
            Console.WriteLine(emailModel.Id + "\t" + emailModel.ReceivedTime + "\t" + subjectBug);
        }
        Console.WriteLine();
        Console.WriteLine("Enter Id of TFSIE Email For Inserting[Eg. 12]");
        int numInput = 0;
        int.TryParse(Console.ReadLine(), out numInput);
        if (numInput > 0 && numInput <= emailModels.Count)
        {
            var emailModel = emailModels.FirstOrDefault(x => x.Id == numInput);
            var subject = emailModel.Subject;
            var bugCategory = Constants.BUGCATEGORY_TRIAGE;
            if (emailModel.Body.Contains("Status\t Assigned to Developer"))
            {
                bugCategory = Constants.BUGCATEGORY_ToBeFixed;
            }
            var changedFieldsBodyIndex = emailModel.Body.IndexOf("Changed fields");
            var changedFiedsDisplay = emailModel.Body.Remove(0, 988);
            var notesAtBottomIndex = changedFiedsDisplay.IndexOf("Notes:");
            changedFiedsDisplay = changedFiedsDisplay.Remove(notesAtBottomIndex);
            Console.WriteLine();
            Console.Write("Below Are the ");
            Console.WriteLine(changedFiedsDisplay);
            Console.WriteLine();
            Console.WriteLine("Below is the Bug Detail");
            Console.WriteLine();
            var indexOfBug = subject.IndexOf("Bug");
            var startIndexOfBugId = indexOfBug + 4;//Will count till Bug[Space]: 4 Characters
            var strBugId = subject.Substring(startIndexOfBugId, 6);//Bug is of 6 Digits
            var startIndexOfBugTitle = startIndexOfBugId + 9;// [ - ] is there after the Bug Id before Bug Title
            var strBugTitle = subject.Substring(startIndexOfBugTitle);
            strBugTitle = Regex.Replace(strBugTitle, @"[^0-9a-zA-Z ]+", "");
            if (strBugTitle.Length > 100)
            {
                strBugTitle = strBugTitle.Substring(0, 100);
            }
            BugAssignmentModelList = new List<BugAssignmentModel>();
            BugAssignmentModelList.Add(new BugAssignmentModel()
            {
                BugId = strBugId,
                Title = strBugTitle,
                BugCategory = bugCategory
            });

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(strBugId + "\t" + strBugTitle);
            Console.WriteLine(sb.ToString());
            strToWrite = sb.ToString();
        }
        else
        {
            Console.WriteLine("Invalid Input");
        }
        return strToWrite;
    }

    static void PeerTestMailOperations(List<EmailModel> emailModels)
    {
        string strToWrite = string.Empty;
        List<string> senderNames = new List<string>() { "Limbachia, Yogesh", "Limbachia, Yogesh" };
        Console.WriteLine();
        Console.WriteLine("Below are the Peer Testing Emails");

        var peerTestEmails = emailModels.Where(x => senderNames.Contains(x.SenderName) && x.Subject.ToLower().Contains("peer test")).ToList();

        Console.WriteLine("----------------------------------------------------------------------------------------------------------");
        Console.WriteLine("Id" + "\t" + "Received DateTime" + "\t" + "Subject");
        Console.WriteLine("----------------------------------------------------------------------------------------------------------");
        foreach (var emailModel in peerTestEmails)
        {
            var mailSubject = emailModel.Subject;
            var senderName = emailModel.SenderName;
            if (emailModel.SenderName.Length > 15)
            {
                senderName = emailModel.SenderName.Substring(0, 15);
            }
            if (mailSubject.Length > 80)
            {
                mailSubject = mailSubject.Substring(0, 80);
            }
            Console.WriteLine(emailModel.Id + "\t" + emailModel.ReceivedTime + "\t" + mailSubject);
        }
        Console.WriteLine();
        Console.WriteLine("Enter Id of Peer Test Email [Eg. 12]");
        int numInput = 0;
        int.TryParse(Console.ReadLine(), out numInput);
        if (numInput > 0 && numInput <= emailModels.Count)
        {
            var emailModel = emailModels.FirstOrDefault(x => x.Id == numInput);
            emailModel.Body = emailModel.Body.Replace("\r", "");
            var linesSplit = emailModel.Body.Split('\n');
            BugAssignmentModelList = new List<BugAssignmentModel>();
            for (int i = 0; i < linesSplit.Length; i++)
            {
                if (linesSplit[i].ToUpper().Contains("SUBHA"))
                {
                    BugAssignmentModelList.Add(new BugAssignmentModel()
                    {
                        BugId = linesSplit[i - 6],
                        Title = linesSplit[i - 4],
                        BugCategory = Constants.BUGCATEGORY_PeerTest
                    });
                }
            }
            StringBuilder sb = new StringBuilder();
            List<string> bugIdList = new List<string>();
            foreach (var item in BugAssignmentModelList)
            {
                sb.AppendLine(item.BugId + "\t" + item.Title);
                bugIdList.Add(item.BugId);
            }
            Console.WriteLine();
            Console.WriteLine("Below is the Peer Test Bug Assignment Detail");
            Console.WriteLine();
            Console.WriteLine(sb.ToString());
            Console.WriteLine();
            Console.WriteLine("Do you want to Create a Textfile with the Bug Assignment, Press Y to Create");
            
            if (Console.ReadLine().ToLower() == "y")
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("------------------------------------------------------------------");
                string bugIdCSV = string.Join(",", bugIdList);
                stringBuilder.AppendLine("Peer Tested Bugs[Comma Separated]: " + bugIdCSV);
                stringBuilder.AppendLine("------------------------------------------------------------------");
                string fileNameTimeStamp = DateTime.Now.ToString("yyyyMMdd_dddd_HHmm");
                string fileName = "TriageEmail_" + " " + fileNameTimeStamp + "_PeerTest.txt";
                string filePath = DailyStatusInputFolderPath + fileName;
                using (FileStream fs = File.Create(filePath))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(stringBuilder.ToString());
                    fs.Write(info, 0, info.Length);
                    Console.WriteLine("File: {0} is Created Successfully", fileName);
                }
            }
        }
        else
        {
            Console.WriteLine("Invalid Input");
        }
        Console.WriteLine("-----End of Application-----");
        Console.ReadKey();
        Environment.Exit(0);
    }



    static void DailyStatusOperations()
    {
        if (BugAssignmentModelList != null && BugAssignmentModelList.Any())
        {
            Console.WriteLine("Do you want to Create Text File For Daily Status Input, Press Y to Create any other key to exit");
            if (Console.ReadLine().ToLower() == "y")
            {
                string fileNameTimeStamp = DateTime.Now.ToString("yyyyMMdd_dddd_HHmm");
                string fileName = "TriageEmail_" + " " + fileNameTimeStamp + ".txt";
                string filePath = DailyStatusInputFolderPath + fileName;
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

    }
}
class EmailModel
{
    public int Id { get; set; }
    public string SenderName { get; set; }
    public string To { get; set; }
    public string Subject { get; set; }
    public DateTime ReceivedTime { get; set; }
    public string Body { get; set; }
}
class BugAssignmentModel
{
    public string BugId { get; set; }
    public string Title { get; set; }
    public string BugCategory { get; set; }
}
public static class Constants
{
    public const string BUGCATEGORY_TRIAGE = "Triage";
    public const string BUGCATEGORY_ToBeFixed = "ToBeFixed";
    public const string BUGCATEGORY_PeerTest = "PeerTest";
}
