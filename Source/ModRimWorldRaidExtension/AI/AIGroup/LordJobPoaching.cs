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

namespace SR.ModRimWorld.RaidExtension
{
    public class LordJobPoaching : LordJobSiegeBase
    {
        private static readonly IntRange WaitTime = new IntRange(500, 1000); //集合等待时间
        private Pawn _targetAnimal; //集群AI想要猎杀的动物

        public Pawn TargetAnimal
        {
            get => _targetAnimal;
            set => _targetAnimal = value;
        }

        public LordJobPoaching()
        {
        }

        public LordJobPoaching(IntVec3 siegeSpot, Pawn targetAnimal) : base(siegeSpot)
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
            //添加流程 集结
            var lordToilStage = new LordToil_Stage(siegeSpot);
            stateGraph.AddToil(lordToilStage);
            //添加流程 偷猎
            var lordToilPoaching = new LordToilPoaching();
            stateGraph.AddToil(lordToilPoaching);
            //添加流程 带着猎物离开
            var lordToilTakePreyExit = new LordToilTakePreyExit();
            stateGraph.AddToil(lordToilTakePreyExit);
            var faction = lord.faction;
            //过渡 集合到开始偷猎
            var transition = new Transition(lordToilStage, lordToilPoaching);
            transition.AddTrigger(new Trigger_TicksPassed(WaitTime.RandomInRange));
            //唤醒成员
            transition.AddPostAction(new TransitionAction_WakeAll());
            stateGraph.AddTransition(transition);
            //过渡 偷猎到带着猎物离开
            var transitionPoachingToTakePreyExit = new Transition(lordToilPoaching, lordToilTakePreyExit);
            //触发条件 目标猎物被击倒
            var triggerTargetAnimalDead = new TriggerTargetAnimalDead(_targetAnimal);
            transitionPoachingToTakePreyExit.AddTrigger(triggerTargetAnimalDead);
            transitionPoachingToTakePreyExit.AddPreAction(new TransitionAction_Message(
                "SrTakePreyExit".Translate(faction.def.pawnsPlural.CapitalizeFirst(),
                    faction.Name), MessageTypeDefOf.ThreatSmall));
            stateGraph.AddTransition(transitionPoachingToTakePreyExit);
            return stateGraph;
        }
    }
}