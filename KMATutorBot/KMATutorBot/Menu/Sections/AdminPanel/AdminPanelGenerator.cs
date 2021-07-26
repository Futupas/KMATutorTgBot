using System;
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
        private static void GenerateAdminPanel()
        {
            var adminPanel = new MenuSection()
            {
                Id = NextMenuSectionId(),
                IsForUser = MenuSection.FORUSER_SAMPLE_ADMINS_ONLY,
                Text = BotMessages.ADMIN_PANEL_MENU_TEXT,
                CustomText = (ctx) => BotMessages.ADMIN_PANEL_MENU_TEXT
            };

            AddMenuSection(_Root, adminPanel);

            GenerateLicensesEditor(adminPanel);
        }

        private static void GenerateLicensesEditor(MenuSection parent)
        {
            var licenseEditor = new MenuSection()
            {
                Id = NextMenuSectionId(),
                IsForUser = MenuSection.FORUSER_SAMPLE_ADMINS_ONLY,
                Text = BotMessages.ADMIN_PANEL_LICENSES_EDITOR_MENU_TEXT,
                CustomText = (ctx) => BotMessages.ADMIN_PANEL_LICENSES_EDITOR_MESSAGE,
                Handle = async (ctx) =>
                {
                    var text = ctx.MessageEvent.Message.Text;
                    if (await MenuSection.HandleEmptyOrBackString(text, ctx, null)) return true;

                    if (ctx.User.Data == null || !ctx.User.Data.TryGetValue("current_license_editing_user", out object data))
                    {
                        if (ctx.User.Data == null) ctx.User.Data = new();
                        var nickname = text.StartsWith("@") ? text[1..] : text;
                        var user = ctx.DB.GetUserByTelegramNickname(nickname);
                        if (user == null)
                        {
                            await ctx.TelegramCLient.SendTextMessageAsync(
                                chatId: ctx.MessageEvent.Message.Chat,
                                text: BotMessages.ADMIN_PANEL_LICENSES_USER_WITH_THIS_NICK_NOT_EXISTS,
                                replyMarkup: new ReplyKeyboardMarkup()
                                {
                                    Keyboard = GenerateKeyboardWithBacks(ctx.Menu, null)
                                },
                                parseMode: ParseMode.Html
                            );
                        }
                        else
                        {
                            ctx.User.Data.Add("current_license_editing_user", nickname);
                            ctx.DB.UpdateUserData(ctx.User);
                            await ctx.TelegramCLient.SendTextMessageAsync(
                                chatId: ctx.MessageEvent.Message.Chat,
                                text: BotMessages.ADMIN_PANEL_LICENSES_SET_USER_LICENSE(ctx, user),
                                replyMarkup: new ReplyKeyboardMarkup()
                                {
                                    Keyboard = GenerateKeyboardWithBacks(ctx.Menu,
                                        BotMessages.ADMIN_PANEL_LICENSES_DEFAULT_PLAN_1_MINUTE,
                                        BotMessages.ADMIN_PANEL_LICENSES_DEFAULT_PLAN_1_DAY,
                                        BotMessages.ADMIN_PANEL_LICENSES_DEFAULT_PLAN_1_WEEK,
                                        BotMessages.ADMIN_PANEL_LICENSES_DEFAULT_PLAN_10000_DAYS,
                                        BotMessages.ADMIN_PANEL_LICENSES_DEFAULT_PLAN_REMOVE_LICENSE)
                                },
                                parseMode: ParseMode.Html
                            );
                        }
                        return true;
                    }
                    else
                    {
                        var nickname = (string)data; // Can be exception
                        if (await MenuSection.HandleEmptyOrBackString(text, ctx, null)) return true;
                        var user = ctx.DB.GetUserByTelegramNickname(nickname);

                        void RemoveDataFromUser()
                        {
                            var newData = ctx.User.Data;
                            if (newData != null && newData.ContainsKey("current_license_editing_user"))
                            {
                                newData.Remove("current_license_editing_user");
                                ctx.User.Data = newData;
                            }
                            ctx.DB.UpdateUserData(ctx.User);
                        }
                        async void SendOkMsgAndReturn(DateTime? time)
                        {
                            var parentMenu = ctx.Menu.Parent;
                            ctx.DB.UpdateUserMenuSection(ctx.User, parentMenu);

                            await ctx.TelegramCLient.SendTextMessageAsync(
                                chatId: ctx.MessageEvent.Message.Chat,
                                text: BotMessages.ADMIN_PANEL_LICENSES_UPDATED_SUCCESSFULLY(ctx, user, time),
                                replyMarkup: new ReplyKeyboardMarkup()
                                {
                                    Keyboard = GenerateKeyboardWithBacks(parentMenu, parentMenu.GetSubMenus(ctx.User))
                                },
                                parseMode: ParseMode.Html
                            );
                        }

                        if (text == BotMessages.ADMIN_PANEL_LICENSES_DEFAULT_PLAN_1_MINUTE)
                        {
                            var newTime = DateTime.Now + new TimeSpan(0, 1, 0);
                            user.LicenseExpired = newTime;
                            ctx.DB.UpdateUserLicenseExpired(user);
                            RemoveDataFromUser();
                            SendOkMsgAndReturn(newTime);
                            return true;
                        }
                        else if (text == BotMessages.ADMIN_PANEL_LICENSES_DEFAULT_PLAN_1_DAY)
                        {
                            var newTime = DateTime.Now + new TimeSpan(1, 0, 0, 0);
                            user.LicenseExpired = newTime;
                            ctx.DB.UpdateUserLicenseExpired(user);
                            RemoveDataFromUser();
                            SendOkMsgAndReturn(newTime);
                            return true;
                        }
                        else if (text == BotMessages.ADMIN_PANEL_LICENSES_DEFAULT_PLAN_1_WEEK)
                        {
                            var newTime = DateTime.Now + new TimeSpan(7, 0, 0, 0);
                            user.LicenseExpired = newTime;
                            ctx.DB.UpdateUserLicenseExpired(user);
                            RemoveDataFromUser();
                            SendOkMsgAndReturn(newTime);
                            return true;
                        }
                        else if (text == BotMessages.ADMIN_PANEL_LICENSES_DEFAULT_PLAN_10000_DAYS)
                        {
                            var newTime = DateTime.Now + new TimeSpan(10000, 0, 0, 0);
                            user.LicenseExpired = newTime;
                            ctx.DB.UpdateUserLicenseExpired(user);
                            RemoveDataFromUser();
                            SendOkMsgAndReturn(newTime);
                            return true;
                        }
                        else if (text == BotMessages.ADMIN_PANEL_LICENSES_DEFAULT_PLAN_REMOVE_LICENSE)
                        {
                            user.LicenseExpired = null;
                            ctx.DB.UpdateUserLicenseExpired(user);
                            RemoveDataFromUser();
                            SendOkMsgAndReturn(null);
                            return true;
                        }
                        else if (uint.TryParse(text, out var days))
                        {
                            var newTime = DateTime.Now + new TimeSpan((int)days, 0, 0, 0);
                            user.LicenseExpired = newTime;
                            ctx.DB.UpdateUserLicenseExpired(user);
                            RemoveDataFromUser();
                            return true;
                        }
                    }

                    return await MenuSection.SendUnknownCommand(ctx, null);
                }
            };

            AddMenuSection(parent, licenseEditor);
        }
    }
}
