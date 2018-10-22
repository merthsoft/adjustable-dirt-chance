using Harmony;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace Merthsoft.NoDirt {
    public class NoDirt : Mod {
        const string STR_Namespace = "com.Merthsoft.NoDirt";
        const string STR_Settings_In_Home           = "merthsoft_nodirt_settings_in_home";
        const string STR_Settings_Outside_Home      = "merthsoft_nodirt_settings_outside_home";
        const string STR_Settings_Filth_Desc        = "merthsoft_nodirt_settings_filth_desc";
        const string STR_Settings_Category          = "merthsoft_nodirt_settings_category";

        public static NoDirtSettings Settings;
        public static HarmonyInstance Harmony;

        static IEnumerable<string> allFilthTypes;
        Vector2 scrollPosition = Vector2.zero;

        public NoDirt(ModContentPack content) : base(content) {
            Settings = GetSettings<NoDirtSettings>();
            Harmony = HarmonyInstance.Create(STR_Namespace);
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override string SettingsCategory() => STR_Settings_Category.Translate();
        public static int GetChance(string defName, bool isHome) => Settings.GetMapping(defName, isHome);
        
        public override void DoSettingsWindowContents(Rect inRect) {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);

            var buffer = Settings.DefaultInsideHomeAreaPercentageChange.ToString();
            listing_Standard.TextFieldNumericLabeled<int>("Default inside home area percentage chance ",
                ref Settings.DefaultInsideHomeAreaPercentageChange, ref buffer);

            buffer = Settings.DefaultOutsideHomeAreaPercentageChange.ToString();
            listing_Standard.TextFieldNumericLabeled<int>("Default outside home area percentage chance ",
                ref Settings.DefaultOutsideHomeAreaPercentageChange, ref buffer);

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
                var rect = new Rect(inRect.x, inRect.y + 40f, inRect.width, inRect.height - 85);
                var viewRect = new Rect(0, 0, inRect.width - 16, Settings.Count() * 120);
                listing_Standard.BeginScrollView(rect, ref scrollPosition, ref viewRect);

                foreach (var setting in Settings) {
                    listing_Standard.GapLine();
                    listing_Standard.Label(STR_Settings_Filth_Desc.Translate(setting.FilthDefName));
                    var inHomeArea = setting.PercentChanceInsideHomeArea;
                    var outsideHomeArea = setting.PercentChanceOutsideHomeArea;

                    buffer = inHomeArea.ToString();
                    listing_Standard.TextFieldNumericLabeled(STR_Settings_In_Home.Translate(), ref inHomeArea, ref buffer, 0, 100);

                    buffer = outsideHomeArea.ToString();
                    listing_Standard.TextFieldNumericLabeled(STR_Settings_Outside_Home.Translate(), ref outsideHomeArea, ref buffer, 0, 100);

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
