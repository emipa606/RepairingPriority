using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace RepairingPriority;

internal class WorkGiver_FixBrokenDownBuildingPrioritized : WorkGiver_FixBrokenDownBuilding
{
    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t is not Building building)
        {
            return false;
        }

        if (!building.def.building.repairable)
        {
            return false;
        }

        if (t.Faction != pawn.Faction)
        {
            return false;
        }

        if (!t.IsBrokenDown())
        {
            return false;
        }

        if (t.IsForbidden(pawn))
        {
            return false;
        }

        if (pawn.Faction == Faction.OfPlayer)
        {
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
        }

        if (!pawn.CanReserve(building, 1, -1, null, forced))
        {
            return false;
        }

        if (pawn.Map.designationManager.DesignationOn(building, DesignationDefOf.Deconstruct) != null)
        {
            return false;
        }

        if (building.IsBurning())
        {
            return false;
        }

        if (FindClosestComponent(pawn) != null)
        {
            return true;
        }

        JobFailReason.Is("NoComponentsToRepair".Translate());
        return false;
    }

    private Thing FindClosestComponent(Pawn pawn)
    {
        return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map,
            ThingRequest.ForDef(ThingDefOf.ComponentIndustrial), PathEndMode.InteractionCell,
            TraverseParms.For(pawn, pawn.NormalMaxDanger()), 9999f, x => !x.IsForbidden(pawn) && pawn.CanReserve(x));
    }
}