using HarmonyLib;
using RimWorld;
using Verse;

namespace RepairingPriority.ListerRepairablesNotifiers;

[HarmonyPatch(typeof(BreakdownManager), nameof(BreakdownManager.Notify_BrokenDown))]
internal class BreakdownManager_Notify_BrokenDown
{
    private static void Postfix(Thing thing)
    {
        if (thing is Building b)
        {
            b.Map?.GetComponent<RepairManager_MapComponent>().MarkNeedToRecalculate();
        }
    }
}