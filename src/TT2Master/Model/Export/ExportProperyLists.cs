using System.Collections.Generic;
using TT2Master.Resources;

namespace TT2Master
{
    /// <summary>
    /// Class for exportable properties
    /// </summary>
    public static class ExportProperyLists
    {
        /// <summary>
        /// List of exportable properties
        /// </summary>
        /// <returns></returns>
        public static List<CsvExportProperty> GetClanMemberProperties() => new List<CsvExportProperty>()
            {
                new CsvExportProperty()
                {
                    ExportReference = "ClanMember",
                    SortId = 0,
                    ID = "ID",
                    DisplayName = "ID",
                    IsExportWished = true,
                },
                new CsvExportProperty()
                {
                    ExportReference = "ClanMember",
                    SortId = 1,
                    ID = "Name",
                    DisplayName = AppResources.NameHeader,
                    IsExportWished = true,
                },
                new CsvExportProperty()
                {
                    ExportReference = "ClanMember",
                    SortId = 2,
                    ID = "StageMax",
                    DisplayName = AppResources.MaximumStage,
                    IsExportWished = true,
                },
                new CsvExportProperty()
                {
                    ExportReference = "ClanMember",
                    SortId = 3,
                    ID = "ArtifactCount",
                    DisplayName = AppResources.ArtifactCount,
                    IsExportWished = true,
                },
                new CsvExportProperty()
                {
                    ExportReference = "ClanMember",
                    SortId = 4,
                    ID = "WeeklyTicketCount",
                    DisplayName = AppResources.WeeklyTicketCount,
                    IsExportWished = true,
                },
                new CsvExportProperty()
                {
                    ExportReference = "ClanMember",
                    SortId = 5,
                    ID = "TournamentCount",
                    DisplayName = AppResources.TournamentCount,
                    IsExportWished = true,
                },
                new CsvExportProperty()
                {
                    ExportReference = "ClanMember",
                    SortId = 6,
                    ID = "TitanPoints",
                    DisplayName = AppResources.TitanPoints,
                    IsExportWished = true,
                },
                new CsvExportProperty()
                {
                    ExportReference = "ClanMember",
                    SortId = 7,
                    ID = "ClanRole",
                    DisplayName = AppResources.ClanRole,
                    IsExportWished = true,
                },
                new CsvExportProperty()
                {
                    ExportReference = "ClanMember",
                    SortId = 8,
                    ID = "LastTimestamp",
                    DisplayName = AppResources.LastTimestamp,
                    IsExportWished = true,
                },
                new CsvExportProperty()
                {
                    ExportReference = "ClanMember",
                    SortId = 9,
                    ID = "RaidTotalXP",
                    DisplayName = AppResources.RaidTotalXP,
                    IsExportWished = true,
                },
                new CsvExportProperty()
                {
                    ExportReference = "ClanMember",
                    SortId = 10,
                    ID = "RaidAttackCount",
                    DisplayName = AppResources.RaidAttackCount,
                    IsExportWished = true,
                },
                new CsvExportProperty()
                {
                    ExportReference = "ClanMember",
                    SortId = 11,
                    ID = "RaidTicketsCollected",
                    DisplayName = AppResources.RaidTicketsCollected,
                    IsExportWished = true,
                },
                new CsvExportProperty()
                {
                    ExportReference = "ClanMember",
                    SortId = 12,
                    ID = "RaidTotalCardLevel",
                    DisplayName = AppResources.RaidTotalCardLevel,
                    IsExportWished = true,
                },
                new CsvExportProperty()
                {
                    ExportReference = "ClanMember",
                    SortId = 13,
                    ID = "RaidUniqueSkillCount",
                    DisplayName = AppResources.RaidUniqueSkillCount,
                    IsExportWished = true,
                },
                new CsvExportProperty()
                {
                    ExportReference = "ClanMember",
                    SortId = 14,
                    ID = "RaidBaseDamage",
                    DisplayName = AppResources.RaidBaseDamage,
                    IsExportWished = true,
                },
                new CsvExportProperty()
                {
                    ExportReference = "ClanMember",
                    SortId = 15,
                    ID = "RaidPlayerLevel",
                    DisplayName = AppResources.RaidPlayerLevel,
                    IsExportWished = true,
                },
                new CsvExportProperty()
                {
                    ExportReference = "ClanMember",
                    SortId = 16,
                    ID = "EquipmentSetCount",
                    DisplayName = AppResources.EquipmentSetCount,
                    IsExportWished = true,
                },
                new CsvExportProperty()
                {
                    ExportReference = "ClanMember",
                    SortId = 17,
                    ID = "CraftingShardsSpent",
                    DisplayName = AppResources.CraftingShardsSpent,
                    IsExportWished = true,
                },
                new CsvExportProperty()
                {
                    ExportReference = "ClanMember",
                    SortId = 18,
                    ID = "TotalPetLevels",
                    DisplayName = AppResources.TotalPetLevels,
                    IsExportWished = true,
                },
                new CsvExportProperty()
                {
                    ExportReference = "ClanMember",
                    SortId = 19,
                    ID = "TotalSkillPoints",
                    DisplayName = AppResources.TotalSkillPoints,
                    IsExportWished = true,
                },
                new CsvExportProperty()
                {
                    ExportReference = "ClanMember",
                    SortId = 20,
                    ID = "TotalHelperWeapons",
                    DisplayName = AppResources.TotalHelperWeapons,
                    IsExportWished = true,
                },
                new CsvExportProperty()
                {
                    ExportReference = "ClanMember",
                    SortId = 21,
                    ID = "TotalHelperScrolls",
                    DisplayName = AppResources.TotalHelperScrolls,
                    IsExportWished = true,
                },
                new CsvExportProperty()
                {
                    ExportReference = "ClanMember",
                    SortId = 0,
                    ID = "SaveDate",
                    DisplayName = "SaveDate",
                    IsExportWished = true,
                },
                new CsvExportProperty()
                {
                    ExportReference = "ClanMember",
                    SortId = 0,
                    ID = "UndisputedWins",
                    DisplayName = "UndisputedWins",
                    IsExportWished = true,
                },
            };
    }
}
