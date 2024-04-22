using HarmonyLib;
using Verse;

namespace RepairingPriority.ListerRepairablesNotifiers;

[HarmonyPatch(typeof(AreaManager), "NotifyEveryoneAreaRemoved")]
internal class AreaRemoved
{
    private static void Postfix(Map ___map, Area area)
    {
        ___map?.GetComponent<RepairManager_MapComponent>()?.OnAreaDeleted(area);
    }
}