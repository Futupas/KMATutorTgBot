using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KMATutorBot.Menu;
using KMATutorBot.Models;

namespace KMATutorBot
{
    internal class Context
    {
        public BotUser User { get; init; }
        public MenuSection Menu { get; init; }
        public Database DB { get; init; }
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

        public (string message, string[] menus) HandleText(string text)
        {
            //todo this method needs to be expanded: we can also return different types of content etc
            var currentMenu = GetCurrentMenuSection();
            if (!currentMenu.HasLogic)
            {
                var submenu = currentMenu.NextMenuSection(User, text);
                if (submenu == null)
                {
                    return ($"Unknown button", currentMenu.GetSubMenus(User));
                }
                DB.UpdateUserMenuSection(User, submenu);
                return ($"U are on menu section {submenu.Text}", submenu.GetSubMenus(User));
            }
            else
            {
                return ($"Menu with logic", currentMenu.GetSubMenus(User));
                //todo what to dowhen menu has logic...
            }
        }
    }
}
