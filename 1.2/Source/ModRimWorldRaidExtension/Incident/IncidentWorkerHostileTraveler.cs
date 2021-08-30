// ******************************************************************
//       /\ /|       @file       IncidentWorkerHostileTraveler.cs
//       \ V/        @brief      事件 敌对旅行者
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-07-11 18:39:50
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace SR.ModRimWorld.RaidExtension
{
    [UsedImplicitly]
    public class IncidentWorkerHostileTraveler : IncidentWorker_TravelerGroup
    {
        protected override PawnGroupKindDef PawnGroupKindDef => PawnGroupKindDefOf.Combat;

        /// <summary>
        /// 派系可以成为组
        /// </summary>
        /// <param name="f"></param>
        /// <param name="map"></param>
        /// <param name="desperate"></param>
        /// <returns></returns>
        protected override bool FactionCanBeGroupSource(Faction f, Map map, bool desperate = false)
        {
            return !f.IsPlayer && !f.defeated && !f.temporary &&
                   (desperate || f.def.allowedArrivalTemperatureRange.Includes(map.mapTemperature.OutdoorTemp) &&
                       f.def.allowedArrivalTemperatureRange.Includes(map.mapTemperature.SeasonalTemp)) && !f.Hidden &&
                   f.HostileTo(Faction.OfPlayer) &&
                   f.def.pawnGroupMakers != null && f.def.pawnGroupMakers.Any(x => x.kindDef == PawnGroupKindDef);
        }
    }
}