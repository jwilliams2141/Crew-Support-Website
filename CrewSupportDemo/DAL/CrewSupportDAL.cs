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
    public class CrewSupportDAL
    {
        static Data objData;
        public CrewSupportDAL()
        {
            objData = new Data();
        }
       
        public static DataSet BindGrid(string Status)
        {
            try
            {
                objData = new Data();
                DataSet dsSearchData = new DataSet();
                objData.AddParameterToCommand("paramStatus", Status);
                dsSearchData = objData.FillDataSet("P_WebsiteAllPilotDataByStatus", "P_WebsiteAllPilotDataByStatus");
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

        public static DataSet GetPilotFullDetail(string piloltid)
        {
            try
            {
                objData = new Data();
                DataSet dsSearchData = new DataSet();
                objData.AddParameterToCommand("parampiloltid", piloltid);
                dsSearchData = objData.FillDataSet("P_WebsiteGetPilotFullDetail", "P_WebsiteGetPilotFullDetail");
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

        public static int P_WebsiteActiveDeActivePilot(bool status,int Pilotid)
        {
            try
            {
                objData = new Data();
                objData.CreateCommand("P_WebsiteActiveDeActivePilot");
                objData.AddParameterToCommand("paramfkPilotid", Pilotid);
                objData.AddParameterToCommand("paramIsVoid", status);
                objData.AddOutPutParameter();
                int result = objData.DLLExecuteNonQuery();
                return result;
            }
            catch (Exception ex)
            {
                return -1;
            }
            finally
            {
                objData = null;
            }
        }

        

    }
}
