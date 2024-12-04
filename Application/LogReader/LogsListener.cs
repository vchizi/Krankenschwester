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
        private static string PatternDied = @"\]\s[#&]?(<.*\>\s)?:\s(?<name>.*)\shas been slain.$";
        private static Regex Rgx = new Regex(Pattern);
        private static Regex RgxHideout = new Regex(PatternHideout);
        private static Regex RgxDied = new Regex(PatternDied);
        private readonly AppSettings settings;
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

        private bool zoneWithoutFlask = true;

        public LogsListener(AppSettings settings)
        {
            this.settings = settings;
        }

        public async void Read(string line)
        {
            Match match = Regex.Match(line, PatternDied);
            if (match.Success && settings.Main.StopOnDeath)
            {
                if (settings.GetActivePreset()?.InGameName.Trim() != match.Groups["name"].Value.Trim())
                {
                    return;
                }

                TmpLogger.WriteLine(match.Groups["name"].Value + " has died. Stopping processes");

                zoneWithoutFlask = true;
                Messenger.Default.Send(new EnteredZone(false));

                return;
            }

            MatchCollection matches = Rgx.Matches(line);
            if (matches.Count > 0)
            {
                if (zoneWithoutFlask) Messenger.Default.Send(new EnteredZone(false));

                string zoneName = matches[0].Groups["zoneName"].Value.Trim('.');

                if (ZonesWithoutFlask.ContainsKey(zoneName) == false && RgxHideout.Matches(zoneName).Count == 0 && zoneWithoutFlask == true)
                {
                    await Task.Run(() => InitializeMouse());

                    TmpLogger.WriteLine(zoneName + " Zone with flasks");
                }
                else if (ZonesWithoutFlask.ContainsKey(zoneName) == true || RgxHideout.Matches(zoneName).Count > 0)
                {
                    TmpLogger.WriteLine(zoneName + " Zone without flasks");

                    zoneWithoutFlask = true;
                    Messenger.Default.Send(new EnteredZone(false));
                }
            }
        }

        public bool ZoneFound(string line)
        {
            MatchCollection matches = Rgx.Matches(line);
            if (matches.Count > 0)
            {
                return true;
            }

            return false;
        }

        private void InitializeMouse()
        {
            MouseHook.Start();
            MouseHook.MouseAction += MouseEvent;
        }

        private void MouseEvent(object sender, MouseClickEvent e)
        {
            Messenger.Default.Send(new EnteredZone(true));

            zoneWithoutFlask = false;
            MouseHook.MouseAction -= MouseEvent;
            MouseHook.Stop();
        }
    }
}
