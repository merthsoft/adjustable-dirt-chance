using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Merthsoft.NoDirt;

public class NoDirtSettings : ModSettings, IEnumerable<FilthSetting>
{
    private readonly Dictionary<string, FilthSetting> filthMappings = new();

    private int DefaultInsideHomeAreaPercentageChance = 0;
    private int DefaultOutsideHomeAreaPercentageChance = 0;

    private Vector2 scrollPosition = Vector2.zero;

    public (int insideHome, int outsideHome) DefaultPercentChances
    {
        set => (DefaultInsideHomeAreaPercentageChance, DefaultOutsideHomeAreaPercentageChance) = value;
    }

    public FilthSetting PopulateFilthType(string type) 
        => filthMappings[type] = new FilthSetting(type, DefaultInsideHomeAreaPercentageChance, DefaultOutsideHomeAreaPercentageChance);

    public void DoSettingsWindowContext(Rect inRect)
    {
        var labelRect = new Rect(inRect);
        Widgets.LabelCacheHeight(ref labelRect, TranslationKeys.Explanation.Translate());
        inRect.y += labelRect.height;

        DefaultPercentChances = (
            PercentageSliderTranslate(ref inRect, TranslationKeys.InsideHomeAreaValue, DefaultInsideHomeAreaPercentageChance),
            PercentageSliderTranslate(ref inRect, TranslationKeys.OutsideHomeAreaValue, DefaultOutsideHomeAreaPercentageChance)
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
            if (ButtonText(ref inRect, TranslationKeys.Add))
                Find.WindowStack.Add(new FloatMenu(menuItems));
        }
        else
        {
            Widgets.Label(inRect, TranslationKeys.AllFilthTypesAdded.Translate());
            inRect.y += Text.LineHeight;
            Widgets.DrawLineHorizontal(inRect.x + 10, inRect.y, inRect.width - 10);
            inRect.y += 5;
        }

        if (this.Any())
        {
            var outRect = inRect
                            .AddWidth(-16)
                            .AddHeight(-inRect.y);

            var viewRect = new Rect(0, 0, outRect.width - 16, this.Count() * 165);
            Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);

            var index = 0;
            foreach (var setting in this)
            {
                setting.DoSubWindowContents(ref viewRect);
                if (index < filthMappings.Count - 1)
                {
                    Widgets.DrawLineHorizontal(viewRect.x + 10, viewRect.y + 2, viewRect.width - 10);
                    viewRect.y += 5;
                }
                index++;
            }

            Widgets.EndScrollView();
        }

        RemoveDeleted();
        Write();
    }

    public static bool ButtonText(ref Rect inRect, string label)
    {
        var buttonLabel = label.AttemptTranslate();
        var buttonRect = new Rect(inRect.x, inRect.y, Text.CalcSize(buttonLabel).x + 20, 30);
        var button = Widgets.ButtonText(buttonRect, buttonLabel);
        inRect.y += 30;

        return button;
    }

    public static int PercentageSliderTranslate(ref Rect inRect, string label, int value, string formatLabel = "value")
    {            
        var rect = new Rect(inRect).WithHeight(Text.LineHeight);
        inRect.y += 2 * Text.LineHeight + 10;

        Widgets.Label(rect, label.AttemptTranslate(value, formatLabel));
        rect.y += Text.LineHeight;
        return (int)Widgets.HorizontalSlider(rect, value, 0, 100);
    }

    public override void ExposeData()
    {
        base.ExposeData();

        Scribe_Values.Look(ref DefaultInsideHomeAreaPercentageChance, "default_inHome", 0, true);
        Scribe_Values.Look(ref DefaultOutsideHomeAreaPercentageChance, "default_outHome", 0, true);

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
            return inHome ? DefaultInsideHomeAreaPercentageChance : DefaultOutsideHomeAreaPercentageChance;

        var mapping = filthMappings[areaName];
        return inHome ? mapping.PercentChanceInsideHomeArea : mapping.PercentChanceOutsideHomeArea;
    }

    public IEnumerator<FilthSetting> GetEnumerator() 
        => filthMappings.OrderBy(s => s.Key).Select(s => s.Value).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() 
        => GetEnumerator();

    public int RemoveDeleted() 
        => filthMappings.RemoveAll(s => s.Value.Delete);

    public void Clear()
        => filthMappings.Clear();
}
