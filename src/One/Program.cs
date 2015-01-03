using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace One
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("数据结构分为两种 数据的逻辑结构 和 数据的物理结构（存储结构）");

            Console.WriteLine("存储结构分为 1. 顺序存储（数组）   2. 链式存储（指针）");


            FirstType firstType = new FirstType(2, new SecondType() { Name = "xxx"}) { Name = "FirstType" };
            //SecondType second1 = (SecondType)firstType;

            firstType.Second.Name = "yyyy";

            if (firstType is FirstType)
            {
                FirstType second2 = firstType as FirstType;
            }

            SecondType s1 = new SecondType() { Name="xxxx"};
            SecondType s2 = new SecondType() { Name = "yyyyy" };
            s2 = s1 + s2;

            var p1 = new Person("NB0903100006");
            var p2 = new Person("NB0904140001");

            Console.WriteLine(p1.GetHashCode());
            Console.WriteLine(p2.GetHashCode());



            Console.ReadKey();
        }
    }


    #region 类型转换

    public class FirstType
    {
        private readonly int i1 = 4;

        private readonly SecondType second;

        public FirstType(int _i1, SecondType _second)
        {
            i1 = _i1;
            i1 = 3;
            second = _second;
        }

        public SecondType Second
        {
            get
            {
                return second;
            }
        }

        public string Name { get; set; }
    }

    public class SecondType
    {
        public string Name { get; set; }

        public static explicit operator SecondType(FirstType firstType)
        {
            SecondType secondType = new SecondType() { Name = firstType.Name };
            return secondType;
        }

        public static SecondType operator +(SecondType s1, SecondType s2)
        {
            s2.Name += s1.Name;
            return s2;


        }

    }


    #endregion


    #region 类型比较

    public class Person : IEquatable<Person>, IFormattable
    {
        public string IDCode { get; private set; }

        public Person(string idCode)
        {
            this.IDCode = idCode;
        }

        public override bool Equals(object obj)
        {
            return this.IDCode == (obj as Person).IDCode;
        }

        public override int GetHashCode()
        {
            //return this.IDCode.GetHashCode();
            string s = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "#" + this.IDCode;
            return s.GetHashCode();
        }




        public bool Equals(Person other)
        {
            return this.IDCode == other.IDCode;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

}
