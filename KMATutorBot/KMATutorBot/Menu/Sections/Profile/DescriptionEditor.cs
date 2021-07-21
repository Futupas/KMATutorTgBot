﻿using System;
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
        private static void GenerateDescriptionProfileEditor(MenuSection regMenu)
        {

            var registrationMenuDescription = new MenuSection()
            {
                Id = NextMenuSectionId(),
                IsForUser = MenuSection.FORUSER_SAMPLE_ALL_USERS,
                Text = BotMessages.MY_PROFILE_DESCRIPTION_MENU_TEXT,
                Handle = async (ctx) =>
                {
                    var text = ctx.MessageEvent.Message.Text;
                    if (string.IsNullOrEmpty(text))
                    {
                        await ctx.TelegramCLient.SendTextMessageAsync(
                            chatId: ctx.MessageEvent.Message.Chat,
                            text: BotMessages.MY_PROFILE_ENTER_NOT_EMPTY_DESCRIPTION,
                            replyMarkup: new ReplyKeyboardMarkup()
                            {
                                Keyboard = new KeyboardButton[][] { new KeyboardButton[] { new(MenuSection.BACK_TEXT) }, new KeyboardButton[] { new(MenuSection.BACK_TO_START_TEXT) } }
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

                    var parentMenu = ctx.Menu.Parent;
                    ctx.DB.UpdateUserDescription(ctx.User, text);
                    ctx.DB.UpdateUserMenuSection(ctx.User, parentMenu);

                    await ctx.TelegramCLient.SendTextMessageAsync(
                        chatId: ctx.MessageEvent.Message.Chat,
                        text: BotMessages.MY_PROFILE_DESCRIPTION_UPDATED(ctx),
                        replyMarkup: new ReplyKeyboardMarkup()
                        {
                            Keyboard = parentMenu.GetSubMenus(ctx.User).Select(menu => new KeyboardButton[] { new(menu) })
                        }
                    );
                    return true;
                }
            };

            AddMenuSection(regMenu, registrationMenuDescription);
        }
    }
}
