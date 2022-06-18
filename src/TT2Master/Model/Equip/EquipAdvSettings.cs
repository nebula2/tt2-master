using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using TT2Master.Shared.Models;

namespace TT2Master
{
    [Table("EQUIPADVSETTINGS")]
    public class EquipAdvSettings
    {
        [PrimaryKey]
        public string ID { get; set; } = "1";

        public EquipBuildEnum CurrentBuild { get; set; } = EquipBuildEnum.Ship;

        public GoldType CurrentGoldType { get; set; } = GoldType.pHoM;

        public HeroBaseType CurrentHeroType { get; set; } = HeroBaseType.NotSet;

        public HeroDmgType CurrentHeroDmgType { get; set; } = HeroDmgType.NotSet;

        public EquipAdvSettings()
        {

        }

        public override string ToString() => $"ID: {ID}, CurrentBuild: {CurrentBuild}, CurrentGoldType: {CurrentGoldType}, CurrentHeroType: {CurrentHeroType}, CurrentHeroDmgType: {CurrentHeroDmgType}";
    }
}
