using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace TT2Master.Shared.Raids
{
    public class RaidSeedParser
    {
        public List<RaidSeed> Seeds { get; set; }

        public bool LoadSeedsFromJsonString(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString)) return false;

            var seeds = GetRaidSeeds(jsonString);

            if (seeds == null) return false;

            Seeds = seeds;

            return PopulateSeeds(jsonString);
        }

        private List<RaidSeed> GetRaidSeeds(string s)
        {
            try
            {
                return JsonConvert.DeserializeObject<List<RaidSeed>>(s);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private bool PopulateSeeds(string s)
        {
            try
            {
                var arr = JArray.Parse(s);

                foreach (var item in arr.Children())
                {
                    var itemProperties = item.Children<JProperty>();
                    var tierProperty = itemProperties.FirstOrDefault(x => x.Name == GetPropertyAttribute<RaidSeed>(t => t.Tier));
                    var levelProperty = itemProperties.FirstOrDefault(x => x.Name == GetPropertyAttribute<RaidSeed>(t => t.Level));

                    var seed = Seeds.Single(x => x.Tier == tierProperty.Value.ToString() && x.Level == levelProperty.Value.ToString());

                    seed.Enemies = new List<RaidSeedEnemy>();

                    for (int i = 0; i < seed.EnemyNames.Length; i++)
                    {
                        var enemyProperty = itemProperties.First(x => x.Name == seed.EnemyNames[i]);
                        var enemy = JsonConvert.DeserializeObject<RaidSeedEnemy>(enemyProperty.Value.ToString());
                        enemy.Name = seed.EnemyNames[i];
                        seed.Enemies.Add(enemy);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetPropertyAttribute<TType>(Expression<Func<TType, object>> property)
        {
            return !(property.Body is MemberExpression memberExpression)
                ? throw new ArgumentException("Expression must be a property")
                : memberExpression.Member.GetCustomAttribute<JsonPropertyAttribute>().PropertyName;
        }
    }
}
