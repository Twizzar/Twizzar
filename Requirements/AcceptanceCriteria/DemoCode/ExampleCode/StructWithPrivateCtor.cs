namespace DemoCode.ExampleCode
{
    public class StructWithPrivateCtor
    {
        private StructWithPrivateCtor(int myInt) => MyProperty = myInt;

        public int MyProperty { get; set; }
    }
}
