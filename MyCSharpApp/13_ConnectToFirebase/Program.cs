using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
/*






*/


class Program
{
    IFirebaseConfig config;
    IFirebaseClient client;
    public Program()
    {
        config = new FirebaseConfig
        {
            //Auth secret can be found at: https://console.firebase.google.com/project/csharp-subhadb/settings/serviceaccounts/databasesecrets
            AuthSecret = "idgLpOJl3yTB0wjn58YFqxU0D7HkrBpKBtEzzfip",
            BasePath = "https://csharp-subhadb.firebaseio.com/"
        };
        client = new FirebaseClient(config);

    }
    public bool CheckForInternetConnection()
    {
        try
        {
            using (var client = new WebClient())
            using (client.OpenRead(config.BasePath))
                return true;
        }
        catch
        {
            Console.WriteLine("Not Connected To Server");
            Console.ReadKey();
            return false;
        }
    }
    public static Student GetStudent()
    {
        Student student = new Student();
        Console.WriteLine("Enter Name");
        student.Name = Console.ReadLine();
        Console.WriteLine("Enter Age");
        int age = 0;
        int.TryParse(Console.ReadLine(), out age);
        student.Age = age;
        return student;

    }
    public async Task InsertFirebaseStudent()
    {
        StringBuilder sbName = new StringBuilder();
        Student stud = GetStudent();
        //string nodeName = "StudentProgram/"+stud.Name + "-" + stud.Age;
        PushResponse response = await client.PushAsync("StudentProgram/", stud);
        if (response != null && !string.IsNullOrEmpty(response.Body) && response.StatusCode.ToString() == "OK")
        {
            Console.WriteLine(stud.Name + " Inserted");
        }
        Console.ReadKey();
    }
    public async Task RetrieveFirebaseStudent()
    {

        FirebaseResponse response = await client.GetAsync("StudentProgram");
        if (response != null && response.StatusCode.ToString() == "OK")
        {
            Console.WriteLine("Retrieved");
            var mList = JsonConvert.DeserializeObject<IDictionary<string, Student>>(response.Body);
            var studentList = new List<Student>();
            foreach (var item in mList)
            {
                studentList.Add(new Student
                {
                    Name = item.Value.Name,
                    Age = item.Value.Age,
                    Key = item.Key
                });
            }
            Console.WriteLine("Name\t\tAge\t\tkey");
            foreach (var studResp in studentList)
            {
                var name = (studResp.Name.PadRight(15)).Substring(0, 15);

                Console.WriteLine("{0}\t{1}\t\t{2}", name, studResp.Age, studResp.Key);
            }
        }
        Console.ReadKey();
    }

    static void Main(string[] args)
    {

        Program program = new Program();
        if (program.CheckForInternetConnection())
        {
            Console.WriteLine("Enter 1 to Insert Record, 2 to View Records");
            switch (Console.ReadLine())
            {
                case "1":
                    program.InsertFirebaseStudent().Wait(); ;
                    break;
                case "2":
                    program.RetrieveFirebaseStudent().Wait();
                    break;
                default:
                    break;
            }

        }

    }
}

public class Student
{
    public string Name { get; set; }
    public string Key { get; set; }
    public int Age { get; set; }
}