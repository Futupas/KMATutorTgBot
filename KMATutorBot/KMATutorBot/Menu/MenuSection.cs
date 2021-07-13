using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMATutorBot.Menu
{
    internal class MenuSection
    {
        public const string BACK_TEXT = @"Back";
        public const string BACK_TO_START_TEXT = @"Back To Menu";

        public int Id { get; init; }
        public string Text { get; init; }
        public MenuSection[] Children { get; init; }
        public MenuSection? Parent { get; init; }
        public MenuSection Root { get; init; }

        public bool ForStudents { get; init; } = true;
        public bool ForTeachers { get; init; } = true;
        public bool ForAdmins { get; init; } = false;

        public string[]? GetSubMenus()
        {
            return Children.Select(el => el.Text).Concat(new[] { BACK_TEXT, BACK_TO_START_TEXT }).ToArray();
        }
    }
}
