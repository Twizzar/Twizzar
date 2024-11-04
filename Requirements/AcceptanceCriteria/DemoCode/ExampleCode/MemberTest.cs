using DemoCode.Interfaces.ExampleCode;

namespace DemoCode.ExampleCode
{
    public class MemberTest
    {
        private string privateField;
        protected char protectedField;
        public int publicField;
        internal long internalField;

        private string privateProperty { get; }
        protected char protectedProperty { get; }
        public IVehicle publicProperty { get; }
        internal int internalProperty { get; }

        public MemberTest(int a, int b)
        {

        }

        public class NestedMemberTEst
        {
            private string privateField;
            protected char protectedField;
            public int publicField;
            internal long internalField;
        }
    }
}
