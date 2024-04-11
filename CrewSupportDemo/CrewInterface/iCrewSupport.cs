using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrewInterface
{
    public interface iCrewSupport
    {
        string OwnerCount { get; set; }
        string InstructorCount { get; set; }
        string PilotCount { get; set; }
        string FACount { get; set; }
        string SICCount { get; set; }
       
    }
}
