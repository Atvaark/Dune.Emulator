using System;
using System.Linq;
using System.Text;
using Dune.Packets;

namespace Dune.Client
{
    public class OnlineUrDragon
    {
        public struct DragonHeart
        {
            public uint Health;
            public uint MaxHealth;
        }

        public static readonly byte[] AllPropertyIndices;

        public uint Generation { get; set; }
        public long Health
        {
            get { return _hearts.Sum(h => h.Health); }
        }
        public long MaxHealth
        {
            get { return _hearts.Sum(h => h.MaxHealth); }
        }
        public uint FightCount { get; set; }
        public DateTime? SpawnTime { get; set; }
        public DateTime? GraceTime { get; set; }
        public uint KillCount { get; set; }
        public DragonHeart[] Heats
        {
            get { return _hearts; }
        }

        private readonly DragonHeart[] _hearts = new DragonHeart[30];

        static OnlineUrDragon()
        {
            const byte maxProperties = 43; // [0-42 Ur Dragon] [43-63 not used] 
            AllPropertyIndices = new byte[maxProperties];
            for (byte i = 0; i < maxProperties; i++)
            {
                AllPropertyIndices[i] = i;
            }
        }

        public void Update(TusProperty[] properties)
        {
            foreach (var property in properties)
            {
                if (property.Index == 0)
                {
                    Generation = property.Value2;
                }
                else if (property.Index >= 1 && property.Index < 16)
                {
                    _hearts[(property.Index - 1) * 2].Health = property.Value1;
                    _hearts[(property.Index - 1) * 2 + 1].Health = property.Value2;
                }
                else if (property.Index >= 16 && property.Index < 31)
                {
                    _hearts[(property.Index - 16) * 2].MaxHealth = property.Value1;
                    _hearts[(property.Index - 16) * 2 + 1].MaxHealth = property.Value2;
                }
                else if (property.Index == 31)
                {
                    FightCount = property.Value2;
                }
                else if (property.Index == 32)
                {
                    GraceTime = UnixTimeStampToLocalTime(property.Value2);
                }
                else if (property.Index == 33)
                {
                    KillCount = property.Value2;
                }
                else if (property.Index == 42)
                {
                    SpawnTime = UnixTimeStampToLocalTime(property.Value2);
                }
            }
        }

        public string ToText()
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine("Generation: " + Generation);
            text.AppendLine("Grace: " + (GraceTime == null ? "No" : "Yes"));
            text.AppendLine("Spawned: " + SpawnTime);
            text.AppendLine("Fights: " + FightCount);
            text.AppendLine("Kills: " + KillCount);
            text.AppendLine("HP: " + Health);
            text.AppendLine("HP (max): " + MaxHealth);
            if (MaxHealth > 0)
            {
                text.AppendLine(string.Format("HP (percentage): {0:p}", Decimal.Divide(Health, MaxHealth)));
            }

            return text.ToString();
        }

        private static DateTime? UnixTimeStampToLocalTime(uint unixTimeStamp)
        {
            if (unixTimeStamp == 0)
            {
                return null;
            }

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTimeStamp).ToLocalTime();
        }
    }
}