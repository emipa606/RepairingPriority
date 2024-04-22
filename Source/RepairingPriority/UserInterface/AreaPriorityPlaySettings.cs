using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RepairingPriority.UserInterface;

[HarmonyPatch(typeof(PlaySettings), nameof(PlaySettings.DoPlaySettingsGlobalControls))]
internal class AreaPriorityPlaySettings
{
    public static bool showingPrioritySettings = false;

    private static void Postfix(WidgetRow row, bool worldView)
    {
        if (worldView)
        {
            return;
        }

        var mouseOverRect = new Rect(row.FinalX - WidgetRow.IconSize, row.FinalY, WidgetRow.IconSize,
            WidgetRow.IconSize);
        MouseoverSounds.DoRegion(mouseOverRect, SoundDefOf.Mouseover_ButtonToggle);
        if (Mouse.IsOver(mouseOverRect))
        {
            Find.CurrentMap.GetComponent<RepairManager_MapComponent>().MarkAllForDraw();
        }

        if (!row.ButtonIcon(TextureLoader.priorityWindowButton, "OpenRepairingPriorityDialog".Translate()))
        {
            return;
        }

        if (!Find.WindowStack.IsOpen<Dialog_RepairingPriority>())
        {
            Find.WindowStack.Add(new Dialog_RepairingPriority(Find.CurrentMap));
        }
        else
        {
            Find.WindowStack.TryRemove(typeof(Dialog_RepairingPriority));
        }
    }
}