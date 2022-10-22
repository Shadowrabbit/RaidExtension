// ******************************************************************
//       /\ /|       @file       DutyDefOf.cs
//       \ V/        @brief      责任定义
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-14 20:54:23
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using JetBrains.Annotations;
using RimWorld;
using Verse.AI;

namespace SR.ModRimWorld.RaidExtension
{
    [DefOf]
    public static class DutyDefOf
    {
        [UsedImplicitly] public static readonly DutyDef SrLogging; // 伐木
        [UsedImplicitly] public static readonly DutyDef SrTakeWoodExit; // 带着木材离开
        [UsedImplicitly] public static readonly DutyDef SrPoaching; // 偷猎
        [UsedImplicitly] public static readonly DutyDef SrTakePreyExit; // 带着猎物离开
    }
}