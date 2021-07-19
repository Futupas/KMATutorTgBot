using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMATutorBot.Menu.Sections
{
    internal static partial class MenuSectionsGenerator
    {
        private static void GenerateProfile()
        {
            var registrationMenu = new MenuSection()
            {
                Id = NextMenuSection,
                IsForUser = MenuSection.FORUSER_SAMPLE_ALL_USERS,
                Text = "My profile",
            };
            AddMenuSection(_Root, registrationMenu);

            GenerateNameProfileEditor(registrationMenu);
            GenerateDescriptionProfileEditor(registrationMenu);

            // name
            // description
            // students
            // teachers
        }
    }
}
