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
using KMATutorBot.Models;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types.ReplyMarkups;

namespace KMATutorBot.Menu
{
    internal class MenuSection
    {
        public const string BACK_TEXT = @"Back";
        public const string BACK_TO_START_TEXT = @"Back To Menu";

        public int Id { get; init; }
        public string Text { get; init; }
        public MenuSection[] Children { get; init; }
        public MenuSection Parent { get; init; }
        public MenuSection Root { get; init; }

        public Func<BotUser, bool> IsForUser { get; init; } = FORUSER_SAMPLE_ALL_USERS;
        public static readonly Func<BotUser, bool> FORUSER_SAMPLE_STUDENTS_ONLY = (BotUser user) => user.StudentCategories != null;
        public static readonly Func<BotUser, bool> FORUSER_SAMPLE_TEACHERS_ONLY = (BotUser user) => user.TeacherCategories != null;
        public static readonly Func<BotUser, bool> FORUSER_SAMPLE_ADMINS_ONLY = (BotUser user) => user.IsAdmin;
        public static readonly Func<BotUser, bool> FORUSER_SAMPLE_ALL_USERS = (BotUser user) => true;

        public bool HasLogic { get; init; } = false;
        public Func<Context, Task<bool>> Handle = DEFAULT_MENU_HANDLER;
        public static async Task<bool> DEFAULT_MENU_HANDLER (Context context) 
        {
            var currentMenu = context.Menu;
            var text = context.MessageEvent.Message.Text;
            var returningText = "default text";
            string[] returningMenus = default;

            var submenu = currentMenu.NextMenuSection(context.User, text);

            context.User = context.DB.UpdateUserMenuSection(context.User, submenu);

            if (submenu == null)
            {
                returningText = $"Unknown button";
                returningMenus = currentMenu.GetSubMenus(context.User);
            }
            else
            {
                returningText = $"U are on menu section {submenu.Text}";
                returningMenus = submenu.GetSubMenus(context.User);
            }

            await context.TelegramCLient.SendTextMessageAsync(
                chatId: context.MessageEvent.Message.Chat,
                text: returningText,
                replyMarkup: new ReplyKeyboardMarkup()
                {
                    Keyboard = returningMenus.Select(menu => new KeyboardButton[] { new(menu) })
                }
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
            var allSections = new List<MenuSection>();
            var root = new MenuSection(true)
            {
                Id = 0,
                Children = new MenuSection[3],
                IsForUser = FORUSER_SAMPLE_ALL_USERS,
                Text = "main menu",
            };
            allSections.Add(root);

            var child1 = new MenuSection()
            {
                Id = 1,
                Children = new MenuSection[0],
                IsForUser = FORUSER_SAMPLE_ALL_USERS,
                Text = "child1",
                Root = root,
                Parent = root,
            };
            allSections.Add(child1);

            var child2 = new MenuSection()
            {
                Id = 2,
                Children = new MenuSection[0],
                IsForUser = FORUSER_SAMPLE_ALL_USERS,
                Text = "child2",
                Root = root,
                Parent = root,
            };
            allSections.Add(child2);

            #region profile
            var registrationMenu = new MenuSection()
            {
                Id = 3,
                Children = new MenuSection[2], // 4
                IsForUser = FORUSER_SAMPLE_ALL_USERS,
                Text = "My profile",
                Root = root,
                Parent = root,
            };
            allSections.Add(registrationMenu);

            var registrationMenuName = new MenuSection()
            {
                Id = 4,
                Children = new MenuSection[0],
                IsForUser = FORUSER_SAMPLE_ALL_USERS,
                Text = "Display name",
                Root = root,
                Parent = registrationMenu,
                Handle = async (ctx) =>
                {
                    var text = ctx.MessageEvent.Message.Text;
                    if (string.IsNullOrEmpty(text))
                    {
                        await ctx.TelegramCLient.SendTextMessageAsync(
                            chatId: ctx.MessageEvent.Message.Chat,
                            text: $"Enter not empty name",
                            replyMarkup: new ReplyKeyboardMarkup()
                            {
                                Keyboard = new KeyboardButton[][] { new KeyboardButton[] { new(BACK_TEXT) }, new KeyboardButton[] { new(BACK_TO_START_TEXT) } }
                            }
                        );
                        return true;
                    }
                    if (text == BACK_TEXT || text == BACK_TO_START_TEXT)
                    {
                        var newMenu = ctx.Menu.NextMenuSection(ctx.User, text);
                        ctx.User = ctx.DB.UpdateUserMenuSection(ctx.User, newMenu);
                        await ctx.TelegramCLient.SendTextMessageAsync(
                            chatId: ctx.MessageEvent.Message.Chat,
                            text: text,
                            replyMarkup: new ReplyKeyboardMarkup()
                            {
                                Keyboard = newMenu.GetSubMenus(ctx.User).Select(menu => new KeyboardButton[] { new(menu) })
                            }
                        );
                        return true;
                    }

                    var parentMenu = ctx.Menu.Parent;
                    ctx.DB.UpdateUserDisplayName(ctx.User, text);
                    ctx.DB.UpdateUserMenuSection(ctx.User, parentMenu);

                    await ctx.TelegramCLient.SendTextMessageAsync(
                        chatId: ctx.MessageEvent.Message.Chat,
                        text: $"Update user {ctx.MessageEvent.Message.Chat.Id} with name {ctx.MessageEvent.Message.Text}",
                        replyMarkup: new ReplyKeyboardMarkup()
                        {
                            Keyboard = parentMenu.GetSubMenus(ctx.User).Select(menu => new KeyboardButton[] { new(menu) })
                        }
                    );
                    return true;
                }
            };
            allSections.Add(registrationMenuName);
            registrationMenu.Children[0] = registrationMenuName;

            var registrationMenuDescription = new MenuSection()
            {
                Id = 5,
                Children = new MenuSection[0],
                IsForUser = FORUSER_SAMPLE_ALL_USERS,
                Text = "About me",
                Root = root,
                Parent = registrationMenu,
                Handle = async (ctx) =>
                {
                    var text = ctx.MessageEvent.Message.Text;
                    if (string.IsNullOrEmpty(text))
                    {
                        await ctx.TelegramCLient.SendTextMessageAsync(
                            chatId: ctx.MessageEvent.Message.Chat,
                            text: $"Enter not empty about me text",
                            replyMarkup: new ReplyKeyboardMarkup()
                            {
                                Keyboard = new KeyboardButton[][] { new KeyboardButton[] { new(BACK_TEXT) }, new KeyboardButton[] { new(BACK_TO_START_TEXT) } }
                            }
                        );
                        return true;
                    }
                    if (text == BACK_TEXT || text == BACK_TO_START_TEXT)
                    {
                        var newMenu = ctx.Menu.NextMenuSection(ctx.User, text);
                        ctx.User = ctx.DB.UpdateUserMenuSection(ctx.User, newMenu);
                        await ctx.TelegramCLient.SendTextMessageAsync(
                            chatId: ctx.MessageEvent.Message.Chat,
                            text: text,
                            replyMarkup: new ReplyKeyboardMarkup()
                            {
                                Keyboard = newMenu.GetSubMenus(ctx.User).Select(menu => new KeyboardButton[] { new(menu) })
                            }
                        );
                        return true;
                    }

                    var parentMenu = ctx.Menu.Parent;
                    ctx.DB.UpdateUserDescription(ctx.User, text);
                    ctx.DB.UpdateUserMenuSection(ctx.User, parentMenu);

                    await ctx.TelegramCLient.SendTextMessageAsync(
                        chatId: ctx.MessageEvent.Message.Chat,
                        text: $"Update user {ctx.MessageEvent.Message.Chat.Id} with about me text {ctx.MessageEvent.Message.Text}",
                        replyMarkup: new ReplyKeyboardMarkup()
                        {
                            Keyboard = parentMenu.GetSubMenus(ctx.User).Select(menu => new KeyboardButton[] { new(menu) })
                        }
                    );
                    return true;
                }
            };
            allSections.Add(registrationMenuDescription);
            registrationMenu.Children[1] = registrationMenuDescription;

            // name
            // description
            // students
            // teachers
            #endregion 

            root.Children[0] = child1;
            root.Children[1] = child2;
            root.Children[2] = registrationMenu;

            return (root, allSections);
        }
    }
}
