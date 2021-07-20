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
        private static void GenerateNameProfileEditor(MenuSection regMenu)
        {
            var registrationMenuName = new MenuSection()
            {
                Id = NextMenuSectionId(),
                IsForUser = MenuSection.FORUSER_SAMPLE_ALL_USERS,
                Text = "Display name",
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

            AddMenuSection(regMenu, registrationMenuName);
        }
    }
}
