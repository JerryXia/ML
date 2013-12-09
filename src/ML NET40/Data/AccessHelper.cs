using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace ML.Data
{
    public class AccessHelper
    {
        #region private method & constructor

        private AccessHelper() { }

        private static void PrepareCommand(OleDbCommand command, OleDbConnection connection, OleDbTransaction transaction, string commandText, OleDbParameter[] commandParameters, out bool mustCloseConnection)
        {
            if (null == command) throw new ArgumentNullException("command");
            if (null == commandText || string.Empty == commandText) throw new ArgumentNullException("commandText");

            if (ConnectionState.Closed == connection.State)
            {
                connection.Open();
                mustCloseConnection = true;
            }
            else
            {
                mustCloseConnection = false;
            }

            command.Connection = connection;

            command.CommandText = commandText;

            if (null != transaction)
            {
                if (null == transaction.Connection) throw new ArgumentException("transaction对象commited和rollbacked需要一个已打开的事务", "transaction");
                command.Transaction = transaction;
            }

            command.CommandType = CommandType.Text;

            if (null != commandParameters)
            {
                AttachParameters(command, commandParameters);
            }
        }

        private static void AttachParameters(OleDbCommand command, OleDbParameter[] commandParameters)
        {
            if (null == command) throw new ArgumentNullException("command");

            if (null != commandParameters)
            {
                foreach (OleDbParameter p in commandParameters)
                {
                    if (null != p)
                    {
                        if ((ParameterDirection.Input == p.Direction || ParameterDirection.InputOutput == p.Direction) && (null == p.Value))
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p);
                    }
                }
            }
        }

        #endregion private method & constructor

        #region ExecuteNonQuery

        public static int ExecuteNonQuery(string connectionstring, string commandText)
        {
            return ExecuteNonQuery(connectionstring, commandText, (OleDbParameter[])null);
        }

        public static int ExecuteNonQuery(string connectionString, string commandText, params OleDbParameter[] commandParameters)
        {
            if (null == connectionString || string.Empty == connectionString) throw new ArgumentNullException("connectionString");

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                return ExecuteNonQuery(connection, commandText, commandParameters);
            }
        }

        public static int ExecuteNonQuery(OleDbConnection connection, string commandText)
        {
            return ExecuteNonQuery(connection, commandText, (OleDbParameter[])null);
        }

        public static int ExecuteNonQuery(OleDbConnection connection, string commandText, params OleDbParameter[] commandParameters)
        {
            if (null == connection) throw new ArgumentNullException("connection");

            OleDbCommand cmd = new OleDbCommand();
            bool mustCloseConnection = false;

            PrepareCommand(cmd, connection, (OleDbTransaction)null, commandText, commandParameters, out mustCloseConnection);

            int retval = cmd.ExecuteNonQuery();

            cmd.Parameters.Clear();

            if (mustCloseConnection) connection.Close();

            return retval;
        }

        public static int ExecuteNonQuery(OleDbTransaction transaction, string commandText)
        {
            return ExecuteNonQuery(transaction, commandText, (OleDbParameter[])null);
        }

        public static int ExecuteNonQuery(OleDbTransaction transaction, string commandText, params OleDbParameter[] commandParameters)
        {
            if (null == transaction) throw new ArgumentNullException("transaction");
            if (null != transaction && null == transaction.Connection) throw new ArgumentException("transaction对象commited和rollbacked需要一个已打开的事务", "transaction");

            OleDbCommand cmd = new OleDbCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandText, commandParameters, out mustCloseConnection);

            int retval = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();

            return retval;
        }

        #endregion ExecuteNonQuery

        #region ExecuteScalar

        public static object ExecuteScalar(string connectionString, string commandText)
        {
            return ExecuteScalar(connectionString, commandText, (OleDbParameter[])null);
        }

        public static object ExecuteScalar(string connectionString, string commandText, params OleDbParameter[] commandParameters)
        {
            if (null == connectionString && string.Empty == connectionString) throw new ArgumentNullException("connectionString");

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                return ExecuteScalar(connection, commandText, commandParameters);
            }
        }

        public static object ExecuteScalar(OleDbConnection connection, string commandText)
        {
            return ExecuteScalar(connection, commandText, (OleDbParameter[])null);
        }

        public static object ExecuteScalar(OleDbConnection connection, string commandText, params OleDbParameter[] commandParameters)
        {
            if (null == connection) throw new ArgumentNullException("connection");

            OleDbCommand cmd = new OleDbCommand();

            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (OleDbTransaction)null, commandText, commandParameters, out mustCloseConnection);

            object retval = cmd.ExecuteScalar();
            cmd.Parameters.Clear();

            if (mustCloseConnection)
                connection.Close();

            return retval;
        }

        public static object ExecuteScalar(OleDbTransaction transaction, string commandText)
        {
            return ExecuteScalar(transaction, commandText, (OleDbParameter[])null);
        }

        public static object ExecuteScalar(OleDbTransaction transaction, string commandText, params OleDbParameter[] commandParameters)
        {
            if (null == transaction) throw new ArgumentNullException("transaction");
            if (null != transaction && null == transaction.Connection) throw new ArgumentException("transaction对象commited和rollbacked需要一个已打开的事务", "transaction");

            OleDbCommand cmd = new OleDbCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandText, commandParameters, out mustCloseConnection);

            object retval = cmd.ExecuteScalar();
            cmd.Parameters.Clear();

            return retval;
        }

        #endregion ExecuteScalar

        #region ExecuteReader

        private enum OleDbConnectionOwnership
        {
            /// <summary>
            /// 连接对象是由SqlHelper管理的
            /// </summary>
            Internal,
            /// <summary>
            /// 连接对象是由调用者管理的
            /// </summary>
            External
        }

        private static OleDbDataReader ExecuteReader(OleDbConnection connection, OleDbTransaction transaction, string commandText, OleDbParameter[] commandParameters, OleDbConnectionOwnership connectionOwnership)
        {
            if (null == connection) throw new ArgumentNullException("connection");

            bool mustCloseConnection = false;

            OleDbCommand cmd = new OleDbCommand();
            try
            {
                PrepareCommand(cmd, connection, transaction, commandText, commandParameters, out mustCloseConnection);

                OleDbDataReader dataReader;

                if (OleDbConnectionOwnership.External == connectionOwnership)
                {
                    dataReader = cmd.ExecuteReader();
                }
                else
                {
                    dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }

                bool canClear = true;

                foreach (OleDbParameter commandParameter in cmd.Parameters)
                {
                    if (ParameterDirection.Input != commandParameter.Direction) canClear = false;
                }

                if (canClear)
                {
                    cmd.Parameters.Clear();
                }

                return dataReader;
            }
            catch
            {
                if (mustCloseConnection) connection.Close();
                throw;
            }
        }

        public static OleDbDataReader ExecuteReader(OleDbConnection connection, string commandText)
        {
            return ExecuteReader(connection, commandText, (OleDbParameter[])null);
        }

        public static OleDbDataReader ExecuteReader(OleDbConnection connection, string commandText, params OleDbParameter[] commandParameters)
        {
            return ExecuteReader(connection, (OleDbTransaction)null, commandText, commandParameters, OleDbConnectionOwnership.External);
        }

        public static OleDbDataReader ExecuteReader(OleDbTransaction transaction, string commandText)
        {
            return ExecuteReader(transaction, commandText, (OleDbParameter[])null);
        }

        public static OleDbDataReader ExecuteReader(OleDbTransaction transaction, string commandText, params OleDbParameter[] commandParameters)
        {
            if (null == transaction) throw new ArgumentNullException("transaction");
            if (null != transaction && null == transaction.Connection) throw new ArgumentException("transaction对象commited和rollbacked需要一个已打开的事务", "transaction");

            return ExecuteReader(transaction.Connection, transaction, commandText, commandParameters, OleDbConnectionOwnership.External);
        }

        public static OleDbDataReader ExecuteReader(string connectionString, string commandText)
        {
            return ExecuteReader(connectionString, commandText, (OleDbParameter[])null);
        }

        public static OleDbDataReader ExecuteReader(string connectionString, string commandText, params OleDbParameter[] commandParameters)
        {
            if (null == connectionString || string.Empty == connectionString) throw new ArgumentNullException("connectionString");
            OleDbConnection connection = null;
            try
            {
                connection = new OleDbConnection(connectionString);
                connection.Open();

                return ExecuteReader(connection, (OleDbTransaction)null, commandText, commandParameters, OleDbConnectionOwnership.Internal);
            }
            catch
            {
                if (connection != null) connection.Close();
                throw;
            }
        }

        #endregion ExecuteReader

        #region ExecuteDataset

        public static DataSet ExecuteDataset(string connectionString, string commandText)
        {
            return ExecuteDataset(connectionString, commandText, (OleDbParameter[])null);
        }

        public static DataSet ExecuteDataset(string connectionString, string commandText, params OleDbParameter[] commandParameters)
        {
            if (null == connectionString || string.Empty == connectionString) throw new ArgumentNullException("connectionString");

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                return ExecuteDataset(connectionString, commandText, commandParameters);
            }
        }

        public static DataSet ExecuteDataset(OleDbConnection connection, string commandText)
        {
            return ExecuteDataset(connection, commandText, (OleDbParameter[])null);
        }

        public static DataSet ExecuteDataset(OleDbConnection connection, string commandText, params OleDbParameter[] commandParameters)
        {
            if (null == connection) throw new ArgumentNullException("connection");

            OleDbCommand cmd = new OleDbCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (OleDbTransaction)null, commandText, commandParameters, out mustCloseConnection);

            using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                da.Fill(ds);

                cmd.Parameters.Clear();

                if (mustCloseConnection)
                    connection.Close();

                return ds;
            }
        }

        public static DataSet ExecuteDataset(OleDbTransaction transaction, string commandText)
        {
            return ExecuteDataset(transaction, commandText, (OleDbParameter[])null);
        }

        public static DataSet ExecuteDataset(OleDbTransaction transaction, string commandText, params OleDbParameter[] commandParameters)
        {
            if (null == transaction) throw new ArgumentNullException("transaction");
            if (null != transaction && null == transaction.Connection) throw new ArgumentException("transaction对象commited和rollbacked需要一个已打开的事务", "transaction");

            OleDbCommand cmd = new OleDbCommand();
            bool mustCloseConnection = false;

            PrepareCommand(cmd, transaction.Connection, transaction, commandText, commandParameters, out mustCloseConnection);

            using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
            {
                DataSet ds = new DataSet();

                da.Fill(ds);

                cmd.Parameters.Clear();

                return ds;
            }
        }

        #endregion ExecuteDataset
    }
}
