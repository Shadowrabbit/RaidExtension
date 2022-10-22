// ******************************************************************
//       /\ /|       @file       MapExtension.cs
//       \ V/        @brief      地图扩展
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-07-27 11:46:48
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************
using System.Linq;
using RimWorld;
using Verse;

namespace SR.ModRimWorld.RaidExtension
{
    public static class MapExtension
    {
        /// <summary>
        /// 当前地图上是否存在可以列为目标的动物
        /// </summary>
        /// <param name="map"></param>
        /// <param name="minTargetRequireHealthScale">健康缩放最小需求 用来判断动物强度</param>
        /// <returns></returns>
        public static bool IsAnimalTargetExist(this Map map, float minTargetRequireHealthScale)
        {
            bool SpoilValidator(Thing t) => t is Pawn animal && animal.RaceProps != null && animal.RaceProps.Animal &&
                                            !animal.Dead && animal.RaceProps.baseHealthScale >=
                                            minTargetRequireHealthScale;

            return map.mapPawns != null && Enumerable.Any(map.mapPawns.AllPawnsSpawned, SpoilValidator);
        }

        /// <summary>
        /// 当前地图是否存在树
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static bool IsTreeExist(this Map map)
        {
            bool SpoilValidator(Thing t) => t is Plant plant && !plant.IsBurning() && plant.IsTree();
            return map.spawnedThings != null && map.spawnedThings.Any(SpoilValidator);
        }
    }
}
