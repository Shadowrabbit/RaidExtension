// ******************************************************************
//       /\ /|       @file       JobDriverTakeWoodExit.cs
//       \ V/        @brief      行为节点具体驱动 带着木头离开
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-07-11 13:23:39
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using Verse;
using Verse.AI;

namespace SR.ModRimWorld.RaidExtension
{
    [UsedImplicitly]
    public class JobDriverTakeWoodExit : JobDriver_TakeAndExitMap
    {
        /// <summary>
        /// 工作前预定
        /// </summary>
        /// <param name="errorOnFailed"></param>
        /// <returns></returns>
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            //无法预留
            if (!pawn.Reserve(job.GetTarget(TargetIndex.A), job, 1, -1, null, errorOnFailed))
            {
                return false;
            }

            pawn.ReserveAsManyAsPossible(job.GetTargetQueue(TargetIndex.A), job);
            return true;
        }

        /// <summary>
        /// 具体步骤
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            //走过去
            var toilGoto = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch)
                .FailOnSomeonePhysicallyInteracting(TargetIndex.A);
            //搬运
            var toilStartCarry = Toils_Haul.StartCarryThing(TargetIndex.A);
            //继续搬运
            var toilJumpIfAlsoCollectingNextTarget =
                Toils_Haul.JumpIfAlsoCollectingNextTargetInQueue(toilGoto, TargetIndex.A);
            //走到地图边缘
            var toilGotoEdge = Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);
            toilGotoEdge.AddPreTickAction(delegate
            {
                if (Map.exitMapGrid.IsExitCell(pawn.Position))
                {
                    pawn.ExitMap(true, CellRect.WholeMap(Map).GetClosestEdge(pawn.Position));
                }
            });
            //离开地图
            var toilExitMap = new Toil
            {
                initAction = delegate
                {
                    if (pawn.Position.OnEdge(pawn.Map) ||
                        pawn.Map.exitMapGrid.IsExitCell(pawn.Position))
                    {
                        pawn.ExitMap(true, CellRect.WholeMap(Map).GetClosestEdge(pawn.Position));
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return toilGoto;
            yield return toilStartCarry;
            yield return toilJumpIfAlsoCollectingNextTarget;
            yield return toilGotoEdge;
            yield return toilExitMap;
        }
    }
}