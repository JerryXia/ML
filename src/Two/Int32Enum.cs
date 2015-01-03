namespace Two
{
    public class Int32Enum : EnumBase<int>, System.IComparable<int>, System.IEquatable<int>
    {
        #region 枚举

        public static readonly Int32Enum Success = new Int32Enum("Success", 1);
        public static readonly Int32Enum Fail = new Int32Enum("Fail");
        public static readonly Int32Enum Test = new Int32Enum("Test", 2);
        public static readonly Int32Enum Test2 = new Int32Enum("Test2");

        #endregion

        #region 变量

        private static int enumCounter = -1;

        private static int startEnumValue = -1;
        private static System.Collections.Concurrent.ConcurrentDictionary<string, int> dict
            = new System.Collections.Concurrent.ConcurrentDictionary<string, int>();

        protected static System.Collections.Generic.SortedDictionary<int, Int32Enum> members
            = new System.Collections.Generic.SortedDictionary<int, Int32Enum>();

        #endregion

        #region Constructor

        protected Int32Enum(string name)
        {
            _name = name;
            _value = ++startEnumValue;
            bool isSuccess = dict.TryAdd(_name, _value);
            if (isSuccess)
            {
                enumCounter += 1;
                members.Add(enumCounter, this);
            }
            else
            {
                startEnumValue -= 1;
                throw new System.Exception("Int32Enum dict add failed");
            }
        }

        protected Int32Enum(string name, int value)
        {
            _name = name;
            _value = value;
            bool isSuccess = dict.TryAdd(_name, _value);
            if (isSuccess)
            {
                startEnumValue = value;
                enumCounter += 1;
                members.Add(enumCounter, this);
            }
            else
            {
                throw new System.Exception("Int32Enum dict add failed");
            }
        }

        #endregion

        #region 重写Object基类方法

        public override bool Equals(object obj)
        {
            Int32Enum int32Enum = obj as Int32Enum;
            if (int32Enum != null)
            {
                return _value == int32Enum._value;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            string s = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "#" + this._name;
            return s.GetHashCode();

            //HEnum std = (HEnum)hashTable[this.Value];
            //if (std.Name == this.Name)
            //    return base.GetHashCode();
            //return std.GetHashCode();
        }

        #endregion

        #region IComparable<T> 接口继承

        public int CompareTo(int other)
        {
            //return _value.CompareTo(other);
            return _value - other;
        }

        #endregion

        #region IEquatable<T> 接口继承

        public bool Equals(int other)
        {
            return _value == other;
        }

        #endregion

        #region 重载操作符

        /// <summary>
        /// 从目标类型显式强制转换
        /// </summary>
        /// <param name="secondEnum"></param>
        /// <returns></returns>
        public static explicit operator Int32Enum(int instance)
        {
            foreach (var item in members)
            {
                if (item.Value._value == instance)
                {
                    return item.Value;
                }
            }
            return new Int32Enum(instance.ToString(), instance);
        }

        /// <summary>
        /// 向int类型隐式转换
        /// </summary>
        /// <param name="otherEnum"></param>
        /// <returns></returns>
        public static implicit operator int(Int32Enum other)
        {
            return other._value;
        }

        public static bool operator !=(Int32Enum e1, Int32Enum e2)
        {
            return e1._value != e2._value;
        }

        public static bool operator ==(Int32Enum e1, Int32Enum e2)
        {
            return e1._value == e2._value;
        }

        public static bool operator <(Int32Enum e1, Int32Enum e2)
        {
            return e1._value < e2._value;
        }

        public static bool operator <=(Int32Enum e1, Int32Enum e2)
        {
            return e1._value <= e2._value;
        }

        public static bool operator >(Int32Enum e1, Int32Enum e2)
        {
            return e1._value > e2._value;
        }

        public static bool operator >=(Int32Enum e1, Int32Enum e2)
        {
            return e1._value >= e2._value;
        }

        #endregion

    }
}
