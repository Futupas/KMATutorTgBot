using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KMATutorBot.MessageTexts;

namespace KMATutorBot.Menu.Sections
{
    internal static partial class MenuSectionsGenerator
    {
        private static void GenerateProfile()
        {
            var registrationMenu = new MenuSection()
            {
                Id = NextMenuSectionId(),
                IsForUser = MenuSection.FORUSER_SAMPLE_ALL_USERS,
                Text = BotMessages.MY_PROFILE_MENU_TEXT,
            };
            AddMenuSection(_Root, registrationMenu);

            GenerateNameProfileEditor(registrationMenu);
            GenerateDescriptionProfileEditor(registrationMenu);
            GenerateStudentCategoriesProfileEditor(registrationMenu);
            GenerateTeacherCategoriesProfileEditor(registrationMenu);
        }
    }
}
