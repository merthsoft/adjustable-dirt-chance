using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace Merthsoft.NoDirt;

public class NoDirt : Mod
{
    public static string HarmonyId => "com.Merthsoft.NoDirt";

    public static NoDirtSettings Settings;
    public static Harmony Harmony;

    public NoDirt(ModContentPack content) : base(content)
    {
        Settings = GetSettings<NoDirtSettings>();
        Harmony = new Harmony(HarmonyId);
        Harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    public override string SettingsCategory() 
        => TranslationKeys.SettingsCategory.Translate();

    public static int GetChance(string defName, bool isHome) 
        => Settings.GetMapping(defName, isHome);

    public override void DoSettingsWindowContents(Rect inRect)
        => Settings.DoSettingsWindowContext(inRect);
}
