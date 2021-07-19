using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMATutorBot.Menu.Sections
{
    internal static partial class MenuSectionsGenerator
    {
        private static void GenerateSampleChildren()
        {
            var child1 = new MenuSection()
            {
                Id = NextMenuSection,
                IsForUser = MenuSection.FORUSER_SAMPLE_ALL_USERS,
                Text = "child1",
            };

            var child2 = new MenuSection()
            {
                Id = NextMenuSection,
                IsForUser = MenuSection.FORUSER_SAMPLE_ALL_USERS,
                Text = "child2",
            };

            AddMenuSection(_Root, child1, child2);
        }
    }
}
