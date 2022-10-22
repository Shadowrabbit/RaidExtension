// ******************************************************************
//       /\ /|       @file       LordJobLogging.cs
//       \ V/        @brief      集群AI 伐木
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-17 23:45:54
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using RimWorld;
using Verse;
using Verse.AI.Group;

namespace SR.ModRimWorld.RaidExtension
{
    public class LordJobLogging : LordJobSiegeBase
    {
        private const int ExitTime = 2000; //离开时间
        private const int WaitTime = 500; //集合等待时间

        public LordJobLogging()
        {
        }

        public LordJobLogging(IntVec3 siegeSpot) : base(siegeSpot)
        {
        }

        public override StateGraph CreateGraph()
        {
            //集群AI流程状态机
            var stateGraph = new StateGraph();
            //添加流程 集结
            var lordToilStage = new LordToil_Stage(siegeSpot);
            stateGraph.AddToil(lordToilStage);
            //添加流程 伐木
            var lordToilLogging = new LordToilLogging();
            stateGraph.AddToil(lordToilLogging);
            //添加流程 带着木材离开
            var lordToilTakeWoodExit = new LordToilTakeWoodExit();
            stateGraph.AddToil(lordToilTakeWoodExit);
            var faction = lord.faction;
            //过渡 集合到开始伐木
            var transition = new Transition(lordToilStage, lordToilLogging);
            transition.AddTrigger(new Trigger_TicksPassed(WaitTime));
            //唤醒成员
            transition.AddPostAction(new TransitionAction_WakeAll());
            stateGraph.AddTransition(transition);
            //过渡 伐木到带着木材离开
            var transitionLoggingToTakeWoodExit = new Transition(lordToilLogging, lordToilTakeWoodExit);
            var triggerTicksPassed = new Trigger_TicksPassed(ExitTime);
            transitionLoggingToTakeWoodExit.AddTrigger(triggerTicksPassed);
            transitionLoggingToTakeWoodExit.AddPreAction(new TransitionAction_Message(
                "SrTakeWoodExit".Translate(faction.def.pawnsPlural.CapitalizeFirst(),
                    faction.Name), MessageTypeDefOf.ThreatSmall));
            stateGraph.AddTransition(transitionLoggingToTakeWoodExit);
            return stateGraph;
        }
    }
}