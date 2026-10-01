using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

[Serializable]
public class Achievement
{
    public string id;
    public string name;
    public string description;
    public AchievementType type;
    public int targetValue;
    public string iconPath;
    public int points;
    public bool isSecret;
}

public enum AchievementType
{
    Simple,      // Just unlock (0 or 1)
    Count,       // Increment to target
    Bitfield     // Multiple conditions
}

[CreateAssetMenu(fileName = "AchievementDefinitions", menuName = "Scriptable Objects/Achievement Definitions")]
public class AchievementDefinitions : ScriptableObject
{
    [Header("All Game Achievements")]
    public List<Achievement> achievements = new List<Achievement>
    {
        // COMBAT ACHIEVEMENTS (8)
        new Achievement
        {
            id = "first_kill",
            name = "First Blood",
            description = "Kill your first enemy",
            type = AchievementType.Count,
            targetValue = 1,
            points = 10,
            isSecret = false
        },
        
        new Achievement
        {
            id = "kill_100_enemies",
            name = "Rookie Slayer",
            description = "Kill 100 enemies",
            type = AchievementType.Count,
            targetValue = 100,
            points = 25,
            isSecret = false
        },
        
        new Achievement
        {
            id = "kill_500_enemies",
            name = "Veteran Warrior",
            description = "Kill 500 enemies",
            type = AchievementType.Count,
            targetValue = 500,
            points = 50,
            isSecret = false
        },
        
        new Achievement
        {
            id = "kill_1000_enemies",
            name = "Master Destroyer",
            description = "Kill 1000 enemies",
            type = AchievementType.Count,
            targetValue = 1000,
            points = 100,
            isSecret = false
        },
        
        new Achievement
        {
            id = "critical_master",
            name = "Critical Strike Master",
            description = "Land 100 critical hits",
            type = AchievementType.Count,
            targetValue = 100,
            points = 30,
            isSecret = false
        },
        
        new Achievement
        {
            id = "life_steal_vampire",
            name = "Vampire",
            description = "Heal 500 HP through life steal",
            type = AchievementType.Count,
            targetValue = 500,
            points = 35,
            isSecret = false
        },
        
        new Achievement
        {
            id = "melee_master",
            name = "Melee Master",
            description = "Kill 100 enemies with melee weapons",
            type = AchievementType.Count,
            targetValue = 100,
            points = 40,
            isSecret = false
        },
        
        new Achievement
        {
            id = "throwing_expert",
            name = "Throwing Expert",
            description = "Kill 50 enemies with throwing weapons",
            type = AchievementType.Count,
            targetValue = 50,
            points = 45,
            isSecret = false
        },
        
        // WAVE PROGRESSION (6)
        new Achievement
        {
            id = "wave_5_complete",
            name = "Getting Started",
            description = "Complete Wave 5",
            type = AchievementType.Simple,
            targetValue = 1,
            points = 20,
            isSecret = false
        },
        
        new Achievement
        {
            id = "wave_10_complete",
            name = "Halfway Hero",
            description = "Complete Wave 10",
            type = AchievementType.Simple,
            targetValue = 1,
            points = 40,
            isSecret = false
        },
        
        new Achievement
        {
            id = "wave_15_complete",
            name = "Victory Royale",
            description = "Complete all 15 waves",
            type = AchievementType.Simple,
            targetValue = 1,
            points = 75,
            isSecret = false
        },
        
        new Achievement
        {
            id = "endless_10",
            name = "Endless Explorer",
            description = "Reach Wave 25 (10 endless waves)",
            type = AchievementType.Simple,
            targetValue = 1,
            points = 60,
            isSecret = false
        },
        
        new Achievement
        {
            id = "endless_25",
            name = "Infinity Warrior",
            description = "Reach Wave 40 (25 endless waves)",
            type = AchievementType.Simple,
            targetValue = 1,
            points = 100,
            isSecret = false
        },
        
        new Achievement
        {
            id = "speed_demon",
            name = "Speed Demon",
            description = "Complete a wave in under 30 seconds",
            type = AchievementType.Simple,
            targetValue = 1,
            points = 30,
            isSecret = false
        },
        
        // BOSS & ELITE COMBAT (4)
        new Achievement
        {
            id = "first_boss",
            name = "Boss Hunter",
            description = "Defeat your first boss",
            type = AchievementType.Count,
            targetValue = 1,
            points = 25,
            isSecret = false
        },
        
        new Achievement
        {
            id = "boss_slayer",
            name = "Boss Slayer",
            description = "Defeat 10 bosses",
            type = AchievementType.Count,
            targetValue = 10,
            points = 75,
            isSecret = false
        },
        
        new Achievement
        {
            id = "untouchable",
            name = "Untouchable",
            description = "Defeat a boss without taking damage",
            type = AchievementType.Simple,
            targetValue = 1,
            points = 50,
            isSecret = false
        },
        
        new Achievement
        {
            id = "damage_dealer",
            name = "Damage Dealer",
            description = "Deal 10,000 total damage in one run",
            type = AchievementType.Count,
            targetValue = 10000,
            points = 40,
            isSecret = false
        },
        
        // CHARACTER & PROGRESSION (5)
        new Achievement
        {
            id = "first_unlock",
            name = "Character Collector",
            description = "Unlock your first character",
            type = AchievementType.Count,
            targetValue = 1,
            points = 15,
            isSecret = false
        },
        
        new Achievement
        {
            id = "advanced_collector",
            name = "Advanced Collector",
            description = "Unlock 5 advanced characters",
            type = AchievementType.Count,
            targetValue = 5,
            points = 60,
            isSecret = false
        },
        
        new Achievement
        {
            id = "legendary_hunter",
            name = "Legendary Hunter",
            description = "Unlock a legendary character",
            type = AchievementType.Count,
            targetValue = 1,
            points = 100,
            isSecret = false
        },
        
        new Achievement
        {
            id = "max_level",
            name = "Experience Master",
            description = "Reach maximum level in a single run",
            type = AchievementType.Simple,
            targetValue = 1,
            points = 45,
            isSecret = false
        },
        
        new Achievement
        {
            id = "currency_hoarder",
            name = "Currency Hoarder",
            description = "Save up 5,000 coins",
            type = AchievementType.Count,
            targetValue = 5000,
            points = 35,
            isSecret = false
        },
        
        // WEAPON & EQUIPMENT (3)
        new Achievement
        {
            id = "weapon_collector",
            name = "Weapon Collector",
            description = "Use all 10 weapon types",
            type = AchievementType.Count,
            targetValue = 10,
            points = 50,
            isSecret = false
        },
        
        new Achievement
        {
            id = "upgrade_master",
            name = "Upgrade Master",
            description = "Upgrade weapons 20 times",
            type = AchievementType.Count,
            targetValue = 20,
            points = 40,
            isSecret = false
        },
        
        new Achievement
        {
            id = "recycling_expert",
            name = "Recycling Expert",
            description = "Recycle 10 weapons for currency",
            type = AchievementType.Count,
            targetValue = 10,
            points = 25,
            isSecret = false
        },
        
        // SURVIVAL & CHALLENGE (4)
        new Achievement
        {
            id = "minimalist",
            name = "Minimalist",
            description = "Complete 5 waves without buying from shop",
            type = AchievementType.Simple,
            targetValue = 1,
            points = 55,
            isSecret = false
        },
        
        new Achievement
        {
            id = "survivor",
            name = "Survivor",
            description = "Survive 10 waves with less than 20% health",
            type = AchievementType.Count,
            targetValue = 10,
            points = 45,
            isSecret = false
        },
        
        new Achievement
        {
            id = "dodge_master",
            name = "Dodge Master",
            description = "Dodge 100 attacks",
            type = AchievementType.Count,
            targetValue = 100,
            points = 35,
            isSecret = false
        },
        
        new Achievement
        {
            id = "marathon_runner",
            name = "Marathon Runner",
            description = "Survive for 30 minutes in endless mode",
            type = AchievementType.Simple,
            targetValue = 1,
            points = 80,
            isSecret = false
        },
        
        // COLLECTION & ECONOMY (4)
        new Achievement
        {
            id = "candy_collector",
            name = "Sweet Tooth",
            description = "Collect 1,000 candy (XP)",
            type = AchievementType.Count,
            targetValue = 1000,
            points = 30,
            isSecret = false
        },
        
        new Achievement
        {
            id = "gem_hunter",
            name = "Gem Hunter",
            description = "Collect 100 gems",
            type = AchievementType.Count,
            targetValue = 100,
            points = 50,
            isSecret = false
        },
        
        new Achievement
        {
            id = "shop_addict",
            name = "Shop Addict",
            description = "Make 50 shop purchases",
            type = AchievementType.Count,
            targetValue = 50,
            points = 40,
            isSecret = false
        },
        
        new Achievement
        {
            id = "perfectionist",
            name = "Perfectionist",
            description = "Complete a full run without dying",
            type = AchievementType.Simple,
            targetValue = 1,
            points = 100,
            isSecret = true
        }
    };
    
    [Header("Achievement Categories")]
    public List<string> categories = new List<string>
    {
        "Combat",
        "Progression", 
        "Survival",
        "Collection",
        "Challenge",
        "Secret"
    };
    
    public Achievement GetAchievement(string id)
    {
        return achievements.Find(a => a.id == id);
    }
    
    public List<Achievement> GetAchievementsByCategory(string category)
    {
        List<Achievement> categoryAchievements = new List<Achievement>();
        
        foreach (var achievement in achievements)
        {
            bool matches = false;
            
            switch (category.ToLower())
            {
                case "combat":
                    matches = achievement.id.Contains("kill") || achievement.id.Contains("critical") || 
                             achievement.id.Contains("damage") || achievement.id.Contains("melee") ||
                             achievement.id.Contains("throwing") || achievement.id.Contains("life_steal");
                    break;
                    
                case "progression":
                    matches = achievement.id.Contains("wave") || achievement.id.Contains("endless") ||
                             achievement.id.Contains("unlock") || achievement.id.Contains("level") ||
                             achievement.id.Contains("boss");
                    break;
                    
                case "survival":
                    matches = achievement.id.Contains("survivor") || achievement.id.Contains("untouchable") ||
                             achievement.id.Contains("dodge") || achievement.id.Contains("marathon") ||
                             achievement.id.Contains("minimalist");
                    break;
                    
                case "collection":
                    matches = achievement.id.Contains("collector") || achievement.id.Contains("currency") ||
                             achievement.id.Contains("candy") || achievement.id.Contains("gem") ||
                             achievement.id.Contains("weapon_collector");
                    break;
                    
                case "challenge":
                    matches = achievement.id.Contains("speed") || achievement.id.Contains("upgrade") ||
                             achievement.id.Contains("recycling") || achievement.id.Contains("shop");
                    break;
                    
                case "secret":
                    matches = achievement.isSecret;
                    break;
            }
            
            if (matches)
                categoryAchievements.Add(achievement);
        }
        
        return categoryAchievements;
    }
    
    [Header("Statistics")]
    public int TotalAchievements => achievements.Count;
    public int TotalPoints => achievements.Sum(a => a.points);
}
