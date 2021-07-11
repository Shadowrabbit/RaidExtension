// ******************************************************************
//       /\ /|       @file       TriggerTargetAnimalDowned.cs
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
    public class TriggerTargetAnimalDowned : Trigger
    {
        private readonly Pawn _targetAnimal; //狩猎目标
        private const int CheckEveryTicks = 600;

        public TriggerTargetAnimalDowned(Pawn targetAnimal)
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

            return _targetAnimal.Downed || _targetAnimal.Dead;
        }
    }
}
