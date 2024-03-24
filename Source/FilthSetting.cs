using UnityEngine;
using Verse;

namespace Merthsoft.NoDirt;

public class FilthSetting
{
    private const string FilthDefNameArgumentLabel = "filthName";

    public string FilthDefName { get; }

    private string InsideHomeFilthName => FilthDefName + "_inHome";

    private string OutsideHomeFilthName => FilthDefName + "_outHome";

    public int PercentChanceInsideHomeArea { get; private set; }

    public int PercentChanceOutsideHomeArea { get; private set; }

    public bool Delete { get; set; }

    public (int insideHome, int outsideHome) PercentChances
    {
        set => (PercentChanceInsideHomeArea, PercentChanceOutsideHomeArea) = value;
    }

    public FilthSetting(string filthDefName, int defaultInsideHomeAreaPercentageChange, int defaultOutsideHomeAreaPercentageChange)
    {
        FilthDefName = filthDefName;

        PercentChanceInsideHomeArea = defaultInsideHomeAreaPercentageChange;
        PercentChanceOutsideHomeArea = defaultOutsideHomeAreaPercentageChange;
    }

    public void ExposeData()
        => PercentChances = (
            InsideHomeFilthName.Look(PercentChanceInsideHomeArea), 
            OutsideHomeFilthName.Look(PercentChanceOutsideHomeArea)
        );

    public void DoSubWindowContents(ref Rect inRect)
    {
        Widgets.Label(inRect, TranslationKeys.FilthDescription.Translate(new NamedArgument(FilthDefName, FilthDefNameArgumentLabel)));
        inRect.y += Text.LineHeight;
        PercentChances = (
            NoDirtSettings.PercentageSliderTranslate(ref inRect, TranslationKeys.InsideHomeAreaValue, PercentChanceInsideHomeArea),
            NoDirtSettings.PercentageSliderTranslate(ref inRect, TranslationKeys.OutsideHomeAreaValue, PercentChanceOutsideHomeArea)
        );

        Delete = NoDirtSettings.ButtonText(ref inRect, TranslationKeys.Delete);
    }
}
