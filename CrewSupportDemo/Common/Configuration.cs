using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
  public  class Configuration
    {
        public static String ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            }
        }
    }
}
