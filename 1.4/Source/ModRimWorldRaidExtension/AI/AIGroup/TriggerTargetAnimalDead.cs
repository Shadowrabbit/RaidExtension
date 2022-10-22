// ******************************************************************
//       /\ /|       @file       TriggerTargetAnimalDead.cs
//       \ V/        @brief      目标动物被击倒
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-07-08 13:00:28
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using Verse;
using Verse.AI.Group;

namespace SR.ModRimWorld.RaidExtension
{
    public class TriggerTargetAnimalDead : Trigger
    {
        private Pawn _targetAnimal; //狩猎目标
        private const int CheckEveryTicks = 100;

        public TriggerTargetAnimalDead(Pawn targetAnimal)
        {
            _targetAnimal = targetAnimal;
        }

        public override bool ActivateOn(Lord lord, TriggerSignal signal)
        {
            //信号类型不关心
            if (signal.type != TriggerSignalType.Tick)
            {
                Log.Warning($"{MiscDef.LogTag}signal don't care");
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
                Log.Error($"{MiscDef.LogTag}lordJob is wrong");
                return false;
            }

            //地图上的物体列表异常
            if (lord.Map?.listerThings == null)
            {
                Log.Error($"{MiscDef.LogTag}listerThings is null");
                return false;
            }

            //lord异常
            if (lord.ownedPawns == null || lord.ownedPawns.Count <= 0)
            {
                Log.Error($"{MiscDef.LogTag}no pawns in lord");
                return false;
            }

            //异常原因目标动物被销毁
            if (_targetAnimal == null)
            {
                Log.Warning($"{MiscDef.LogTag}unknown anomaly causes the target to disappear");
                //尝试重新查找
                var animal = lord.ownedPawns[0].FindTargetAnimal(MiscDef.MinTargetRequireHealthScale);
                //没有目标
                if (animal == null)
                {
                    return true;
                }

                _targetAnimal = animal;
                lordJobPoaching.TargetAnimal = animal;
                return false;
            }

            //目标存活
            if (!_targetAnimal.Dead)
            {
                return false;
            }

            //目标死亡 队长半径10以内如果有动物的话继续狩猎
            foreach (var thing in GenRadial.RadialDistinctThingsAround(lord.ownedPawns[0].Position, lord.Map, 20f,
                true))
            {
                if (!(thing is Pawn animal))
                {
                    continue;
                }

                //不符合目标要求
                if (!lord.ownedPawns[0].IsTargetAnimalValid(animal, MiscDef.MinTargetRequireHealthScale))
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