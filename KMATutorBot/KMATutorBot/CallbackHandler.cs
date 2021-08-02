using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KMATutorBot.MessageTexts;
using Telegram.Bot.Types.ReplyMarkups;

namespace KMATutorBot
{
    internal static class CallbackHandler
    {
        public static async Task<(bool ok, string message)> HandleCallback(Context context)
        {
            var message = context.CallbackEvent.CallbackQuery.Data;
            if (string.IsNullOrEmpty(message))
            {
                return (true, "");
            }
            if (message.StartsWith("match"))
            {
                await HandleMatchCallback(context);
                return (false, "");
            }

            return (true, "hello world");
        }
        public static async Task<bool> HandleMatchCallback(Context context)
        {
            var message = context.CallbackEvent.CallbackQuery.Data;

            var data = message.Split("_");
            if (data.Length != 4) throw new Exception("Incorrect callback data");
            try
            {
                var studentId = (int)context.User.Id;
                var teacherId = int.Parse(data[2]);
                var categoryId = int.Parse(data[3]);
                var save = data[1] == "s";

                context.DB.AddMatch(studentId, teacherId, categoryId, save ? Models.MatchResultType.Save : Models.MatchResultType.Next);
                Console.WriteLine($"student {studentId} matched teacher {teacherId} in category {categoryId} with match {(save ? "save" : "next")}");
                //todo remove console writeline

                await context.TelegramCLient.AnswerCallbackQueryAsync(
                    context.CallbackEvent.CallbackQuery.Id, 
                    save ? BotMessages.MATCH_SAVE_ANSWER_TEXT : BotMessages.MATCH_NEXT_ANSWER_TEXT
                );
                await context.TelegramCLient.EditMessageReplyMarkupAsync(
                    context.CallbackEvent.CallbackQuery.Message.Chat.Id,
                    context.CallbackEvent.CallbackQuery.Message.MessageId,
                    new InlineKeyboardMarkup(Enumerable.Empty<IEnumerable<InlineKeyboardButton>>())
                 );

            }
            catch(Exception ex)
            {
                throw new Exception("Incorrect callback data", ex);
            }

            return false;
        }
    }
}
