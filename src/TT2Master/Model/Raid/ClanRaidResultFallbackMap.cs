using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using TT2Master.Shared.Assets;

namespace TT2Master.Model.Raid
{
    public class ClanRaidResultFallbackMap : ClassMap<ClanRaidResult>
    {
        private readonly ClanRaidResult _me = new ClanRaidResult();

        public ClanRaidResultFallbackMap()
        {
            Map(m => m.ID).Convert(row => row.Row.Context.Parser.RawRow - 1);

            Map(m => m.PlayerName).Index(0);
            Map(m => m.PlayerCode).Index(1);
            Map(m => m.TotalRaidAttacks).Index(2);
            Map(m => m.TitanNumber).Index(3);
            Map(m => m.TitanName).Index(4);
            Map(m => m.TitanDamage).Index(5);

            Map(m => m.ArmorHead).Index(6);
            Map(m => m.ArmorTorso).Index(7);
            Map(m => m.ArmorLeftArm).Index(8);
            Map(m => m.ArmorRightArm).Index(9);
            Map(m => m.ArmorLeftHand).Index(10);
            Map(m => m.ArmorRightHand).Index(11);
            Map(m => m.ArmorLeftLeg).Index(12);
            Map(m => m.ArmorRightLeg).Index(13);

            Map(m => m.BodyHead).Index(14);
            Map(m => m.BodyTorso).Index(15);
            Map(m => m.BodyLeftArm).Index(16);
            Map(m => m.BodyRightArm).Index(17);
            Map(m => m.BodyLeftHand).Index(18);
            Map(m => m.BodyRightHand).Index(19);
            Map(m => m.BodyLeftLeg).Index(20);
            Map(m => m.BodyRightLeg).Index(21);

            Map(m => m.SkeletonHead).Index(22);
            Map(m => m.SkeletonTorso).Index(23);
            Map(m => m.SkeletonLeftArm).Index(24);
            Map(m => m.SkeletonRightArm).Index(25);
            Map(m => m.SkeletonLeftHand).Index(26);
            Map(m => m.SkeletonRightHand).Index(27);
            Map(m => m.SkeletonLeftLeg).Index(28);
            Map(m => m.SkeletonRightLeg).Index(29);
        }
    }
}
