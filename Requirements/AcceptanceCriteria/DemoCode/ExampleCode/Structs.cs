namespace DemoCode.ExampleCode
{

    public struct TestStruct
    {
        public TestStruct(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public double X { get; }
        public double Y { get; private set; }
        public double Z => 4;
    }

    public struct EmptyStruct { }

    public struct AnotherTestStruct
    {
        public AnotherTestStruct(TestStruct left, TestStruct right)
        {
            this.Left = left;
            this.Right = right;
        }

        public TestStruct Left { get; set; }

        public TestStruct Right { get; set; }
    }
}
