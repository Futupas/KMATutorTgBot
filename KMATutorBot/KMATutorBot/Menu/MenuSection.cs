using KMATutorBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types.ReplyMarkups;
using KMATutorBot.MessageTexts;

namespace KMATutorBot.Menu
{
    internal class MenuSection
    {
        public const string BACK_TEXT = BotMessages.BACK_TEXT;
        public const string BACK_TO_START_TEXT = BotMessages.BACK_TO_ROOT_TEXT;

        public int Id { get; init; }
        public string Text { get; init; }
        public List<MenuSection> Children { get; set; } = new();
        public MenuSection Parent { get; set; }
        public MenuSection Root { get; set; }

        public Func<BotUser, bool> IsForUser { get; init; } = FORUSER_SAMPLE_ALL_USERS;
        public static readonly Func<BotUser, bool> FORUSER_SAMPLE_STUDENTS_ONLY = (BotUser user) => user.StudentCategories != null;
        public static readonly Func<BotUser, bool> FORUSER_SAMPLE_TEACHERS_ONLY = (BotUser user) => user.TeacherCategories != null;
        public static readonly Func<BotUser, bool> FORUSER_SAMPLE_ADMINS_ONLY = (BotUser user) => user.IsAdmin;
        public static readonly Func<BotUser, bool> FORUSER_SAMPLE_ALL_USERS = (BotUser user) => true;

        /// <summary>Needs back and backToMenu</summary>
        public Func<Context, KeyboardButton[][]> CustomKeyboard { get; set; } = null;

        public Func<Context, Task<bool>> OnOpen { get; init; } = null;

        public bool HasLogic { get; init; } = false;
        public Func<Context, Task<bool>> Handle = DEFAULT_MENU_HANDLER;
        public static async Task<bool> DEFAULT_MENU_HANDLER (Context context) 
        {
            var currentMenu = context.Menu;
            var text = context.MessageEvent.Message.Text;

            var submenu = currentMenu.NextMenuSection(context.User, text);

            context.User = context.DB.UpdateUserMenuSection(context.User, submenu);

            if (submenu == null)
            {
                await context.TelegramCLient.SendTextMessageAsync(
                    chatId: context.MessageEvent.Message.Chat,
                    text: BotMessages.UNKNOWN_COMMAND,
                    replyMarkup: new ReplyKeyboardMarkup()
                    {
                        Keyboard = currentMenu.CustomKeyboard == null ?
                            currentMenu.GetSubMenus(context.User).Select(menu => new KeyboardButton[] { new(menu) }).ToArray() :
                            currentMenu.CustomKeyboard(context)
                    }
                );
            }
            else
            {
                if (submenu.OnOpen == null)
                {
                    await context.TelegramCLient.SendTextMessageAsync(
                        chatId: context.MessageEvent.Message.Chat,
                        text: BotMessages.YOU_ARE_ON_MENU_SECTION(submenu.Text),
                        replyMarkup: new ReplyKeyboardMarkup()
                        {
                            Keyboard = submenu.CustomKeyboard == null ?
                                submenu.GetSubMenus(context.User).Select(menu => new KeyboardButton[] { new(menu) }).ToArray() :
                                submenu.CustomKeyboard(context)
                        }
                    );
                }
                else
                {
                    await submenu.OnOpen(context);
                    //todo handle false result
                }
                
            }
            return true;
        }

        public MenuSection (bool isRoot = false)
        {
            if (isRoot)
            {
                this.Parent = this;
                this.Root = this;
            }
        }
        public bool IsRoot
        {
            get
            {
                return this.Root == this;
            }
        }

        public string[] GetSubMenus(BotUser user)
        {
            return Children
                .Where(el => el.IsForUser(user))
                .Select(el => el.Text)
                .Concat(new[] { BACK_TEXT, BACK_TO_START_TEXT }).ToArray();
            //todo don't show back text and backtoMenu text in some cases
        }

        public MenuSection NextMenuSection(BotUser user, string text)
        {
            if (text == BACK_TEXT) return this.IsRoot ? this : this.Parent;
            if (text == BACK_TO_START_TEXT) return this.IsRoot ? this : this.Root;
            var children = Children
                .Where(el => el.IsForUser(user) && el.Text == text);
            return children.Any() ? children.First() : null;
        }

        public static (MenuSection root, List<MenuSection> allSections) GenerateDefaultMenu()
        {
            return Sections.MenuSectionsGenerator.GenerateDefaultMenu();
        }
    }
}
