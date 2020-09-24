using ConsoleCSharpPrograms.Target;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleCSharpPrograms.Adapter
{
    class EmployeeAdapter : EmployeeManager, IEmployeeManager
    {
        public override string GetAllEmployees()
        {
            string xmlData = base.GetAllEmployees();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlData);
            string jsonData = JsonConvert.SerializeObject(doc, Newtonsoft.Json.Formatting.Indented);
            return jsonData;
        }
    }
}
