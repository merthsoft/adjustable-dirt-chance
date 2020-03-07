using System.Linq;
using Verse;

namespace Merthsoft.NoDirt {
    public static class Extensions {
        public static T Look<T>(this string label, T value, T defaultValue = default, bool forceSave = true) {
            Scribe_Values.Look(ref value, label, defaultValue, forceSave);
            return value;
        }

        public static int PercentageSliderTranslate(this Listing_Standard listing, string label, int value, string formatLabel = "value") {
            listing.Label(label.CanTranslate() ? label.Translate(new NamedArgument(value, formatLabel)).Resolve() : label);
            return (int)listing.Slider(value, 0, 100);
        }
    }
}
