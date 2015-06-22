using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalizeDemo
{
    class China : World
    {
        public China()
            : base()
        {

        }


        ~China()
        {
            Log.Write("China's destructor is called");
        }
        /*

        protected override void Finalize()
        {
            Log.Write("China's destructor is called");
        }
        */
    }
}
