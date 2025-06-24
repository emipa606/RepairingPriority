using HarmonyLib;
using Verse;

namespace RepairingPriority.ListerRepairablesNotifiers;

[HarmonyPatch(typeof(Area), "Set")]
internal class Area_Set
{
    private static void Postfix(AreaManager ___areaManager)
    {
        ___areaManager?.map?.GetComponent<RepairManager_MapComponent>()?.MarkNeedToRecalculate();
    }
}