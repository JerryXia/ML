using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqLambdaTest
{
    public static class LambdaOperator
    {
        #region 动态生成i=>i>5的表达式
        public static IEnumerable<int> getLambda(int[] ints, int intNum)
        {
            //创建参数
            var parameter = Expression.Parameter(typeof(int), "i");
            //创建常亮 5
            var constant = Expression.Constant(intNum);
            //创建比较表达式i>5
            var bin = Expression.GreaterThan(parameter, constant);
            //获取Lambda表达式
            var lambda = Expression.Lambda<Func<int, bool>>(bin, parameter);
            //通过Compile方法获取Delegate
            var _r = ints.Where(lambda.Compile());

            return _r;
        }
        #endregion

        #region 动态生成ints.Where(i => i > 5 && i <= 7);
        public static IEnumerable<int> getLambda2(int[] ints, int iMax, int iMin)
        {
            //创建参数
            var parameter = Expression.Parameter(typeof(int), "i");

            var con1 = Expression.Constant(iMax);
            var bin1 = Expression.GreaterThan(parameter, con1);

            var con2 = Expression.Constant(iMin);
            var bin2 = Expression.LessThanOrEqual(parameter, con2);
            //组合两个表达式
            var body = Expression.Add(bin1, bin2);
            //并使用 And 完成多个表达式的组装

            //获取Lambda表达式
            var lambda = Expression.Lambda<Func<int, bool>>(body, parameter);

            //通过Compile方法获取Delegate
            var _r = ints.Where(lambda.Compile());
            return _r;
        }
        #endregion

        #region 动态生成ints.Where(i => (i > 5 && i <= 7) || (i == 3));
        public static IEnumerable<int> getLambda3(int[] ints, int[] args)
        {
            //创建参数
            var parameter = Expression.Parameter(typeof(int), "i");

            var con1 = Expression.Constant(args[0]);
            var bin1 = Expression.GreaterThan(parameter, con1);

            var con2 = Expression.Constant(args[1]);
            var bin2 = Expression.LessThanOrEqual(parameter, con2);

            var con3 = Expression.Constant(args[2]);
            var bin3 = Expression.Equal(parameter, con2);

            //组合两个表达式
            var body = Expression.Add(bin1, bin2);
            //并使用 And 完成多个表达式的组装
            body = Expression.Or(body, bin3);

            //获取Lambda表达式
            var lambda = Expression.Lambda<Func<int, bool>>(body, parameter);

            //通过Compile方法获取Delegate
            var _r = ints.Where(lambda.Compile());
            return _r;
        }
        #endregion

        #region 动态生成ints.Select(i => i % 2 == 0 ? i : 0);
        public static IEnumerable<int> getLambda4(int[] ints, int[] args)
        {
            //创建参数
            var parameter = Expression.Parameter(typeof(int), "i");

            //i%2
            var con1 = Expression.Constant(args[0]);
            var bin1 = Expression.Modulo(parameter, con1);

            // (i%2)==0
            var con2 = Expression.Constant(args[1]);
            var bin2 = Expression.Equal(bin1, con2);

            // IF(((I%2)=0),i,0)
            var bin3 = Expression.Condition(bin2, parameter, Expression.Constant(args[1]));
            //返回i值 或者 默认参数的值 

            //获取Lambda表达式
            var lambda = Expression.Lambda<Func<int, int>>(bin3, parameter);

            //通过Compile方法获取Delegate
            var _r = ints.Select(lambda.Compile());

            return _r;
        }
        #endregion

        #region 动态生成Array.ForEach<int>(ints, i => Console.WriteLine(i));
        public static void getLambda5(int[] ints, int intNum)
        {
            //创建参数
            var parameter = Expression.Parameter(typeof(int), "i");

            //获取 Console.WriteLine MethodInfo
            MethodInfo method = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(int) });

            //创建表达式
            var call = Expression.Call(method, parameter);

            var lambda = Expression.Lambda<Action<int>>(call, parameter);

            //通过Compile方法获取Delegate
            Array.ForEach<int>(ints, lambda.Compile());

        }
        #endregion

    }
}
