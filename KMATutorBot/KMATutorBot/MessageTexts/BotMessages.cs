using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMATutorBot.MessageTexts
{
    internal static class BotMessages
    {
        public static string HELLO_MESSAGE (Context ctx) => $"Hello, and welcome to KMA Tutor bot";
        public const string BACK_TEXT = @"Back";
        public const string BACK_TO_ROOT_TEXT = @"Back to root";
        public const string UNKNOWN_COMMAND = @"Unknown command(";
        public static string YOU_ARE_ON_MENU_SECTION(string menuSection) => $"U are on menu section {menuSection}";
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
        public const string MY_PROFILE_IM_NOT_A_TEACHER_TEXT = @"I'm not a teacher";
        public const string ADD_CATEGORY_TEXT = "+ ";
        public const string REMOVE_CATEGORY_TEXT = "- ";
        public const string MY_PROFILE_TEACHER_INCORRECT_CATEGORY_TEXT = "Incorrect category";
        public const string MY_PROFILE_USE_ONE_OF_THE_PROPOSED_CATEGORIES = @"Use one of the proposed categories";
        public const string MY_PROFILE_MODIFY_TEACHER_CATEGORIES_TEXT = @"Modify teacher categories";
        public const string MY_PROFILE_YOU_ARE_NOT_A_TEACHER_ANYMORE = @"You are not a teacher anymore";
        public static string MY_PROFILE_TEACHER_ADDED_CATEGORY(string category) =>
            $"U successfully added teacher category {category}";
        public static string MY_PROFILE_TEACHER_REMOVED_CATEGORY(string category) =>
            $"U successfully removed teacher category {category}";

        public const string FINDER_FIND_TEACHERS_MENU_TEXT = @"Find teachers";
        public const string FINDER_FIND_TEACHERS_SELECT_CATEGORY_TEXT = @"Select ur category";
        public const string FINDER_NO_TEACHERS_TEXT = @"We couldn't find any teachers in this category";
        public static string FINDER_WE_FOUND_TEACHERS_TEXT(IEnumerable<Models.BotUser> teachers)
        {
            if (teachers == null || !teachers.Any()) return FINDER_NO_TEACHERS_TEXT;
            //todo add link to profile
            var body = teachers
                .Select(tch =>
                {
                    var bodyNamePart = $"<a href=\"tg://user?id={tch.Id}\">{tch.DisplayName ?? "Unnamed teacher"}</a>";
                    var bodyDescriptionPart = tch.Description == null ? "" : $"\n{tch.Description}";
                    return bodyNamePart + bodyDescriptionPart;
                });
            return $"Ookey, for u we found these students (limit 10):\n\n" + string.Join("\n\n", body);
        }

        public const string ADMIN_PANEL_MENU_TEXT = @"Admin panel";
        public const string ADMIN_PANEL_LICENSES_EDITOR_MENU_TEXT = @"Licenses editor";
        public const string ADMIN_PANEL_LICENSES_EDITOR_MESSAGE = @"Okey, now enter telegram nickname of person you want to change licence";
        public const string ADMIN_PANEL_LICENSES_USER_WITH_THIS_NICK_NOT_EXISTS = @"User with such nickname does not exist";
        public static string ADMIN_PANEL_LICENSES_SET_USER_LICENSE(Context ctx, Models.BotUser user) =>
            $"Okey, now choose license plan for user {user.DisplayName}, or enter your quantity of days";
        public const string ADMIN_PANEL_LICENSES_DEFAULT_PLAN_1_MINUTE = @"1 minute";
        public const string ADMIN_PANEL_LICENSES_DEFAULT_PLAN_1_DAY = @"1 day";
        public const string ADMIN_PANEL_LICENSES_DEFAULT_PLAN_1_WEEK = @"1 week";
        public const string ADMIN_PANEL_LICENSES_DEFAULT_PLAN_10000_DAYS = @"10k days";
        public const string ADMIN_PANEL_LICENSES_DEFAULT_PLAN_REMOVE_LICENSE = @"Remove license";
        public static string ADMIN_PANEL_LICENSES_UPDATED_SUCCESSFULLY(Context ctx, Models.BotUser user, DateTime? newTime) =>
            $"For user {user.DisplayName} license {(newTime == null ? "was ended" : $"will expire at {newTime.Value.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss")}")}";
    }

    //todo make commands
    internal static class BotCommands
    {
        public const string BACK_TO_ROOT = @"/back_to_root";
    }
}
