﻿using System;
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
using KMATutorBot.MessageTexts;

namespace KMATutorBot.Menu.Sections
{
    internal static partial class MenuSectionsGenerator
    {
        private static MenuSection _Root;
        private static List<MenuSection> AllSections = new();
        public static int _SectionMaxId { get; private set; } = 0;
        public static int NextMenuSectionId()
        {
            return _SectionMaxId++;
        }
        
        /// <summary>
        /// Adds children to parent, sets all nesessary fields, adds to all sections
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="children"></param>
        private static void AddMenuSection (MenuSection parent, params MenuSection[] children)
        {
            foreach (var child in children)
            {
                if (child.Text == null)
                    throw new Exception("MenuSection.Text mustn't be null!");

                AllSections.Add(child);
                parent.Children.Add(child);
                child.Root = _Root;
                child.Parent = parent;
            }
        }

        private static void GenerateRoot()
        {
            var root = new MenuSection(true)
            {
                Id = NextMenuSectionId(),
                IsForUser = MenuSection.FORUSER_SAMPLE_ALL_USERS,
                Text = BotMessages.ROOT_MENU_TEXT
            };
            _Root = root;
            AllSections.Add(root);
        }

        public static (MenuSection root, List<MenuSection> allSections) GenerateDefaultMenu()
        {
            GenerateRoot();
            GenerateProfile();
            GenerateFinder();

            return (_Root, AllSections);
        }


        private static KeyboardButton[][] GenerateKeyboard(params string[] buttons)
        {
            if (buttons == null) return null;

            // todo maybe add trim
            var result = buttons
                .Where(btn => !string.IsNullOrEmpty(btn))
                .Distinct()
                .Select(btn => new KeyboardButton[] { new(btn) })
                .ToArray();
            return result;
        }
    }
}
