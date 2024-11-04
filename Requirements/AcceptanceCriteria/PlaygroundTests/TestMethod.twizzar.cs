using Twizzar.Fixture;
using PlaygroundTests;

namespace PlaygroundTests
{
    partial class TestMethod
    {
        private class IMyMethodTest0fbcBuilder : ItemBuilder<PlaygroundTests.IMyMethodTest, IMyMethodTest0fbcBuilderPaths>
        {
            public IMyMethodTest0fbcBuilder()
            {
                this.With(p => p.Value2.Value(2));
                this.With(p => p.FirstMethod__Int32.Value(2));
                this.With(p => p.GetMethodTest.SomeComplicatedMethod__Dictionary2_Single_String.Item1.Value(5));
                this.With(p => p.GetMethodTest.SomeComplicatedMethod__Dictionary2_Single_String.Item2.Value("Test"));
            }
        }
    }
}