using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MyCSharpApp
{

    
    public class PracticeCodes
    {
        static List<string> ListStrLineElements = new List<string>();//Populated from the file InputFile.txt, method-ReadFromInputFile
        static List<string> ListStrLineElements2 = new List<string>();//Populated from the file InputFile2.txt, method-ReadFromInputFile
        static StringBuilder sbTextToWriteInOutput = new StringBuilder(); //Written to o/p file - OutputFile.txt from method WriteToOutputFile
        public PracticeCodes()
        {
            //TO PRACTICE CODE DONT FORGET TO SET THIS PROJECT AS THE STARTUP PROJECT
            EmployeeListOperation();
            Console.ReadKey();
        }
         static void EmployeeListOperation()
        {
            Console.WriteLine("The EXE is on this path: ");
            Console.WriteLine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            List<Employee> employees = new List<Employee>();
            employees.Add(new Employee() { FirstName = "John", Age = 29 });
            employees.Add(new Employee() { FirstName = "Matts", Age = 25 });
            employees.Add(new Employee() { FirstName = "Eliza", Age = 30 });
            employees.Add(new Employee() { FirstName = "Plank", Age = 29 });

            var FirstName = employees.OrderByDescending(x => x.Age).FirstOrDefault().FirstName.ToUpper();
            //Find The First Name of the Highest Aged Employee in Capitalized format using LINQ(It should output ELIZA)

            
            Console.WriteLine(FirstName);
            
            //Find All employees between the age 24 and (Both Inclusive)
        }

        static void testRegex()
        {
            

            Console.WriteLine("Enter Input CaseNumber");
            var CaseNumber = Console.ReadLine();
            if (CaseNumber == "break")
            {
                return;
            }
            Regex regex = new Regex(@"^\d+$");
            if (!string.IsNullOrEmpty(CaseNumber))
            {
                if (!regex.IsMatch(CaseNumber.Trim()))
                {
                    Console.WriteLine("DocumentID  Error for Input CaseNumber: " + CaseNumber);
                }
                else
                {
                    Console.WriteLine("No Error/Input is Numberic for CaseNumber" + CaseNumber);
                }
            }
            Console.WriteLine("---------------------------------------------------------------");
            testRegex();
        }
        static void TestContainsSplit()
        {
            string data = null;
            var containsdata = data.Split(',').Contains("ABC");
            Console.WriteLine(containsdata);
        }
        static void OrderByTest()
        {
            List<ModelData> myList = new List<ModelData>();
            myList.Add(new ModelData() {str = "ABCD",intNum=123,boolVar = false });
            myList.Add(new ModelData() { str = "DEF", intNum = 456, boolVar = true });
            myList.Add(new ModelData() { str = "GHI", intNum = 789, boolVar = false });
            myList = myList.OrderByDescending(x => x.boolVar).ToList();
            foreach (var item in myList)
            {
                Console.WriteLine(item.str);
            }
        }

        static void EnumOperation()
        {
            string str = "VirtualGroup_AddIndividualX";
            var vgLIst = Enum.GetNames(typeof(VirtualGroupMode)).ToList();
           var dic = Enum.GetValues(typeof(VirtualGroupMode))
               .Cast<VirtualGroupMode>()
               .ToDictionary(t => (int)t, t => t.ToString());

            List<string> listStr = new List<string>();
            listStr.Add("abc");
            listStr.Add("def");
            foreach (var st in listStr)
            {
                Console.WriteLine(st);
                listStr.Remove(st);
            }
            Console.WriteLine(listStr.Count);

            //if (VirtualGroupMode.VirtualGroup_AddIndividual.ToString() == str)
            //{
            //    Console.WriteLine(true);
            //}
            //Console.WriteLine((VirtualGroupMode.VirtualGroup_AddIndividual));
        }

        static void DatesCheck()
        {
            var TTDate = Convert.ToDateTime("10/20/2021");//Convert.ToDateTime("01/04/2022");

            var DispositionDate = Convert.ToDateTime("2023-09-18");
            if (DispositionDate != null && ((DateTime)DispositionDate).AddDays(31) > TTDate)
            {
                Console.WriteLine("Add Program");
            }
        }

        static void ReadFromInputFile()
        {
            var exePath = Assembly.GetExecutingAssembly().Location;
            int lastIndexOfDebug = exePath.LastIndexOf("Debug");
            string filePath = exePath.Substring(0, lastIndexOfDebug) + @"Debug\Files_PracticeCode\InputFile.txt";
            var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    ListStrLineElements.Add(line);
                }
                streamReader.Close();
            }
            string filePath2 = exePath.Substring(0, lastIndexOfDebug) + @"Debug\Files_PracticeCode\InputFile2.txt";
            var fileStream2 = new FileStream(filePath2, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using (var streamReader2 = new StreamReader(fileStream2, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader2.ReadLine()) != null)
                {
                    ListStrLineElements2.Add(line);
                }
                streamReader2.Close();
            }
           // performOperationsOnInput();
        }
        static void FindUncommon()
        {
            //ListStrLineElements contains the sharepoint group members multiline.
            //ListStrLineElements2 contains 2780 members all in one line.
            List<string> GroupEmails = new List<string>();
            List<string> Scon2780Emails = new List<string>();
            foreach (var data in ListStrLineElements)
            {
                var splitarr = data.ToLower().Split(',');
                foreach (var email in splitarr)
                {
                    var em = email.Trim();
                    if (!string.IsNullOrEmpty(em) && !string.IsNullOrWhiteSpace(em) && !GroupEmails.Contains(em))
                    {
                        GroupEmails.Add(em);
                    }
                }
            }
            foreach (var data in ListStrLineElements2)
            {
                var splitarr = data.ToLower().Split(',');
                foreach (var email in splitarr)
                {
                    var em = email.Trim();
                    if (!string.IsNullOrEmpty(em) && !string.IsNullOrWhiteSpace(em) && !Scon2780Emails.Contains(em))
                    {
                        Scon2780Emails.Add(em);
                    }
                }
            }
            foreach (var scon in Scon2780Emails)
            {
                if (!GroupEmails.Contains(scon))
                {
                    sbTextToWriteInOutput.AppendLine(scon);
                }
            }
            /*
              output.Add(splitarr[1].Trim() + "\t" + splitarr[0].Trim());
                sbTextToWriteInOutput.AppendLine(splitarr[1].Trim() + "\t" + splitarr[0].Trim());
             */
        }
        static void FindFirstLastName()
        {
            List<string> output = new List<string>();
            foreach (var data in ListStrLineElements)
            {
                var splitarr = data.Split(',');
                output.Add(splitarr[1].Trim() + "\t" + splitarr[0].Trim());
                sbTextToWriteInOutput.AppendLine(splitarr[1].Trim() + "\t" + splitarr[0].Trim());
            }
        }

        static void UpdateListElement()
        {
            var SelectedProgramsList = new List<string>();
            SelectedProgramsList.Add("SN");
            SelectedProgramsList.Add("MG");
            if (SelectedProgramsList.Contains("MG"))
            {
                SelectedProgramsList[SelectedProgramsList.IndexOf("MG")] = "MA";
            }
            Console.WriteLine(string.Join(",", SelectedProgramsList));

        }
        static void performOperationsOnInput()
        {
            ListStrLineElements = ListStrLineElements.Distinct().ToList();
            ListStrLineElements2 = ListStrLineElements2.Distinct().ToList();


            var commonRoles = new List<string>();

            sbTextToWriteInOutput.AppendLine("Common Roles of both the list");
            foreach (var line in ListStrLineElements)
            {
                if (ListStrLineElements2.Any(x => x.ToLower() == line.ToLower()))
                {
                    commonRoles.Add(line);
                    sbTextToWriteInOutput.AppendLine(line);
                }

            }
            WriteToOutputFile();
        }
       
        static void WriteToOutputFile()
        {
            if (sbTextToWriteInOutput.Length > 0)
            {
                string strOutput = sbTextToWriteInOutput.ToString();
                var exePath = Assembly.GetExecutingAssembly().Location;
                int lastIndexOfDebug = exePath.LastIndexOf("Debug");
                string filePath = exePath.Substring(0, lastIndexOfDebug) + @"Debug\Files_PracticeCode\OutputFile.txt";
                File.WriteAllText(filePath, strOutput);
                Console.WriteLine("Output File Updated");
            }
        }
        static void CheckConvertTryParse()
        {
            string strnum = "abc";
            int num = Convert.ToInt32(strnum);
            Console.WriteLine(num);



        }
        static void DateRangeValidationTest()
        {
            //Dates are in the format MM/DD/YYYY
            var startDate1 = Convert.ToDateTime("01/05/2021");
            var endDate1 = Convert.ToDateTime("02/01/2021");

            var startDate2 = Convert.ToDateTime("01/05/2021");
            var endDate2 = Convert.ToDateTime("02/25/2021");

            //Checking if Date2s lies between date1s
            if ((startDate2 >= startDate1 && startDate2 <= endDate1) && (endDate2 >= startDate1 && endDate2 <= endDate1))
            {
                Console.WriteLine("In range");
            }
            else
            {
                Console.WriteLine("Not in Range");
            }
        }
        static void testJoins()
        {
            List<ModelData> md1 = new List<ModelData>();
            List<ModelData2> md2 = new List<ModelData2>();

            md1.Add(new ModelData() { intNum = 101, str = "oNEooNE" });
            md1.Add(new ModelData() { intNum = 102, str = "oNEotWO" });
            md1.Add(new ModelData() { intNum = 103, str = "oNEotHREE" });
            //md2.Add(new ModelData2() { intNum = 1, identifier = "102" });           
            //foreach (var data in md1)
            //{
            //    Console.WriteLine(data.str);
            //}

            for (int i = 0; i <= md1.Count; i++)
            {
                if (md1.ElementAtOrDefault(i) != null)
                {
                    Console.WriteLine(md1[i].str);
                }
            }
        }
        static void testIntList()
        {
            List<int> intlist = new List<int>();
            intlist.Add(3);
            intlist.Add(4);

            var strList = intlist.Select(x => x.ToString()).ToList();

            var intlist2 = strList.Select(x => int.Parse(x)).Distinct().ToList();


        }
        static void testTryPaarse()
        {

            int intNum = 0;
            float floatNum = 0;
            string str = "50";

            float.TryParse(str, out floatNum);
            int.TryParse(str, out intNum);
            Console.WriteLine("Int : " + intNum);
            Console.WriteLine("float : " + floatNum);


            // logModelList = logModelList.Where(x => !string.IsNullOrWhiteSpace(x.InstanceId) && int.TryParse(x.InstanceId,out tempInstanceId)).ToList();

            List<ModelData> modelDatas = new List<ModelData>();
            modelDatas.Add(new ModelData() { str = "12345", intNum = 1 });
            modelDatas.Add(new ModelData() { str = "sdeb", intNum = 2 });

            int tempInstanceId = 0;
            var data = modelDatas.Where(x => !string.IsNullOrEmpty(x.str) && int.TryParse(x.str, out tempInstanceId));

            if (int.TryParse("abc", out tempInstanceId))
            {
                Console.WriteLine("Yes parsed");
            }

            Console.WriteLine(data.Count());
        }
        static void timestamp()
        {
            string fileNameTimeStamp = DateTime.Now.ToString("yyyyMMdd_dddd_HHmmss");
            Console.WriteLine(fileNameTimeStamp);

        }
        static void nullCheck(string str, int a)
        {
            Console.WriteLine(a);
        }
        public void testSpinner()
        {
            var spinner = new Spinner(1, 1);
            spinner.Start();
            Thread.Sleep(5000);
            spinner.Stop();
        }

        public void OneTestNullException()
        {
            /*
                viewModel.SelectedMciIndividualResult.MciIndividualsInformation = new List<MciIndividualInformation>();
            viewModel.SelectedMciIndividualResult.MciIndividualsInformation.AddRange(selectedMCIIndividualInfo.MciIndividualsInformation);
             */

            List<ModelData> modelDatas = new List<ModelData>();
            modelDatas.Add(new ModelData() { str = "String One", intNum = 1 });


            List<ModelData> modelDatas2 = null;//new List<ModelData>();
                                               //modelDatas2.Add(new ModelData() { str = "String Two", intNum = 2 });
                                               //modelDatas2.Add(new ModelData() { str = "String Three", intNum = 3 });


            //Console.WriteLine("modelDatas2");
            //foreach (var data in modelDatas2)
            //{
            //    Console.WriteLine(data.intNum + "\t" + data.str);
            //}

            modelDatas.AddRange(modelDatas2);

            Console.WriteLine("modelDatas");
            foreach (var data in modelDatas)
            {
                Console.WriteLine(data.intNum + "\t" + data.str);
            }
        }

        public void twoTestRegex()
        {
            var data = "2020-11-01 00:00:00.000";
            var op = Regex.Replace(data, @"[^0-9a-zA-Z ,./:()-]+", "");
            Console.WriteLine(op);

        }
        static void tryWriteToFile()
        {
            var exePath = Assembly.GetExecutingAssembly().Location;
            int lastIndexOfDebug = exePath.LastIndexOf("Debug");
            string filePath = exePath.Substring(0, lastIndexOfDebug) + @"Debug\Files\MyLogger.txt";
            //File.AppendAllText(filePath, Environment.NewLine);
            //File.AppendAllText(filePath, "This is a line at " + DateTime.Now.ToShortTimeString());
            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Please prepend this at the beginning " + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.Second.ToString());
            sb.AppendLine("-----------");
            PrependString(sb.ToString(), file);


        }
        public static void PrependString(string value, FileStream file)
        {
            var buffer = new byte[file.Length];

            while (file.Read(buffer, 0, buffer.Length) != 0)
            {
            }

            if (!file.CanWrite)
                throw new ArgumentException("The specified file cannot be written.", "file");

            file.Position = 0;
            var data = Encoding.UTF8.GetBytes(value);
            file.SetLength(buffer.Length + data.Length);
            file.Write(data, 0, data.Length);
            file.Write(buffer, 0, buffer.Length);
        }




    }

    public class ModelData
    {
        public string str { get; set; }
        public int intNum { get; set; }
        public bool boolVar { get; set; }
    }

    public class ModelData2
    {
        public string str { get; set; }
        public int intNum { get; set; }

        public string identifier { get; set; }
    }
    public class Spinner : IDisposable
    {
        private const string Sequence = @"/-\|";
        private int counter = 0;
        private readonly int left;
        private readonly int top;
        private readonly int delay;
        private bool active;
        private readonly Thread thread;

        public Spinner(int left, int top, int delay = 100)
        {
            this.left = left;
            this.top = top;
            this.delay = delay;
            thread = new Thread(Spin);
        }

        public void Start()
        {
            active = true;
            if (!thread.IsAlive)
                thread.Start();
        }

        public void Stop()
        {
            active = false;
            Draw(' ');
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
            Console.SetCursorPosition(left, top);
            Console.Write("Waiting...");
            Console.Write(c);
        }

        private void Turn()
        {
            Draw(Sequence[++counter % Sequence.Length]);
        }

        public void Dispose()
        {
            Stop();
        }

        static class MyStaticClass
        {
            static MyStaticClass() { }//A. Will it work or compile error
            //MyStaticClass() { }//B. Will it work or compile error
        }
        abstract class MyAbstractClass
        {
        }
        sealed class MySealedClass : myClass
        {

        }
        class myClass
        {


        }
        

    }
    public enum VirtualGroupMode
    {
        VirtualGroup_AddIndividual,
        VirtualGroup_AddAddress,
        VirtualGroup_AddProgram,
        VirtualGroup_ProgramBasedNavigation,
        VirtualGroup_TToNonTMember,
        VirtualGroup_CaptureABDInformation,
        VirtualGroup_Safe,
        VirtualGroup_MemberOutOfHousehold,
        VirtualGroup_FullExpenseNavigation,
        VirtualGroup_FullIncomeNavigation,
        VirtualGroup_AddEditIncomeDeduction,
        VirtualGroup_OutOfStateAddress,
        VirtualGroup_RACFlow,
        VirtualGroup_RenewalFlow,
        DSNAPFlow,
        VirtualGroup_MidCertification,
        VirtualGroup_MemberDetermination,
        VirtualGroup_VisibleInSC,
        VirtualGroup_IncomeFlowNavigation
    }

    public class Employee { public string FirstName { get; set; } public int Age { get; set; } }
}



