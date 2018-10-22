using Harmony;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Merthsoft.NoDirt {
    [HarmonyPatch(typeof(GenSpawn), "Spawn", 
        new[] { typeof(Thing), typeof(IntVec3), typeof(Map), typeof(Rot4), typeof(WipeMode), typeof(bool) })]
    class GenSpawn_Spawn_Patch {
        public static bool Prefix(ref Thing newThing, ref IntVec3 loc, ref Map map) {
            if (!newThing.def.IsFilth) { return true; }

            var roll = Rand.Range(0, 100);
            var isHome = map.areaManager.Home.ActiveCells.Contains(loc);
            var chance = NoDirt.GetChance(newThing.def.defName, isHome);

            return roll < chance;
        }
    }
}
