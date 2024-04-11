using CrewInterface;
using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
   public class CrewSupportBLL : iCrewSupport
    {
        string _OwnerCount, _InstructorCount, _PilotCount, _FACount, _SICCount;
        string   _MembershipType, _PilotGender, _PilotFname, _PilotLName, _EmailId, _CuLocCountry, _cellNumber;


        public string OwnerCount { get { return _OwnerCount; } set { _OwnerCount = value; } }
        public string InstructorCount { get { return _InstructorCount; } set { _InstructorCount = value; } }
        public string PilotCount { get { return _PilotCount; } set { _PilotCount = value; } }
        public string FACount { get { return _FACount; } set { _FACount = value; } }
        public string SICCount { get { return _SICCount; } set { _SICCount = value; } }
        public List<GridDetail> lstgrid { get; set; }


        public virtual DataSet BindGrid(string status)
        {
            return CrewSupportDAL.BindGrid(status);
        }
        public virtual DataSet GetPilotFullDetail(string pilotid)
        {
            return CrewSupportDAL.GetPilotFullDetail(pilotid);
        }
        public virtual int P_WebsiteActiveDeActivePilot(bool status, int Pilotid)
        {
            return CrewSupportDAL.P_WebsiteActiveDeActivePilot(status, Pilotid);
        }        
    }

    public class GridDetail
    {
       public int pkPilotId { get; set; }
       public string MembershipType { get; set; }
       public string PilotGender { get; set; }
       public string PilotFname { get; set; }
       public string PilotLName { get; set; }
       public string EmailId { get; set; }
       public string CuLocCountry { get; set; }
        public string cellNumber { get; set; }
        public bool IsVoid { get; set; }
    }
}
