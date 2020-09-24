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






*/



class Program
{
    public const string OutputFilePath = @"C:\Users\subdeb\Documents\ProjectWP\DefectFixingSIT\00Input_Copy.txt";
    public const string OutputFileRunEXEPath = @"C:\Users\subdeb\Documents\ProjectWP\DefectFixingSIT\ConsoleCSharpPrograms.exe";


    static void Main(string[] args)
    {
        MailOperations();
        Console.WriteLine("-----End of Application-----");
        Console.ReadKey();
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
            List<BugAssignmentModel> bugAssignmentModels = new List<BugAssignmentModel>();
            for (int i = 0; i < linesSplit.Length; i++)
            {
                if (linesSplit[i].ToUpper().Contains("SUBHA"))
                {
                    bugAssignmentModels.Add(new BugAssignmentModel()
                    {
                        BugId = linesSplit[i - 6],
                        Title = linesSplit[i - 4]
                    });
                }
            }
            StringBuilder sb = new StringBuilder();
            foreach (var item in bugAssignmentModels)
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

    static void MailOperations()
    {
        Application outlookApplication = new Application();
        NameSpace outlookNamespace = outlookApplication.GetNamespace("MAPI");
        MAPIFolder inboxFolder = outlookNamespace.GetDefaultFolder(OlDefaultFolders.olFolderInbox);

        //inboxFolder.Items.Restrict("[ReceivedTime] > '" + dt.ToString("MM/dd/yyyy HH:mm") + "'");

        StringBuilder stringBuilder = new StringBuilder();
        Items mailItems = inboxFolder.Items;
        Console.WriteLine("Enter Days Like \t 1- Today \t 2- Yesterday \t 3-Two Days Ago");
        int numInput = 0;
        int.TryParse(Console.ReadLine(), out numInput);
        if (numInput >= 1 && numInput <= 3)
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

        Console.WriteLine("Enter Operation Like \t 1.Triage Assignment Mail Operation \t 2. TFSIE Mail Operation");
        numInput = 0;
        int.TryParse(Console.ReadLine(), out numInput);
        if (numInput > 0 & numInput <= 2)
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
                    return;
            }
        }
        else
        {
            Console.WriteLine("Invalid Input");
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
}
