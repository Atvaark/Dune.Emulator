using System;
using System.Linq;
using System.Text;
using Dune.Packets;

namespace Dune
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
        public DragonHeart[] Hearts
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

        public TusProperty[] ToProperties()
        {
            TusProperty[] properties = new TusProperty[43];
            properties[0] = new TusProperty(0, Generation);

            byte index = 1;
            int maxIndex = index + _hearts.Length / 2;
            for (int i = 0; index < maxIndex && i < _hearts.Length; )
            {
                properties[index] = new TusProperty(
                    _hearts[i++].Health,
                    i < _hearts.Length ? _hearts[i++].Health : 0);
                index++;
            }
            
            index = 16;
            maxIndex = index + _hearts.Length / 2;
            for (int i = 0; index < maxIndex && i < _hearts.Length; )
            {
                properties[index] = new TusProperty(
                    _hearts[i++].MaxHealth,
                    i < _hearts.Length ? _hearts[i++].MaxHealth : 0);
                index++;
            }
            
            properties[31] = new TusProperty(0, FightCount);
            properties[32] = new TusProperty(0, LocalTimeToUnixTimeStampT(GraceTime));
            properties[33] = new TusProperty(0, KillCount);

            // Unknowns
            properties[35] = new TusProperty(17825793, 0);
            properties[37] = new TusProperty(17825793, 0);
            properties[39] = new TusProperty(17825793, 0);
            // Armor?
            properties[41] = new TusProperty(0, 10800);

            properties[42] = new TusProperty(0, LocalTimeToUnixTimeStampT(SpawnTime));

            for (byte i = 0; i < properties.Length; i++)
            {
                properties[i].Index = i;
            }

            return properties;
        }

        public TusProperty[] ToProperties(byte[] indices)
        {
            return ToProperties().Where(p => indices.Contains(p.Index)).ToArray();
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

            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTimeStamp).ToLocalTime();
        }

        private static uint LocalTimeToUnixTimeStampT(DateTime? time)
        {
            if (time == null)
            {
                return 0;
            }

            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return (uint)time.Value.ToUniversalTime().Subtract(epoch).TotalSeconds;
        }
    }
}