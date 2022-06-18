using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using TT2Master.Shared.Assets;

namespace TT2Master.Model.Raid
{
    public class ClanRaidResultMap : ClassMap<ClanRaidResult>
    {
        private readonly ClanRaidResult _me = new ClanRaidResult();

        public ClanRaidResultMap()
        {
            Map(m => m.ID).Convert(row => row.Row.Context.Parser.RawRow - 1);

            Map(m => m.PlayerName).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.PlayerName)));
            Map(m => m.PlayerCode).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.PlayerCode)));
            Map(m => m.TotalRaidAttacks).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.TotalRaidAttacks)));
            Map(m => m.TitanNumber).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.TitanNumber)));
            Map(m => m.TitanName).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.TitanName)));
            Map(m => m.TitanDamage).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.TitanDamage)));

            Map(m => m.ArmorHead).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ArmorHead)));
            Map(m => m.ArmorTorso).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ArmorTorso)));
            Map(m => m.ArmorLeftArm).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ArmorLeftArm)));
            Map(m => m.ArmorRightArm).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ArmorRightArm)));
            Map(m => m.ArmorLeftHand).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ArmorLeftHand)));
            Map(m => m.ArmorRightHand).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ArmorRightHand)));
            Map(m => m.ArmorLeftLeg).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ArmorLeftLeg)));
            Map(m => m.ArmorRightLeg).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ArmorRightLeg)));
            
            Map(m => m.BodyHead).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BodyHead)));
            Map(m => m.BodyTorso).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BodyTorso)));
            Map(m => m.BodyLeftArm).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BodyLeftArm)));
            Map(m => m.BodyRightArm).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BodyRightArm)));
            Map(m => m.BodyLeftHand).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BodyLeftHand)));
            Map(m => m.BodyRightHand).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BodyRightHand)));
            Map(m => m.BodyLeftLeg).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BodyLeftLeg)));
            Map(m => m.BodyRightLeg).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BodyRightLeg)));
            
            Map(m => m.SkeletonHead).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.SkeletonHead)));
            Map(m => m.SkeletonTorso).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.SkeletonTorso)));
            Map(m => m.SkeletonLeftArm).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.SkeletonLeftArm)));
            Map(m => m.SkeletonRightArm).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.SkeletonRightArm)));
            Map(m => m.SkeletonLeftHand).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.SkeletonLeftHand)));
            Map(m => m.SkeletonRightHand).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.SkeletonRightHand)));
            Map(m => m.SkeletonLeftLeg).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.SkeletonLeftLeg)));
            Map(m => m.SkeletonRightLeg).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.SkeletonRightLeg)));
        }
    }
}
