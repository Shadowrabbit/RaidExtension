// ******************************************************************
//       /\ /|       @file       LordJobPoaching.cs
//       \ V/        @brief      集群AI 偷猎
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-17 23:28:46
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using RimWorld;
using Verse;
using Verse.AI.Group;

namespace SR.ModRimWorld.FactionalWar
{
    public class LordJobPoaching : LordJob
    {
        private static readonly IntRange ExitTime = new IntRange(5000, 8000); //离开时间
        private Pawn _targetAnimal; //集群AI想要猎杀的动物
        public Pawn TargetAnimal => _targetAnimal;

        public LordJobPoaching()
        {
        }

        public LordJobPoaching(Pawn targetAnimal)
        {
            _targetAnimal = targetAnimal;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref _targetAnimal, "_targetAnimal");
        }

        public override StateGraph CreateGraph()
        {
            //集群AI流程状态机
            var stateGraph = new StateGraph();
            //添加流程 偷猎
            var lordToilPoaching = new LordToilPoaching();
            stateGraph.AddToil(lordToilPoaching);
            //添加流程 带着猎物离开
            var lordToilTakePreyExit = new LordToilTakePreyExit();
            stateGraph.AddToil(lordToilTakePreyExit);
            //设置初始状态
            stateGraph.StartingToil = lordToilPoaching;
            var faction = lord.faction;
            //过渡 偷猎到带着猎物离开
            var transitionPoachingToTakePreyExit = new Transition(lordToilPoaching, lordToilTakePreyExit);
            //触发条件 目标猎物被击倒
            var triggerTargetAnimalDowned = new TriggerTargetAnimalDowned(_targetAnimal);
            transitionPoachingToTakePreyExit.AddTrigger(triggerTargetAnimalDowned);
            transitionPoachingToTakePreyExit.AddPreAction(new TransitionAction_Message(
                "SrTakePreyExit".Translate(faction.def.pawnsPlural.CapitalizeFirst(),
                    faction.Name), MessageTypeDefOf.ThreatSmall));
            stateGraph.AddTransition(transitionPoachingToTakePreyExit);
            return stateGraph;
        }
    }
}
