using System;
using System.Collections.Generic;
using System.Text;

namespace TT2Master.Model.Raid
{
    public class RaidResultAnalysisEntry
    {
        public string Name { get; set; }

        public int Attacks { get; set; }

        public double Damage { get; set; }

        public double DamagePerAttack { get; set; }

        public double Overkill { get; set; }

        public bool IsOneOfWorstOverkills { get; set; }

        public bool IsOneOfWorstParticipents { get; set; }

        public bool IsBelowMinAverageDamage { get; set; }

        public static string GetCsvHeaderline()
        {
            var del = LocalSettingsORM.CsvDelimiter;

            return $"{nameof(Name)}{del}{nameof(Attacks)}{del}{nameof(Damage)}{del}{nameof(DamagePerAttack)}{del}{nameof(Overkill)}";
        }

        public string GetCsvString(string del)
        {
            return $"{Name}{del}{Attacks}{del}{Damage.ToString("N0")}{del}{DamagePerAttack.ToString("N0")}{del}{Overkill.ToString("N0")}";
        }
    }
}
