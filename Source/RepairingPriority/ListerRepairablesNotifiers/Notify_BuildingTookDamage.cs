using HarmonyLib;
using RimWorld;
using Verse;

namespace RepairingPriority.ListerRepairablesNotifiers;

[HarmonyPatch(typeof(ListerBuildingsRepairable), nameof(ListerBuildingsRepairable.Notify_BuildingTookDamage))]
internal class Notify_BuildingTookDamage
{
    private static void Postfix(Building b)
    {
        b?.Map?.GetComponent<RepairManager_MapComponent>().MarkNeedToRecalculate();
    }
}