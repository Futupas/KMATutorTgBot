using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KMATutorBot.MessageTexts;
using Telegram.Bot.Types.ReplyMarkups;

namespace KMATutorBot.Menu.Sections
{
    internal static partial class MenuSectionsGenerator
    {
        private static void GenerateFinder()
        {
            //var registrationMenu = new MenuSection()
            //{
            //    Id = NextMenuSectionId(),
            //    IsForUser = MenuSection.FORUSER_SAMPLE_ALL_USERS,
            //    Text = BotMessages.MY_PROFILE_MENU_TEXT,
            //};
            //AddMenuSection(_Root, registrationMenu);

            //GenerateNameProfileEditor(registrationMenu);
            //GenerateDescriptionProfileEditor(registrationMenu);
            //GenerateStudentCategoriesProfileEditor(registrationMenu);
            //GenerateTeacherCategoriesProfileEditor(registrationMenu);

            GenerateStudentFinder();
        }

        private static void GenerateStudentFinder()
        {
            var finderMenu = new MenuSection()
            {
                Id = NextMenuSectionId(),
                IsForUser = MenuSection.FORUSER_SAMPLE_TEACHERS_ONLY,
                Text = BotMessages.FINDER_FIND_STUDENTS_MENU_TEXT,
                OnOpen = async (ctx) =>
                {
                    var students = ctx.DB.GetAllFreeStudents(ctx.User.TeacherCategories)
                        .Where(st => st.Id != ctx.User.Id);
                    // students == null || !students.Any() - check on BotMessages
                    await ctx.TelegramCLient.SendTextMessageAsync(
                        chatId: ctx.MessageEvent.Message.Chat,
                        text: BotMessages.FINDER_WE_FOUND_STUDENTS_TEXT(ctx.User, students),
                        replyMarkup: new ReplyKeyboardMarkup()
                        {
                            Keyboard = GenerateKeyboard(BotMessages.FINDER_SEARCH_AGAIN_TEXT, MenuSection.BACK_TEXT, MenuSection.BACK_TO_START_TEXT)
                        }
                    );
                    return true;
                },
                Handle = async (ctx) =>
                {
                    var text = ctx.MessageEvent.Message.Text;
                    if (string.IsNullOrEmpty(text))
                    {
                        await ctx.TelegramCLient.SendTextMessageAsync(
                            chatId: ctx.MessageEvent.Message.Chat,
                            text: BotMessages.UNKNOWN_COMMAND,
                            replyMarkup: new ReplyKeyboardMarkup()
                            {
                                Keyboard = GenerateKeyboard(BotMessages.FINDER_SEARCH_AGAIN_TEXT, MenuSection.BACK_TEXT, MenuSection.BACK_TO_START_TEXT)
                            }
                        );
                        return true;
                    }
                    if (text == MenuSection.BACK_TEXT || text == MenuSection.BACK_TO_START_TEXT)
                    {
                        var newMenu = ctx.Menu.NextMenuSection(ctx.User, text);
                        ctx.User = ctx.DB.UpdateUserMenuSection(ctx.User, newMenu);
                        await ctx.TelegramCLient.SendTextMessageAsync(
                            chatId: ctx.MessageEvent.Message.Chat,
                            text: newMenu.Text,
                            replyMarkup: new ReplyKeyboardMarkup()
                            {
                                Keyboard = newMenu.GetSubMenus(ctx.User).Select(menu => new KeyboardButton[] { new(menu) })
                            }
                        );
                        return true;
                    }
                    if (text == BotMessages.FINDER_SEARCH_AGAIN_TEXT)
                    {
                        if (ctx.Menu.OnOpen == null) return false; // Impossible condition
                        return await ctx.Menu.OnOpen(ctx);
                    }

                    await ctx.TelegramCLient.SendTextMessageAsync(
                        chatId: ctx.MessageEvent.Message.Chat,
                        text: BotMessages.UNKNOWN_COMMAND,
                        replyMarkup: new ReplyKeyboardMarkup()
                        {
                            Keyboard = GenerateKeyboard(BotMessages.FINDER_SEARCH_AGAIN_TEXT, MenuSection.BACK_TEXT, MenuSection.BACK_TO_START_TEXT)
                        }
                    );
                    return true;
                }
            };

            AddMenuSection(_Root, finderMenu);
        }
    }
}
