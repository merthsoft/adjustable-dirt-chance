using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Merthsoft.NoDirt {
    public class NoDirtSettings : ModSettings, IEnumerable<FilthSetting>{
        public int DefaultInsideHomeAreaPercentageChange = 0;
        public int DefaultOutsideHomeAreaPercentageChange = 0;
        
        Dictionary<string, FilthSetting> filthMappings = new Dictionary<string, FilthSetting>();

        public void PopulateFilthType(string type) {
            filthMappings[type] = new FilthSetting(type);
        }

        public override void ExposeData() {
            base.ExposeData();

            Scribe_Values.Look(ref DefaultInsideHomeAreaPercentageChange, "default_inHome", 0, true);
            Scribe_Values.Look(ref DefaultOutsideHomeAreaPercentageChange, "default_outHome", 0, true);

            var filthNames = filthMappings.Any() ? string.Join("|", filthMappings.Keys.ToArray()) : "";
            Scribe_Values.Look(ref filthNames, "filth_names");
            var loadedNames = filthNames?.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var filthName in loadedNames) {
                if (!filthMappings.ContainsKey(filthName)) {
                    PopulateFilthType(filthName);
                }

                var setting = filthMappings[filthName];
                int inHome = setting.PercentChanceInsideHomeArea;
                int outHome = setting.PercentChanceOutsideHomeArea;

                Scribe_Values.Look(ref inHome, filthName + "_inHome", 0, true);
                Scribe_Values.Look(ref outHome, filthName + "_outHome", 0, true);

                setting.PercentChanceInsideHomeArea = inHome;
                setting.PercentChanceOutsideHomeArea = outHome;
            }
        }

        public void SetMapping(string areaName, int inHome, int outHome) {
            if (!filthMappings.ContainsKey(areaName)) {
                PopulateFilthType(areaName);
            }

            filthMappings[areaName].PercentChanceInsideHomeArea = inHome;
            filthMappings[areaName].PercentChanceOutsideHomeArea = outHome;
        }

        public void SetMapping(string areaName, FilthSetting mapping) {
            SetMapping(areaName, mapping.PercentChanceInsideHomeArea, mapping.PercentChanceOutsideHomeArea);
        }

        public int GetMapping(string areaName, bool inHome) {
            if (!filthMappings.ContainsKey(areaName)) {
                PopulateFilthType(areaName);
            }

            var mapping = filthMappings[areaName];
            return inHome ? mapping.PercentChanceInsideHomeArea : mapping.PercentChanceOutsideHomeArea;
        }

        public FilthSetting GetMapping(string areaName) => filthMappings[areaName];

        public IEnumerator<FilthSetting> GetEnumerator() {
            return filthMappings.OrderBy(s => s.Key).Select(s => s.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        internal void RemoveDeleted() {
            filthMappings.RemoveAll(s => s.Value.Delete);
        }
    }
}
