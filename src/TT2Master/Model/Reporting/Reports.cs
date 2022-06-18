using System;
using System.Collections.Generic;
using System.Text;
using TT2Master.Resources;

namespace TT2Master.Model.Reporting
{
    public static class Reports
    {
        public static List<Report> AvailableStandardReports => new List<Report>
        {
            new Report
            {
                Id = "ClanMemberCompleteReport",
                Name = AppResources.CompleteMemberReport,
                Destination = "ClanMemberCompleteReportPage"
            },
            new Report
            {
                Id = "ClanMemberBaseStatsReport",
                Name = AppResources.MemberBaseStatsReport,
                Destination = "ClanMemberBaseStatsReportPage"
            },
            new Report
            {
                Id = "ClanMemberRaidStatsReport",
                Name = AppResources.MemberRaidStatsReport,
                Destination = "ClanMemberRaidStatsReportPage"
            },
        };
    }
}
