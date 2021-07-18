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
        public PracticeCodes()
        {
            CheckConvertTryParse();
            //TO PRACTICE CODE DONT FORGET TO SET THIS PROJECT AS THE STARTUP PROJECT
            Console.ReadKey();
        }


        static void CheckConvertTryParse()
        {
            string strnum = "abc";
            int num = Convert.ToInt32(strnum);
            Console.WriteLine( num);

            

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

            var intlist2 = strList.Select(x=> int.Parse(x)).Distinct().ToList();


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
    }

}



