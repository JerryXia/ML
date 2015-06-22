using System.Collections.Generic;

namespace LinqLambdaTest
{
    class Program
    {
        static void Main(string[] args)
        {

            //test 1
            var ints = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            IEnumerable<int> list1 = LambdaOperator.getLambda(ints, 5);

            //IEnumerable<int> list2 = LambdaOperator.getLambda2(ints, 7, 5);

            IEnumerable<int> list3 = LambdaOperator.getLambda3(ints, new int[] { 5, 7, 3 });


        }
    }
}
