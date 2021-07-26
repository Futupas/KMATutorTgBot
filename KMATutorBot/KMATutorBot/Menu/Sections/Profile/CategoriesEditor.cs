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
using KMATutorBot.MessageTexts;

namespace KMATutorBot.Menu.Sections
{
    internal static partial class MenuSectionsGenerator
    {
        private const string IM_NOT_A_TEACHER_TEXT = BotMessages.MY_PROFILE_IM_NOT_A_TEACHER_TEXT;
        private const string ADD_CATEGORY_TEXT = BotMessages.ADD_CATEGORY_TEXT;
        private const string REMOVE_CATEGORY_TEXT = BotMessages.REMOVE_CATEGORY_TEXT;

        private static KeyboardButton[][] GenerateTeacherCategoriesReplyMarkup(BotUser user)
        {
            return Application.Categories
                .OrderBy(el => el.Value) // Actually, we can order by Id
                .Select(cat =>
                {
                    var sign = (user.TeacherCategories != null && user.TeacherCategories.Contains(cat.Key)) ?
                        REMOVE_CATEGORY_TEXT : ADD_CATEGORY_TEXT;
                    return sign + cat.Value;
                })
                .Concat(new[] { IM_NOT_A_TEACHER_TEXT })
                .Concat(new string[] { MenuSection.BACK_TEXT, MenuSection.BACK_TO_START_TEXT })
                .Select(str => new KeyboardButton[] { new(str) })
                .ToArray();
        }

        private static void GenerateTeacherCategoriesProfileEditor(MenuSection regMenu)
        {
            var section = new MenuSection()
            {
                Id = NextMenuSectionId(),
                IsForUser = MenuSection.FORUSER_SAMPLE_ALL_USERS,
                Text = BotMessages.MY_PROFILE_MODIFY_TEACHER_CATEGORIES_TEXT,
                CustomKeyboard = (ctx) => GenerateTeacherCategoriesReplyMarkup(ctx.User),
                Handle = async (ctx) =>
                {
                    var text = ctx.MessageEvent.Message.Text;
                    if (string.IsNullOrEmpty(text))
                    {
                        await ctx.TelegramCLient.SendTextMessageAsync(
                            chatId: ctx.MessageEvent.Message.Chat,
                            text: BotMessages.MY_PROFILE_USE_ONE_OF_THE_PROPOSED_CATEGORIES,
                            replyMarkup: new ReplyKeyboardMarkup()
                            {
                                Keyboard = GenerateTeacherCategoriesReplyMarkup(ctx.User)
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

                    if (text == IM_NOT_A_TEACHER_TEXT)
                    {
                        ctx.DB.UpdateUserTeacherCategories(ctx.User, null);
                        await ctx.TelegramCLient.SendTextMessageAsync(
                            chatId: ctx.MessageEvent.Message.Chat,
                            text: BotMessages.MY_PROFILE_YOU_ARE_NOT_A_TEACHER_ANYMORE,
                            replyMarkup: new ReplyKeyboardMarkup()
                            {
                                Keyboard = GenerateTeacherCategoriesReplyMarkup(ctx.User)
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
                            var newCategories = (ctx.User.TeacherCategories ?? Array.Empty<int>())
                                .Where(c => c != catId)
                                .Concat(new[] { catId })
                                .ToArray();
                            ctx.User = ctx.DB.UpdateUserTeacherCategories(ctx.User, newCategories);
                            await ctx.TelegramCLient.SendTextMessageAsync(
                                chatId: ctx.MessageEvent.Message.Chat,
                                text: BotMessages.MY_PROFILE_TEACHER_ADDED_CATEGORY(category),
                                replyMarkup: new ReplyKeyboardMarkup()
                                {
                                    Keyboard = GenerateTeacherCategoriesReplyMarkup(ctx.User)
                                }
                            );
                        }
                        else
                        {
                            await ctx.TelegramCLient.SendTextMessageAsync(
                                chatId: ctx.MessageEvent.Message.Chat,
                                text: BotMessages.MY_PROFILE_USE_ONE_OF_THE_PROPOSED_CATEGORIES,
                                replyMarkup: new ReplyKeyboardMarkup()
                                {
                                    Keyboard = GenerateTeacherCategoriesReplyMarkup(ctx.User)
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
                            var newCategories = (ctx.User.TeacherCategories ?? Array.Empty<int>())
                                .Where(c => c != catId)
                                .ToArray();
                            ctx.User = ctx.DB.UpdateUserTeacherCategories(ctx.User, newCategories);
                            await ctx.TelegramCLient.SendTextMessageAsync(
                                chatId: ctx.MessageEvent.Message.Chat,
                                text: BotMessages.MY_PROFILE_TEACHER_REMOVED_CATEGORY(category),
                                replyMarkup: new ReplyKeyboardMarkup()
                                {
                                    Keyboard = GenerateTeacherCategoriesReplyMarkup(ctx.User)
                                }
                            );
                        }
                        else
                        {
                            await ctx.TelegramCLient.SendTextMessageAsync(
                                chatId: ctx.MessageEvent.Message.Chat,
                                text: BotMessages.MY_PROFILE_USE_ONE_OF_THE_PROPOSED_CATEGORIES,
                                replyMarkup: new ReplyKeyboardMarkup()
                                {
                                    Keyboard = GenerateTeacherCategoriesReplyMarkup(ctx.User)
                                }
                            );
                        }
                    }
                    else
                    {
                        await ctx.TelegramCLient.SendTextMessageAsync(
                            chatId: ctx.MessageEvent.Message.Chat,
                            text: BotMessages.MY_PROFILE_USE_ONE_OF_THE_PROPOSED_CATEGORIES,
                            replyMarkup: new ReplyKeyboardMarkup()
                            {
                                Keyboard = GenerateTeacherCategoriesReplyMarkup(ctx.User)
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
