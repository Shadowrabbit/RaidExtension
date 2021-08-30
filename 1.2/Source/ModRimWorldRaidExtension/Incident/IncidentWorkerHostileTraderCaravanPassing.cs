// ******************************************************************
//       /\ /|       @file       IncidentWorkerHostileTraderCaravanPassing.cs
//       \ V/        @brief      敌对商队经过
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-07-11 21:00:39
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace SR.ModRimWorld.RaidExtension
{
    [UsedImplicitly]
    public class IncidentWorkerHostileTraderCaravanPassing : IncidentWorker_TraderCaravanArrival
    {
        /// <summary>
        /// 派系选中条件
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
                   f.HostileTo(Faction.OfPlayer) && f.def.pawnGroupMakers != null &&
                   f.def.pawnGroupMakers.Any(x => x.kindDef == PawnGroupKindDef) &&
                   f.def.caravanTraderKinds.Count != 0 &&
                   f.def.caravanTraderKinds.Any(t => TraderKindCommonality(t, map, f) > 0f);
        }

        /// <summary>
        /// 事件执行
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            var map = (Map) parms.target;
            if (!TryResolveParms(parms))
            {
                return false;
            }

            if (!parms.faction.HostileTo(Faction.OfPlayer))
            {
                return false;
            }

            if (!RCellFinder.TryFindTravelDestFrom(parms.spawnCenter, map, out var travelDest))
            {
                Log.Warning(
                    $"{MiscDef.LogTag}Failed to do hostile trader caravan Passing incident from {parms.spawnCenter} : Couldn't find anywhere for the traveler to go.");
                return false;
            }

            var list = SpawnPawns(parms);
            if (list.Count == 0)
            {
                return false;
            }

            foreach (var t in list.Where(t => t.needs?.food != null))
            {
                t.needs.food.CurLevel = t.needs.food.MaxLevel;
            }

            var traderKind = (from pawn in list where pawn.TraderKind != null select pawn.TraderKind).FirstOrDefault();
            SendLetter(parms, list, traderKind);
            var jobTravelAndExit = new LordJobHostileTraderCaravanTravelAndExit(travelDest);
            LordMaker.MakeNewLord(parms.faction, jobTravelAndExit, map, list);
            return true;
        }

        /// <summary>
        /// 发送信件
        /// </summary>
        /// <param name="parms"></param>
        /// <param name="pawns"></param>
        /// <param name="traderKind"></param>
        protected override void SendLetter(IncidentParms parms, List<Pawn> pawns, TraderKindDef traderKind)
        {
            var letterLabel = "SrHostileTraderCaravanPassing"
                .Translate((NamedArgument) parms.faction.Name, (NamedArgument) traderKind.label).CapitalizeFirst();
            var letterText = "LetterTraderCaravanArrival"
                .Translate(parms.faction.NameColored, (NamedArgument) traderKind.label).CapitalizeFirst();
            PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter(pawns, ref letterLabel, ref letterText,
                "LetterRelatedPawnsNeutralGroup".Translate((NamedArgument) Faction.OfPlayer.def.pawnsPlural), true);
            SendStandardLetter(letterLabel, letterText, LetterDefOf.NegativeEvent, parms, pawns[0]);
        }

        /// <summary>
        /// 解决点数
        /// </summary>
        /// <param name="parms"></param>
        protected override void ResolveParmsPoints(IncidentParms parms)
        {
            parms.points = TraderCaravanUtility.GenerateGuardPoints() * 2;
        }
    }
}