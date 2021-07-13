using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMATutorBot
{
    internal class Application
    {
        public Database DB { get; private set; }
        public Bot TelegramBot { get; private set; }

        public Application()
        {
            try
            {
                DB = new();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initiaizing Database: {ex.Message}");
            }

            try
            {
                TelegramBot = new(DB);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initiaizing telegram bot: {ex.Message}");
            }
        }
    }
}
