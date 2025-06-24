using HarmonyLib;
using Verse;

namespace RepairingPriority.ListerRepairablesNotifiers;

[HarmonyPatch(typeof(Area), nameof(Area.Invert))]
internal class Area_Invert
{
    private static void Prefix(Area __instance)
    {
        __instance.Map.GetComponent<RepairManager_MapComponent>().MarkNeedToRecalculate();
    }
}