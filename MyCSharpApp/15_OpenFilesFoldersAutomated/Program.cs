using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class Program
{
    static void Main(string[] args)
    {
       
        MustDoTrackerPathOperations();

    }
    static void MustDoTrackerPathOperations()
    {
        List<FileFolderPathModel> fileFolderPathModels = new List<FileFolderPathModel>();
        fileFolderPathModels.Add(new FileFolderPathModel()
        {
            Title = "Must Do JS",
            Path = @"https://americas.internal.deloitteonline.com/sites/MustDoTracker/Dev/SiteAssets/js/ConfigurationScreens/MustDoConfig.js"
        });
        fileFolderPathModels.Add(new FileFolderPathModel()
        {
            Title = "Must Do HTML",
            Path = @"https://americas.internal.deloitteonline.com/sites/MustDoTracker/Dev/SiteAssets/html/ConfigurationScreens/MustDoConfig.html"
        });

        foreach (var item in fileFolderPathModels)
        {
            Process myProcess = new Process();
            Process.Start("notepad++.exe", "file//"+item.Path);
        }

    }
}


class FileFolderPathModel
{
    public string Title { get; set; }
    public string Path { get; set; }
}