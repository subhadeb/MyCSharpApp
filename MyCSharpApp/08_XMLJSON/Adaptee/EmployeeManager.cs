using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;


namespace ConsoleCSharpPrograms
{
    public class EmployeeManager
    {
        List<Employee> empList;
        public EmployeeManager()
        {
            Employee emp1 = new Employee(101, "John");
            Employee emp2 = new Employee(102, "Marie");
            Employee emp3 = new Employee(101, "Sam");
            empList = new List<Employee>();
            empList.Add(emp1);
            empList.Add(emp2);
            empList.Add(emp3);
        }
        //Gets all the Employees in XML format.
        //With Adapter Pattern need to make it virutal.
        public virtual string GetAllEmployees()
        {
            var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(empList.GetType());
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            using (var stream = new StringWriter())
            {
                using (var writer = XmlWriter.Create(stream, settings))
                {
                    serializer.Serialize(writer, empList, emptyNamespaces);
                    return stream.ToString();
                }
            }
        }
    }
}