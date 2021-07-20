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

namespace KMATutorBot.Menu.Sections
{
    internal static partial class MenuSectionsGenerator
    {
        private const string IM_NOT_A_STUDENT_TEXT = "I'm not a student";
        private const string ADD_CATEGORY_TEXT = "+ ";
        private const string REMOVE_CATEGORY_TEXT = "- ";

        private static KeyboardButton[][] GenerateCategoriesReplyMarkup(BotUser user)
        {
            return Application.Categories
                .Select(cat =>
                {
                    var sign = (user.StudentCategories != null && user.StudentCategories.Contains(cat.Key)) ?
                        REMOVE_CATEGORY_TEXT : ADD_CATEGORY_TEXT;
                    return sign + cat.Value;
                })
                .Concat(new[] { IM_NOT_A_STUDENT_TEXT })
                .Concat(new string[] { MenuSection.BACK_TEXT, MenuSection.BACK_TO_START_TEXT })
                .Select(str => new KeyboardButton[] { new(str) })
                .ToArray();
        }

        private static void GenerateStudentCategoriesProfileEditor(MenuSection regMenu)
        {
            var section = new MenuSection()
            {
                Id = NextMenuSectionId(),
                IsForUser = MenuSection.FORUSER_SAMPLE_ALL_USERS,
                Text = "Modify student categories",
                CustomKeyboard = (ctx) => GenerateCategoriesReplyMarkup(ctx.User),
                Handle = async (ctx) =>
                {
                    var text = ctx.MessageEvent.Message.Text;
                    if (string.IsNullOrEmpty(text))
                    {
                        await ctx.TelegramCLient.SendTextMessageAsync(
                            chatId: ctx.MessageEvent.Message.Chat,
                            text: $"You should use one of proposed categories",
                            replyMarkup: new ReplyKeyboardMarkup()
                            {
                                Keyboard = GenerateCategoriesReplyMarkup(ctx.User)
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
                            text: text,
                            replyMarkup: new ReplyKeyboardMarkup()
                            {
                                Keyboard = newMenu.GetSubMenus(ctx.User).Select(menu => new KeyboardButton[] { new(menu) })
                            }
                        );
                        return true;
                    }

                    if (text == IM_NOT_A_STUDENT_TEXT)
                    {
                        ctx.DB.UpdateUserStudentCategories(ctx.User, null);
                        await ctx.TelegramCLient.SendTextMessageAsync(
                            chatId: ctx.MessageEvent.Message.Chat,
                            text: $"You are not a student anymore",
                            replyMarkup: new ReplyKeyboardMarkup()
                            {
                                Keyboard = GenerateCategoriesReplyMarkup(ctx.User)
                            }
                        );
                    }
                    else if (text.StartsWith(ADD_CATEGORY_TEXT))
                    {
                        //todo different checks
                        var category = text.Substring(ADD_CATEGORY_TEXT.Length);
                        if (Application.Categories.ContainsValue(category))
                        {
                            var catId = Application.Categories.FirstOrDefault(kvp => kvp.Value == category).Key;
                            var newCategories = (ctx.User.StudentCategories ?? Array.Empty<int>())
                                .Where(c => c != catId)
                                .Concat(new[] { catId })
                                .ToArray();
                            ctx.User = ctx.DB.UpdateUserStudentCategories(ctx.User, newCategories);
                            await ctx.TelegramCLient.SendTextMessageAsync(
                                chatId: ctx.MessageEvent.Message.Chat,
                                text: $"U successfully added student category {category}",
                                replyMarkup: new ReplyKeyboardMarkup()
                                {
                                    Keyboard = GenerateCategoriesReplyMarkup(ctx.User)
                                }
                            );
                        }
                        else
                        {
                            await ctx.TelegramCLient.SendTextMessageAsync(
                                chatId: ctx.MessageEvent.Message.Chat,
                                text: $"Use one of proposed categories",
                                replyMarkup: new ReplyKeyboardMarkup()
                                {
                                    Keyboard = GenerateCategoriesReplyMarkup(ctx.User)
                                }
                            );
                        }
                    }
                    else if (text.StartsWith(REMOVE_CATEGORY_TEXT))
                    {
                        //todo different checks
                        var category = text.Substring(ADD_CATEGORY_TEXT.Length);
                        if (Application.Categories.ContainsValue(category))
                        {
                            var catId = Application.Categories.FirstOrDefault(kvp => kvp.Value == category).Key;
                            var newCategories = (ctx.User.StudentCategories ?? Array.Empty<int>())
                                .Where(c => c != catId)
                                .ToArray();
                            ctx.User = ctx.DB.UpdateUserStudentCategories(ctx.User, newCategories);
                            await ctx.TelegramCLient.SendTextMessageAsync(
                                chatId: ctx.MessageEvent.Message.Chat,
                                text: $"U successfully removed student category {category}",
                                replyMarkup: new ReplyKeyboardMarkup()
                                {
                                    Keyboard = GenerateCategoriesReplyMarkup(ctx.User)
                                }
                            );
                        }
                        else
                        {
                            await ctx.TelegramCLient.SendTextMessageAsync(
                                chatId: ctx.MessageEvent.Message.Chat,
                                text: $"Use one of proposed categories",
                                replyMarkup: new ReplyKeyboardMarkup()
                                {
                                    Keyboard = GenerateCategoriesReplyMarkup(ctx.User)
                                }
                            );
                        }
                    }
                    else
                    {
                        await ctx.TelegramCLient.SendTextMessageAsync(
                            chatId: ctx.MessageEvent.Message.Chat,
                            text: $"Use one of proposed categories",
                            replyMarkup: new ReplyKeyboardMarkup()
                            {
                                Keyboard = GenerateCategoriesReplyMarkup(ctx.User)
                            }
                        );
                    }
                    return true;
                }
            };

            AddMenuSection(regMenu, section);
        }
    }
}
