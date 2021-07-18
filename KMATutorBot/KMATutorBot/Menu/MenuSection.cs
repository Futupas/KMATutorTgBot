using KMATutorBot.Models;
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
        public MenuSection Parent { get; init; }
        public MenuSection Root { get; init; }

        public bool ForStudents { get; init; } = true;
        public bool ForTeachers { get; init; } = true;
        public bool ForAdmins { get; init; } = false;

        public bool HasLogic { get; init; } = false;
        //todo add logic handler

        public MenuSection (bool isRoot = false)
        {
            if (isRoot)
            {
                this.Parent = this;
                this.Root = this;
            }
        }
        public bool IsRoot
        {
            get
            {
                return this.Root == this;
            }
        }

        public string[] GetSubMenus(BotUser user)
        {
            return Children
                .Where(el => el.ForAdmins == user.IsAdmin && el.ForStudents == (user.StudentCategories != null) && el.ForTeachers == (user.TeacherCategories != null))
                .Select(el => el.Text)
                .Concat(new[] { BACK_TEXT, BACK_TO_START_TEXT }).ToArray();
            //todo don't show back text and backtoMenu text in some cases
        }

        public MenuSection NextMenuSection(BotUser user, string text)
        {
            //todo if has logic...
            if (text == BACK_TEXT) return this.IsRoot ? this : this.Parent;
            if (text == BACK_TO_START_TEXT) return this.IsRoot ? this : this.Root;
            var children = Children
                .Where(el => el.ForAdmins == user.IsAdmin && el.ForStudents == (user.StudentCategories != null) && el.ForTeachers == (user.TeacherCategories != null) && el.Text == text);
            return children.Any() ? children.First() : null;
        }

        public static (MenuSection root, List<MenuSection> allSections) GenerateDefaultMenu()
        {
            var allSections = new List<MenuSection>();
            var root = new MenuSection(true)
            {
                Id = 0,
                Children = new MenuSection[2],
                ForAdmins = true,
                ForStudents = true,
                ForTeachers = true,
                Text = "main menu",
            };
            allSections.Add(root);

            var child1 = new MenuSection()
            {
                Id = 1,
                Children = new MenuSection[0],
                ForAdmins = true,
                ForStudents = true,
                ForTeachers = true,
                Text = "child1",
                Root = root,
                Parent = root,
            };
            allSections.Add(child1);

            var child2 = new MenuSection()
            {
                Id = 2,
                Children = new MenuSection[0],
                ForAdmins = true,
                ForStudents = true,
                ForTeachers = true,
                Text = "child2",
                Root = root,
                Parent = root,
            };
            allSections.Add(child2);

            root.Children[0] = child1;
            root.Children[1] = child2;

            return (root, allSections);
        }
    }
}
