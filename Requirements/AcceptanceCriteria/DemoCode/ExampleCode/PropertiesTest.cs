using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoCode.ExampleCode
{
    public class PropertiesTest
    {
        private int propertyWithBackingField;

        public int PropertyWithBackingField 
        { 
            get 
            {
                Console.WriteLine(nameof(PropertyWithBackingField) + " has been called.");
                return propertyWithBackingField;
            } 
            set => propertyWithBackingField = value; 
        }
    }
}
