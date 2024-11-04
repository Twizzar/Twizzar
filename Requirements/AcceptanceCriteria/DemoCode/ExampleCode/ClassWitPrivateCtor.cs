namespace DemoCode.ExampleCode
{
    public class ClassWitPrivateCtor
    {
        private ClassWitPrivateCtor()
        {
            MyInt = 5;
            MyString = "test";
        }

        private ClassWitPrivateCtor(int i)
        {
            MyInt = i;
            MyString = "test";
        }

        private ClassWitPrivateCtor(string s, int i)
        {
            MyInt = i;
            MyString = s;
        }

        //protected ClassWitPrivateCtor(string s, int i, int b)
        //{
        //    MyInt = i;
        //    MyString = s;
        //}

        public int MyInt { get; set; }

        public string MyString { get; set; }
    }
}
