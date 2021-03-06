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
        private static void GenerateDescriptionProfileEditor(MenuSection regMenu)
        {

            var registrationMenuDescription = new MenuSection()
            {
                Id = NextMenuSectionId(),
                IsForUser = MenuSection.FORUSER_SAMPLE_ALL_USERS,
                Text = BotMessages.MY_PROFILE_DESCRIPTION_MENU_TEXT,
                CustomKeyboard = (ctx) => GenerateKeyboardWithBacks(null, null),
                Handle = async (ctx) =>
                {
                    var text = ctx.MessageEvent.Message.Text;

                    if (await MenuSection.HandleEmptyOrBackString(text, ctx, null))
                    {
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
                            Keyboard = GenerateKeyboardWithBacks(parentMenu, parentMenu.GetSubMenus(ctx.User))
                        },
                        parseMode: ParseMode.Html
                    );
                    return true;
                }
            };

            AddMenuSection(regMenu, registrationMenuDescription);
        }
    }
}
