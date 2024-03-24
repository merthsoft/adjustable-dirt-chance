using System.Linq;
using UnityEngine;
using Verse;
using static Verse.TranslatorFormattedStringExtensions;

namespace Merthsoft.NoDirt;

public static class Extensions
{
    public static T Look<T>(this string label, T value, T defaultValue = default, bool forceSave = true)
    {
        Scribe_Values.Look(ref value, label, defaultValue, forceSave);
        return value;
    }

    public static string AttemptTranslate(this string label)
        => label.CanTranslate() ? label.Translate() : label;

    public static string AttemptTranslate(this string label, object arg, string argLabel)
        => label.CanTranslate()
            ? label.Translate(new NamedArgument(arg, argLabel)).Resolve()
            : label;

    public static Rect AddYMin(this Rect t, float val)
    {
        var ret = new Rect(t);
        ret.yMin += val;
        return ret;
    }

    public static Rect AddYMax(this Rect t, float val)
    {
        var ret = new Rect(t);
        ret.yMax += val;
        return ret;
    }

    public static Rect AddWidth(this Rect t, float val)
    {
        var ret = new Rect(t);
        ret.width += val;
        return ret;
    }

    public static Rect AddHeight(this Rect t, float val)
    {
        var ret = new Rect(t);
        ret.height += val;
        return ret;
    }

    public static Rect WithHeight(this Rect t, float val)
        => new(t) { height = val };
}
