using Microsoft.VisualStudio.TestTools.UnitTesting;

using Twizzar.SharedKernel.NLog.Logging;

#pragma warning disable IDE0060 // Remove unused parameter

namespace Twizzar.Design.Test;

[TestClass]
public class TestInitializer
{
    [AssemblyInitialize]
    public static void AssemblyInit(TestContext context)
    {
        LoggerFactory.SetConfig(new LoggerConfigurationBuilder());
    }
}