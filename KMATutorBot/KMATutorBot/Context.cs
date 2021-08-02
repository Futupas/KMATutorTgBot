using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KMATutorBot.Menu;
using KMATutorBot.Models;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace KMATutorBot
{
    internal class Context
    {
        public BotUser User { get; set; } // Actually it must be { get; init; }, but this is a lil kostyl
        public MenuSection Menu { get; set; }
        public Database DB { get; init; }
        public MessageEventArgs MessageEvent { get; init; }
        public CallbackQueryEventArgs CallbackEvent { get; init; }
        public ITelegramBotClient TelegramCLient { get; init; }

        public List<MenuSection> MenuSections { get; init; }

        public Context (BotUser user, Database dB, List<MenuSection> menuSections)
        {
            this.User = user;
            this.DB = dB;
            this.MenuSections = menuSections;
            this.Menu = GetCurrentMenuSection();
        }

        /// <summary>Requires initializing all fields</summary>
        private MenuSection GetCurrentMenuSection()
        {
            return this.MenuSections.FirstOrDefault(el => el.Id == this.User.MenuSection);
            //todo check also permissions, throw error
        }
    }
}
