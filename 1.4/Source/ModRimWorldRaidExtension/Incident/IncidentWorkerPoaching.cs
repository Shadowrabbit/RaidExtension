// ******************************************************************
//       /\ /|       @file       IncidentWorkerPoaching.cs
//       \ V/        @brief      事件 偷猎
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-17 13:45:19
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace SR.ModRimWorld.RaidExtension
{
    [UsedImplicitly]
    public class IncidentWorkerPoaching : IncidentWorker_RaidEnemy
    {
        /// <summary>
        /// 是否可以生成事件
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!(parms.target is Map map))
            {
                Log.Error($"{MiscDef.LogTag}target must be a map.");
                return false;
            }

            var isAnimalTargetExist = map.IsAnimalTargetExist(MiscDef.MinTargetRequireHealthScale);

            //目标动物不存在 无法触发事件
            if (!isAnimalTargetExist)
            {
                Log.Warning($"{MiscDef.LogTag}can't find any animal.");
                return false;
            }

            //候选派系列表
            var candidateFactionList = CandidateFactions(map).ToList();
            return Enumerable.Any(candidateFactionList, faction => faction.HostileTo(Faction.OfPlayer));
        }

        /// <summary>
        /// 派系能否成为资源组
        /// </summary>
        /// <param name="f"></param>
        /// <param name="map"></param>
        /// <param name="desperate"></param>
        /// <returns></returns>
        protected override bool FactionCanBeGroupSource(Faction f, Map map, bool desperate = false)
        {
            return base.FactionCanBeGroupSource(f, map, desperate) && f.def.humanlikeFaction && !f.Hidden;
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!(parms.target is Map map))
            {
                Log.Error($"{MiscDef.LogTag}target must be a map.");
                return false;
            }

            var isAnimalTargetExist = map.IsAnimalTargetExist(MiscDef.MinTargetRequireHealthScale);
            //目标动物不存在 无法触发事件
            if (!isAnimalTargetExist)
            {
                Log.Warning($"{MiscDef.LogTag}can't find any animal.");
                return false;
            }

            //处理袭击点数
            ResolveRaidPoints(parms);
            //处理袭击派系
            if (!TryResolveRaidFaction(parms))
            {
                Log.Warning($"{MiscDef.LogTag}cant find raid factions");
                return false;
            }

            //角色组定义 战斗
            var combat = PawnGroupKindDefOf.Combat;
            //处理袭击策略
            ResolveRaidStrategy(parms, combat);
            //处理入场方式
            ResolveRaidArriveMode(parms);
            //尝试生成威胁（参数）
            parms.raidStrategy.Worker.TryGenerateThreats(parms);
            //尝试解决袭击召唤中心
            if (!parms.raidArrivalMode.Worker.TryResolveRaidSpawnCenter(parms))
            {
                Log.Warning($"{MiscDef.LogTag}cant resolve raid spawn center: {parms}");
                return false;
            }

            //生成派系部队
            var pawnList = parms.raidStrategy.Worker.SpawnThreats(parms);
            if (pawnList.Count == 0)
            {
                Log.Warning($"{MiscDef.LogTag}Got no pawns spawning raid from parms {parms}");
                return false;
            }

            //解决信件
            ResolveLetter(parms, pawnList);
            //设置集群AI
            if (!(parms.raidStrategy.Worker is RaidStrategyWorkerPoaching raidStrategyWorkerPoaching))
            {
                Log.Error($"{MiscDef.LogTag}strategy must be RaidStrategyWorkerPoaching");
                return false;
            }

            //设置狩猎目标
            var animal = pawnList[0].FindTargetAnimal(MiscDef.MinTargetRequireHealthScale);
            if (animal == null)
            {
                Log.Warning($"{MiscDef.LogTag}can't find any animal.");
                return false;
            }

            raidStrategyWorkerPoaching.TempAnimal = animal;
            raidStrategyWorkerPoaching.MakeLords(parms, pawnList);
            //袭击时设置一倍速
            Find.TickManager.slower.SignalForceNormalSpeedShort();
            //更新参与袭击的敌人记录
            Find.StoryWatcher.statsRecord.numRaidsEnemy++;
            return true;
        }

        /// <summary>
        /// 解决突袭策略
        /// </summary>
        /// <param name="parms"></param>
        /// <param name="groupKind"></param>
        public override void ResolveRaidStrategy(IncidentParms parms, PawnGroupKindDef groupKind)
        {
            parms.raidStrategy = RaidStrategyDefOf.SrPoaching;
        }

        /// <summary>
        /// 袭击点数
        /// </summary>
        /// <param name="parms"></param>
        protected override void ResolveRaidPoints(IncidentParms parms)
        {
            if (parms.points > MiscDef.MaxThreatPoints)
            {
                parms.points = MiscDef.MaxThreatPoints;
            }

            if (parms.points < MiscDef.MinThreatPoints)
            {
                parms.points = MiscDef.MinThreatPoints;
            }
        }

        /// <summary>
        /// 获取信件定义
        /// </summary>
        /// <returns></returns>
        protected override LetterDef GetLetterDef()
        {
            return LetterDefOf.ThreatSmall;
        }

        /// <summary>
        /// 解决信件
        /// </summary>
        /// <param name="parms"></param>
        /// <param name="pawnList"></param>
        protected virtual void ResolveLetter(IncidentParms parms, List<Pawn> pawnList)
        {
            var letterLabel = (TaggedString) GetLetterLabel(parms);
            var letterText = (TaggedString) GetLetterText(parms, pawnList);
            PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter(pawnList, ref letterLabel, ref letterText,
                GetRelatedPawnsInfoLetterText(parms), true);
            SendStandardLetter(letterLabel, letterText, GetLetterDef(), parms, pawnList,
                Array.Empty<NamedArgument>());
        }
    }
}