// ******************************************************************
//       /\ /|       @file       JobDefOf.cs
//       \ V/        @brief      工作定义
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-07-11 14:14:10
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace SR.ModRimWorld.RaidExtension
{
    [DefOf]
    public static class JobDefOf
    {
        [UsedImplicitly] public static readonly JobDef SrTakeWoodExit; //带着木材离开
    }
}