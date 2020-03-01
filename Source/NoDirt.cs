using HarmonyLib;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace Merthsoft.NoDirt {
    public class NoDirt : Mod {
        public static string HarmonyId => "com.Merthsoft.NoDirt";

        public static NoDirtSettings Settings;
        public static Harmony Harmony;

        Vector2 scrollPosition = Vector2.zero;

        public NoDirt(ModContentPack content) : base(content) {
            Settings = GetSettings<NoDirtSettings>();
            Harmony = new Harmony(HarmonyId);
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override string SettingsCategory() => TranslationKeys.SettingsCategory.Translate();
        public static int GetChance(string defName, bool isHome) => Settings.GetMapping(defName, isHome);
        
        public override void DoSettingsWindowContents(Rect inRect) {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);

            listing_Standard.Label(TranslationKeys.Explanation.Translate());

            listing_Standard.PercentageSliderTranslate(TranslationKeys.SettingsInHome, ref Settings.DefaultInsideHomeAreaPercentageChange);
            listing_Standard.PercentageSliderTranslate(TranslationKeys.SettingsOutsideHome, ref Settings.DefaultOutsideHomeAreaPercentageChange);

            if (listing_Standard.ButtonText("Add filth type")) {
                var menuItems =
                    DefDatabase<ThingDef>.AllDefs
                        .Where(t => t.IsFilth)
                        .Select(t => t.defName)
                        .Where(s => !Settings.Any(t => s == t.FilthDefName))
                        .OrderBy(s => s)
                        .Select(s => new FloatMenuOption(s, () => Settings.PopulateFilthType(s))).ToList();

                var floatMenu = new FloatMenu(menuItems);
                Find.WindowStack.Add(floatMenu);
            }

            if (Settings.Any()) {
                var rect = new Rect(inRect.x, inRect.y + 175f, inRect.width, inRect.height - 85);
                var viewRect = new Rect(0, 0, inRect.width - 16, Settings.Count() * 120);
                listing_Standard.BeginScrollView(rect, ref scrollPosition, ref viewRect);

                foreach (var setting in Settings) {
                    listing_Standard.GapLine();
                    listing_Standard.Label(TranslationKeys.SettingsFilth.Translate(setting.FilthDefName));
                    var inHomeArea = setting.PercentChanceInsideHomeArea;
                    var outsideHomeArea = setting.PercentChanceOutsideHomeArea;

                    listing_Standard.PercentageSliderTranslate(TranslationKeys.SettingsInHome, ref inHomeArea);
                    listing_Standard.PercentageSliderTranslate(TranslationKeys.SettingsOutsideHome, ref outsideHomeArea);

                    Settings.SetMapping(setting.FilthDefName, inHomeArea, outsideHomeArea);

                    setting.Delete = listing_Standard.ButtonText("Delete");
                }

                listing_Standard.EndScrollView(ref viewRect);
            }

            Settings.RemoveDeleted();
            Settings.Write();
            listing_Standard.End();
        }
    }
}
