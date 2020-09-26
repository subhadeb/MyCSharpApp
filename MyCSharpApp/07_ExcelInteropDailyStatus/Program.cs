using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using Outlook = Microsoft.Office.Interop.Outlook;
/*






*/



class Program
{
    //Configurable Paths and FileName Constants.
    public const string ExcelFilePath = @"C:\Users\subdeb\Documents\Subha_Deb_497290\Study\Dot_Net_Study\2 OOPS and C Sharp\CSharp Apps Programs  Backup\7_ExcelInteropDailyStatus\InputFiles\";
    public const string ExcelFilePathProdForSave = @"C:\Users\subdeb\Documents\ProjectWP\Other Tasks\00_Daily_Status\aa_ProgrammedExcelFile\";
    public const string InputExcelFileName = "InputFile_5.xlsx";
    public static List<string> ReceipentsEmailIdsList = new List<string>() { "abhishekkumar4@DELOITTE.com", "raparanjpe@deloitte.com", "ylimbachia@deloitte.com" };
    public const bool IsDev = false;//Set if to False for Prod

    //Application Level Variables
    static List<ConstantsModel> constantModelList;
    static List<DailyStatusModel> dailyStatusModelList;
    static DateTime StatusDate;
    static string GeneratedExcelFileNamePath;
    static void Main(string[] args)
    {
        populateConstants();
        populateStatusDate();
        int input = 0;
        Console.WriteLine("Enter 1 For ConsoleInput; 2 For TextFile Input");
        int.TryParse(Console.ReadLine(), out input);
        if (input == 1)
        {
            GetDailyStatusModelListInput();
        }
        else if (input == 2)
        {
            GetDailyStatusModelListFromInputFile();
        }
        else
        {
            Console.WriteLine("Invalid Input");
            Console.ReadKey();
            Environment.Exit(0);
        }
        ExcelOperations();
        MailOperations();
    }

    static void populateStatusDate()
    {
        var isValid = false;
        while (!isValid)
        {
            Console.WriteLine("Enter Month/Date [eg. 12/31]");
            string inputStatusDate = Console.ReadLine();
            inputStatusDate = inputStatusDate + "/2020";
            StatusDate = new DateTime();
            DateTime.TryParse(inputStatusDate, out StatusDate);
            if (StatusDate == DateTime.MinValue)
            {
                Console.WriteLine("Invalid Date Please Enter Input in Correct Format");
                Console.WriteLine();
            }
            else
            {
                isValid = true;
            }
        }
    }

    static List<DailyStatusModel> GetDailyStatusModelListInput()
    {
        dailyStatusModelList = new List<DailyStatusModel>();
        int activityNum = 1;//1 is added Temporararily so that while loop get's executed. With Do While Loop this can be ignored.
        var loopCounterForId = 1;
        while (activityNum >= 1 && activityNum <= 5)
        {
            Console.WriteLine("Enter the Input as below");
            Console.WriteLine("1. Triage | 2. Code Fix | 3. Build Activity(Smoke Test) | 4. Peer Test | 5. Ad - Hoc |||| Any other input to Save and Exit");
            activityNum = 0;//Reset to 0
            int.TryParse(Console.ReadLine(), out activityNum);
            if (activityNum >= 1 && activityNum <= 5)
            {
                DailyStatusModel dailyStatusModel = new DailyStatusModel();

                dailyStatusModel.Date = StatusDate.ToString();
                dailyStatusModel.Activity = constantModelList.FirstOrDefault(x => x.Id == activityNum).value1;
                dailyStatusModel.Id = loopCounterForId;
                loopCounterForId++;
                switch (activityNum)
                {
                    case 1://Triage
                    case 2://Code Fix
                        Console.WriteLine("Enter " + dailyStatusModel.Activity + " Bug ID[Eg. 101010]");
                        dailyStatusModel.TFSID = Console.ReadLine();
                        Console.WriteLine("Enter Module [eg. Data Collectiion]");
                        dailyStatusModel.Module = Console.ReadLine();
                        Console.WriteLine("Enter Activity Details: [Eg. Sign and Submit Bug]");
                        dailyStatusModel.ActivityDetails = Console.ReadLine();
                        Console.WriteLine("Is it Complete? Press [Y/N] (Y: Complete; N: In-Progress)");
                        if (Console.ReadLine().ToLower() == "y")
                        {
                            dailyStatusModel.Comments = "Completed";
                        }
                        else
                        {
                            dailyStatusModel.Comments = "In-Progress";
                        }

                        break;
                    case 3: //Smoke Test Build Activity
                        dailyStatusModel.Module = "WP";
                        dailyStatusModel.Activity = "Build Activity";
                        dailyStatusModel.ActivityDetails = "Smoke Test";
                        dailyStatusModel.TFSID = "N/A";
                        dailyStatusModel.Comments = "Completed";
                        break;
                    case 4: //Peer Test
                        Console.WriteLine("Enter total Number of Bugs Tested[Eg. 2]");
                        int totalPeerTestedBugs = Convert.ToInt32(Console.ReadLine());
                        List<string> peerTestedBugIds = new List<string>();
                        for (int i = 0; i < totalPeerTestedBugs; i++)
                        {
                            Console.WriteLine("Enter Peer Tested Bugid [{0}]", i + 1);
                            peerTestedBugIds.Add(Console.ReadLine());
                        }
                        dailyStatusModel.TFSID = string.Join(",", peerTestedBugIds);
                        dailyStatusModel.Module = "WP";
                        dailyStatusModel.Activity = "Peer Test";
                        dailyStatusModel.ActivityDetails = "Peer Tested Bugs";
                        dailyStatusModel.Comments = "Completed";
                        break;
                    case 5: //Ad-Hoc Task
                        Console.WriteLine("Enter Ad-Hoc Work Activity Details: [Eg. Table Column Description Analysis]");
                        dailyStatusModel.ActivityDetails = Console.ReadLine();
                        dailyStatusModel.Module = "WP";
                        dailyStatusModel.TFSID = "N/A";
                        dailyStatusModel.Comments = "Completed";
                        break;

                }

                dailyStatusModelList.Add(dailyStatusModel);
            }
            else
            {
                break;
            }
        }
        return dailyStatusModelList;
    }
    static List<DailyStatusModel> GetDailyStatusModelListFromInputFile()
    {
        Console.WriteLine();
        string inputFilePath = Environment.CurrentDirectory + @"\InputOutput\InputFile.txt";
        var fileUpdatedDateTime = File.GetLastWriteTime(inputFilePath);
        string displayDateTime = fileUpdatedDateTime.ToShortTimeString() + " " + fileUpdatedDateTime.DayOfWeek + " " + fileUpdatedDateTime.ToShortDateString();
        Console.WriteLine("InputFile File Was Last Updated On {0}", displayDateTime);
        Console.WriteLine("Press [Y] to Continue, any other key to open the containing Folder");
        if (Console.ReadLine().ToLower() != "y")
        {
            System.Diagnostics.Process.Start(Environment.CurrentDirectory + @"\InputOutput");
            Environment.Exit(0);

        }
        dailyStatusModelList = new List<DailyStatusModel>();
        //DateTime lastModified = System.IO.File.GetLastWriteTime(strFilePath);
        var loopCounterForId = 1;
        var inputFileLines = new List<string>();
        var fileStream = new FileStream(inputFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
        {
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                inputFileLines.Add(line);
                if (line == "END OF INPUT")
                {
                    break;
                }

            }
            streamReader.Close();
        }
        DailyStatusModel dailyStatusModel;
        foreach (var line in inputFileLines)
        {
            if (line.Contains("Triage Bug ID") || line.Contains("Code Fix Bug ID"))
            {
                var indexOfLine = inputFileLines.IndexOf(line);
                dailyStatusModel = new DailyStatusModel();
                dailyStatusModel.Date = StatusDate.ToString();
                if (line.Contains("Triage Bug ID"))
                {
                    dailyStatusModel.Activity = constantModelList.FirstOrDefault(x => x.Id == 1).value1;
                }
                else
                {
                    dailyStatusModel.Activity = constantModelList.FirstOrDefault(x => x.Id == 2).value1;
                }
                dailyStatusModel.TFSID = line.Substring(line.IndexOf(':') + 1).Trim();
                var lineModule = inputFileLines[indexOfLine + 1];
                dailyStatusModel.Module = lineModule.Substring(lineModule.IndexOf(':') + 1).Trim();
                var lineActivityDetails = inputFileLines[indexOfLine + 2];
                dailyStatusModel.ActivityDetails = lineActivityDetails.Substring(lineActivityDetails.IndexOf(':') + 1).Trim();
                var lineCompleted = inputFileLines[indexOfLine + 3];
                var Completed = lineCompleted.Substring(lineCompleted.IndexOf(':') + 1).Trim();
                if (Completed.ToLower() == "y")
                {
                    dailyStatusModel.Comments = "Completed";
                }
                else if (Completed.ToLower() == "n")
                {
                    dailyStatusModel.Comments = "In-Progress";
                }
                dailyStatusModel.Id = loopCounterForId;
                loopCounterForId++;
                dailyStatusModelList.Add(dailyStatusModel);
            }
            else if (line.Contains("Smoke Testing"))
            {
                dailyStatusModel = new DailyStatusModel();
                dailyStatusModel.Module = "WP";
                dailyStatusModel.Date = StatusDate.ToString();
                dailyStatusModel.Activity = constantModelList.FirstOrDefault(x => x.Id == 3).value1;
                dailyStatusModel.Id = loopCounterForId;
                loopCounterForId++;
                dailyStatusModel.ActivityDetails = "Smoke Test";
                dailyStatusModel.TFSID = "N/A";
                dailyStatusModel.Comments = "Completed";
                dailyStatusModelList.Add(dailyStatusModel);
            }
            else if (line.Contains("Peer Tested Bugs"))
            {
                dailyStatusModel = new DailyStatusModel();
                dailyStatusModel.TFSID = dailyStatusModel.TFSID = line.Substring(line.IndexOf(':') + 1).Trim();
                dailyStatusModel.Module = "WP";
                dailyStatusModel.Date = StatusDate.ToString();
                dailyStatusModel.Activity = constantModelList.FirstOrDefault(x => x.Id == 4).value1;
                dailyStatusModel.Id = loopCounterForId;
                loopCounterForId++;
                dailyStatusModel.ActivityDetails = "Peer Tested Bugs";
                dailyStatusModel.Comments = "Completed";
                dailyStatusModelList.Add(dailyStatusModel);
            }
            else if (line.Contains("Ad-Hoc Activity Details"))
            {
                dailyStatusModel = new DailyStatusModel();
                dailyStatusModel.Module = "WP";
                dailyStatusModel.Date = StatusDate.ToString();
                dailyStatusModel.Activity = constantModelList.FirstOrDefault(x => x.Id == 5).value1;
                dailyStatusModel.Id = loopCounterForId;
                loopCounterForId++;
                dailyStatusModel.TFSID = "N/A";
                dailyStatusModel.ActivityDetails = line.Substring(line.IndexOf(':') + 1).Trim();
                dailyStatusModel.Comments = "Completed";
                dailyStatusModelList.Add(dailyStatusModel);
            }
        }
        return dailyStatusModelList;
    }




    static void ExcelOperations()
    {
        Excel.Application excelApp = new Excel.Application();
        Excel.Workbook excelBook = excelApp.Workbooks.Open(ExcelFilePath + InputExcelFileName);
        Excel.Workbook excelBook2 = excelApp.Workbooks.Add(Type.Missing);

        try
        {
            Excel._Worksheet excelSheet = excelBook.Sheets[1];
            Excel.Range excelRange = excelSheet.Columns[1];
            Excel.Range sourceHeader = excelSheet.Range["A1:I1"];
            Excel.Range sourceBlankDataRow = excelSheet.Range["A10:I10"];
            var currentTime = DateTime.Now.ToShortTimeString();


            //For New Excel File
            Excel._Worksheet excelSheetNew = excelBook2.Sheets[1];
            excelSheetNew.Name = StatusDate.ToString("MM_dd_yyyy");
            Excel.Range destHeader = excelSheetNew.Range["A1:I1"];
            Excel.Range destFirstDataRow = excelSheetNew.Range["A2:I2"];
            sourceHeader.Copy(destHeader);
            excelSheetNew.Range["A1:I1"].RowHeight = 15;
            for (int i = 0; i < dailyStatusModelList.Count; i++)
            {
                var excelRangeRow = i + 2;
                string strDestRange = string.Format("A{0}:I{1}", excelRangeRow, excelRangeRow);
                Excel.Range destDailyStatusRow = excelSheetNew.Range[strDestRange];
                var dailyStatusModel = dailyStatusModelList[i];
                sourceBlankDataRow.Copy(destDailyStatusRow);
                excelSheetNew.Cells[excelRangeRow, 1] = dailyStatusModel.Id;
                excelSheetNew.Cells[excelRangeRow, 2] = dailyStatusModel.Date;
                excelSheetNew.Cells[excelRangeRow, 3] = dailyStatusModel.Track;
                excelSheetNew.Cells[excelRangeRow, 4] = dailyStatusModel.Module;
                excelSheetNew.Cells[excelRangeRow, 5] = dailyStatusModel.Practitioner;
                excelSheetNew.Cells[excelRangeRow, 6] = dailyStatusModel.Activity;
                excelSheetNew.Cells[excelRangeRow, 7] = dailyStatusModel.ActivityDetails;
                excelSheetNew.Cells[excelRangeRow, 8] = dailyStatusModel.TFSID;
                excelSheetNew.Cells[excelRangeRow, 9] = dailyStatusModel.Comments;
            }
            excelSheetNew.Columns[1].ColumnWidth = 18;
            excelSheetNew.Columns[2].ColumnWidth = 18;
            excelSheetNew.Columns[3].ColumnWidth = 18;
            excelSheetNew.Columns[4].ColumnWidth = 30;
            excelSheetNew.Columns[5].ColumnWidth = 20;
            excelSheetNew.Columns[6].ColumnWidth = 20;
            excelSheetNew.Columns[7].ColumnWidth = 18;
            excelSheetNew.Columns[8].ColumnWidth = 18;
            excelSheetNew.Columns[9].ColumnWidth = 25;
            if (IsDev)
            {
                var currentDateTime = DateTime.Now.ToString("MMddyyyy-HHmmss");
                var newFileName = "Status_" + currentDateTime + ".xlsx";
                GeneratedExcelFileNamePath = ExcelFilePath + newFileName;
                excelBook2.SaveAs2(GeneratedExcelFileNamePath);
            }
            else
            {
                var newFileName = "Status_SubhaDeb_" + StatusDate.ToString("MM_dd_yyyy") + ".xlsx";
                GeneratedExcelFileNamePath = ExcelFilePathProdForSave + newFileName;
                excelBook2.SaveAs2(GeneratedExcelFileNamePath);
            }

            Console.WriteLine("Excel File Created");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            excelBook.Save();
            excelBook.Close();
            //excelBook2.Save();
            //excelBook2.Close();
            excelApp.Quit();
        }

    }
    static void MailOperations()
    {
        Console.WriteLine("Do You want to Compose a Mail with the Same Excel Attachement? Press [Y] for Yes any other key for N");
        string input = Console.ReadLine();
        if (input.ToLower().Contains("y"))
        {
            try
            {
                // Create the Outlook application.
                Outlook.Application oApp = new Microsoft.Office.Interop.Outlook.Application();
                // Create a new mail item.
                Outlook.MailItem oMsg = (Outlook.MailItem)oApp.CreateItem(Outlook.OlItemType.olMailItem);
                // Set HTMLBody. 
                //add the body of the email
                var statusDate = StatusDate.ToString("dd MMMM");
                oMsg.Subject = "WP Daily Update – Subha Deb - " + statusDate;
                StringBuilder htmlBody = new StringBuilder();
                htmlBody.Append("Hi,<br/> <br/> PFA the Status for " + statusDate);
                htmlBody.Append("<br/> <br/> Thanks, <br> Subha Deb");
                oMsg.HTMLBody = htmlBody.ToString();
                //Add an attachment.
                int iPosition = (int)oMsg.Body.Length + 1;
                int iAttachType = (int)Outlook.OlAttachmentType.olByValue;
                //now attached the file
                Outlook.Attachment oAttach = oMsg.Attachments.Add(GeneratedExcelFileNamePath, iAttachType, iPosition);
                // Add a recipient.
                Outlook.Recipients oRecips = (Outlook.Recipients)oMsg.Recipients;
                // Change the recipient in the next line if necessary.
                foreach (var email in ReceipentsEmailIdsList)
                {
                    Outlook.Recipient oRecip = (Outlook.Recipient)oRecips.Add(email);
                    oRecip.Resolve();
                    oRecip = null;
                }
                oMsg.Display(true);
                // Send.
                //oMsg.Send();
                // Clean up.
                oRecips = null;
                oMsg = null;
                oApp = null;
            }//end of try block
            catch (Exception ex)
            {
                Console.WriteLine("Got Exception");
                Console.WriteLine(ex.ToString());
            }//end of catch
        }
    }
    static void populateConstants()
    {
        constantModelList = new List<ConstantsModel>();
        constantModelList.Add(new ConstantsModel { Id = 1, value1 = "Triage" });
        constantModelList.Add(new ConstantsModel { Id = 2, value1 = "Code Fix" });
        constantModelList.Add(new ConstantsModel { Id = 3, value1 = "Build Activity" });
        constantModelList.Add(new ConstantsModel { Id = 4, value1 = "Peer Test" });
        constantModelList.Add(new ConstantsModel { Id = 5, value1 = "Ad-Hoc" });
    }
}
class DailyStatusModel
{
    public DailyStatusModel() //All common Details are initialized.
    {
        this.Track = "WP";
        this.Practitioner = "Subha Deb";
    }
    public int Id { get; set; }
    public string Date { get; set; }
    public string Track { get; set; }
    public string Module { get; set; }
    public string Practitioner { get; set; }
    public string Name { get; set; }
    public string Activity { get; set; }
    public string ActivityDetails { get; set; }
    public string TFSID { get; set; }
    public string Comments { get; set; }
}

class ConstantsModel
{
    public int Id { get; set; }
    public string value1 { get; set; }
    public string value2 { get; set; }
}