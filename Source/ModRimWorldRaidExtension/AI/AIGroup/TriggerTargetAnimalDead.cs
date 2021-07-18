// ******************************************************************
//       /\ /|       @file       TriggerTargetAnimalDead.cs
//       \ V/        @brief      目标动物被击倒
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-07-08 13:00:28
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace SR.ModRimWorld.RaidExtension
{
    public class TriggerTargetAnimalDead : Trigger
    {
        private Pawn _targetAnimal; //狩猎目标
        private const float MinTargetRequireHealthScale = 1.2f; //健康缩放最小需求 用来判断动物强度
        private const int CheckEveryTicks = 100;

        public TriggerTargetAnimalDead(Pawn targetAnimal)
        {
            _targetAnimal = targetAnimal;
        }

        public override bool ActivateOn(Lord lord, TriggerSignal signal)
        {
            if (signal.type != TriggerSignalType.Tick)
            {
                return false;
            }

            //降低触发频率 优化性能
            if (Find.TickManager.TicksGame % CheckEveryTicks != 0)
            {
                return false;
            }

            //集群AI错误
            if (!(lord?.LordJob is LordJobPoaching lordJobPoaching))
            {
                return false;
            }

            //目标存活
            if (!_targetAnimal.Dead)
            {
                return false;
            }

            //如果目标在非死亡状态逃离地图 重新选取目标
            if (!lord.Map.listerThings.Contains(_targetAnimal))
            {
                lordJobPoaching.TargetAnimal = lord.ownedPawns[0].FindTargetAnimal(MinTargetRequireHealthScale);
                _targetAnimal = lordJobPoaching.TargetAnimal;
            }

            //队长半径10以内如果有动物的话继续狩猎
            foreach (var thing in GenRadial.RadialDistinctThingsAround(lord.ownedPawns[0].Position, lord.Map, 20f,
                true))
            {
                if (!(thing is Pawn animal))
                {
                    continue;
                }

                if (animal.RaceProps == null)
                {
                    continue;
                }

                if (!animal.RaceProps.Animal)
                {
                    continue;
                }

                if (!lord.ownedPawns[0].CanReserve(animal))
                {
                    continue;
                }

                if (animal.Dead)
                {
                    continue;
                }

                lordJobPoaching.TargetAnimal = animal;
                _targetAnimal = animal;
                return false;
            }

            return true;
        }
    }
}