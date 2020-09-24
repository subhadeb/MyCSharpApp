using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class Program
{
    public const string InputFilePath = @"C:\Temp\InputQueryResult.txt";
    public const string OutputFilePath = @"C:\Temp\OutputDMLQuery.txt";

    public const string TableNameForPlainText = "DataCollection.Hell";  //"@TableName123";


    //select BENSequenceNumber,SSN,CaseNumber,IndividualId,ClaimNumber,IsIncomeUpdated,IncomeUpdateReason,*  from Interface.BENDEXMaster where IndividualId = 803001209

    public const string ExistingSSNForBendex = "544662042";
    public const string ExistingCaseNumberForBendex = "400345112";
    public const string ExistingIndividualIdForBendex = "800840232";//---Goes to Income Table
    public const string ExistingClaimNumberForBendex = "544689894A ";//---Goes to Income Table


    public const string SSNForBendexDev5 = "544662042";
    public const string CaseNumberForBendexDev5 = "100016030";
    public const string IndividualIdForBendexDev5 = "990242795";//---Goes to Income Table
    public const string ClaimNumberForBendexDev5 = "544689894A";//---Goes to Income Table


    public static void Main(string[] args)
    {
        InsertStatementCreatorFromPlainText();
        //UpdateStatementCreatorFromPlainText();
        //InsertStatementCreatorFromPlainTextSDX();
        /*InsertStatementCreatorFromPlainTextSDX();*/

    }
    static void InsertStatementCreatorFromPlainText()
    {

        List<string> listStrLineElements = new List<string>();
        StringBuilder sbQuery = new StringBuilder();
        //Step 1/4: Read the Input File and Push the Line Items into a List of String(listStrLineElements)
        var fileStream = new FileStream(InputFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
        {
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                listStrLineElements.Add(line);
            }
            streamReader.Close();
        }
        //Step 2/4 Create the Query to list the table and the column names with 8 columns in one line(using newLineCounter)
        if (listStrLineElements.Count > 1)
        {
            var columnNamesArray = listStrLineElements[0].Split('\t');
            var rowValuesArray = listStrLineElements[1].Split('\t');
            sbQuery.Append("INSERT INTO " + TableNameForPlainText + " ");
            sbQuery.Append("(");
            int newLineCounter = 0;
            for (int i = 0; i < columnNamesArray.Length; i++)
            {
                var colName = columnNamesArray[i];
                if (i != columnNamesArray.Length - 1)
                {
                    sbQuery.Append(colName + ",");
                    newLineCounter++;
                }
                else
                {
                    sbQuery.Append(colName + ")");
                    newLineCounter++;
                }

                if (newLineCounter % 8 == 0)
                {
                    sbQuery.Append("\n");
                }

            }

            sbQuery.Append("\n");
            sbQuery.Append("VALUES(");
            newLineCounter = 0;
            //Step 3/4 Create the Query to insert values into the correspondinig column names with 8 columns in one line(using newLineCounter)
            for (int i = 0; i < rowValuesArray.Length; i++)
            {
                var itemValue = rowValuesArray[i];
                if (i != rowValuesArray.Length - 1)//If i is not the last column put Comma after each element
                {
                    if (itemValue == "NULL")
                    {
                        sbQuery.Append(itemValue + ",");
                    }
                    else if (itemValue.Contains("0x00"))//For RowVersionStamp
                    {
                        sbQuery.Append("NULL" + ",");
                    }
                    else
                    {
                        sbQuery.Append("'" + itemValue + "',");
                    }

                    newLineCounter++;
                }
                else//It is the Last Column(For Appending Closing First Bracket instead of Comma)
                {
                    if (itemValue == "NULL")
                    {
                        sbQuery.Append(itemValue + ")");
                    }
                    else if (itemValue.Contains("0x00"))//For RowVersionStamp
                    {
                        sbQuery.Append("NULL" + ")");
                    }
                    else
                    {
                        sbQuery.Append("'" + itemValue + "')");
                    }
                    newLineCounter++;
                }

                if (newLineCounter % 8 == 0)
                {
                    sbQuery.Append("\n");
                }
            }

        }
        sbQuery.Append("\n");
        sbQuery.Append("SELECT SCOPE_IDENTITY() AS NewlyInsertedIdentityValue");
        Console.WriteLine(sbQuery);
        //Step 4/4 Write the String Builder into the O/P Fiile
        var fileStreamOP = new FileStream(OutputFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        //File.WriteAllText(OutputFilePath, "");
        using (var streamWriter = new StreamWriter(fileStreamOP, Encoding.UTF8))
        {
            streamWriter.Write(sbQuery);
        }

        Console.WriteLine("Write Here");

    }
    static void UpdateStatementCreatorFromPlainText()
    {
        List<string> listStrLineElements = new List<string>();
        StringBuilder sbQuery = new StringBuilder();
        //Step 1/3: Read the Input File and Push the Line Items into a List of String(listStrLineElements)
        var fileStream = new FileStream(InputFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
        {
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                listStrLineElements.Add(line);
            }
            streamReader.Close();
        }
        //Step 2/3 Create the Query to list the table and the column names = values with 3 columns in one line(using newLineCounter)
        if (listStrLineElements.Count > 1)
        {
            var columnNamesArray = listStrLineElements[0].Split('\t');
            var rowValuesArray = listStrLineElements[1].Split('\t');
            sbQuery.Append("UPDATE @TableName ");
            sbQuery.Append("\n");
            sbQuery.Append("SET ");
            int newLineCounter = 0;
            for (int i = 0; i < columnNamesArray.Length; i++)
            {
                var colName = columnNamesArray[i];
                var itemValue = rowValuesArray[i];
                if (i != columnNamesArray.Length - 1)
                {
                    if (itemValue != "NULL")
                    {
                        sbQuery.Append(colName + " = '" + itemValue + "',");
                    }
                    else
                    {
                        sbQuery.Append(colName + " = " + itemValue + ",");
                    }

                    newLineCounter++;
                }
                else
                {
                    if (itemValue != "NULL")
                    {
                        sbQuery.Append(colName + " = '" + itemValue + "'");
                    }
                    else
                    {
                        sbQuery.Append(colName + " = " + itemValue);
                    }
                    newLineCounter++;
                }

                if (newLineCounter % 3 == 0)
                {
                    sbQuery.Append("\n");
                }
            }
            sbQuery.Append("\n");
            sbQuery.Append("WHERE 1 = 0 //Put the condition here for Update");
            sbQuery.Append("\n");
        }
        Console.WriteLine(sbQuery);
        //Step 3/3 Write the String Builder into the O/P Fiile
        var fileStreamOP = new FileStream(OutputFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        using (var streamWriter = new StreamWriter(fileStreamOP, Encoding.UTF8))
        {
            streamWriter.Write(sbQuery);
        }

        Console.WriteLine("Write Here");

    }
    static void InsertStatementCreatorFromPlainTextBendex()
    {
        //SSN(304465469),CaseNumber(100014582),IndividualId(990234323),ClaimNumber(304465469A),IsIncomeUpdated-R5(NULL),IncomeUpdateReason-R5(NULL),RowVersionStamp-R5(NULL)


        List<string> listStrLineElements = new List<string>();
        StringBuilder sbQuery = new StringBuilder();
        //Step 1/4: Read the Input File and Push the Line Items into a List of String(listStrLineElements)
        var fileStream = new FileStream(InputFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
        {
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                listStrLineElements.Add(line);
            }
            streamReader.Close();
        }
        //Step 2/4 Create the Query to list the table and the column names with 8 columns in one line(using newLineCounter)
        if (listStrLineElements.Count > 1)
        {
            var columnNamesArray = listStrLineElements[0].Split('\t');
            var rowValuesArray = listStrLineElements[1].Split('\t');
            if (columnNamesArray[0] == "BENDEXMasterId")
            {
                sbQuery.Append("INSERT INTO Interface.BENDEXMaster ");
            }
            if (columnNamesArray[0] == "BENSequenceNumber")
            {
                sbQuery.Append("INSERT INTO Interface.BENDEXStaging ");
            }

            sbQuery.Append("(");
            int newLineCounter = 0;
            for (int i = 0; i < columnNamesArray.Length; i++)
            {
                var colName = columnNamesArray[i];
                if (i != columnNamesArray.Length - 1)
                {
                    sbQuery.Append(colName + ",");
                    newLineCounter++;
                }
                else
                {
                    sbQuery.Append(colName + ")");
                    newLineCounter++;
                }

                if (newLineCounter % 8 == 0)
                {
                    sbQuery.Append("\n");
                }

            }

            sbQuery.Append("\n");
            sbQuery.Append("VALUES(");
            newLineCounter = 0;
            //Step 3/4 Create the Query to insert values into the correspondinig column names with 8 columns in one line(using newLineCounter)
            for (int i = 0; i < rowValuesArray.Length; i++)
            {
                var itemValue = rowValuesArray[i];
                if (i != rowValuesArray.Length - 1)//If i is not the last column put Comma after each element
                {
                    if (itemValue == "NULL")
                    {
                        sbQuery.Append(itemValue + ",");
                    }
                    else if (itemValue.Contains("0x00"))//For RowVersionStamp
                    {
                        sbQuery.Append("NULL" + ",");
                    }
                    else
                    {
                        sbQuery.Append("'" + itemValue + "',");
                    }

                    newLineCounter++;
                }
                else//It is the Last Column(For Appending Closing First Bracket instead of Comma)
                {
                    if (itemValue == "NULL")
                    {
                        sbQuery.Append(itemValue + ")");
                    }
                    else if (itemValue.Contains("0x00"))//For RowVersionStamp
                    {
                        sbQuery.Append("NULL" + ")");
                    }
                    else
                    {
                        sbQuery.Append("'" + itemValue + "')");
                    }
                    newLineCounter++;
                }

                if (newLineCounter % 8 == 0)
                {
                    sbQuery.Append("\n");
                }
            }

            sbQuery.Append("\n");
            sbQuery.Append("SELECT SCOPE_IDENTITY() AS NewlyInsertedIdentityValue");

            //Replacing the SSN/Claimnumbers with Dev5 Compatibility
            sbQuery.Replace(ExistingSSNForBendex, SSNForBendexDev5);
            sbQuery.Replace(ExistingCaseNumberForBendex, CaseNumberForBendexDev5);
            sbQuery.Replace(ExistingIndividualIdForBendex, IndividualIdForBendexDev5);
            sbQuery.Replace(ExistingClaimNumberForBendex, ClaimNumberForBendexDev5);

        }
        Console.WriteLine(sbQuery);
        //Step 4/4 Write the String Builder into the O/P Fiile
        var fileStreamOP = new FileStream(OutputFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        //File.WriteAllText(OutputFilePath, "");
        using (var streamWriter = new StreamWriter(fileStreamOP, Encoding.UTF8))
        {
            streamWriter.Write(sbQuery);
        }

        Console.WriteLine("Write Here");

    }
    static void InsertStatementCreatorFromPlainTextSDX()
    {
        //SSN(304465469),CaseNumber(100014582),IndividualId(990234323),ClaimNumber(304465469A),IsIncomeUpdated-R5(NULL),IncomeUpdateReason-R5(NULL),RowVersionStamp-R5(NULL)


        List<string> listStrLineElements = new List<string>();
        StringBuilder sbQuery = new StringBuilder();
        //Step 1/4: Read the Input File and Push the Line Items into a List of String(listStrLineElements)
        var fileStream = new FileStream(InputFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
        {
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                listStrLineElements.Add(line);
            }
            streamReader.Close();
        }
        //Step 2/4 Create the Query to list the table and the column names with 8 columns in one line(using newLineCounter)
        if (listStrLineElements.Count > 1)
        {
            var columnNamesArray = listStrLineElements[0].Split('\t');
            var rowValuesArray = listStrLineElements[1].Split('\t');
            if (columnNamesArray[0] == "SDXMasterId")
            {
                sbQuery.Append("INSERT INTO Interface.SDXMaster ");
            }
            if (columnNamesArray[0] == "SDXSequenceNumber")
            {
                sbQuery.Append("INSERT INTO Interface.SDXStaging ");
            }

            sbQuery.Append("(");
            int newLineCounter = 0;
            for (int i = 0; i < columnNamesArray.Length; i++)
            {
                var colName = columnNamesArray[i];
                if (i != columnNamesArray.Length - 1)
                {
                    sbQuery.Append(colName + ",");
                    newLineCounter++;
                }
                else
                {
                    sbQuery.Append(colName + ")");
                    newLineCounter++;
                }

                if (newLineCounter % 8 == 0)
                {
                    sbQuery.Append("\n");
                }

            }

            sbQuery.Append("\n");
            sbQuery.Append("VALUES(");
            newLineCounter = 0;
            //Step 3/4 Create the Query to insert values into the correspondinig column names with 8 columns in one line(using newLineCounter)
            for (int i = 0; i < rowValuesArray.Length; i++)
            {
                var itemValue = rowValuesArray[i];
                if (i != rowValuesArray.Length - 1)//If i is not the last column put Comma after each element
                {
                    if (itemValue == "NULL")
                    {
                        sbQuery.Append(itemValue + ",");
                    }
                    else if (itemValue.Contains("0x00"))//For RowVersionStamp
                    {
                        sbQuery.Append("NULL" + ",");
                    }
                    else
                    {
                        sbQuery.Append("'" + itemValue + "',");
                    }

                    newLineCounter++;
                }
                else//It is the Last Column(For Appending Closing First Bracket instead of Comma)
                {
                    if (itemValue == "NULL")
                    {
                        sbQuery.Append(itemValue + ")");
                    }
                    else if (itemValue.Contains("0x00"))//For RowVersionStamp
                    {
                        sbQuery.Append("NULL" + ")");
                    }
                    else
                    {
                        sbQuery.Append("'" + itemValue + "')");
                    }
                    newLineCounter++;
                }

                if (newLineCounter % 8 == 0)
                {
                    sbQuery.Append("\n");
                }
            }

            sbQuery.Append("\n");
            sbQuery.Append("SELECT SCOPE_IDENTITY() AS NewlyInsertedIdentityValue");

            //Replacing the SSN/Claimnumbers with Dev5 Compatibility
            //sbQuery.Replace(ExistingSSNForBendex, SSNForBendexDev5);
            //sbQuery.Replace(ExistingCaseNumberForBendex, CaseNumberForBendexDev5);
            //sbQuery.Replace(ExistingIndividualIdForBendex, IndividualIdForBendexDev5);
            //sbQuery.Replace(ExistingClaimNumberForBendex, ExistingClaimNumberForBendex);

        }
        Console.WriteLine(sbQuery);
        //Step 4/4 Write the String Builder into the O/P Fiile
        var fileStreamOP = new FileStream(OutputFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        //File.WriteAllText(OutputFilePath, "");
        using (var streamWriter = new StreamWriter(fileStreamOP, Encoding.UTF8))
        {
            streamWriter.Write(sbQuery);
        }

        Console.WriteLine("Write Here");

    }
}

