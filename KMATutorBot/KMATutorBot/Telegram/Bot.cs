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

namespace KMATutorBot
{
    internal class Bot
    {
        private const string BOT_TOKEN = @"1822606315:AAGOAm6NN6ubNRm5rvjXDzxdaSb0z-kh8eA";
        private ITelegramBotClient botClient;
        private IDatabase DB;
        protected IServiceProvider serviceProvider;

        public Bot(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.DB = serviceProvider.GetService<IDatabase>();

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

                await botClient.SendTextMessageAsync(
                  chatId: e.Message.Chat,
                  text: $"You said:\n{e.Message.Text}\nYou are { (userTuple.isNew ? "" : "NOT ") }new user"
                );
            }
        }
    }
}
