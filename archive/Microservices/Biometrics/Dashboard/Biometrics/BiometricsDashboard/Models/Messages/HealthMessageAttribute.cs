using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class HealthMessageAttribute : Attribute
    {
        public HealthMessageAttribute(string name, string group)
        {
            Name = name;
            Group = group;
        }
        public string Name { get; private set; }
        public string Group { get; private set; }
    }
}
