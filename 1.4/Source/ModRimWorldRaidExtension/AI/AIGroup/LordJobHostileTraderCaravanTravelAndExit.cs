// ******************************************************************
//       /\ /|       @file       LordJobHostileTraderCaravanTravelAndExit.cs
//       \ V/        @brief      集群AI 敌对商队旅行和离开
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-07-11 22:42:10
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using JetBrains.Annotations;
using Verse;
using Verse.AI.Group;

namespace SR.ModRimWorld.RaidExtension
{
    public class LordJobHostileTraderCaravanTravelAndExit : LordJob
    {
        private IntVec3 _travelDest;

        [UsedImplicitly]
        public LordJobHostileTraderCaravanTravelAndExit()
        {
        }

        public LordJobHostileTraderCaravanTravelAndExit(IntVec3 travelDest) => _travelDest = travelDest;

        /// <summary>
        /// 序列化
        /// </summary>
        public override void ExposeData() => Scribe_Values.Look(ref _travelDest, "travelDest");

        /// <summary>
        /// 创建状态机
        /// </summary>
        /// <returns></returns>
        public override StateGraph CreateGraph()
        {
            var stateGraph = new StateGraph();
            //旅行状态
            var lordToilTravel = stateGraph.AttachSubgraph(new LordJob_Travel(_travelDest).CreateGraph()).StartingToil;
            stateGraph.StartingToil = lordToilTravel;
            //离开状态
            var lordToilExitMap = new LordToil_ExitMap();
            stateGraph.AddToil(lordToilExitMap);
            //掩护逃跑状态
            var lordToilExitMapTraderFighting = new LordToil_ExitMapTraderFighting();
            //过渡 旅行到离开
            var transitionTravelToExitMap = new Transition(lordToilTravel, lordToilExitMap);
            var triggerArrived = new Trigger_Memo("TravelArrived");
            transitionTravelToExitMap.AddTrigger(triggerArrived);
            stateGraph.AddTransition(transitionTravelToExitMap);
            //过渡 任意状态到掩护逃跑
            foreach (var lordToil in stateGraph.lordToils)
            {
                var transitionAnyToExitMapAndEscortCarriers =
                    new Transition(lordToil, lordToilExitMapTraderFighting);
                var triggerPawnLost = new Trigger_PawnLost();
                transitionAnyToExitMapAndEscortCarriers.AddTrigger(triggerPawnLost);
                stateGraph.AddTransition(transitionAnyToExitMapAndEscortCarriers, true);
            }

            stateGraph.AddToil(lordToilExitMapTraderFighting);
            return stateGraph;
        }
    }
}