using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TT2Master.Interfaces
{
    public interface IAskForAttPermission
    {
        Task<bool> AskUserAsync();
    }
}
