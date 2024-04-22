using HarmonyLib;
using RimWorld;
using Verse;

namespace RepairingPriority.ListerRepairablesNotifiers;

[HarmonyPatch(typeof(ListerBuildingsRepairable), nameof(ListerBuildingsRepairable.Notify_BuildingRepaired))]
internal class Notify_BuildingRepaired
{
    private static void Postfix(Building b)
    {
        b?.Map?.GetComponent<RepairManager_MapComponent>().MarkNeedToRecalculate();
    }
}