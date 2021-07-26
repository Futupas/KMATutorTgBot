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
using KMATutorBot.Menu.Sections;

namespace KMATutorBot.Menu
{
    internal class MenuSection
    {
        public const string BACK_TEXT = BotMessages.BACK_TEXT;
        public const string BACK_TO_START_TEXT = BotMessages.BACK_TO_ROOT_TEXT;

        public int Id { get; init; }
        /// <summary>Text in keyboard</summary>
        public string Text { get; init; }
        public List<MenuSection> Children { get; set; } = new();
        public MenuSection Parent { get; set; }
        public MenuSection Root { get; set; }

        public Func<BotUser, bool> IsForUser { get; init; } = FORUSER_SAMPLE_ALL_USERS;
        //public static readonly Func<BotUser, bool> FORUSER_SAMPLE_STUDENTS_ONLY = (BotUser user) => user.StudentCategories != null;
        public static readonly Func<BotUser, bool> FORUSER_SAMPLE_TEACHERS_ONLY = (BotUser user) => user.TeacherCategories != null;
        public static readonly Func<BotUser, bool> FORUSER_SAMPLE_ADMINS_ONLY = (BotUser user) => user.IsAdmin;
        public static readonly Func<BotUser, bool> FORUSER_SAMPLE_ALL_USERS = (BotUser user) => true;

        /// <summary>Needs back and backToMenu</summary>
        public Func<Context, KeyboardButton[][]> CustomKeyboard { get; set; } = null;
        /// <summary>Text in message</summary>
        public Func<Context, string> CustomText { get; set; } = null;

        public Func<Context, Task<bool>> OnOpen { get; init; } = null;

        public bool HasLogic { get; init; } = false;
        public Func<Context, Task<bool>> Handle = DEFAULT_MENU_HANDLER;
        public static async Task<bool> DEFAULT_MENU_HANDLER (Context context) 
        {
            var currentMenu = context.Menu;
            var text = context.MessageEvent.Message.Text;

            var submenu = currentMenu.NextMenuSection(context.User, text);

            context.User = context.DB.UpdateUserMenuSection(context.User, submenu);
            context.Menu = submenu;

            if (submenu == null)
            {
                await context.TelegramCLient.SendTextMessageAsync(
                    chatId: context.MessageEvent.Message.Chat,
                    text: BotMessages.UNKNOWN_COMMAND,
                    replyMarkup: new ReplyKeyboardMarkup()
                    {
                        Keyboard = currentMenu.CustomKeyboard == null ?
                            MenuSectionsGenerator.GenerateKeyboardWithBacks(currentMenu, currentMenu.GetSubMenus(context.User)) :
                            currentMenu.CustomKeyboard(context)
                    },
                    parseMode: ParseMode.Html
                );
            }
            else
            {
                if (submenu.OnOpen == null)
                {
                    await context.TelegramCLient.SendTextMessageAsync(
                        chatId: context.MessageEvent.Message.Chat,
                        text: submenu.CustomText == null ? BotMessages.YOU_ARE_ON_MENU_SECTION(submenu.Text) : submenu.CustomText(context),
                        replyMarkup: new ReplyKeyboardMarkup()
                        {
                            Keyboard = submenu.CustomKeyboard == null ?
                                MenuSectionsGenerator.GenerateKeyboardWithBacks(submenu, submenu.GetSubMenus(context.User)) :
                                submenu.CustomKeyboard(context)
                        },
                        parseMode: ParseMode.Html
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

        public static async Task<bool> HandleEmptyOrBackString(string? text, Context context, KeyboardButton[][]? keyboardButtons)
        {
            if (string.IsNullOrEmpty(text))
            {
                await context.TelegramCLient.SendTextMessageAsync(
                    chatId: context.MessageEvent.Message.Chat,
                    text: BotMessages.UNKNOWN_COMMAND,
                    replyMarkup: new ReplyKeyboardMarkup()
                    {
                        Keyboard = keyboardButtons ??
                            (context.Menu.CustomKeyboard == null ? MenuSectionsGenerator.GenerateKeyboardWithBacks(context.Menu, null) : context.Menu.CustomKeyboard(context))
                            
                    },
                    parseMode: ParseMode.Html
                );
                return true;
            }
            if (text == MenuSection.BACK_TEXT || text == MenuSection.BACK_TO_START_TEXT)
            {
                var newMenu = context.Menu.NextMenuSection(context.User, text);
                context.User = context.DB.UpdateUserMenuSection(context.User, newMenu);
                await context.TelegramCLient.SendTextMessageAsync(
                    chatId: context.MessageEvent.Message.Chat,
                    text: newMenu.Text,
                    replyMarkup: new ReplyKeyboardMarkup()
                    {
                        Keyboard = MenuSectionsGenerator.GenerateKeyboardWithBacks(newMenu, newMenu.GetSubMenus(context.User))
                    },
                    parseMode: ParseMode.Html
                );
                return true;
            }
            return false;
        }

        public static async Task<bool> SendUnknownCommand(Context context, KeyboardButton[][]? keyboardButtons)
        {
            await context.TelegramCLient.SendTextMessageAsync(
                chatId: context.MessageEvent.Message.Chat,
                text: BotMessages.UNKNOWN_COMMAND,
                replyMarkup: new ReplyKeyboardMarkup()
                {
                    Keyboard = keyboardButtons ??
                        (context.Menu.CustomKeyboard == null ? MenuSectionsGenerator.GenerateKeyboardWithBacks(context.Menu, null) : context.Menu.CustomKeyboard(context))
                        
                },
                parseMode: ParseMode.Html
            );
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
                .ToArray();
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
            return MenuSectionsGenerator.GenerateDefaultMenu();
        }
    }
}
