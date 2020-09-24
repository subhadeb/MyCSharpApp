using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsoleCSharpPrograms
{
    [Serializable]
    public class Employee
    {
        public Employee() { }

        public Employee(int id,string name)
        {
            this.ID = id;
            this.Name = name;
        }
        [XmlAttribute]
        public int ID { get; set; }

        [XmlAttribute]
        public string Name { get; set; }
    }
}