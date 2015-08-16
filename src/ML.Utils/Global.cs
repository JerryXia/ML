using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ML.Utils
{
    public class Global
    {
        static Global()
        {

        }

        /// <summary>
        /// 当前运行环境是否是Mono
        /// </summary>
        public static readonly bool IsMonoRuntime = Type.GetType("Mono.Runtime") != null;

    }
}
