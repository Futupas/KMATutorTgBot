using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMATutorBot.MessageTexts
{
    internal static class BotMessages
    {
        public const string BACK_TEXT = @"Back";
        public const string BACK_TO_ROOT_TEXT = @"Back to root";
        public const string UNKNOWN_COMMAND = @"Unknown command(";
        public static string YOU_ARE_ON_MENU_SECTION (string menuSection) => $"U are on menu section {menuSection}";
        public const string ROOT_MENU_TEXT = @"Root menu";
        public const string MY_PROFILE_MENU_TEXT = @"My profile";
        public const string MY_PROFILE_DISPLAY_NAME_MENU_TEXT = @"Display name";
        public const string MY_PROFILE_ENTER_NOT_EMPTY_NAME = @"Enter not empty name";
        public static string MY_PROFILE_DISPLAY_NAME_UPDATED(Context ctx) =>
            $"Update user {ctx.MessageEvent.Message.Chat.Id} with name {ctx.MessageEvent.Message.Text}";
        public const string MY_PROFILE_DESCRIPTION_MENU_TEXT = @"About me";
        public const string MY_PROFILE_ENTER_NOT_EMPTY_DESCRIPTION = @"Enter not empty about me text";
        public static string MY_PROFILE_DESCRIPTION_UPDATED(Context ctx) =>
            $"Update user {ctx.MessageEvent.Message.Chat.Id} with about me text {ctx.MessageEvent.Message.Text}";
        public const string MY_PROFILE_IM_NOT_A_STUDENT_TEXT = @"I'm not a student";
        public const string MY_PROFILE_IM_NOT_A_TEACHER_TEXT = @"I'm not a teacher";
        public const string ADD_CATEGORY_TEXT = "+ ";
        public const string REMOVE_CATEGORY_TEXT = "- ";
        public const string MY_PROFILE_MODIFY_STUDENT_CATEGORIES_TEXT = @"Modify student categories";
        public const string MY_PROFILE_USE_ONE_OF_THE_PROPOSED_CATEGORIES = @"Use one of the proposed categories";
        public const string MY_PROFILE_YOU_ARE_NOT_A_STUDENT_ANYMORE = @"You are not a student anymore";
        public static string MY_PROFILE_STUDENT_ADDED_CATEGORY(string category) =>
            $"U successfully added student category {category}";
        public static string MY_PROFILE_STUDENT_REMOVED_CATEGORY(string category) =>
            $"U successfully removed student category {category}";
        public const string MY_PROFILE_MODIFY_TEACHER_CATEGORIES_TEXT = @"Modify teacher categories";
        public const string MY_PROFILE_YOU_ARE_NOT_A_TEACHER_ANYMORE = @"You are not a teacher anymore";
        public static string MY_PROFILE_TEACHER_ADDED_CATEGORY(string category) =>
            $"U successfully added student category {category}";
        public static string MY_PROFILE_TEACHER_REMOVED_CATEGORY(string category) =>
            $"U successfully removed student category {category}";

    }

    //todo make commands
    internal static class BotCommands
    {
        public const string BACK_TO_ROOT = @"/back_to_root";
    }
}
