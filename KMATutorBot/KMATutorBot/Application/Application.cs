using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using KMATutorBot.Models;

namespace KMATutorBot
{
    internal class Application
    {
        //todo remove this huge kostyl
        public static List<Category> Categories =
                new() { new("Math"), new("Programming"), new("Philosophy") };

        public Database DB { get; private set; }
        public Bot TelegramBot { get; private set; }

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
