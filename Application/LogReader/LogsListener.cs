using GalaSoft.MvvmLight.Messaging;
using Krankenschwester.Domain.Events;
using Krankenschwester.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Krankenschwester.Application.LogReader
{
    public class LogsListener
    {
        private static string Pattern = @"\]\s[#&]?(<.*\>\s)?:\s(You have entered ){1}(?<zoneName>.*).$";
        private static string PatternHideout = @"^((?!Syndicate).)* Hideout$";
        private static string PatternDied = @"\]\s[#&]?(<.*\>\s)?:\s(.*)\shas been slain.$";
        private static Regex Rgx = new Regex(Pattern);
        private static Regex RgxHideout = new Regex(PatternHideout);
        private static Regex RgxDied = new Regex(PatternDied);

        private Dictionary<string, bool> ZonesWithoutFlask = new Dictionary<string, bool>() {
            {"Lioneye's Watch", true},
            {"The Forest Encampment", true},
            {"The Sarn Encampment", true},
            {"Overseer's Tower", true},
            {"The Bridge Encampment", true},
            {"Highgate", true},
            {"Oriath Docks", true},
            {"Aspirants' Plaza", true},
            {"The Rogue Harbour", true},
            {"Karui Shores", true},
            {"Kingsmarch", true},
            //{"The Menagerie", false}, can't be without flask we have fights here
            //{"Azurite Mine", true},
        };

        private bool zoneWithoutFlask = false;

        public void Read(string line)
        {
            //MatchCollection matchesDied = RgxDied.Matches(line);
            //{
            //    if (matchesDied.Count > 0)
            //    {
            //        zoneWithoutFlask = true;
            //        FlaskUsageHandler.Stop();

            //        return;
            //    }
            //}

            MatchCollection matches = Rgx.Matches(line);
            if (matches.Count > 0)
            {
                string zoneName = matches[0].Groups["zoneName"].Value;

                //Logger.WriteLine("You have entered: " + zoneName + ". Zone Without Flasks: " + ZonesWithoutFlask.ContainsKey(zoneName));
                if (ZonesWithoutFlask.ContainsKey(zoneName) == false && RgxHideout.Matches(zoneName).Count == 0 && zoneWithoutFlask == true)
                {
                    MouseHook.Start();
                    MouseHook.MouseAction += MouseEvent;
                }
                else if (ZonesWithoutFlask.ContainsKey(zoneName) == true || RgxHideout.Matches(zoneName).Count > 0)
                {
                    zoneWithoutFlask = true;
                    Messenger.Default.Send(new EnteredZone(false));
                }
            }
        }

        private void MouseEvent(object sender, MouseClickEvent e)
        {
            Messenger.Default.Send(new EnteredZone(true));

            zoneWithoutFlask = false;
            MouseHook.MouseAction -= MouseEvent;
        }
    }
}
