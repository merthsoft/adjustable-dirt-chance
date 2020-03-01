using Verse;

namespace Merthsoft.NoDirt {
    public static class Extensions {
        public static void PercentageSliderTranslate(this Listing_Standard listing, string label, ref int value)
            => listing.PercentageSlider(label.Translate(value), ref value);

        public static void PercentageSlider(this Listing_Standard listing, string label, ref int value) {
            listing.Label(label);
            value = (int)listing.Slider(value, 0, 100);
        }
    }
}
