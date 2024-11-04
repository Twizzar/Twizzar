namespace DemoCode.ExampleCode
{
    public class ConstructorTest
    {
        public int? IntProp { get; set; }

        public ClassA ClassA { get; set; }

        public string Text { get; set; }

        public ConstructorTest(int? intVal, ClassA classA, string text, bool isDefault, bool? isDefaultNullable)
        {
            IntProp = intVal;
            ClassA = classA;
            Text = text;
        }
    }
}
