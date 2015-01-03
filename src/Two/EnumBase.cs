namespace Two
{
    public class EnumBase<T>
    {
        protected string _name;
        protected T _value;

        public override string ToString()
        {
            return _name.ToString();
        }

        /// <summary>
        /// 向目标T类型显式转换
        /// </summary>
        /// <param name="enumInstance"></param>
        /// <returns></returns>
        public static explicit operator T(EnumBase<T> otherEnum)
        {
            return otherEnum._value;
        }

        public string Name
        {
            get { return _name; }
        }

        public T Value
        {
            get { return _value; }
        }

    }
}
