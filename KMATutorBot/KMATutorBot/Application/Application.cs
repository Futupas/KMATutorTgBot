using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace KMATutorBot
{
    internal class Application
    {
        //todo remove this huge kostyl
        public static Dictionary<int, string> Categories =
                new() { { 1, "Math" }, { 2, "Programming" }, { 3, "Philosophy" } };

        public Database DB { get; private set; }
        public Bot TelegramBot { get; private set; }
        public (List<(int id, string name)> Students, List<(int id, string name)> Teachers) Categiries = (new(), new());

        public Application()
        {
            //var services = new ServiceCollection();
            //services.AddSingleton<IDatabase>(new Database());

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
