using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Configuration;


namespace Common
{
    public class Data
    {
        #region << Variable Declarations >>
        public static string b = "";

        public static string connectionString = ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
        //public static string connectionString = "server=97.74.232.75;database=docms;uid=pmtsa;password=india@2141;";
        //public static string connectionString = "server=166.62.119.183;database=docmstemp;uid=pmtsa;password=india@2141;";        

        protected MySqlConnection conn = null;
        protected MySqlTransaction txn = null;
        public MySqlTransaction Transaction
        {
            get
            {
                return txn;
            }
            set
            {
                txn = value;
            }
        }

        #region << Define Variable >>
        MySqlConnection cnn = new MySqlConnection();
        MySqlCommand cmd = new MySqlCommand();
        MySqlDataAdapter da = new MySqlDataAdapter();
        DataSet ds = new DataSet();
        //MySqlTransaction objTrans;
        public DataSet m_DataSet;
        public DataTable m_datatable;

        #endregion


        #endregion

        #region << Constructor >>
        public Data()
        {
            try
            {
                cnn = new MySqlConnection(connectionString);
                cmd = new MySqlCommand();
                cmd.Connection = cnn;

            }
            catch (Exception ex)
            {

                throw ex;
            }
            
            //GetDB_ServerName();
        }
        ~Data()
        {
        }
        #endregion

        #region << Public Methods >>
        public void SetConnection(string servername, string username, string catlog, string password)
        {
            // ConfigurationManager.AppSettings["ConnString"] = GetConnection();
        }

        public string GetConnection()
        {
            return connectionString;
        }

        public void CloseConnection()
        {
            try
            {
                if (conn != null)
                {
                    conn.Close(); //Closes connection
                    conn = null;
                }
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (txn == null)
                {
                    if (conn != null)
                    {
                        conn.Close(); //Closes connection
                        conn = null;
                    }
                }
            }
        }

        public void BeginTransaction()
        {
            try
            {
                if (null == conn)
                {
                    conn = new MySqlConnection(connectionString);
                    conn.Open();
                }

                txn = conn.BeginTransaction();

            }
            catch (MySqlException ex)
            {
                throw ex;
            }
        }

        public void CommitTransaction()
        {
            try
            {
                if (txn.Connection.State == ConnectionState.Closed)
                    txn.Connection.Open();
                txn.Commit();
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                if (txn != null)
                {
                    if (txn.Connection.State == ConnectionState.Closed)
                        txn.Connection.Open();
                    txn.Rollback();
                    txn = null;
                }
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (txn == null)
                {
                    if (conn != null)
                    {
                        conn.Close(); //Closes connection
                        conn = null;
                    }
                }
            }
        }

        public DataTable GetSchema(string tableName)
        {
            try
            {
                using (DataTable dt = new DataTable())
                {
                    if (null == conn)
                    {
                        conn = new MySqlConnection(connectionString);
                        conn.Open();
                    }
                    using (MySqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "select * from " + tableName + " where 1 = 1";  // yet to be created.
                        cmd.CommandType = CommandType.Text;
                        if (txn != null)
                            cmd.Transaction = this.txn;
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataSet v_ds = new DataSet();
                            adapter.Fill(v_ds, tableName);
                            return v_ds.Tables[0];
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (txn == null)
                {
                    if (conn != null)
                    {
                        conn.Close();
                        conn = null;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v_ds"></param>
        /// <param name="name"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        public void FillDataSet(DataSet v_ds, string name, string cmdText, MySqlParameter[] cmdParms)
        {
            try
            {
                if (null == v_ds)
                    v_ds = new DataSet();
                if (null == conn)
                {
                    conn = new MySqlConnection(connectionString);
                    conn.Open();
                }
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (null != cmdParms)
                    {
                        foreach (MySqlParameter parm in cmdParms)
                            cmd.Parameters.Add(parm);
                    }
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        //cmd.ExecuteNonQuery();
                        adapter.Fill(v_ds, name);
                    }
                }
            }
            catch (MySqlException ex)
            {
                RollbackTransaction();
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn = null;
                }
            }
        }

        /// <summary>
        ///  return DataTable 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        public DataTable GetDataByReader(string name, string cmdText, SqlParameter[] cmdParms)
        {
            try
            {
                using (DataTable dt = new DataTable())
                {
                    if (null == conn)
                    {
                        conn = new MySqlConnection(connectionString);
                        conn.Open();
                    }

                    using (MySqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = cmdText;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Transaction = this.txn;
                        if (null != cmdParms)
                        {
                            foreach (SqlParameter parm in cmdParms)
                                cmd.Parameters.Add(parm);
                        }
                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            dt.Load(dr);
                            return dt;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                RollbackTransaction();
                throw ex;
            }
            finally
            {
                if (txn == null)
                {
                    if (conn != null)
                    {
                        conn.Close();
                        conn = null;
                    }
                }
            }
        }

        public MySqlDataReader ExecuteDataReader(string name, string cmdText, SqlParameter[] cmdParms)
        {
            try
            {
                using (DataTable dt = new DataTable())
                {
                    if (null == conn)
                    {
                        conn = new MySqlConnection(connectionString);
                        conn.Open();
                    }

                    using (MySqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = cmdText;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Transaction = this.txn;
                        if (null != cmdParms)
                        {
                            foreach (SqlParameter parm in cmdParms)
                                cmd.Parameters.Add(parm);
                        }
                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            return dr;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                RollbackTransaction();
                throw ex;
            }
            finally
            {
                if (txn == null)
                {
                    if (conn != null)
                    {
                        conn.Close();
                        conn = null;
                    }
                }
            }
        }

        /// <summary>
        /// <param name="cmdText"></param>
        /// <param name="v_dt"></param>
        /// </summary>
        public int DataTableUpdate(string cmdText, DataTable v_dt)
        {
            try
            {
                if (null == conn)
                {
                    conn = new MySqlConnection(connectionString);
                    conn.Open();
                }
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Transaction = this.txn;
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        using (MySqlCommandBuilder builder = new MySqlCommandBuilder(adapter))
                        {
                            return adapter.Update(v_dt);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                throw ex;
                //return -1;
            }
            finally
            {
                if (txn == null)
                {
                    if (conn != null)
                    {
                        conn.Close(); //Closes connection
                        conn = null;
                    }
                }
            }
        }

        /// <summary>      
        /// <param name="cmdText"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns> 
        /// </summary>
        public int ExecuteNonQuery(string cmdText, SqlParameter[] cmdParms)
        {
            try
            {
                if (null == conn)
                {
                    conn = new MySqlConnection(connectionString);
                    conn.Open();
                }

                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = this.txn;
                    if (null != cmdParms)
                    {
                        foreach (SqlParameter parm in cmdParms)
                            cmd.Parameters.Add(parm);
                    }
                    int val = cmd.ExecuteNonQuery();
                    return val;
                }
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (txn == null)
                {
                    if (conn != null)
                    {
                        conn.Close(); //Closes connection
                        conn = null;
                    }
                }
            }
        }

        public string ExecuteNonQueryOutParam(string cmdText, SqlParameter[] cmdParms)
        {
            try
            {
                if (null == conn)
                {
                    conn = new MySqlConnection(connectionString);
                    conn.Open();
                }

                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = this.txn;
                    if (null != cmdParms)
                    {
                        foreach (SqlParameter parm in cmdParms)
                            cmd.Parameters.Add(parm);
                    }
                    cmd.Parameters["@EmployeeId"].Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                    string val = cmd.Parameters[1].Value.ToString();
                    return val;
                }
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (txn == null)
                {
                    if (conn != null)
                    {
                        conn.Close(); //Closes connection
                        conn = null;
                    }
                }
            }
        }
        /// <summary>
        /// <param name="cmdText"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        /// </summary>
        /// 
        public void BulkInsert(DataTable dt)
        {

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
            {
                bulkCopy.DestinationTableName = "dbo.testTrack";
                try
                {
                    bulkCopy.WriteToServer(dt);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public object ExecuteScalar(string cmdText, SqlParameter[] cmdParms)
        {
            try
            {
                if (null == conn)
                {
                    conn = new MySqlConnection(connectionString);
                    conn.Open();
                }

                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = this.txn;
                    if (null != cmdParms)
                    {
                        foreach (SqlParameter parm in cmdParms)
                            cmd.Parameters.Add(parm);
                    }
                    object val = cmd.ExecuteScalar();
                    return val;
                }
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (txn == null)
                {
                    if (conn != null)
                    {
                        conn.Close(); //Closes connection
                        conn = null;
                    }
                }
            }
        }

        /// <summary>
        /// This Method is used to Update Single Field for Provided Table
        /// </summary>
        /// <param name="TableName">Name of Table for which record is to be Updated</param>
        /// <param name="PrimaryKeyField">Name of Field of Primary / Unique Key for Specified Table</param>
        /// <param name="PrimaryKeyValue">Value of Primary Key Field</param>
        /// <param name="FieldToUpdate">Name of field for which Value is updated</param>
        /// <param name="NewValueForFieldToUpdate">New Value of FieldToUpdate</param>
        public int ExecuteSingleFieldUpdate(string tableName, string primaryKeyField, string primaryKeyValue, string fieldToUpdate, string newValue)
        {
            try
            {
                if (null == conn)
                {
                    conn = new MySqlConnection(connectionString);
                    conn.Open();
                }

                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "OMSingleFieldUpdate"; //Yet to be Created.
                    cmd.Parameters.AddWithValue("?_TableName", tableName);
                    cmd.Parameters.AddWithValue("?_PrimaryKeyField", primaryKeyField);
                    cmd.Parameters.AddWithValue("?_PrimaryKeyValue", primaryKeyValue);
                    cmd.Parameters.AddWithValue("?_FieldToUpdate", fieldToUpdate);
                    cmd.Parameters.AddWithValue("?_NewValueForFieldToUpdate", newValue);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = this.txn;
                    int val = cmd.ExecuteNonQuery();
                    return val;
                }
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (txn == null)
                {
                    if (conn != null)
                    {
                        conn.Close(); //Closes connection
                        conn = null;
                    }
                }
            }
        }

        public int ExecuteSingleFieldUpdateByQuery(string tableName, string primaryKeyField, string primaryKeyValue, string fieldToUpdate, string newValue)
        {
            try
            {
                if (null == conn)
                {
                    conn = new MySqlConnection(connectionString);
                    conn.Open();
                }
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    string Query = "UPDATE " + tableName + " SET " + fieldToUpdate + " = '" + newValue + "' WHERE " + primaryKeyField + " = '" + primaryKeyValue + "'";
                    cmd.CommandText = Query;
                    cmd.CommandType = CommandType.Text;
                    cmd.Transaction = this.txn;
                    int val = cmd.ExecuteNonQuery();
                    return val;
                }
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (txn == null)
                {
                    if (conn != null)
                    {
                        conn.Close(); //Closes connection
                        conn = null;
                    }
                }
            }
        }

        public DataTable GetSingleRecord(string tableName, string primaryKeyField, string primaryKeyValue)
        {
            try
            {
                using (DataTable dt = new DataTable())
                {
                    if (null == conn)
                    {
                        conn = new MySqlConnection(connectionString);
                        conn.Open();
                    }

                    using (MySqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "select * from " + tableName + " where " + primaryKeyField + " = '" + primaryKeyValue + "'";
                        cmd.CommandType = CommandType.Text;
                        if (txn != null)
                            cmd.Transaction = this.txn;
                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            dt.Load(dr);
                            return dt;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (txn == null)
                {
                    if (conn != null)
                    {
                        conn.Close();
                        conn = null;
                        //if (log.IsInfoEnabled)
                        //    log.Info("[" + System.DateTime.Now.ToString() + "] DataAccess: CloseConnection:: Database Connection Closed. ");
                    }
                }
            }
        }

        public int GetMaxID(string tableName, string primaryKeyField)
        {
            try
            {
                if (null == conn)
                {
                    conn = new MySqlConnection(connectionString);
                    conn.Open();
                }
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select max(" + primaryKeyField + ") from " + tableName;
                    cmd.CommandType = CommandType.Text;
                    if (txn != null)
                        cmd.Transaction = this.txn;
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        int maxID = 0;
                        if (dr.Read())
                        {
                            if (dr[0] == System.DBNull.Value)
                                maxID = 0;
                            else
                                maxID = Convert.ToInt32(dr.GetValue(0));
                        }
                        return maxID;
                    }
                }
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (txn == null)
                {
                    if (conn != null)
                    {
                        conn.Close();
                        conn = null;
                        //if (log.IsInfoEnabled)
                        //    log.Info("[" + System.DateTime.Now.ToString() + "] DataAccess: CloseConnection:: Database Connection Closed. ");
                    }
                }
            }
        }

        /// <summary>
        /// This method is returns the records of any table without filtering.
        /// </summary>
        /// <param name="tableName">Provide the table name to return the records.</param>
        /// <returns></returns>
        public DataTable GetAllRecords(string tableName)
        {
            try
            {
                using (DataTable dt = new DataTable())
                {
                    if (null == conn)
                    {
                        conn = new MySqlConnection(connectionString);
                        conn.Open();
                    }
                    using (MySqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "select * from " + tableName;
                        cmd.CommandType = CommandType.Text;
                        if (txn != null)
                            cmd.Transaction = this.txn;
                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            dt.Load(dr);
                            return dt;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (txn == null)
                {
                    if (conn != null)
                    {
                        conn.Close();
                        conn = null;
                    }
                }
            }
        }

        public DataTable GetDataByQuery(string Query)
        {
            try
            {
                using (DataTable dt = new DataTable())
                {
                    if (null == conn)
                    {
                        conn = new MySqlConnection(connectionString);
                        conn.Open();
                    }
                    using (MySqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.CommandType = CommandType.Text;
                        cmd.Transaction = this.txn;

                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            dt.Load(dr);
                            return dt;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (txn == null)
                {
                    if (conn != null)
                    {
                        conn.Close();
                        conn = null;
                    }
                }
            }
        }

        public string GetConncetionStr()
        {
            return connectionString;//User For Get Current Database Connection String.
        }
        #endregion

        #region << create table in database from XML file >>
        //create table
        public void SaveTable()
        {
            DataSet ds = new DataSet();
            // Loop through our Data Set's tables
            for (int tIdx = 0; tIdx < ds.Tables.Count; tIdx++)
            {
                // Create a Data Table in memory
                using (DataTable dt = ds.Tables[tIdx])
                {
                    //add two static column
                    dt.Columns.Add("VIN", typeof(string));
                    dt.Columns.Add("ID", typeof(Int64));
                    // Display Table Name
                    string tn = "KIA_XML_" + dt.TableName;
                    //check and create table
                    string s = CreateTABLE(connectionString, tn, dt);
                    MySqlConnection cnn = new MySqlConnection(connectionString);
                    cnn.Open();
                    MySqlCommand cmd = new MySqlCommand(s, cnn);
                    cmd.ExecuteNonQuery();
                    cnn.Close();
                }
            }
        }

        #region insert value in DATABASE Table from XML file data
        public string insertValues(DataSet ds, string vin)
        {
            for (int tIdx = 0; tIdx < ds.Tables.Count; tIdx++)
            {
                // Create a Data Table in memory
                using (DataTable dt = ds.Tables[tIdx])
                {
                    // Display Table Name
                    string tn = "KIA_XML_" + dt.TableName;
                    //go to insert method to insert value in table
                    b = insert(connectionString, tn, dt, vin);
                }
            }
            return b;
        }

        //insert  value in particular table
        public static string insert(string connectionString, string tableName, DataTable table, string VIN)
        {
            string sqlsc;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                // sqlsc = "drop TABLE " + tableName ;
                sqlsc = "insert into " + tableName + "(";
                for (int i = 0; i <= table.Columns.Count; i++)
                {
                    if (i == (table.Columns.Count))
                    {
                        sqlsc += "\n [" + "VIN" + "],";
                    }
                    else
                    {
                        if (table.Columns[i].ColumnName != "ID")
                        {

                            sqlsc += "\n [" + table.Columns[i].ColumnName + "],";
                        }
                    }
                }
                if (sqlsc.EndsWith(","))
                {
                    string s = sqlsc;
                    sqlsc = "";
                    sqlsc = s.Remove(s.Length - 1, 1);
                }

                sqlsc += ")Values(";
                string sqlscR = sqlsc;
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    string finalQ = "";

                    sqlsc = "";
                    sqlsc = sqlscR;
                    for (int j = 0; j <= table.Columns.Count; j++)
                    {
                        if (j == (table.Columns.Count))
                        {
                            sqlsc += "\n '" + VIN + "',";
                        }
                        else
                        {
                            if (table.Columns[j].ColumnName != "ID")
                            {
                                if (table.Columns[j].ColumnName == "VIN")
                                {
                                    sqlsc += "\n '" + VIN + "',";
                                }
                                else
                                {
                                    b += "\n " + Convert.ToString(table.Rows[i].Table.Columns[j].ColumnName) + ":  " + Convert.ToString(table.Rows[i][j].ToString());
                                    string v = Convert.ToString(table.Rows[i][j]);
                                    string f = v.Replace("'", "");
                                    sqlsc += "\n '" + f + "',";
                                }
                            }
                        }

                    }

                    if (sqlsc.EndsWith(","))
                    {
                        string t = sqlsc;
                        sqlsc = "";
                        sqlsc = t.Remove(t.Length - 1, 1);
                        finalQ = sqlsc + ")";

                    }
                    MySqlCommand cmd = new MySqlCommand(finalQ, connection);
                    cmd.ExecuteNonQuery();

                }

                connection.Close();
            }

            return b;
        }

        #endregion

        public static string CreateTABLE(string connectionString, string tableName, DataTable table)
        {
            string sqlsc;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                // sqlsc = "drop TABLE " + tableName ;
                sqlsc = "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'AND TABLE_NAME='" + tableName + "') begin CREATE TABLE " + tableName + "(";
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    sqlsc += "\n [" + table.Columns[i].ColumnName + "] ";

                    if (Convert.ToString(table.Columns[i]) != "ID")
                    {
                        if (table.Columns[i].DataType.ToString().Contains("System.DateTime"))
                            sqlsc += " datetime ";
                        else if (table.Columns[i].DataType.ToString().Contains("System.String"))
                            sqlsc += " nvarchar(MAX) ";
                        else
                            sqlsc += " nvarchar(MAX) ";
                    }
                    else
                    {
                        if (Convert.ToString(table.Columns[i]) == "ID")
                            sqlsc += "int IDENTITY(1,1) NOT NULL";

                    }
                    sqlsc += ",";
                }
                connection.Close();
            }
            return sqlsc.Substring(0, sqlsc.Length - 1) + ")end";
            //return sqlsc;
        }

        #endregion

        #region << Global Function >>
        // Database Connection Open
        public void Databaseconn()
        {
            try
            {
                if (cnn.State != ConnectionState.Open)
                {
                    cnn.ConnectionString = connectionString;
                    cnn.Open();
                }
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        //MessageBox.Show("Can not connect to server", "IDSVault Connection", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                        break;
                    case 1045:
                        // MessageBox.Show("Can not connect to server", "IDSVault Connection", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                        break;
                    case 1042:
                        //MessageBox.Show("Can not connect to server", "IDSVault Connection", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                        break;
                    default:
                        // MessageBox.Show(ex.Message, "IDSVault Connection", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                        break;
                }

                Environment.Exit(0);
                //MessageBox.Show("Connection has been lost", "Connection problem", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //Environment.Exit(0);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString(), "IDSVault Connection", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                Environment.Exit(0);
                //MessageBox.Show("Connection has been lost", "Connection problem", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //Environment.Exit(0);
            }
        }

        public bool IsSqlServerConnected()
        {
            MySqlConnection DBConnection = new MySqlConnection();
            DBConnection.ConnectionString = Convert.ToString(GetConnection());
            using (var l_oConnection = new MySqlConnection(DBConnection.ConnectionString))
            {
                try
                {
                    l_oConnection.Open();
                    return true;
                }
                catch (MySqlException)
                {
                    return false;
                }
                finally
                {
                    l_oConnection.Close();
                }
            }
        }

        // Database Connection Close
        public void DatabaseClose()
        {
            if (cnn.State == ConnectionState.Open)
            {
                cnn.Close();
            }
        }
        // Create Command Object
        public void CreateCommand(string SpName)
        {
            cmd = new MySqlCommand(SpName, cnn);
            cmd.CommandType = CommandType.StoredProcedure;

        }
        // Add Paramater to Command
        public void AddParameterToCommand(string StrParameterName, object objvalue)
        {
            cmd.Parameters.AddWithValue(StrParameterName, objvalue);
        }
        // Add OutPut Parameter...
        public void AddOutPutParameter()
        {
            MySqlParameter op = new MySqlParameter("ReturnValue", MySqlDbType.Int16);
            op.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(op);
        }
        public void AddOutPutParameterBigInt()
        {
            MySqlParameter op = new MySqlParameter("ReturnValue", MySqlDbType.Float);
            op.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(op);
        }
        public void AddOutPutParameterForString()
        {
            MySqlParameter op = new MySqlParameter("ReturnValue", MySqlDbType.VarChar);
            op.Direction = ParameterDirection.Output;
            op.Size = 50;
            cmd.Parameters.Add(op);
        }
        // Add OutPut Employee Parameter...
        public void AddOutPutEmployeeIDParameter(string Employeeid)
        {
            MySqlParameter op = new MySqlParameter(Employeeid, MySqlDbType.String);
            op.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(op);
        }
        //After Execute Query Clear Paramater
        public void ClearParameter()
        {
            cmd.Parameters.Clear();
        }
        //For Insertion,Update,Delete with return Parameter value : @ReturnValue
        public int DLLExecuteNonQuery()
        {
            try
            {
                int m_Return = 0;
                Databaseconn();
               
                cmd.ExecuteNonQuery();

                // Get the Output Parameter Value....
                m_Return = int.Parse(cmd.Parameters["@ReturnValue"].Value.ToString());

                DatabaseClose();
                ClearParameter();
                cmd.Dispose();
               
                return m_Return;

            }
            catch (Exception ex)
            {
                
                throw ex;

            }
        }

        public string DLLExecuteNonQueryForString()
        {
            try
            {
                string m_Return = string.Empty;
                Databaseconn();
                cmd.ExecuteNonQuery();

                // Get the Output Parameter Value....
                m_Return = Convert.ToString(cmd.Parameters["@ReturnValue"].Value.ToString());

                DatabaseClose();
                ClearParameter();
                cmd.Dispose();

                return m_Return;

            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public Int64 DLLExecuteNonQueryForBigint()
        {
            try
            {
                Int64 m_Return = 0;
                Databaseconn();
                cmd.ExecuteNonQuery();

                // Get the Output Parameter Value....
                m_Return = Int64.Parse(cmd.Parameters["@ReturnValue"].Value.ToString());

                DatabaseClose();
                ClearParameter();
                cmd.Dispose();

                return m_Return;

            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        //For Insertion,Update,Delete with return Default Parameter
        public int ExecuteNonQuery()
        {
            try
            {
                int m_Return = 0;
                Databaseconn();
                cmd.CommandTimeout = 0;
                // Get the Output Parameter Value....
                

                m_Return = cmd.ExecuteNonQuery();

                DatabaseClose();
                ClearParameter();
                cmd.Dispose();
                
                return m_Return;

            }
            catch (Exception ex)
            {
                
                throw ex;

            }
            finally
            {
                ClearParameter();
                DatabaseClose();
            }
        }

        //For Return Value by MySqlDataReader
        public MySqlDataReader ExecuteDataReader()
        {
            try
            {
                Databaseconn();
                MySqlDataReader reader = cmd.ExecuteReader();
                return reader;
            }
            catch (MySqlException ex)
            {
                throw ex;
            }

        }
        //this method use for return value in (max,avg,etc)
        public object ExecuteScalar(string strSql)
        {

            cmd.CommandText = strSql;
            Databaseconn();

            object obj = new object();
            obj = cmd.ExecuteScalar();
            DatabaseClose();
            return obj;

        }
        // this method use for select query in DataTable
        public DataTable ExecuteQuery(string strSql)
        {
            try
            {
                m_datatable = null;
                cmd.CommandText = strSql;
                cmd.CommandType = CommandType.StoredProcedure;
                da = new MySqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds, "List");
                if (ds.Tables.Count > 0)
                {
                    m_datatable = ds.Tables[0];
                }
                return m_datatable;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public DataTable ExecuteDataTable()
        {

            try
            {
                //Databaseconn();
                //DataTable dtTable = new DataTable();
                //cmd.CommandTimeout = 120;
                //cmd.CommandType = CommandType.StoredProcedure;
                //MySqlDataReader reader = cmd.ExecuteReader();

                //dtTable.Load(reader);

                //reader.Close();

                //return dtTable;

                DataTable dtTable = new DataTable();
                cmd.Connection = cnn;
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                da = new MySqlDataAdapter();
                da.SelectCommand = cmd;
                
                da.Fill(dtTable);
               
                return dtTable;
            }
            catch (Exception exp)
            {
               
                throw exp;
                //return null;
            }
            finally
            {
                ClearParameter();
                DatabaseClose();
            }
        }

        // Select Data with Dataset with commandtype as Text
        public DataSet FillDataSet(string Psql, string Ptablename)
        {
            try
            {
                cmd.Connection = cnn;
                
                cmd.CommandTimeout = 0;
                cmd.CommandText = Psql;
                cmd.CommandType = CommandType.StoredProcedure;
               
                da = new MySqlDataAdapter();
                m_DataSet = new DataSet();
                //writeLogs.WriteToLog("getMessageList", "Pilot.asmx", "Connection: " + Convert.ToString(cmd.Connection.State));
                da.SelectCommand = cmd;
                da.Fill(m_DataSet, Ptablename);
               // writeLogs.WriteToLog("getMessageList", "Pilot.asmx", "Connection: " + Convert.ToString(cmd.Connection.State));

               
                return m_DataSet;

            }
            catch (Exception exp)
            {
               
                throw exp;                
            }
            finally
            {
                ClearParameter();
                DatabaseClose();
            }

        }

        public string ExecuteNonQueryOutParam(string cmdText)
        {
            try
            {
                Databaseconn();
                cmd.ExecuteNonQuery();

                // Get the Output Parameter Value....
                string val = cmd.Parameters["@EmployeeId"].Value.ToString();

                DatabaseClose();
                ClearParameter();
                cmd.Dispose();

                return val;
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}