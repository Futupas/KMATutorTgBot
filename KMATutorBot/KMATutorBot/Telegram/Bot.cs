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
            botClient.StartReceiving();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            botClient.StopReceiving();


        }
        private async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");

                var user = new BotUser()
                {
                    Id = e.Message.From.Id,
                    TelegramName = e.Message.From.FirstName + " " + e.Message.From.LastName,
                };
                var userTuple = DB.GetOrCreateUser(user);
                user = userTuple.user;
                //todo handle exceptions

                var ctx = new Context(user, DB, this.Menu.sections) { 
                    TelegramCLient = botClient,
                    MessageEvent = e
                };

                await ctx.Menu.Handle(ctx);

                //var handledResult = ctx.HandleText(e.Message.Text);

                //await botClient.SendTextMessageAsync(
                //    chatId: e.Message.Chat,
                //    text: handledResult.message,
                //    replyMarkup: new ReplyKeyboardMarkup()
                //    {
                //        Keyboard = handledResult.menus.Select(menu => new KeyboardButton[] { new (menu) })
                //    }
                //);
            }
        }
    }
}

/*

                                        //replyMarkup: new ReplyKeyboardMarkup()
                                        //{
                                        //    Keyboard = new KeyboardButton[][]
                                        //    {
                                        //        new KeyboardButton[]
                                        //        {
                                        //            new KeyboardButton("1-1"),
                                        //            new KeyboardButton("1-2")
                                        //        },

                                        //        new KeyboardButton[]
                                        //        {
                                        //            new KeyboardButton("2")
                                        //        },

                                        //        new KeyboardButton[]
                                        //        {
                                        //            new KeyboardButton("3-1"),
                                        //            new KeyboardButton("3-2"),
                                        //            new KeyboardButton("3-3")
                                        //        }
                                        //    }
                                        //} 
 */