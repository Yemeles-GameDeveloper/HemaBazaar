using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    internal enum LogType
    {
        Insert,
        Update,
        Delete,
        Error,
        Warning,
        NotFound,
        NonValidation
    }
}
