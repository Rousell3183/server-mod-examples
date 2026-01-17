using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services.Mod;

namespace _26WeaponVariant;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "com.sp-tarkov.10mm";
    public override string Name { get; init; } = "WeaponVariant10mm";
    public override string Author { get; init; } = "ElCapnCrippl";
    public override List<string>? Contributors { get; init; }
    public override SemanticVersioning.Version Version { get; init; } = new("1.0.0");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");
    
    
    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public override string? Url { get; init; }
    public override bool? IsBundleMod { get; init; }
    public override string License { get; init; } = "MIT";
}

// Unique IDs for our custom items
public static class Custom10mmIds
{
    public const string AMMO_10MM_JHP = "696bbd26ddc277320902e641";
    public const string MP5_10MM_RECEIVER = "696bbd26ddc277320902e63f";
    public const string UMP45_10MM_RECEIVER = "696bbd26ddc277320902e642";
    public const string CHAMBER_SLOT_ID = "696bbd26ddc277320902e643";
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class WeaponVariant10mm(
    ISptLogger<WeaponVariant10mm> logger,
    CustomItemService customItemService) : IOnLoad
{

    public Task OnLoad()
    {
        logger.Info("Starting 10mm weapon variant creation");

        // Step 1: Create 10mm JHP Ammo
        CreateAmmo10mmJhp();

        // Step 2: Create MP5 10mm Receiver
        CreateMP510mmReceiver();

        // Step 3: Create UMP45 10mm Receiver
        CreateUMP4510mmReceiver();

        logger.Success("10mm weapon variants created successfully");
        return Task.CompletedTask;
    }

    private void CreateAmmo10mmJhp()
    {
        var ammo10mmJhp = new NewItemFromCloneDetails
        {
            ItemTplToClone = ItemTpl.AMMO_9X19_QUAKEMAKER, // Clone QuakeMaker as base (JHP profile)
            ParentId = "5485a8684bdc2da71d8b4567", // Ammunition parent ID
            NewId = Custom10mmIds.AMMO_10MM_JHP,
            FleaPriceRoubles = 450,
            HandbookPriceRoubles = 400,
            HandbookParentId = "5b47574386f77428ca22b33b", // Ammo handbook category
            Locales = new Dictionary<string, LocaleDetails>
            {
                {
                    "en", new LocaleDetails
                    {
                        Name = "10x25mm JHP",
                        ShortName = "10mm JHP",
                        Description = "10x25mm Auto JHP (Jacketed Hollow Point) cartridge with 180-grain hollow-point bullet. Designed for maximum stopping power with excellent expansion characteristics. Offers superior ballistic performance compared to 9mm with higher velocity and energy."
                    }
                }
            },
            OverrideProperties = new TemplateItemProperties
            {
                // Ballistics - scaled from 9x19 Quakemaker to 10mm
                Damage = 135, // 85 * 1.58x multiplier for 10mm power
                PenetrationPower = 11, // Slightly higher due to caliber, but JHP reduces penetration
                ArmorDamage = 32, // Scaled armor damage
                InitialSpeed = 420, // Higher velocity for 10mm
                
                // Physical properties
                BulletDiameterMilimeters = 10,
                BulletMassGram = 11.66f, // 180 grains in grams
                Caliber = "Caliber10mm",
                Weight = 0.016f, // Heavier than 9mm
                
                // Ballistic coefficient and behavior
                BallisticCoeficient = 0.140f,
                SpeedRetardation = 0.0001f,
                
                // Bleeding and trauma
                HeavyBleedingDelta = 0.25f, // Good expansion for JHP
                LightBleedingDelta = 0.18f,
                
                // Fragmentation (JHP characteristic - expansion not fragmentation)
                FragmentationChance = 0.08f, // Lower than armor-piercing
                RicochetChance = 0.08f,
                
                // Durability and reliability
                DurabilityBurnModificator = 1.12f,
                MalfFeedChance = 0.110f,
                MalfMisfireChance = 0.135f,
                MisfireChance = 0.03f,
                
                // Heat and recoil
                HeatFactor = 0.99f,
                
                // Ammo properties
                AmmoTooltipClass = "Expansive",
                RarityPvE = "Rare",
                
                // Stamina
                StaminaBurnPerDamage = 0.192f,
                
                // Stack settings
                StackMaxSize = 50,
                StackMaxRandom = 40,
                StackMinRandom = 15,
            }
        };

        customItemService.CreateItemFromClone(ammo10mmJhp);
        logger.Info("Created 10mm JHP ammunition");
    }

    private void CreateMP510mmReceiver()
    {
        var mp510mm = new NewItemFromCloneDetails
        {
            ItemTplToClone = ItemTpl.RECIEVER_MP5_9X19,
            ParentId = "55818a304bdc2db5418b457d", // Assault rifle parent
            NewId = Custom10mmIds.MP5_10MM_RECEIVER,
            FleaPriceRoubles = 85000,
            HandbookPriceRoubles = 65000,
            HandbookParentId = "5b5f796a86f774093f2ed3c0", // Submachine gun handbook category
            Locales = new Dictionary<string, LocaleDetails>
            {
                {
                    "en", new LocaleDetails
                    {
                        Name = "MP5 10mm Receiver",
                        ShortName = "MP5 10mm RX",
                        Description = "MP5 upper receiver converted to fire 10x25mm ammunition. Maintains the renowned reliability and accuracy of the MP5 platform while stepping up to 10mm Auto power."
                    }
                }
            },
            OverrideProperties = new TemplateItemProperties
            {
                Chambers =
                [
                    new Slot
                    {
                        Name = "patron_in_weapon_000",
                        Id = Custom10mmIds.CHAMBER_SLOT_ID,
                        Parent = Custom10mmIds.MP5_10MM_RECEIVER,
                        Properties = new SlotProperties
                        {
                            Filters =
                            [
                                new SlotFilter
                                {
                                    Filter =
                                    [
                                        Custom10mmIds.AMMO_10MM_JHP // Only accepts our custom 10mm JHP
                                    ]
                                }
                            ]
                        },
                        Required = false,
                        MergeSlotWithChildren = false,
                        Prototype = "55d4af244bdc2d962f8b4571"
                    }
                ]
            },
        };

        customItemService.CreateItemFromClone(mp510mm);
        logger.Info("Created MP5 10mm receiver");
    }

    private void CreateUMP4510mmReceiver()
    {
        var ump4510mm = new NewItemFromCloneDetails
        {
            ItemTplToClone = ItemTpl.RECIEVER_UMP45,
            ParentId = "55818a304bdc2db5418b457d", // Assault rifle parent
            NewId = Custom10mmIds.UMP45_10MM_RECEIVER,
            FleaPriceRoubles = 90000,
            HandbookPriceRoubles = 70000,
            HandbookParentId = "5b5f796a86f774093f2ed3c0", // Submachine gun handbook category
            Locales = new Dictionary<string, LocaleDetails>
            {
                {
                    "en", new LocaleDetails
                    {
                        Name = "UMP45 10mm Receiver",
                        ShortName = "UMP45 10mm RX",
                        Description = "UMP45 upper receiver chambered in 10x25mm Auto. Combines the versatile ergonomics and modularity of the UMP platform with the stopping power of 10mm ammunition."
                    }
                }
            },
            OverrideProperties = new TemplateItemProperties
            {
                Chambers =
                [
                    new Slot
                    {
                        Name = "patron_in_weapon_000",
                        Id = "696bbd26ddc277320902e644", // Unique ID for UMP chamber
                        Parent = Custom10mmIds.UMP45_10MM_RECEIVER,
                        Properties = new SlotProperties
                        {
                            Filters =
                            [
                                new SlotFilter
                                {
                                    Filter =
                                    [
                                        Custom10mmIds.AMMO_10MM_JHP // Only accepts our custom 10mm JHP
                                    ]
                                }
                            ]
                        },
                        Required = false,
                        MergeSlotWithChildren = false,
                        Prototype = "55d4af244bdc2d962f8b4571"
                    }
                ]
            },
        };

        customItemService.CreateItemFromClone(ump4510mm);
        logger.Info("Created UMP45 10mm receiver");
    }
}
