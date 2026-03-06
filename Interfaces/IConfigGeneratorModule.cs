using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsystentSieciowca.Interfaces
{
    public interface IConfigGeneratorModule
    {
        void Run();
        string GetModuleName();
    }
}
