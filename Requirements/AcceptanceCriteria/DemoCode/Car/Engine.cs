using System;
using System.Collections.Generic;

namespace DemoCode.Car
{
    public class Engine : IEngine
    {
        public byte CylinderCount { get; }
        public float Start()
        {
            throw new NotImplementedException();
        }

        public IEngine AnotherEngine { get; }
        public List<int> NotSupportedList { get; set; }

        public Engine(List<int> notSupportedList)
        {
            if (notSupportedList == null) throw new ArgumentNullException(nameof(notSupportedList));
            NotSupportedList = notSupportedList;
        }
    }
}
