using Twizzar.Design.Shared.CoreInterfaces.Name;
using Twizzar.SharedKernel.CoreInterfaces.FixtureItem.Name;
using Twizzar.TestCommon;

namespace Twizzar.Design.TestCommon
{
    public static class DesignTestHelper
    {
        public static ITypeFullName RandomDesignTypeFullName(string prefix = "", int namespaceSegments = -1)
        {
            var name = TestHelper.RandomTypeFullName(prefix, namespaceSegments);
            return TypeFullName.Create(name.FullName);
        }
    }
}
