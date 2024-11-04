using System;
using Twizzar.Fixture;

#pragma warning disable S1481 // Unused local variables should be removed.
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable IDE0060 // Remove unused parameter

namespace ExampleCode
{
    public class RoslynDocumentReaderExampleCode
    {
        public void Test()
        {
            var number = new ItemBuilder<int>();

            var namedNumber = new ItemBuilder<int>();

            new ItemBuilder<string>();

            new ItemBuilder<string>(new ItemConfig<string>());

            var a = Build
                .New<char>();

            new
                ItemBuilder<char>();

            new ItemBuilder();

            NotTheContainer.New<int>();
            NotTheContainer.New<int>();
        }
    }


}