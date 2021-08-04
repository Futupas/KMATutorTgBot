﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KMATutorBot.MessageTexts;
using Telegram.Bot.Types.Enums;
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
                CustomKeyboard = (ctx) => GenerateKeyboardWithBacks(ctx.Menu, Application.Categories.Select(cat => cat.Name)),
                OnOpen = async (ctx) =>
                {
                    var categories = Application.Categories.Select(cat => cat.Name);

                    await ctx.TelegramCLient.SendTextMessageAsync(
                        chatId: ctx.MessageEvent.Message.Chat,
                        text: BotMessages.FINDER_FIND_TEACHERS_SELECT_CATEGORY_TEXT,
                        replyMarkup: new ReplyKeyboardMarkup()
                        {
                            Keyboard = GenerateKeyboardWithBacks(ctx.Menu, categories)
                        },
                        parseMode: ParseMode.Html
                    );
                    return true;
                },
                Handle = async (ctx) =>
                {
                    var text = ctx.MessageEvent.Message.Text;
                    
                    if (await MenuSection.HandleEmptyOrBackString(text, ctx, null))
                    {
                        return true;
                    }

                    var category = Application.Categories.FirstOrDefault(cat => cat.Name == text);

                    if (category != null)
                    {
                        var teachers = ctx.DB.GetTeachersByCategory(category.Id, ctx.User.Id);
                        var teacherId = teachers.Any() ? teachers.FirstOrDefault().Id : 0;
                        var replyText = BotMessages.FINDER_WE_FOUND_TEACHERS_TEXT(teachers);

                        var replyMarkup = new InlineKeyboardMarkup(new InlineKeyboardButton[][] {
                            new InlineKeyboardButton[] {
                                new() { Text = BotMessages.MATCH_NEXT_TEXT, CallbackData = $"match_n_{teacherId}_{category.Id}" },
                                new() { Text = BotMessages.MATCH_SAVE_TEXT, CallbackData = $"match_s_{teacherId}_{category.Id}" }
                            }
                        });
                        var emptyReplyMarkup = new InlineKeyboardMarkup(new InlineKeyboardButton[][] {
                            new InlineKeyboardButton[] { }
                        });

                        await ctx.TelegramCLient.SendTextMessageAsync(
                            chatId: ctx.MessageEvent.Message.Chat,
                            text: replyText,
                            replyMarkup: teachers.Any() ? replyMarkup : emptyReplyMarkup,
                            parseMode: ParseMode.Html
                        );
                    }
                    else
                    {
                        await ctx.TelegramCLient.SendTextMessageAsync(
                            chatId: ctx.MessageEvent.Message.Chat,
                            text: BotMessages.MY_PROFILE_TEACHER_INCORRECT_CATEGORY_TEXT,
                            replyMarkup: new ReplyKeyboardMarkup()
                            {
                                Keyboard = GenerateKeyboardWithBacks(ctx.Menu, Application.Categories.Select(cat => cat.Name))
                            },
                            parseMode: ParseMode.Html
                        );
                    }

                    return true;
                }
            };

            AddMenuSection(_Root, finderMenu);
        }
    }
}
