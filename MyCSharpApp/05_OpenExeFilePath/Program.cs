using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*



*/


class Program
{
    static void Main(string[] args)
    {
        var path = @"C:\Users\subdeb\source\repos\ConsoleCSharpPrograms\ConsoleCSharpPrograms\bin\Debug";
        Console.WriteLine("Opening Path: " + path);
        //Console.WriteLine(EXEFilePath);
        System.Diagnostics.Process.Start(path);
        Console.ReadKey();
    }
}