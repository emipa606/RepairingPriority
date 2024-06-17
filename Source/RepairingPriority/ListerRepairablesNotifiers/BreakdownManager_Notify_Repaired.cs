using HarmonyLib;
using RimWorld;
using Verse;

namespace RepairingPriority.ListerRepairablesNotifiers;

[HarmonyPatch(typeof(BreakdownManager), nameof(BreakdownManager.Notify_Repaired))]
internal class BreakdownManager_Notify_Repaired
{
    private static void Postfix(Thing thing)
    {
        if (thing is Building b)
        {
            b.Map?.GetComponent<RepairManager_MapComponent>().MarkNeedToRecalculate();
        }
    }
}