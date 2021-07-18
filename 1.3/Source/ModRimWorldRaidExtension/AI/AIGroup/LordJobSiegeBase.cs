// ******************************************************************
//       /\ /|       @file       LordJobSiegeBase.cs
//       \ V/        @brief      集结型集群AI
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-07-11 17:14:56
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using Verse;
using Verse.AI.Group;

namespace SR.ModRimWorld.RaidExtension
{
    public abstract class LordJobSiegeBase : LordJob
    {
        protected IntVec3 siegeSpot; //集结地点

        protected LordJobSiegeBase()
        {
        }

        protected LordJobSiegeBase(IntVec3 siegeSpot)
        {
            this.siegeSpot = siegeSpot;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        public override void ExposeData()
        {
            Scribe_Values.Look(ref siegeSpot, "_siegeSpot");
        }
    }
}