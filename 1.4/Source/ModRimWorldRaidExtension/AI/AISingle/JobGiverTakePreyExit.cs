// ******************************************************************
//       /\ /|       @file       JobGiverTakePreyExit.cs
//       \ V/        @brief      行为节点 带着猎物离开
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-19 12:01:24
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace SR.ModRimWorld.RaidExtension
{
    [UsedImplicitly]
    public class JobGiverTakePreyExit : ThinkNode_JobGiver
    {
        private const float MaxSearchDist = 99f; //最大触发距离

        protected override Job TryGiveJob(Pawn pawn)
        {
            //需要找到最合适的逃生出口
            if (!RCellFinder.TryFindBestExitSpot(pawn, out var spot))
            {
                return null;
            }

            //验证器 搜索者不存在 或者搜索者可以预留当前物体 并且没有禁用 并且物体可以被偷 并且物体没在燃烧中 并且物品周围有敌对派系尸体
            bool Validator(Thing t) => t is Corpse corpseAnimal && corpseAnimal.InnerPawn.RaceProps != null &&
                                       corpseAnimal.InnerPawn.RaceProps.IsFlesh &&
                                       corpseAnimal.InnerPawn.RaceProps.Animal;

            //寻找身边合适的战利品
            var spoils = pawn.TryFindBestSpoilsToTake(pawn.Position, pawn.Map, MaxSearchDist, null, Validator);
            if (spoils == null)
            {
                return null;
            }

            var job = JobMaker.MakeJob(RimWorld.JobDefOf.Steal);
            job.targetA = spoils;
            job.targetB = spot;
            job.count = Mathf.Min(spoils.stackCount,
                (int) (pawn.GetStatValue(StatDefOf.CarryingCapacity) / spoils.def.VolumePerUnit));
            return job;
        }
    }
}