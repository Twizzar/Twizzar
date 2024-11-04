using System.Collections.Generic;

using DemoCode.Interfaces;

namespace DemoCode.ExampleCode
{
    public class ClassWithInternalDependecies
    {
        public int MyPublicInt { get; set;}

        internal int MyInternalInt { get; set; }

        internal List<MyInternalClass> ListTest { get; }

        internal MyInternalClass Test { get; set; }

        internal MyInternalClass _test;
    }
}
