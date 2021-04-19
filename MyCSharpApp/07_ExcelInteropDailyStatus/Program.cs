using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using Outlook = Microsoft.Office.Interop.Outlook;
/*

    DailyStatusModel: Mail Model for Story each Status Items like Triage/Peer test etc. Adding Track as WP and Practitioner as my name in the constructor itself.

    populateConstants(): Populates the global static list constantModelList, For the Different Activities(Can be replced with Constants in static class later).
    populateStatusDate(): Populates the global static DateTime StatusDate, using an while to to iterate until the StatusDate is a valid date.
    GetDailyStatusModelListInput(): Executed if the Console input was to read the Status Details from console. While loop exceuted to take n number of status details. It populates dailyStatusModelList
    GetDailyStatusModelListFromInputFile(): Executed if the console input was to read from Text file. It reads from InputFile.txt and populates dailyStatusModelList
    ShowValidationsAndExit(): If there would be some invalid values in the Input file, The validations will displayed in Red and the Solution would be stopped, if no validations the solution will continue
    ExcelOperations(): with Microsoft.Office.Interop.Excel, it creates the excel file for Daily Status based on dailyStatusModelList
    MailOperations(): with Microsoft.Office.Interop.Outlook, it Composes a new email adding the Recipents from ReceipentsEmailIdsList and attaches the newly created Excel.


*/
//Next Changes: 


class Program
{
    //Configurable Paths and FileName Constants.
    public const string ExcelFilePathProdForSave = @"C:\Users\subdeb\Documents\ProjectWP\Other Tasks\00_Daily_Status\aa_ProgrammedExcelFile\2021\";
    public const string InputExcelFileName = "InputFile_Sample.xlsx";
    public const string InputTextFilePath = @"\InputOutput_DailyStatus\InputFile.txt";
    public static List<string> ReceipentsEmailIdsList = new List<string>() { "abhishekkumar4@DELOITTE.com", "raparanjpe@deloitte.com"};
    public const bool IsDev = false;//Set if to False for Prod

    //Application Level Variables
    static List<DailyStatusModel> dailyStatusModelList;
    static List<string> inputValidationsList;
    static DateTime StatusDate;
    static string GeneratedExcelFileNamePath;
    static void Main(string[] args)
    {
        populateStatusDate();
        GetDailyStatusModelListFromInputFile();
        if (inputValidationsList != null && inputValidationsList.Any())
        {
            ShowValidationsAndExit();
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
            inputStatusDate = inputStatusDate + "/" + DateTime.Now.Year;
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
   
    static List<DailyStatusModel> GetDailyStatusModelListFromInputFile()
    {
        inputValidationsList = new List<string>();
        Console.WriteLine();
        string inputFilePath = Environment.CurrentDirectory + InputTextFilePath;
        var fileUpdatedDateTime = File.GetLastWriteTime(inputFilePath);
        string displayDateTime = fileUpdatedDateTime.ToShortTimeString() + " " + fileUpdatedDateTime.DayOfWeek + " " + fileUpdatedDateTime.ToShortDateString();
        Console.WriteLine("InputFile File Was Last Updated On {0}", displayDateTime);
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
                    dailyStatusModel.Activity = Constants.ActivityTriage;
                }
                else
                {
                    dailyStatusModel.Activity = Constants.ActivityCodeFix;
                }
                dailyStatusModel.TFSID = line.Substring(line.IndexOf(':') + 1).Trim();
                if (dailyStatusModel.TFSID == "<BugId>")
                {
                    inputValidationsList.Add("Invalid: " + line);
                }
                var lineModule = inputFileLines[indexOfLine + 1];
                dailyStatusModel.Module = lineModule.Substring(lineModule.IndexOf(':') + 1).Trim();
                if (dailyStatusModel.Module == "<Module>")
                {
                    inputValidationsList.Add("Invalid: " + lineModule);
                }
                var lineActivityDetails = inputFileLines[indexOfLine + 2];
                dailyStatusModel.ActivityDetails = lineActivityDetails.Substring(lineActivityDetails.IndexOf(':') + 1).Trim();
                if (dailyStatusModel.ActivityDetails == "<Details>")
                {
                    inputValidationsList.Add("Invalid: " + lineActivityDetails);
                }
                else if (dailyStatusModel.ActivityDetails.StartsWith("..."))
                {
                    inputValidationsList.Add("Invalid: Activity Details Starts with ...");
                }
                var lineCompleted = inputFileLines[indexOfLine + 3];
                var Completed = lineCompleted.Substring(lineCompleted.IndexOf(':') + 1).Trim();
                if (Completed == "<Y/N>")
                {
                    inputValidationsList.Add("Invalid: " + lineCompleted);
                }

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
                dailyStatusModel.Activity = Constants.ActivityBuildActivity;
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
                dailyStatusModel.Activity = Constants.ActivityPeerTest;
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
                dailyStatusModel.Activity = Constants.ActivityAdHoc;
                dailyStatusModel.Id = loopCounterForId;
                loopCounterForId++;
                dailyStatusModel.TFSID = "N/A";
                dailyStatusModel.ActivityDetails = line.Substring(line.IndexOf(':') + 1).Trim();
                var indexOfLine = inputFileLines.IndexOf(line);
                var lineCompleted = inputFileLines[indexOfLine + 1];
                var Completed = lineCompleted.Substring(lineCompleted.IndexOf(':') + 1).Trim();
                if (Completed == "<Y/N>")
                {
                    inputValidationsList.Add("Invalid: " + lineCompleted);
                }
                if (Completed.ToLower() == "y")
                {
                    dailyStatusModel.Comments = "Completed";
                }
                else if (Completed.ToLower() == "n")
                {
                    dailyStatusModel.Comments = "In-Progress";
                }
                dailyStatusModelList.Add(dailyStatusModel);
            }
            else if (line.Contains("Worked On CR"))
            {
                dailyStatusModel = new DailyStatusModel();
                dailyStatusModel.Module = "WP";
                dailyStatusModel.Date = StatusDate.ToString();
                dailyStatusModel.Activity = Constants.ActivityCR;
                dailyStatusModel.Id = loopCounterForId;
                loopCounterForId++;
                dailyStatusModel.TFSID = "N/A";
                dailyStatusModel.ActivityDetails = line.Substring(line.IndexOf(':') + 1).Trim();
                if (dailyStatusModel.ActivityDetails == "<CR Title>")
                {
                    inputValidationsList.Add("Invalid: " + line);
                }
                var indexOfLine = inputFileLines.IndexOf(line);
                var lineCompleted = inputFileLines[indexOfLine + 1];
                var Completed = lineCompleted.Substring(lineCompleted.IndexOf(':') + 1).Trim();
                if (Completed == "<Y/N>")
                {
                    inputValidationsList.Add("Invalid: " + lineCompleted);
                }

                if (Completed.ToLower() == "y")
                {
                    dailyStatusModel.Comments = "Completed";
                }
                else if (Completed.ToLower() == "n")
                {
                    dailyStatusModel.Comments = "In-Progress";
                }
                dailyStatusModelList.Add(dailyStatusModel);
            }
        }
        return dailyStatusModelList;
    }
    static void ShowValidationsAndExit()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Below are the validations");
        foreach (var validationItem in inputValidationsList)
        {
            Console.WriteLine(validationItem);
        }
        Console.ResetColor();
        Console.ReadKey();
        Environment.Exit(0);
    }
    static void ExcelOperations()
    {
        int spinnerInterval = 200;
        var spinner = new Spinner(spinnerInterval);
        spinner.Start();
        Excel.Application excelApp = new Excel.Application();
        Excel.Workbook excelBook = excelApp.Workbooks.Open(Environment.CurrentDirectory + @"\InputOutput_DailyStatus\" + InputExcelFileName);
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
                var newFileName = "Dev_" + currentDateTime + ".xlsx";
                GeneratedExcelFileNamePath = ExcelFilePathProdForSave + newFileName;
                excelBook2.SaveAs2(GeneratedExcelFileNamePath);
            }
            else
            {
                var newFileName = "Status_SubhaDeb_" + StatusDate.ToString("MM_dd_yyyy") + ".xlsx";
                GeneratedExcelFileNamePath = ExcelFilePathProdForSave + newFileName;
                excelBook2.SaveAs2(GeneratedExcelFileNamePath);
            }
            Console.WriteLine();
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
            spinner.Stop();
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


class Constants
{
    public const string ActivityTriage = "Triage";
    public const string ActivityCodeFix = "Code Fix";
    public const string ActivityBuildActivity = "Build Activity";
    public const string ActivityPeerTest = "Peer Test";
    public const string ActivityAdHoc = "Ad-Hoc";
    public const string ActivityCR = "CR";
}
public class Spinner : IDisposable
{
    private const string Sequence = @"/-\|";
    private int counter = 0;
    private readonly int delay;
    private bool active;
    private readonly Thread thread;

    public Spinner(int delay)
    {
        this.delay = delay;
        thread = new Thread(Spin);
    }

    public void Start()
    {
        Console.Write("Wait ");
        active = true;
        if (!thread.IsAlive)
            thread.Start();
    }

    public void Stop()
    {
        active = false;
        Console.WriteLine();
    }

    private void Spin()
    {
        while (active)
        {
            Turn();
            Thread.Sleep(delay);
        }
    }

    private void Draw(char c)
    {
        Console.Write("|");
    }

    private void Turn()
    {
        Draw(Sequence[++counter % Sequence.Length]);
    }

    public void Dispose()
    {
        Stop();
    }
}
