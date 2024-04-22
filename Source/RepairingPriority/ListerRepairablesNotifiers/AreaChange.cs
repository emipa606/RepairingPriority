using HarmonyLib;
using Verse;

namespace RepairingPriority.ListerRepairablesNotifiers;

[HarmonyPatch(typeof(Area), "Set")]
internal class AreaChange
{
    private static void Postfix(AreaManager ___areaManager)
    {
        ___areaManager?.map?.GetComponent<RepairManager_MapComponent>()?.MarkNeedToRecalculate();
    }
}