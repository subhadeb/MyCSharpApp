using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 For mongo
cmd - Run as Admin.
mongod --dbpath "C:\data\db"


 
 */

class Program
{
    private static MongoClient dbClient;
    static void Main(string[] args)
    {
        Connect();
        GetData();
        //InsertData();
        //UpdateData();
        DeleteData();
        Console.ReadKey();
    }

    static void Connect()
    {
        //Keep Mongo Instance running otherwise would get time out exceptin.
        dbClient  = new MongoClient("mongodb://127.0.0.1:27017");
        var dbList = dbClient.ListDatabases().ToList();
    }
    static void GetData()//READ
    {
        IMongoDatabase dbEmp = dbClient.GetDatabase("CsharpDB");
        var empCollection = dbEmp.GetCollection<BsonDocument>("employees");
        var colDataList = empCollection.Find(new BsonDocument()).ToList();
        foreach (var item in colDataList)
        {
            Console.WriteLine(item.ToString());
            Console.WriteLine(item["Name"]);
        }
    }
    static void InsertData()//CREATE
    {
        IMongoDatabase dbEmp = dbClient.GetDatabase("CsharpDB");
        var empCollection = dbEmp.GetCollection<BsonDocument>("employees");
        BsonDocument empDoc = new BsonDocument();
        Console.WriteLine("Enter Name");
        string nameInput = Console.ReadLine();
        Console.WriteLine("Enter Salary");
        int salaryInput = Convert.ToInt32(Console.ReadLine());
        BsonElement nameElement = new BsonElement("Name", nameInput);
        BsonElement salaryElement = new BsonElement("Salary", salaryInput);
        empDoc.Add(nameElement);
        empDoc.Add(salaryElement);
        empCollection.InsertOne(empDoc);
        Console.WriteLine("Document Inserted Successfully");
    }

    static void UpdateData()//UPDATE
    {
        IMongoDatabase dbEmp = dbClient.GetDatabase("CsharpDB");
        var empCollection = dbEmp.GetCollection<BsonDocument>("employees");
        BsonDocument empDocUpdated = new BsonDocument();
        BsonDocument findEmpDoc = new BsonDocument(new BsonElement("Name", "Marry Heed"));
        Console.WriteLine("Enter Updated Name");
        string nameInput = Console.ReadLine();
        Console.WriteLine("Enter Updated Salary");
        int salaryInput = Convert.ToInt32(Console.ReadLine());
        BsonElement nameElement = new BsonElement("Name", nameInput);
        BsonElement salaryElement = new BsonElement("Salary", salaryInput);
        empDocUpdated.Add(nameElement);
        empDocUpdated.Add(salaryElement);
        var updatedDoc = empCollection.FindOneAndReplace(findEmpDoc, empDocUpdated);
        Console.WriteLine("Document Updated Successfully");
    }

    static void DeleteData()//DELETE
    {
        IMongoDatabase dbEmp = dbClient.GetDatabase("CsharpDB");
        var empCollection = dbEmp.GetCollection<BsonDocument>("employees");
        BsonDocument findEmpDoc = new BsonDocument(new BsonElement("Name", "John Doe"));
        empCollection.FindOneAndDelete(findEmpDoc);
        Console.WriteLine("Document Deleted Successfully");
    }
}
