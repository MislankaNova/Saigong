﻿using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows;
using System.IO;

namespace Saigong
{
    class TextClass
    {
        public string name;
        public string Symbol;
        public Key Key;

        public Style DisplayStyle;
    }

    class TextClasses
    {
        private double PtToPx(double pt)
        {
            return (pt * 96) / 72;
        }

        Dictionary<string, TextClass> Classes;

        public double NormalTextWidth;

        public TextClasses()
        {
            Classes = new Dictionary<string, TextClass>();
            NormalTextWidth = 10;

            foreach (var dir in Directory.GetFiles("Saigong/style/"))
            {
                var rules = ConfigLoader.LoadConfigFile(dir);
                AddTextClass(rules);
            }
        }

        public void AddTextClass
            (
            string name,
            string symbol,
            Key key,
            Style style
            )
        {
            var tc = new TextClass();
            tc.Symbol = symbol;
            tc.Key = key;
            tc.DisplayStyle = style;

            Classes[name] = tc;
        }

        public void AddTextClass(Dictionary<string, string> rules)
        {
            var tc = new TextClass();
            tc.name = rules["name"];
            tc.Symbol = rules["symbol"];
            Enum.TryParse(rules["key"], out tc.Key);

            Style s = new Style
                (
                typeof(Paragraph),
                (Style)App.Current.Resources["BlockBase"]
                );

            s.Setters.Add
                (
                new Setter
                    (
                    Block.FontSizeProperty,
                    PtToPx(Convert.ToDouble(rules["FontSize"]))
                    )
                );

            s.Setters.Add
                (
                new Setter
                    (
                    Block.TextAlignmentProperty,
                    (TextAlignment)Enum.Parse(typeof(TextAlignment), rules["TextAlignment"])
                    )
                );

            s.Setters.Add
                (
                new Setter
                    (
                    Block.ForegroundProperty,
                    (SolidColorBrush)new BrushConverter().ConvertFrom(rules["Foreground"])
                    )
                );
            
            if (rules.ContainsKey("indent"))
            {
                s.Setters.Add
                    (
                    new Setter
                        (
                        Paragraph.TextIndentProperty,
                        PtToPx(Convert.ToDouble(rules["FontSize"]) * 2)
                        )
                    );
            }

            if (rules["name"] == "NormalText")
            {
                App.Current.Resources["NormalText"] = s;
                NormalTextWidth = Convert.ToDouble(rules["FontSize"]);
            }

            tc.DisplayStyle = s;

            Classes[rules["name"]] = tc;
        }

        public TextClass GetTextClassStyleByName(string name)
        {
            if (Classes.ContainsKey(name))
            {
                return Classes[name];
            }
            return null;
        }

        public TextClass GetTextClassStyle(string symbol)
        {
            foreach (var tc in Classes.Values)
            {
                if (tc.Symbol == symbol)
                {
                    return tc;
                }
            }
            return null;
        }

        public TextClass GetTextClassStyle(Key key)
        {
            foreach (var tc in Classes.Values)
            {
                if (tc.Key == key)
                {
                    return tc;
                }
            }
            return null;
        }

        public string[] GetSymbolCollection()
        {
            var symbols = new string[Classes.Count];
            int i = 0;
            foreach (var c in Classes.Values)
            {
                symbols[i] = c.Symbol;
                i++;
            }
            return symbols;
        }

        public Key[] GetKeyCollection()
        {
            var keys = new Key[Classes.Count];
            int i = 0;
            foreach (var c in Classes.Values)
            {
                keys[i] = c.Key;
                i++;
            }
            return keys;
        }
    }
}
