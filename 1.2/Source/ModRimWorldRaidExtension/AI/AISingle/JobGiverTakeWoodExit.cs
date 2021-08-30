// ******************************************************************
//       /\ /|       @file       JobGiverTakeWoodExit.cs
//       \ V/        @brief      行为节点 带着木材离开
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-19 12:01:01
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
    public class JobGiverTakeWoodExit : ThinkNode_JobGiver
    {
        private const float MaxSearchDist = 50f; //最大触发距离
        private const float StackRadius = 40f; //堆叠搜索半径

        protected override Job TryGiveJob(Pawn pawn)
        {
            //需要找到最合适的逃生出口
            if (!RCellFinder.TryFindBestExitSpot(pawn, out var spot))
            {
                return null;
            }

            //验证器 木质的 可堆叠的 不限制为原版木头
            bool Validator(Thing t) => t.def.thingCategories != null &&
                                       t.def.thingCategories.Contains(ThingCategoryDefOf.ResourcesRaw) &&
                                       t.def.stuffProps?.categories != null &&
                                       t.def.stuffProps.categories.Contains(StuffCategoryDefOf.Woody);

            //寻找身边合适的战利品
            var spoils = pawn.TryFindBestSpoilsToTake(pawn.Position, pawn.Map, MaxSearchDist, null, Validator);
            //找不到
            if (spoils == null || GenAI.InDangerousCombat(pawn))
            {
                return null;
            }

            var job = JobMaker.MakeJob(JobDefOf.SrTakeWoodExit);
            job.targetA = spoils;
            job.targetB = spot;
            job.count = Mathf.Min(spoils.stackCount,
                (int) (pawn.GetStatValue(StatDefOf.CarryingCapacity) / spoils.def.VolumePerUnit));
            //附近其他可堆叠的木材 一起带走
            foreach (var otherSpoils in GenRadial.RadialDistinctThingsAround(spoils.Position, spoils.Map, StackRadius,
                true))
            {
                //其他战利品也是木材 并且不是当前准备搬运的 并且可以保留
                if (otherSpoils?.def == spoils.def && otherSpoils != spoils && pawn.CanReserve(otherSpoils))
                {
                    job.GetTargetQueue(TargetIndex.A).Add(otherSpoils);
                }
            }

            return job;
        }
    }
}