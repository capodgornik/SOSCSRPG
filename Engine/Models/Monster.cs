using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Monster : LivingEntity
    {
        
        public string ImageName { get; set; }

        public int MinimumDamage { get; set; }
        public int MaximumDamage { get; set; }

        public int RewardExperiencePoints { get; private set; }

        public Monster(string name, string imageName,
                       int maximumHitPints, int hitPoints,
                       int minimumDamage, int maximumDamage,
                       int rewardExperiencePoints, int rewardGold)
        {
            Name = name;
            ImageName = $"/Engine;component/Images/Monsters/{imageName}";
            CurrentHitPoints = hitPoints;
            MinimumDamage = minimumDamage;
            MaximumDamage = maximumDamage;
            RewardExperiencePoints = rewardExperiencePoints;
            Gold = rewardGold;
        }
    }
}
