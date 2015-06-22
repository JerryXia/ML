using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalizeDemo
{
    class Beijing : China
    {
        public Beijing()
            : base()
        {

        }

        ~Beijing()
        {
            Log.Write("Beijing's destructor is called");
        }
    }
}
