using System;

namespace Merthsoft.NoDirt {
    public class FilthSetting {
        private readonly string filthDefName;
        public string FilthDefName => filthDefName;

        public int PercentChanceInsideHomeArea { get; set; }
        public int PercentChanceOutsideHomeArea { get; set; }
        public bool Delete { get; set; }

        public FilthSetting(string filthDefName) {
            this.filthDefName = filthDefName;

            PercentChanceInsideHomeArea = NoDirt.Settings?.DefaultInsideHomeAreaPercentageChange ?? 0;
            PercentChanceOutsideHomeArea = NoDirt.Settings?.DefaultOutsideHomeAreaPercentageChange ?? 0;
        }
    }
}
