using System.Collections.Generic;

using DemoCode.Interfaces.ExampleCode;

namespace DemoCode.ExampleCode
{
    public class StatusTest
    {
        public IMarkerInterface MarkerInterface { get; set; }

        public IEnumerable<int> Enumberable { get; set; }
    }
}
