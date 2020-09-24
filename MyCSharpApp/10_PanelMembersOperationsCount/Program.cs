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

    public const string InputFilePath1 = @"C:\\SubhaTemp\TempInputFile1.txt";
    public const string InputFilePath2 = @"C:\\SubhaTemp\TempInputFile2.txt";
    public const string OutputFilePath = @"C:\\SubhaTemp\TempOutputFile.txt";
    public const string DirectoryFileCreationPath = @"C:\SubhaTemp\UdemyReactFiles\";
    static List<EmailLevel> LineInput1 = new List<EmailLevel>();
    static List<EmailLevel> LineInput2 = new List<EmailLevel>();

    static void Main(string[] args)
    {
        ReadFromInputFile();
        ProocessAndWriteToOutputFile();
    }
    static void ReadFromInputFile()
    {
        var fileStream1 = new FileStream(InputFilePath1, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        using (var streamReader = new StreamReader(fileStream1, Encoding.UTF8))
        {
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                var splittedArray = line.Split('\t');
                EmailLevel el = new EmailLevel() { Level = splittedArray[0], Email = splittedArray[1].ToLower() };
                LineInput1.Add(el);
            }
            streamReader.Close();
        }
        var fileStream2 = new FileStream(InputFilePath2, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        using (var streamReader = new StreamReader(fileStream2, Encoding.UTF8))
        {
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                var splittedArray = line.Split('\t');
                EmailLevel el = new EmailLevel() { Level = splittedArray[0], Email = splittedArray[1].ToLower() };
                LineInput2.Add(el);
            }
            streamReader.Close();
        }
    }
    static void ProocessAndWriteToOutputFile()
    {
        //LineInput1 is the new list, 2 is the old one


        var commonInBoth = new List<EmailLevel>();
        var commonInBoth2 = new List<EmailLevel>();
        var neeeToAdd = new List<EmailLevel>();
        var neeeToInactivate = new List<EmailLevel>();


        var dupes1 = LineInput1.GroupBy(x => new { x.Email })
                   .Where(x => x.Skip(1).Any()).ToList();

        Console.WriteLine("Duplicates in List1: " + dupes1.Count());


        var dupes2 = LineInput2.GroupBy(x => new { x.Email })
                   .Where(x => x.Skip(1).Any()).ToList();

        Console.WriteLine("Duplicates in List2 : " + dupes2.Count());

        var LineInput2Unique = new List<EmailLevel>();
        foreach (var data in LineInput2)
        {
            if (!LineInput2Unique.Any(x => x.Email == data.Email))
            {
                LineInput2Unique.Add(data);
            }
        }
        Console.WriteLine("Unique List2 count: " + LineInput2Unique.Count());




        foreach (var data in LineInput1)
        {
            if (LineInput2.Any(x => x.Email == data.Email))
            {
                commonInBoth.Add(data);
            }
            else
            {
                neeeToAdd.Add(data);
            }
        }
        foreach (var data in LineInput2Unique)
        {
            if (LineInput1.Any(x => x.Email == data.Email))
            {
                commonInBoth2.Add(data);
            }
            else
            {
                neeeToInactivate.Add(data);
            }
        }
        Console.WriteLine("Common in Both and count: " + commonInBoth.Count);

        Console.WriteLine("Need to Add Count: " + neeeToAdd.Count);
        foreach (var data in neeeToInactivate)
        {
            //   Console.WriteLine(data.Email+ "|" + data.Level);
        }

        Console.WriteLine("Common in Both  2 and count: " + commonInBoth2.Count);
        Console.WriteLine("Need to Inactivate count: " + neeeToInactivate.Count);


        Console.WriteLine(LineInput1.Count);
        Console.WriteLine(LineInput2.Count);

    }
}
class EmailLevel
{
    public string Level { get; set; }
    public string Email { get; set; }
}