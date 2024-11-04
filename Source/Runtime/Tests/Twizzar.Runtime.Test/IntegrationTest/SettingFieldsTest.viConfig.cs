using Twizzar.Fixture;
using Twizzar.Fixture.Member;

namespace Twizzar.Runtime.Test.IntegrationTest
{
    public partial class SettingFieldsTest 
    {
        private class TestClassPath : PathProvider<TestClass>
        {
            public FieldBasetypeMemberPath<TestClass, int> PublicInt => new("PublicInt", RootPath);
            public FieldBasetypeMemberPath<TestClass, int> _privateInt => new("_privateInt", RootPath);
            public FieldBasetypeMemberPath<TestClass, int> ReadonlyInt => new("ReadonlyInt", RootPath);
            public FieldBasetypeMemberPath<TestClass, int> _protectedInt => new("_protectedInt", RootPath);
            public FieldBasetypeMemberPath<TestClass, int> InternalInt => new("InternalInt", RootPath);
            public PropertyBasetypeMemberPath<TestClass, int> PrivateIntValue => new("PrivateIntValue", RootPath);
            public PropertyBasetypeMemberPath<TestClass, int> ProtectedInt => new("ProtectedInt", RootPath);
        }

        private class TestFieldsBuilder : ItemBuilder<TestClass, TestClassPath>
        {
            public TestFieldsBuilder()
            {
                this.With(path => path.PublicInt.Value(5));
                this.With(path => path._privateInt.Value(6));
                this.With(path => path._protectedInt.Value(7));
                this.With(path => path.InternalInt.Value(8));
                this.With(path => path.ReadonlyInt.Value(9));
            }
        }
    }
}