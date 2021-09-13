using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class Program
{
    private static MongoClient dbClient;
    static void Main(string[] args)
    {
        Connect();
        GetData();
        Console.ReadKey();
    }

    static void Connect()
    {
        dbClient  = new MongoClient("mongodb://127.0.0.1:27017");
        var dbList = dbClient.ListDatabases().ToList();
    }
    static void GetData()
    {
        IMongoDatabase dbEmp = dbClient.GetDatabase("CsharpDB");
        var collection = dbEmp.GetCollection<BsonDocument>("employees");
        var colData = collection.Find(new BsonDocument()).ToList();
        foreach (var item in colData)
        {
            Console.WriteLine(item.ToString());
            Console.WriteLine(item["Name"]);
        }


    }
}
