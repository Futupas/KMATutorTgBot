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

namespace KMATutorBot
{
    internal class Bot
    {
        private const string BOT_TOKEN = @"1822606315:AAGOAm6NN6ubNRm5rvjXDzxdaSb0z-kh8eA";
        private ITelegramBotClient botClient;
        private Database DB;
        private (Menu.MenuSection root, List<Menu.MenuSection> sections) Menu = KMATutorBot.Menu.MenuSection.GenerateDefaultMenu();

        public Bot(Database DB)
        {
            this.DB = DB;

            botClient = new TelegramBotClient(BOT_TOKEN);

            var me = botClient.GetMeAsync().Result;
            Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");

            botClient.OnMessage += Bot_OnMessage;
            botClient.OnCallbackQuery += Bot_OnCallback;
            botClient.StartReceiving();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            botClient.StopReceiving();


        }
        private async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");

            var userName = e.Message.From.FirstName + " " + e.Message.From.LastName;
            var user = new BotUser()
            {
                Id = e.Message.From.Id,
                TelegramName = userName,
                DisplayName = userName,
                RegistrationMessage = e.Message.Text,
                RegistrationDate = DateTimeOffset.Now.ToUnixTimeSeconds(),
                TelegramUsername = e.Message.From.Username
            };
            var userTuple = DB.GetOrCreateUser(user);
            user = userTuple.user;

            var ctx = new Context(user, DB, this.Menu.sections)
            {
                TelegramCLient = botClient,
                MessageEvent = e
            };

            if (userTuple.isNew)
            {
                await botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: BotMessages.HELLO_MESSAGE(ctx),
                    replyMarkup: new ReplyKeyboardMarkup()
                    {
                        Keyboard = KMATutorBot.Menu.Sections.MenuSectionsGenerator.GenerateKeyboardWithBacks(ctx.Menu.Root, ctx.Menu.GetSubMenus(ctx.User))
                    },
                    parseMode: ParseMode.Html
                );
                return;
            }

            await ctx.Menu.Handle(ctx);
        }
        private async void Bot_OnCallback(object sender, CallbackQueryEventArgs e)
        {
            Console.WriteLine($"Received a callback message from {e.CallbackQuery.From.Id}.");

            var userName = e.CallbackQuery.From.FirstName + " " + e.CallbackQuery.From.LastName;
            var user = new BotUser()
            {
                Id = e.CallbackQuery.From.Id,
                TelegramName = userName,
                DisplayName = userName,
                TelegramUsername = e.CallbackQuery.From.Username
            };
            var userTuple = DB.GetOrCreateUser(user);
            user = userTuple.user;

            var ctx = new Context(user, DB, this.Menu.sections)
            {
                TelegramCLient = botClient,
                CallbackEvent = e
            };

            if (userTuple.isNew)
            {
                return;
            }

            var resultTuple = await CallbackHandler.HandleCallback(ctx);
            if (resultTuple.ok)
            {
                await botClient.AnswerCallbackQueryAsync(e.CallbackQuery.Id, resultTuple.message);
            }
        }
    }
}
