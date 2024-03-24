using System.Linq;
using HarmonyLib;
using Verse;

namespace Merthsoft.NoDirt;

[HarmonyPatch(typeof(GenSpawn), "Spawn", [typeof(Thing), typeof(IntVec3), typeof(Map), typeof(Rot4), typeof(WipeMode), typeof(bool), typeof(bool)])]
internal class Verse_GenSpawn_Spawn_Patch
{
    public static bool Prefix(ref Thing newThing, ref IntVec3 loc, ref Map map)
    {
        if (!newThing.def.IsFilth)
            return true;
        var roll = Rand.Range(0, 100);
        var isHome = map.areaManager.Home.ActiveCells.Contains(loc);
        var chance = NoDirt.GetChance(newThing.def.defName, isHome);

        return roll < chance;
    }
}
