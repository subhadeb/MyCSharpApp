using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
namespace _14_ReadExcelFile
{
    class Program
    {
        //public const string ExcelFilePath = @"C:\Users\subdeb\Documents\Subha_Deb_497290\Study\Dot_Net_Study\2 OOPS and C Sharp\CSharp Apps Programs  Backup\7_ExcelInteropDailyStatus\InputFiles\";
        //public const string ExcelFilePathProdForSave = @"C:\Users\subdeb\Documents\ProjectWP\Other Tasks\00_Daily_Status\aa_ProgrammedExcelFile\";
        public const string InputExcelFileName = "Input1.xlsx";
        //public static List<string> ReceipentsEmailIdsList = new List<string>() { "abhishekkumar4@DELOITTE.com", "raparanjpe@deloitte.com", "ylimbachia@deloitte.com" };
        //public const bool IsDev = false;//Set if to False for Prod
        static void Main(string[] args)
        {

            ExcelOperations();

        }
        static void ExcelOperations()
        {
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook excelBook = excelApp.Workbooks.Open(Environment.CurrentDirectory + @"\InputOutput\" + InputExcelFileName);

            try
            {
                Excel._Worksheet excelSheet = excelBook.Sheets[1];
                Excel.Range excelRange = excelSheet.Columns[1];
                Excel.Range sourceHeader = excelSheet.Range["A1:I1"];
                Excel.Range sourceBlankDataRow = excelSheet.Range["A10:I10"];
                var currentTime = DateTime.Now.ToShortTimeString();

                var range = excelSheet.UsedRange;
                var rw = range.Rows.Count;
                var cl = 5;//range.Columns.Count;

                var str = string.Empty;
                for (var rCnt = 1; rCnt <= rw; rCnt++)
                {
                    for (var cCnt = 1; cCnt <= cl; cCnt++)
                    {
                        var rangeVal = (range.Cells[rCnt, cCnt] as Excel.Range);
                        if (rangeVal != null)
                        {
                            try
                            {
                                str = Convert.ToString((rangeVal).Value2);
                            }
                            catch(Exception ex)
                            {
                                str = "Got Exception";
                            }

                            Console.Write(str + "\t");
                        }
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                excelBook.Close();
                //excelBook2.Save();
                //excelBook2.Close();
                excelApp.Quit();
            }
            Console.ReadKey();
        }

    }
}
