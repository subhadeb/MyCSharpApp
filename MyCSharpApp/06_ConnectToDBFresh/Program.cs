using System;
using System.Collections.Generic;
using System.Data.SqlClient;

class Program
{
    //myTestTable create and insert script is there at the bottom
    const string connectionString = @"data source=USMUMSUBDEB3\SQLEXPRESS; database=SubhaDB; integrated security=SSPI";
    const string queryToExecute = "select top 1 * from myTestTable";

    static void Main(string[] args)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(queryToExecute, connection))
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine(reader[0]);
                    Console.WriteLine(reader[1]);
                }
            }
        }
    }


    /*
 USE SubhaDB
 IF EXISTS(select 1 from sys.tables where name = 'myTestTable')
 BEGIN
 DROP TABLE myTestTable
 END
 CREATE TABLE myTestTable(id INT IDENTITY, name varchar(30))
 INSERT INTO myTestTable values('John'),('Matt'),('Eli')
 select * from myTestTable
 */
}

