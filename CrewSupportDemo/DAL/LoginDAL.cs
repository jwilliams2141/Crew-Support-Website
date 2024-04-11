using Common;
using CrewInterface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class LoginDAL 
    {
       static Data objData;
        public LoginDAL()
        {
            objData = new Data();
        }

        public static DataSet CheckLogin(iLogin obj)
        {
            try
            {
                objData = new Data();
                objData.AddParameterToCommand("paramUserName", obj.UserName);
                objData.AddParameterToCommand("paramPassword", obj.Password);
                DataSet dsSearchData = new DataSet();
                dsSearchData = objData.FillDataSet("P_WebsiteLogin", "P_WebsiteLogin");
                return dsSearchData;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                objData = null;
            }
        }
    }
}
