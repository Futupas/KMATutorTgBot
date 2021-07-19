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

        //public bool ForStudents { get; init; } = true;
        //public bool ForTeachers { get; init; } = true;
        //public bool ForAdmins { get; init; } = false;

        public Func<BotUser, bool> IsForUser { get; init; } = FORUSER_SAMPLE_ALL_USERS;
        public static readonly Func<BotUser, bool> FORUSER_SAMPLE_STUDENTS_ONLY = (BotUser user) => user.StudentCategories != null;
        public static readonly Func<BotUser, bool> FORUSER_SAMPLE_TEACHERS_ONLY = (BotUser user) => user.TeacherCategories != null;
        public static readonly Func<BotUser, bool> FORUSER_SAMPLE_ADMINS_ONLY = (BotUser user) => user.IsAdmin;
        public static readonly Func<BotUser, bool> FORUSER_SAMPLE_ALL_USERS = (BotUser user) => true;

        public bool HasLogic { get; init; } = false;
        //public Func<Context, Task<bool>> Handle = async (ctx) => { return await DEFAULT_MENU_HANDLER(ctx); };
        public Func<Context, Task<bool>> Handle = DEFAULT_MENU_HANDLER;
        public static async Task<bool> DEFAULT_MENU_HANDLER (Context context) 
        {
            var currentMenu = context.Menu;
            var text = context.MessageEvent.Message.Text;
            var returningText = "default text";
            string[] returningMenus = default;

            var submenu = currentMenu.NextMenuSection(context.User, text);

            context.User = context.DB.UpdateUserMenuSection(context.User, submenu);

            //todo refactor this
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
                Children = new MenuSection[2],
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

            root.Children[0] = child1;
            root.Children[1] = child2;

            return (root, allSections);
        }
    }
}
