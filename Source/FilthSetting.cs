using System;

namespace Merthsoft.NoDirt {
    public class FilthSetting {
        public string FilthDefName { get; }

        public int PercentChanceInsideHomeArea { get; set; }
        public int PercentChanceOutsideHomeArea { get; set; }
        public bool Delete { get; set; }

        public FilthSetting(string filthDefName) {
            FilthDefName = filthDefName;

            PercentChanceInsideHomeArea = NoDirt.Settings?.DefaultInsideHomeAreaPercentageChange ?? 0;
            PercentChanceOutsideHomeArea = NoDirt.Settings?.DefaultOutsideHomeAreaPercentageChange ?? 0;
        }
    }
}
