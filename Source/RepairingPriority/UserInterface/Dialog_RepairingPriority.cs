using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RepairingPriority.UserInterface;

internal class Dialog_RepairingPriority : Window
{
    private const float marginBetweenElements = 6f;
    private const float elementHeight = 24f;
    private const float buttonHeight = 30f;
    private readonly HashSet<Area> addQueue = [];
    private readonly HashSet<Area> insertQueue = [];
    private readonly Map map;

    private readonly HashSet<Area> removeQueue = [];

    private Vector2 scrollPos = new Vector2(0, 0);

    public Dialog_RepairingPriority(Map currentMap)
    {
        map = currentMap;
        doCloseX = true;
    }

    public override Vector2 InitialSize => new Vector2(450f, 400f);

    public override void DoWindowContents(Rect inRect)
    {
        var manager = map.GetComponent<RepairManager_MapComponent>();
        manager.MarkAllForDraw();
        if (addQueue.Any())
        {
            manager.AddAreaRange(addQueue);
            addQueue.Clear();
        }

        if (insertQueue.Any())
        {
            manager.AddAreaRange(insertQueue, true);
            insertQueue.Clear();
        }

        if (removeQueue.Any())
        {
            manager.RemoveAreaRange(removeQueue);
            removeQueue.Clear();
        }

        IEnumerable<Area> addables = manager.AddableAreas;
        var playerCanAdd = addables.Any();
        var listRect = new Rect(0f, 0f, inRect.width - 20f,
            manager.AreaCount * (elementHeight + marginBetweenElements));
        var listHolder = new Rect(inRect.x, inRect.y, inRect.width,
            playerCanAdd ? inRect.height - marginBetweenElements - buttonHeight : inRect.height);
        Widgets.BeginScrollView(listHolder, ref scrollPos, listRect);
        var uiLister = new Listing_Standard();
        uiLister.Begin(listRect);
        uiLister.ColumnWidth = listRect.width;
        uiLister.Gap(marginBetweenElements);
        var switchTuple = new Tuple<int, int>(-1, 0);
        for (var i = 0; i < manager.AreaCount; i++)
        {
            var areaIsPriority = manager.PrioritizedArea == manager[i];
            var result = DoAreaRow(manager[i], uiLister, manager.AreaCount, i, areaIsPriority);
            switch (result)
            {
                case > 0:
                    switchTuple = new Tuple<int, int>(i, i + 1);
                    break;
                case < 0:
                    switchTuple = new Tuple<int, int>(i, i - 1);
                    break;
            }

            uiLister.Gap(marginBetweenElements);
        }

        if (switchTuple.Item1 != -1)
        {
            manager.ReorderPriorities(switchTuple.Item1, switchTuple.Item2);
        }

        uiLister.End();
        Widgets.EndScrollView();
        if (playerCanAdd)
        {
            DoAddRow(listHolder, addables);
        }
    }

    private void DoAddRow(Rect listHolderRect, IEnumerable<Area> addableAreas)
    {
        var buttonRect = new Rect(listHolderRect.x,
            listHolderRect.y + listHolderRect.height + marginBetweenElements, listHolderRect.width, buttonHeight);
        TooltipHandler.TipRegion(buttonRect, "AddAreaForPriorityRepairingTip".Translate());
        if (Widgets.ButtonText(buttonRect.LeftHalf().ContractedBy(1f), "AddAreaForPriorityRepairingFirst".Translate()))
        {
            var menu = MakeAreasFloatMenu(addableAreas);
            if (menu != null)
            {
                Find.WindowStack.Add(menu);
            }
        }

        if (Widgets.ButtonText(buttonRect.RightHalf().ContractedBy(1f), "AddAreaForPriorityRepairingLast".Translate()))
        {
            var menu = MakeAreasFloatMenu(addableAreas, true);
            if (menu != null)
            {
                Find.WindowStack.Add(menu);
            }
        }
    }

    private FloatMenu MakeAreasFloatMenu(IEnumerable<Area> addableAreas, bool last = false)
    {
        var options = new List<FloatMenuOption>();
        foreach (var area in addableAreas)
        {
            options.Add(new FloatMenuOption(area.Label, delegate
            {
                if (last)
                {
                    addQueue.Add(area);
                }
                else
                {
                    insertQueue.Add(area);
                }
            }));
        }

        return options.Count > 0 ? new FloatMenu(options) : null;
    }

    private int DoAreaRow(Area areaToList, Listing_Standard listing, int count, int priority,
        bool isPriority)
    {
        var rowRect = listing.GetRect(elementHeight);
        var returnvalue = 0;

        if (Mouse.IsOver(rowRect))
        {
            GUI.color = areaToList.Color;
            Widgets.DrawHighlightIfMouseover(rowRect);
            GUI.color = Color.white;
        }

        DoAreaTooltip(rowRect, count, priority, isPriority);

        var widgetRow = new WidgetRow(rowRect.x, rowRect.y, UIDirection.RightThenUp, rowRect.width);

        if (count > 1 && priority < count - 1)
        {
            if (widgetRow.ButtonIcon(TexButton.ReorderDown))
            {
                returnvalue = 1;
            }
        }
        else
        {
            widgetRow.ButtonIcon(TexButton.ReorderDown);
        }

        if (count > 1 && priority > 0)
        {
            if (widgetRow.ButtonIcon(TexButton.ReorderUp))
            {
                returnvalue = -1;
            }
        }
        else
        {
            widgetRow.ButtonIcon(TexButton.ReorderUp);
        }

        widgetRow.Icon(areaToList.ColorTexture);
        widgetRow.LabelWithAnchorAndFont(areaToList.Label, -1, TextAnchor.MiddleLeft, GameFont.Small);
        widgetRow.Gap(rowRect.width - (widgetRow.FinalX - rowRect.x) -
                      ((2 * WidgetRow.IconSize) + WidgetRow.DefaultGap));
        if (isPriority)
        {
            widgetRow.Icon(TextureLoader.repair);
        }
        else
        {
            widgetRow.Gap(WidgetRow.IconSize + WidgetRow.DefaultGap);
        }

        if (count > 1 && widgetRow.ButtonIcon(TextureLoader.delete))
        {
            removeQueue.Add(areaToList);
        }

        return returnvalue;
    }

    private void DoAreaTooltip(Rect rowRect, int count, int priority, bool isPrioritized)
    {
        var tooltipString = isPrioritized ? "RepairingAreaIsPrioritized".Translate() : new TaggedString();
        switch (count)
        {
            case 1:
                tooltipString += "OnlyRepairingArea".Translate();
                break;
            case > 1 when priority == 0:
                tooltipString += "RepairingAreaFirst".Translate();
                break;
            case > 1 when priority == count - 1:
                tooltipString += "RepairingAreaLast".Translate();
                break;
            case > 1:
            {
                var manager = map.GetComponent<RepairManager_MapComponent>();
                tooltipString +=
                    "RepairingAreaMiddle".Translate(manager[priority - 1].Label, manager[priority + 1].Label);
                break;
            }
        }

        TooltipHandler.TipRegion(rowRect, tooltipString);
    }
}