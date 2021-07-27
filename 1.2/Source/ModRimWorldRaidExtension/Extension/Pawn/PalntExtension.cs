// ******************************************************************
//       /\ /|       @file       PlantExtension.cs
//       \ V/        @brief      植物扩展
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-07-23 13:16:49
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************
using RimWorld;

namespace SR.ModRimWorld.RaidExtension
{
    public static class PlantExtension
    {
        /// <summary>
        /// 是否是棵树
        /// </summary>
        /// <param name="plant"></param>
        /// <returns></returns>
        public static bool IsTree(this Plant plant)
        {
            return plant.def?.plant != null && plant.def.plant.IsTree;
        }
    }
}
