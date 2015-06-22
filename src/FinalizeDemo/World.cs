using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalizeDemo
{
    class World
    {
        protected FileStream fs = null;
        protected SqlConnection conn = null;

        public World()
        {
            fs = new FileStream(Log.logFilePath, FileMode.Open);
            conn = new SqlConnection();
        }

        protected void Finalize()
        {
            fs.Dispose();
            conn.Dispose();
            Log.Write("World's destructor is called");
        }
    }
}
