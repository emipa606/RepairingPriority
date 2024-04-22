using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace RepairingPriority;

internal class RepairManager_MapComponent : MapComponent, ICellBoolGiver
{
    private readonly CellBoolDrawer priorityAreasDrawer;

    private List<Area> addableAreas = [];

    private bool needToUpdateAddables = true;
    private bool needToUpdatePrioritized = true;

    private Area prioritizedArea;
    private List<Area> priorityList = [];

    public RepairManager_MapComponent(Map map) : base(map)
    {
        priorityAreasDrawer = new CellBoolDrawer(this, map.Size.x, map.Size.z);
    }

    public int AreaCount => priorityList.Count;

    public Area this[int index] => priorityList[index];

    public bool this[Area area] => priorityList.Contains(area);

    public Area PrioritizedArea
    {
        get
        {
            if (!needToUpdatePrioritized)
            {
                return prioritizedArea;
            }

            ReacalculatePriorityArea();
            needToUpdatePrioritized = false;

            return prioritizedArea;
        }
    }

    public List<Area> PrioritizedAreas => priorityList;

    public List<Area> AddableAreas
    {
        get
        {
            if (!needToUpdateAddables)
            {
                return addableAreas;
            }

            addableAreas = map.areaManager.AllAreas.ToList();
            addableAreas.RemoveAll(x => priorityList.Contains(x));
            needToUpdateAddables = false;

            return addableAreas;
        }
    }

    public Color Color => Color.white;

    public bool GetCellBool(int index)
    {
        foreach (var area in priorityList)
        {
            if (area[index])
            {
                return true;
            }
        }

        return false;
    }

    public Color GetCellExtraColor(int index)
    {
        foreach (var area in priorityList)
        {
            if (area[index])
            {
                return area.Color;
            }
        }

        return Color.clear;
    }

    public override void ExposeData()
    {
        Scribe_Collections.Look(ref priorityList, "RepairingPriority", LookMode.Reference);
        RemoveNullsInList();
        EnsureHasAtLeastOneArea();
    }

    public override void FinalizeInit()
    {
        EnsureHasAtLeastOneArea();
    }

    public override void MapComponentUpdate()
    {
        priorityAreasDrawer.CellBoolDrawerUpdate();
    }

    public void AddAreaRange(IEnumerable<Area> rangeToAdd)
    {
        foreach (var area in rangeToAdd)
        {
            priorityList.Insert(0, area);
        }

        MarkNeedToRecalculate();
        MarkAddablesOutdated();
    }

    public void RemoveAreaRange(IEnumerable<Area> rangeToRemove)
    {
        priorityList.RemoveAll(rangeToRemove.Contains);
        EnsureHasAtLeastOneArea();
        MarkNeedToRecalculate();
        MarkAddablesOutdated();
    }

    public void ReorderPriorities(int from, int to)
    {
        (priorityList[from], priorityList[to]) = (priorityList[to], priorityList[from]);
        MarkNeedToRecalculate();
    }

    public void OnAreaDeleted(Area deletedArea)
    {
        priorityList.Remove(deletedArea);
        EnsureHasAtLeastOneArea();
        MarkAddablesOutdated();
        MarkNeedToRecalculate();
    }

    public void MarkAddablesOutdated()
    {
        needToUpdateAddables = true;
    }

    public IEnumerable<Thing> RepairableBuildingsInAnyArea()
    {
        var potentialBuildings = map.listerBuildingsRepairable.RepairableBuildings(Faction.OfPlayer);
        if (potentialBuildings == null || !potentialBuildings.Any())
        {
            yield break;
        }

        var combinedAreas = new HashSet<IntVec3>(priorityList.SelectMany(area => area.ActiveCells));
        foreach (var building in potentialBuildings)
        {
            if (building is { Spawned: true } &&
                combinedAreas.Contains(building.Position) &&
                CanRepair(building))
            {
                yield return building;
            }
        }
    }

    public void MarkNeedToRecalculate()
    {
        priorityAreasDrawer.SetDirty();
        needToUpdatePrioritized = true;
    }

    public void MarkAllForDraw()
    {
        priorityAreasDrawer.MarkForDraw();
    }

    private void RemoveNullsInList()
    {
        priorityList.RemoveAll(x => x == null);
    }

    private void EnsureHasAtLeastOneArea()
    {
        if (!priorityList.Any())
        {
            AddAreaRange(new List<Area> { map.areaManager.Home });
        }
    }

    private void ReacalculatePriorityArea()
    {
        prioritizedArea = null;
        var potentialBuildings = map.listerBuildingsRepairable.RepairableBuildings(Faction.OfPlayer);
        if (!potentialBuildings.Any())
        {
            return;
        }

        foreach (var area in priorityList)
        {
            if (!potentialBuildings.Any(thing =>
                    area[thing.Position] && CanRepair(thing)))
            {
                continue;
            }

            prioritizedArea = area;
            return;
        }
    }

    private static bool CanRepair(Thing building)
    {
        if (building.MaxHitPoints <= building.HitPoints)
        {
            return false;
        }

        if (building.Map.designationManager.DesignationOn(building, DesignationDefOf.Deconstruct) != null)
        {
            return false;
        }

        if (building.def.mineable &&
            building.Map.designationManager.DesignationAt(building.Position, DesignationDefOf.Mine) != null)
        {
            return false;
        }

        if (building.def.mineable &&
            building.Map.designationManager.DesignationAt(building.Position, DesignationDefOf.MineVein) != null)
        {
            return false;
        }

        return !building.IsBurning();
    }

    public bool ThingIsInPriorityAreaSafe(Thing thing)
    {
        return PrioritizedArea != null && PrioritizedArea[thing.Position];
    }

    public bool ThingIsInRepairingArea(Thing thing)
    {
        foreach (var area in priorityList)
        {
            if (area[thing.Position])
            {
                return true;
            }
        }

        return false;
    }
}