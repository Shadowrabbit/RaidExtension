// ******************************************************************
//       /\ /|       @file       JobGiverAIFightAnimal.cs
//       \ V/        @brief      行为节点 攻击动物
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-19 12:59:15
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using JetBrains.Annotations;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace SR.ModRimWorld.RaidExtension
{
    [UsedImplicitly]
    public class JobGiverAIFightAnimal : JobGiver_AIFightEnemies
    {
        private const float MinTargetRequireHealthScale = 1.2f;

        /// <summary>
        /// 尝试分配工作
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        protected override Job TryGiveJob(Pawn pawn)
        {
            //找队长旁边的动物
            var enemyTarget = FindTargetAnimal(pawn);
            if (enemyTarget == null)
            {
                return null;
            }

            //视野外看不见
            if (enemyTarget.IsInvisible())
            {
                return null;
            }

            pawn.mindState.enemyTarget = enemyTarget;
            var allowManualCastWeapons = !pawn.IsColonist;
            //获取攻击动作
            var attackVerb = pawn.TryGetAttackVerb(enemyTarget, allowManualCastWeapons);
            if (attackVerb == null)
            {
                return null;
            }

            //近战的情况
            if (attackVerb.verbProps.IsMeleeAttack)
            {
                var jobMeleeAttack = MeleeAttackJob(enemyTarget);
                jobMeleeAttack.killIncappedTarget = true;
                jobMeleeAttack.attackDoorIfTargetLost = true;
                return jobMeleeAttack;
            }

            //远程的情况 先算当前位置适不适合攻击
            var num1 = (double) CoverUtility.CalculateOverallBlockChance((LocalTargetInfo) pawn,
                enemyTarget.Position, pawn.Map) > 0.01
                ? 1
                : 0;
            var flag1 = pawn.Position.Standable(pawn.Map) &&
                        pawn.Map.pawnDestinationReservationManager.CanReserve(pawn.Position, pawn, pawn.Drafted);
            var flag2 = attackVerb.CanHitTarget(enemyTarget);
            var flag3 = (pawn.Position - enemyTarget.Position).LengthHorizontalSquared < 25;
            var num2 = flag1 ? 1 : 0;
            //适合攻击
            if ((num1 & num2 & (flag2 ? 1 : 0)) != 0 || flag3 & flag2)
            {
                return MakeRangeAttackJob(enemyTarget, attackVerb);
            }

            //寻找掩体位置
            if (!TryFindShootingPosition(pawn, out var dest))
            {
                return null;
            }

            //角色已经在掩体位置了 开枪射击
            if (dest == pawn.Position)
            {
                return MakeRangeAttackJob(enemyTarget, attackVerb);
            }

            //走向掩体
            var job = JobMaker.MakeJob(RimWorld.JobDefOf.Goto, dest);
            job.expiryInterval = ExpiryInterval_ShooterSucceeded.RandomInRange;
            job.checkOverrideOnExpire = true;
            return job;
        }

        /// <summary>
        /// 深拷贝
        /// </summary>
        /// <param name="resolve"></param>
        /// <returns></returns>
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            var jobGiverAIFightHostileFaction = (JobGiverAIFightAnimal) base.DeepCopy(resolve);
            return jobGiverAIFightHostileFaction;
        }

        /// <summary>
        /// 寻找目标动物
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        private static Pawn FindTargetAnimal(Pawn pawn)
        {
            //集群AI
            var lord = pawn.GetLord();
            //集群AI错误
            if (!(lord?.LordJob is LordJobPoaching lordJobPoaching))
            {
                return null;
            }

            var targetAnimal = lordJobPoaching.TargetAnimal;
            return pawn.IsTargetAnimalValid(targetAnimal, MinTargetRequireHealthScale) ? targetAnimal : null;
        }

        /// <summary>
        /// 创建远程攻击工作
        /// </summary>
        /// <param name="enemyTarget"></param>
        /// <param name="verb"></param>
        /// <returns></returns>
        private static Job MakeRangeAttackJob(Thing enemyTarget, Verb verb)
        {
            var rangeAttackJob = JobMaker.MakeJob(RimWorld.JobDefOf.AttackStatic);
            rangeAttackJob.verbToUse = verb;
            rangeAttackJob.targetA = enemyTarget;
            rangeAttackJob.endIfCantShootInMelee = true;
            rangeAttackJob.endIfCantShootTargetFromCurPos = true;
            rangeAttackJob.killIncappedTarget = true;
            rangeAttackJob.attackDoorIfTargetLost = true;
            return rangeAttackJob;
        }
    }
}