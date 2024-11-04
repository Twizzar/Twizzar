using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Twizzar.Runtime.Infrastructure.ApplicationService;
using Twizzar.Runtime.Test.Builder;
using Twizzar.SharedKernel.NLog.Logging;

namespace Twizzar.Runtime.Test.IntegrationTest
{
    [TestClass]
    [TestCategory("IntegrationTest")]
    public class ReflectionDescriptionProviderTest
    {
        [TestInitialize]
        public void Initialize()
        {
            LoggerFactory.SetConfig(new LoggerConfigurationBuilder());
        }

        [TestMethod]
        public void ReflectionDescriptionProvider_GetTypeDescription_CorrectlyResolveEmailMessageBuffer()
        {
            var sut = new ReflectionTypeDescriptionProvider(
                new ReflectionDescriptionFactoryBuilder().Build());
            var result = sut.GetTypeDescription(typeof(TestClass2));

            var emailMessageBuffer = typeof(TestClass2);

            const BindingFlags attr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            Assert.IsNotNull(result);
            Assert.AreEqual(emailMessageBuffer.GetProperties(attr).Length, result.GetDeclaredProperties().Length);
            Assert.AreEqual(emailMessageBuffer.GetFields(attr).Length, result.GetDeclaredFields().Length);
            Assert.AreEqual(emailMessageBuffer.GetConstructors(attr).Length, result.GetDeclaredConstructors().Length);
        }
    }

    // ReSharper disable InconsistentNaming
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0060 // Remove unused parameter

    public class TestClass2
    {
        public int intProp { get; set; }
        public int intProp2 { get; set; }

        public int intField;
        public int intField2;

        public TestClass2(int p1, int p2)
        {

        }
    }
}
