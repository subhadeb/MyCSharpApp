using System;
using System.Collections.Generic;
using System.Linq;
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
            testSpinner();
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


    }

    public class ModelData
    {
        public string str { get; set; }
        public int intNum { get; set; }
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



