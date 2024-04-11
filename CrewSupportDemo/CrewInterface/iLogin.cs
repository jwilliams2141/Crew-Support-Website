using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrewInterface
{
   public interface iLogin
    {
        int PkId { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
    }
}
