using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Merthsoft.NoDirt
{
    public class NoDirtSettings : ModSettings, IEnumerable<FilthSetting>
    {
        private readonly Dictionary<string, FilthSetting> filthMappings = new();

        private int DefaultInsideHomeAreaPercentageChange = 0;
        private int DefaultOutsideHomeAreaPercentageChange = 0;

        private Vector2 scrollPosition = Vector2.zero;

        public (int insideHome, int outsideHome) DefaultPercentChanges
        {
            set => (DefaultInsideHomeAreaPercentageChange, DefaultOutsideHomeAreaPercentageChange) = value;
        }

        public FilthSetting PopulateFilthType(string type) 
            => filthMappings[type] = new FilthSetting(type, DefaultInsideHomeAreaPercentageChange, DefaultOutsideHomeAreaPercentageChange);

        public void DoSettingsWindowContext(Rect inRect)
        {
            var listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.Label(TranslationKeys.Explanation.Translate());

            DefaultPercentChanges = (
                listing.PercentageSliderTranslate(TranslationKeys.InsideHomeAreaValue, DefaultInsideHomeAreaPercentageChange),
                listing.PercentageSliderTranslate(TranslationKeys.OutsideHomeAreaValue, DefaultOutsideHomeAreaPercentageChange)
            );

            var menuItems =
                DefDatabase<ThingDef>.AllDefs
                    .Where(t => t.IsFilth)
                    .Select(t => t.defName)
                    .Where(s => !this.Any(t => s == t.FilthDefName))
                    .OrderBy(s => s)
                    .Select(s => new FloatMenuOption(s, () => PopulateFilthType(s))).ToList();

            if (menuItems.Count > 0)
            {
                if (listing.ButtonText(TranslationKeys.Add.Translate()))
                {
                    var floatMenu = new FloatMenu(menuItems);
                    Find.WindowStack.Add(floatMenu);
                }
            }
            else
            {
                listing.Label(TranslationKeys.AllFilthTypesAdded.Translate());
                listing.GapLine();
            }

            if (this.Any())
            {
                var rect = new Rect(inRect.x, inRect.y + 175f, inRect.width, inRect.height - 255);
                var viewRect = new Rect(0, 0, inRect.width - 16, this.Count() * 165);
                listing.BeginScrollView(rect, ref scrollPosition, ref viewRect);

                foreach (var setting in this)
                {
                    setting.DoSubWindowContents(listing);
                    listing.GapLine();
                }

                listing.EndScrollView(ref viewRect);
            }

            RemoveDeleted();
            Write();
            listing.End();
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref DefaultInsideHomeAreaPercentageChange, "default_inHome", 0, true);
            Scribe_Values.Look(ref DefaultOutsideHomeAreaPercentageChange, "default_outHome", 0, true);

            var filthNames = filthMappings.Any() ? string.Join("|", filthMappings.Keys.ToArray()) : string.Empty;
            Scribe_Values.Look(ref filthNames, "filth_names");
            var loadedNames = filthNames?.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var filthName in loadedNames)
            {
                if (!filthMappings.ContainsKey(filthName))
                    PopulateFilthType(filthName);

                var setting = filthMappings[filthName];
                setting.ExposeData();
            }
        }

        public void SetMapping(string areaName, int inHome, int outHome)
        {
            if (!filthMappings.ContainsKey(areaName))
                PopulateFilthType(areaName);

            filthMappings[areaName].PercentChances = (inHome, outHome);
        }

        public int GetMapping(string areaName, bool inHome)
        {
            if (!filthMappings.ContainsKey(areaName))
                return inHome ? DefaultInsideHomeAreaPercentageChange : DefaultOutsideHomeAreaPercentageChange;

            var mapping = filthMappings[areaName];
            return inHome ? mapping.PercentChanceInsideHomeArea : mapping.PercentChanceOutsideHomeArea;
        }

        public IEnumerator<FilthSetting> GetEnumerator() 
            => filthMappings.OrderBy(s => s.Key).Select(s => s.Value).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        public int RemoveDeleted() 
            => filthMappings.RemoveAll(s => s.Value.Delete);
    }
}
