using System.Collections.Generic;

namespace DemoCode.Car
{
    public interface IEngine
    {
        byte CylinderCount { get; }

        float Start();

        IEngine AnotherEngine { get; }

        List<int>  NotSupportedList { get; }
    }
}