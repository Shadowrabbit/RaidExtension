// ******************************************************************
//       /\ /|       @file       JobGiverLogging.cs
//       \ V/        @brief      行为节点 伐木
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-19 12:00:02
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using JetBrains.Annotations;
using Verse;
using Verse.AI;

namespace SR.ModRimWorld.RaidExtension
{
    [UsedImplicitly]
    public class JobGiverLogging : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            var tree = pawn.FindTree();
            return tree == null ? null : JobMaker.MakeJob(RimWorld.JobDefOf.CutPlant, tree);
        }
    }
}