using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace RepairingPriority;

internal class WorkGiver_RepairPrioritized : WorkGiver_Repair
{
    public override int MaxRegionsToScanBeforeGlobalSearch => 4;

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
        return pawn.Map.GetComponent<RepairManager_MapComponent>().RepairableBuildingsInAnyArea();
    }

    public override bool ShouldSkip(Pawn pawn, bool forced = false)
    {
        return !pawn.Map.GetComponent<RepairManager_MapComponent>().RepairableBuildingsInAnyArea().Any();
    }

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (!RepairUtility.PawnCanRepairNow(pawn, t))
        {
            return false;
        }

        if (t is not Building building)
        {
            return false;
        }

        if (!pawn.Map.GetComponent<RepairManager_MapComponent>().RepairableBuildingsInAnyArea().Contains(t))
        {
            return false;
        }

        Area effectiveAreaRestriction = null;
        if (pawn.playerSettings?.EffectiveAreaRestrictionInPawnCurrentMap is { TrueCount: > 0 } &&
            pawn.playerSettings.EffectiveAreaRestrictionInPawnCurrentMap.Map == t.Map)
        {
            effectiveAreaRestriction = pawn.playerSettings.EffectiveAreaRestrictionInPawnCurrentMap;
        }

        if (!pawn.Map.GetComponent<RepairManager_MapComponent>().ThingIsInPriorityAreaSafe(t) &&
            (!forced || !pawn.Map.GetComponent<RepairManager_MapComponent>().ThingIsInRepairingArea(t)) &&
            (effectiveAreaRestriction == null || !effectiveAreaRestriction[t.Position]))
        {
            return false;
        }

        if (!pawn.CanReserve(building, 1, -1, null, forced))
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
}