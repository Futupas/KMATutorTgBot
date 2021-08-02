using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMATutorBot
{
    internal static class CallbackHandler
    {
        public static async Task<(bool ok, string message)> HandleCallback(Context context)
        {
            return (true, "hello world");
        }
    }
}
