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
   public class LoginBLL : iLogin
    {
        int _PkId;
        string _UserName,_Password, _FirstName, _LastName;

        public int PkId { get { return _PkId; } set { _PkId = value; } }
        public string UserName { get { return _UserName; } set { _UserName = value; } }
        public string Password { get { return _Password; } set { _Password = value; } }
        public string FirstName { get { return _FirstName; } set { _FirstName = value; } }
        public string LastName { get { return _LastName; } set { _LastName = value; } }

        public List<Login> lstLogin { get; set; }

        public virtual DataSet CheckLogin()
        {
            return LoginDAL.CheckLogin(this);
        }
    }
    public class Login
    {
        public int PkId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
