using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMATutorBot.Exceptions
{
    internal class UnknownMenuSectionException: Exception
    {
        public UnknownMenuSectionException() : base() { }
        public UnknownMenuSectionException(string message) : base(message) { }
    }
}
