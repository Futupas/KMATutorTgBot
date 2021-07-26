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
            GenerateTeacherFinder();
        }

        private static void GenerateTeacherFinder()
        {
            var finderMenu = new MenuSection()
            {
                Id = NextMenuSectionId(),
                IsForUser = MenuSection.FORUSER_SAMPLE_ALL_USERS,
                Text = BotMessages.FINDER_FIND_TEACHERS_MENU_TEXT,
                CustomKeyboard = (ctx) => GenerateKeyboardWithBacks(ctx, Application.Categories.Select(cat => cat.Name)),
                OnOpen = async (ctx) =>
                {
                    var categories = Application.Categories.Select(cat => cat.Name);

                    await ctx.TelegramCLient.SendTextMessageAsync(
                        chatId: ctx.MessageEvent.Message.Chat,
                        text: BotMessages.FINDER_FIND_TEACHERS_SELECT_CATEGORY_TEXT,
                        replyMarkup: new ReplyKeyboardMarkup()
                        {
                            Keyboard = GenerateKeyboard(categories, new[] { MenuSection.BACK_TO_START_TEXT })
                        }
                    );
                    return true;
                },
                Handle = async (ctx) =>
                {
                    var text = ctx.MessageEvent.Message.Text;
                    //if (string.IsNullOrEmpty(text))
                    //{
                    //    await ctx.TelegramCLient.SendTextMessageAsync(
                    //        chatId: ctx.MessageEvent.Message.Chat,
                    //        text: BotMessages.UNKNOWN_COMMAND,
                    //        replyMarkup: new ReplyKeyboardMarkup()
                    //        {
                    //            Keyboard = GenerateKeyboard(Application.Categories.Select(cat => cat.Name), new[] { MenuSection.BACK_TO_START_TEXT })
                    //        }
                    //    );
                    //    return true;
                    //}
                    //if (text == MenuSection.BACK_TO_START_TEXT)
                    //{
                    //    var newMenu = ctx.Menu.NextMenuSection(ctx.User, text);
                    //    ctx.User = ctx.DB.UpdateUserMenuSection(ctx.User, newMenu);
                    //    await ctx.TelegramCLient.SendTextMessageAsync(
                    //        chatId: ctx.MessageEvent.Message.Chat,
                    //        text: newMenu.Text,
                    //        replyMarkup: new ReplyKeyboardMarkup()
                    //        {
                    //            Keyboard = newMenu.GetSubMenus(ctx.User).Select(menu => new KeyboardButton[] { new(menu) })
                    //        }
                    //    );
                    //    return true;
                    //}
                    
                    if (await MenuSection.HandleEmptyOrBackString(text, ctx, null))
                    {
                        return true;
                    }

                    var category = Application.Categories.FirstOrDefault(cat => cat.Name == text);

                    if (category != null)
                    {
                        var teachers = ctx.DB.GetTeachersByCategory(category.Id);
                        var replyText = BotMessages.FINDER_WE_FOUND_TEACHERS_TEXT(teachers);
                        await ctx.TelegramCLient.SendTextMessageAsync(
                            chatId: ctx.MessageEvent.Message.Chat,
                            text: replyText,
                            replyMarkup: new ReplyKeyboardMarkup()
                            {
                                Keyboard = GenerateKeyboard(Application.Categories.Select(cat => cat.Name), new[] { MenuSection.BACK_TO_START_TEXT })
                            }
                        );
                    }
                    else
                    {
                        await ctx.TelegramCLient.SendTextMessageAsync(
                            chatId: ctx.MessageEvent.Message.Chat,
                            text: BotMessages.MY_PROFILE_TEACHER_INCORRECT_CATEGORY_TEXT,
                            replyMarkup: new ReplyKeyboardMarkup()
                            {
                                Keyboard = GenerateKeyboard(Application.Categories.Select(cat => cat.Name), new[] { MenuSection.BACK_TO_START_TEXT })
                            }
                        );
                    }

                    return true;
                }
            };

            AddMenuSection(_Root, finderMenu);
        }
    }
}
