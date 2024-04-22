using HarmonyLib;
using Verse;

namespace RepairingPriority.ListerRepairablesNotifiers;

[HarmonyPatch(typeof(AreaManager), nameof(AreaManager.TryMakeNewAllowed))]
internal class AreaAdded
{
    private static void Postfix(Map ___map, bool __result)
    {
        if (!__result)
        {
            return;
        }

        ___map?.GetComponent<RepairManager_MapComponent>()?.MarkAddablesOutdated();
    }
}