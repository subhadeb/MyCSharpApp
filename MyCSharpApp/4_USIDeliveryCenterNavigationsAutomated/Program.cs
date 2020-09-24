using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
Program for Delivery Center File Handlings





*/


class Program
{
    //Used the Constant so for the flexibility of moving the Delivery Center folder to other locations in future.
    const string DeliveryCenterFolderPath = @"C:\Users\subdeb\Documents\Subha_Deb_497290\OfficeWorks\1 Delivery Center\";

    static void Main(string[] args)
    {
        List<DeliveryCenterNameLinks> deliveryCenterNameLinksList = GetDeliveryCenterNameLinks();
        Console.WriteLine("Identifier" + "\t" + "Section");
        foreach (DeliveryCenterNameLinks dcnlObj in deliveryCenterNameLinksList)
        {
            Console.WriteLine(dcnlObj.Identifier + "\t\t" + dcnlObj.DeliveryCenterName + " - " + dcnlObj.Section);
            if (dcnlObj.Section.Contains("Contacts"))//DC Contacts is the last section.
            {
                Console.WriteLine("------------------------------------------------");
            }
        }
        Console.WriteLine();
        Console.WriteLine("Enter The Identifier for for opening the File Path and the URL(Eg. 2.3)");
        string identifierInput = Console.ReadLine();
        DeliveryCenterNameLinks deliveryCenterNameLinkObject = deliveryCenterNameLinksList.FirstOrDefault(x => x.Identifier == identifierInput);
        if (deliveryCenterNameLinkObject != null)
        {
            Console.WriteLine(deliveryCenterNameLinkObject.DeliveryCenterName + " - " + deliveryCenterNameLinkObject.Section + " Path is below");
            Console.WriteLine(deliveryCenterNameLinkObject.FileLocationInPC);
            //Open the File Path in this PC
            System.Diagnostics.Process.Start(deliveryCenterNameLinkObject.FileLocationInPC);
            //Open the File Hyperlink in this Chrome Browser.
            System.Diagnostics.Process.Start("Chrome.exe", deliveryCenterNameLinkObject.SharepointHyperlink);
        }
        else
        {
            Console.WriteLine("Invalid Input");
        }
        Console.ReadKey();
    }
    static List<DeliveryCenterNameLinks> GetDeliveryCenterNameLinks()
    {
        List<DeliveryCenterNameLinks> list = new List<DeliveryCenterNameLinks>();
        //-------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------USI--------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------------------------
        list.Add(new DeliveryCenterNameLinks()
        {
            Identifier = "1.1",
            DeliveryCenterName = "USI",
            Section = "Rate Card & Expense Policy",
            SharepointHyperlink = "https://americas.internal.deloitteonline.com/sites/DelivryCntrPrtl/dev/SitePages/RateCardAndExpensePolicy.aspx",
            FileLocationInPC = DeliveryCenterFolderPath + @"1 USI\USI RateCardAndExpensePolicy\3FileToUpload"
        });

        list.Add(new DeliveryCenterNameLinks()
        {
            Identifier = "1.2",
            DeliveryCenterName = "USI",
            Section = "Work Order Template",
            SharepointHyperlink = "https://americas.internal.deloitteonline.com/sites/DelivryCntrPrtl/dev/SitePages/USIWorkOrderTemplate.aspx",
            FileLocationInPC = DeliveryCenterFolderPath + @"1 USI\USI WorkOrder\3FileToUpload WO"
        });
        list.Add(new DeliveryCenterNameLinks()
        {
            Identifier = "1.3",
            DeliveryCenterName = "USI",
            Section = "Delivery Center Contacts",
            SharepointHyperlink = "https://americas.internal.deloitteonline.com/sites/DelivryCntrPrtl/dev/SitePages/DeliveryCenterContacts.aspx",
            FileLocationInPC = DeliveryCenterFolderPath + @"1 USI\DC Contacts\3_Files To Upload"
        });
        //--------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------ARDC--------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------
        list.Add(new DeliveryCenterNameLinks()
        {
            Identifier = "2.1",
            DeliveryCenterName = "ARDC",
            Section = "Rate Card",
            SharepointHyperlink = "https://americas.internal.deloitteonline.com/sites/DelivryCntrPrtl/dev/SitePages/ARRateCardAndExpensePolicy.aspx",
            FileLocationInPC = DeliveryCenterFolderPath + @"2 ARDC\ARDC Rate Card and Expense Policy\3FileToUpload"
        });
        list.Add(new DeliveryCenterNameLinks()
        {
            Identifier = "2.2",
            DeliveryCenterName = "ARDC",
            Section = "Expense Policy",
            SharepointHyperlink = "https://americas.internal.deloitteonline.com/sites/DelivryCntrPrtl/dev/SitePages/ARRateCardAndExpensePolicy.aspx",
            FileLocationInPC = DeliveryCenterFolderPath + @"2 ARDC\ARDC Rate Card and Expense Policy\3FileToUpload"
        });
        list.Add(new DeliveryCenterNameLinks()
        {
            Identifier = "2.3",
            DeliveryCenterName = "ARDC",
            Section = "Work Order Template",
            SharepointHyperlink = "https://americas.internal.deloitteonline.com/sites/DelivryCntrPrtl/dev/SitePages/ARWorkordertemplate.aspx",
            FileLocationInPC = DeliveryCenterFolderPath + @"2 ARDC\ARDC Work Order Template\3FileToUpload"
        });
        list.Add(new DeliveryCenterNameLinks()
        {
            Identifier = "2.4",
            DeliveryCenterName = "ARDC",
            Section = "Delivery Center Contacts",
            SharepointHyperlink = "https://americas.internal.deloitteonline.com/sites/DelivryCntrPrtl/dev/SitePages/ARDeliveryCenterContacts.aspx",
            FileLocationInPC = DeliveryCenterFolderPath + @"2 ARDC\Contacts\3_Files To Upload"
        });
        //--------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------ERDC--------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------
        list.Add(new DeliveryCenterNameLinks()
        {
            Identifier = "3.1",
            DeliveryCenterName = "ERDC",
            Section = "Rate Card",
            SharepointHyperlink = "https://americas.internal.deloitteonline.com/sites/DelivryCntrPrtl/dev/SitePages/ERRateCardAndExpensePolicy.aspx",
            FileLocationInPC = DeliveryCenterFolderPath + @"3 ERDC\ERDC RateCardAndExpensePolicy\4_FilesToUpload"
        });
        list.Add(new DeliveryCenterNameLinks()
        {
            Identifier = "3.2",
            DeliveryCenterName = "ERDC",
            Section = "Expense Policy",
            SharepointHyperlink = "https://americas.internal.deloitteonline.com/sites/DelivryCntrPrtl/dev/SitePages/ERRateCardAndExpensePolicy.aspx",
            FileLocationInPC = DeliveryCenterFolderPath + @"3 ERDC\ERDC RateCardAndExpensePolicy\4_FilesToUpload"
        });
        list.Add(new DeliveryCenterNameLinks()
        {
            Identifier = "3.3",
            DeliveryCenterName = "ERDC",
            Section = "Work Order Template",
            SharepointHyperlink = "https://americas.internal.deloitteonline.com/sites/DelivryCntrPrtl/dev/SitePages/ERWorkOrderTemplate.aspx",
            FileLocationInPC = DeliveryCenterFolderPath + @"3 ERDC\ERDC Work Order\3FilesToUpload"
        });
        list.Add(new DeliveryCenterNameLinks()
        {
            Identifier = "3.4",
            DeliveryCenterName = "ERDC",
            Section = "Delivery Center Contacts",
            SharepointHyperlink = "https://americas.internal.deloitteonline.com/sites/DelivryCntrPrtl/dev/SitePages/ERDeliveryCenterContacts.aspx",
            FileLocationInPC = DeliveryCenterFolderPath + @"3 ERDC\ERDC Contacts\3_Files To Upload"
        });
        //-------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------PDC--------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------------------------
        list.Add(new DeliveryCenterNameLinks()
        {
            Identifier = "4.1",
            DeliveryCenterName = "PDC",
            Section = "Rate Card & Expense Policy",
            SharepointHyperlink = "https://americas.internal.deloitteonline.com/sites/DelivryCntrPrtl/dev/SitePages/DCPDCRateCardAndExpensePolicy.aspx",
            FileLocationInPC = DeliveryCenterFolderPath + @"4 PDC\PDC Rate Card and Expense Policy\3FileToUpload"
        });

        list.Add(new DeliveryCenterNameLinks()
        {
            Identifier = "4.2",
            DeliveryCenterName = "PDC",
            Section = "Work Order Template",
            SharepointHyperlink = "https://americas.internal.deloitteonline.com/sites/DelivryCntrPrtl/dev/SitePages/DCPDCUSIWorkOrderTemplate.aspx",
            FileLocationInPC = DeliveryCenterFolderPath + @"4 PDC\PDC Work Order\3FileToUpload"
        });
        list.Add(new DeliveryCenterNameLinks()
        {
            Identifier = "4.3",
            DeliveryCenterName = "PDC",
            Section = "Delivery Center Contacts",
            SharepointHyperlink = "https://americas.internal.deloitteonline.com/sites/DelivryCntrPrtl/dev/SitePages/DCPDCDeliveryCenterContacts.aspx",
            FileLocationInPC = DeliveryCenterFolderPath + @"4 PDC\PDC Contacts\3_Files To Upload"
        });


        return list;
    }

}
class DeliveryCenterNameLinks
{
    public string Identifier { get; set; }
    public string DeliveryCenterName { get; set; }
    public string Section { get; set; }
    public string SharepointHyperlink { get; set; }
    public string FileLocationInPC { get; set; }
}