using ConsoleCSharpPrograms;
using ConsoleCSharpPrograms.Adapter;
using ConsoleCSharpPrograms.Target;
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
        IEmployeeManager adapter = new EmployeeAdapter();
        string result = adapter.GetAllEmployees();
        Console.WriteLine(result);
    }
}