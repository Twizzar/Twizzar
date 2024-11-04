using System;
using System.Collections.Generic;

namespace DemoCode.ExampleCode
{
    public class GenericsTest
    {

        public IEnumerable<int> IEnumerableIntArray { get; set; } = new[] { 1, 2, 3, 4, 5 };

        public IEnumerable<int> IEnumerableIntList { get; set; } = new List<int> { 6, 7, 8, 9, 10 };

        public Dictionary<int, string> Dictionary { get; set; } = new Dictionary<int, string>();

        // Not supported yet!
        //public int[] IntArray { get; set; } = new[] { 1, 2, 3, 4, 5 };

        public IList<int> IntIList { get; set; } = new List<int> { 6, 7, 8, 9, 10 };

        public List<int> IntList { get; set; } = new List<int> { 11, 12, 13, 14, 15 };

        public Calculator<int> CalculatorA { get; set; } = new Calculator<int>();

        public BigInt BigInt { get; set; } = new BigInt();

        public Calculator<double> CalculatorB { get; set; } = new Calculator<double>();

        public Calculator<BigInt> CalculatorC { get; set; } = new Calculator<BigInt>();

        public Tuple<int, int> TupleIntInt { get; set; } = new Tuple<int, int>(1, 2);

        public Tuple<int, BigInt> TupleIntBigInt { get; set; } = new Tuple<int, BigInt>(1, new BigInt());

        public IList<Tuple<int, string>> ListOfTupleIntString { get; set; } = new List<Tuple<int, string>>();

        public Nullable<int> PlainNullableInt { get; set; }

        public int? NullableInt { get; set; }

        public GenericTest<int> GenericTestInt {get; set;}
        
        public GenericTest<GenericTest<int>> GenericGenericTestInt { get; set; }

    }

    public class Calculator<T>
    {
        public T AccuA { get; set; }
        public T AccuB { get; set; }
    }

    public class BigInt
    {
        public List<int> BigDigits { get; set; }
        public string Value { get; set; }
    }

    public class GenericTest<T>
    {
        public GenericTest(T value)
        {
            this.Value = value;
        }

        public T Value { get; private set; }

        public T Method(T a) => default(T);

        public int Test { get; }
    }

    public interface IGenericTest<T>
    {
        T MethodWithGenericReturn();

        int MethodWithGenericParameter(T a);
    }
}
