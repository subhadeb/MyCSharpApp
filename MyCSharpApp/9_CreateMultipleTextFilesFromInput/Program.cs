using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
/*






*/


class Program
{

    public const string InputFilePath = @"C:\\SubhaTemp\TempInputFile.txt";
    public const string OutputFilePath = @"C:\\SubhaTemp\TempOutputFile.txt";
    public const string DirectoryFileCreationPath = @"C:\SubhaTemp\UdemyReactFiles\";
    static List<string> ListStrLineElements = new List<string>();
    static void Main(string[] args)
    {
        ReadFromInputFile();
        ProocessAndWriteToOutputFile();
    }
    static void ReadFromInputFile()
    {
        var fileStream = new FileStream(InputFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
        {
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                ListStrLineElements.Add(line);
            }
            streamReader.Close();
        }
    }
    static void ProocessAndWriteToOutputFile()
    {
        StringBuilder sbQuery = new StringBuilder();
        sbQuery.AppendLine();
        sbQuery.AppendLine();
        sbQuery.AppendLine("---------------------------------------------------------------");
        sbQuery.AppendLine();
        sbQuery.AppendLine();
        sbQuery.AppendLine("---------------------------------------------------------------");
        sbQuery.AppendLine();
        sbQuery.AppendLine();
        sbQuery.AppendLine("---------------------------------------------------------------");
        sbQuery.AppendLine();
        sbQuery.AppendLine();
        sbQuery.AppendLine("---------------------------------------------------------------");
        sbQuery.AppendLine();
        sbQuery.AppendLine();
        foreach (var lines in ListStrLineElements)
        {
            var linesSplitArray = lines.Split('\t');
            var titleAfterRemovingSpecialChars = Regex.Replace(linesSplitArray[1], @"[^0-9a-zA-Z ]+", "");
            string fileName = DirectoryFileCreationPath + linesSplitArray[0] + " " + titleAfterRemovingSpecialChars + ".txt";
            // Create a new file     
            using (FileStream fs = File.Create(fileName))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(sbQuery.ToString());
                fs.Write(info, 0, info.Length);
            }


        }
    }
}