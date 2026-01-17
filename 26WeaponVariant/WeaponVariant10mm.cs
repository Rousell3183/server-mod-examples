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
    public const string AMMO_BOX_10MM_JHP = "696bbd26ddc277320902e645";
    public const string MP5_10MM_RECEIVER = "696bbd26ddc277320902e63f";
    public const string MP5_10MM_BARREL = "696bbd26ddc277320902e646";
    public const string UMP45_10MM_RECEIVER = "696bbd26ddc277320902e642";
    public const string UMP45_10MM_BARREL = "696bbd26ddc277320902e647";
    public const string CHAMBER_SLOT_ID = "696bbd26ddc277320902e643";
    
    // Trader IDs
    public const string PEACEKEEPER_ID = "5a7c2eca46aef81a7ca2145d";
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

        // Step 2: Create 10mm JHP Ammo Box
        CreateAmmoBox10mmJhp();

        // Step 3: Create MP5 10mm Receiver
        CreateMP510mmReceiver();

        // Step 4: Create MP5 10mm Barrel
        CreateMP510mmBarrel();

        // Step 5: Create UMP45 10mm Receiver
        CreateUMP4510mmReceiver();

        // Step 6: Create UMP45 10mm Barrel
        CreateUMP4510mmBarrel();

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
                // Prefab - Custom 10mm JHP round model
                Prefab = new PrefabDetails
                {
                    path = "26WeaponVariant\bundles\patron_10mm_jhp.bundle",
                    rcid = ""
                },
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

    private void CreateAmmoBox10mmJhp()
    {
        var ammoBox10mm = new NewItemFromCloneDetails
        {
            ItemTplToClone = ItemTpl.AMMO_BOX_556X45_M193, // Clone from 5.56 ammo box
            ParentId = "543be5cb4bdc2deb348b4568", // Ammunition Box parent ID
            NewId = Custom10mmIds.AMMO_BOX_10MM_JHP,
            FleaPriceRoubles = 2200,
            HandbookPriceRoubles = 2000,
            HandbookParentId = "5b47574386f77428ca22b33b", // Ammo handbook category
            Locales = new Dictionary<string, LocaleDetails>
            {
                {
                    "en", new LocaleDetails
                    {
                        Name = "10x25mm JHP Ammo Box",
                        ShortName = "10mm JHP Box",
                        Description = "A 50-round box of 10x25mm Auto JHP ammunition. Contains 180-grain hollow-point cartridges designed for maximum stopping power and expansion."
                    }
                }
            },
            OverrideProperties = new TemplateItemProperties
            {
                // Prefab - Custom 10mm JHP ammo box model (TBD)
                Prefab = new PrefabDetails
                {
                    path = "26WeaponVariant\bundles\item_ammo_box_10mm_jhp_50_rnd.bundle", // CUSTOM MODEL - ADD YOUR BUNDLE PATH HERE
                    rcid = ""
                },
                // Box properties
                Height = 1,
                Width = 2,
                Weight = 0.8f, // Total weight for 50 rounds in box
                
                // Stack settings - boxes cannot stack, but contain 50 rounds that can stack to 100
                StackMaxSize = 1,
                StackMaxRandom = 1,
                StackMinRandom = 1,
                StackObjectsCount = 1,
                
                // Misc properties
                BackgroundColor = "yellow",
                RarityPvE = "Rare",
                ItemSound = "ammo_pack_generic",
                HideEntrails = true,
                InsuranceDisabled = true,
                ExamineExperience = 10,
                ExamineTime = 1,
                CanSellOnRagfair = true,
                CanRequireOnRagfair = false,
            }
        };

        customItemService.CreateItemFromClone(ammoBox10mm);
        logger.Info("Created 10mm JHP ammo box");
    }

    private void CreateMP510mmReceiver()
    {
        var mp510mm = new NewItemFromCloneDetails
        {
            ItemTplToClone = ItemTpl.RECIEVER_MP5_9X19,
            ParentId = "5447b5e04bdc2d62278b4567", // Submachinegun parent
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
                // Prefab - Uses existing MP5 receiver model
                Prefab = new PrefabDetails
                {
                    path = "assets/content/items/mods/recievers/reciever_mp5_hk_std.bundle",
                    rcid = ""
                },
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

    private void CreateMP510mmBarrel()
    {
        // Note: Barrel model TBD - will use existing MP5 barrel bundle path
        // TODO: Replace with custom 10mm barrel bundle when ready
        var mp510mmBarrel = new NewItemFromCloneDetails
        {
            ItemTplToClone = ItemTpl.BARREL_MP5_STD, // Clone from standard MP5 barrel
            ParentId = "555ef6e44bdc2de9068b457e", // Barrel parent
            NewId = Custom10mmIds.MP5_10MM_BARREL,
            FleaPriceRoubles = 12000,
            HandbookPriceRoubles = 9500,
            HandbookParentId = "5b5f75c686f774094242f19f", // Barrels handbook category
            Locales = new Dictionary<string, LocaleDetails>
            {
                {
                    "en", new LocaleDetails
                    {
                        Name = "MP5 10mm Barrel",
                        ShortName = "MP5 10mm Barrel",
                        Description = "MP5 barrel chambered for 10x25mm ammunition. Converted to fire 10mm Auto cartridges."
                    }
                }
            },
            OverrideProperties = new TemplateItemProperties
            {
                // Prefab - TBD: Replace with custom 10mm barrel model when ready
                // Prefab = new PrefabDetails
                // {
                //     path = "assets/content/items/mods/barrels/barrel_mp5_10mm.bundle", // CUSTOM MODEL TBD
                //     rcid = ""
                // }
            }
        };

        customItemService.CreateItemFromClone(mp510mmBarrel);
        logger.Info("Created MP5 10mm barrel");
    }

    private void CreateUMP4510mmReceiver()
    {
        var ump4510mm = new NewItemFromCloneDetails
        {
            ItemTplToClone = ItemTpl.RECIEVER_UMP45,
            ParentId = "5447b5e04bdc2d62278b4567", // Submachinegun parent
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
                // Prefab - Uses existing UMP45 receiver model (TBD: Verify bundle path)
                // Prefab = new PrefabDetails
                // {
                //     path = "assets/content/items/mods/recievers/reciever_ump45_std.bundle", // VERIFY PATH
                //     rcid = ""
                // },
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

    private void CreateUMP4510mmBarrel()
    {
        // Note: Barrel model TBD - will use existing UMP45 barrel bundle path
        // TODO: Replace with custom 10mm barrel bundle when ready
        var ump4510mmBarrel = new NewItemFromCloneDetails
        {
            ItemTplToClone = ItemTpl.BARREL_UMP45_STD, // Clone from standard UMP45 barrel
            ParentId = "555ef6e44bdc2de9068b457e", // Barrel parent
            NewId = Custom10mmIds.UMP45_10MM_BARREL,
            FleaPriceRoubles = 14000,
            HandbookPriceRoubles = 11000,
            HandbookParentId = "5b5f75c686f774094242f19f", // Barrels handbook category
            Locales = new Dictionary<string, LocaleDetails>
            {
                {
                    "en", new LocaleDetails
                    {
                        Name = "UMP45 10mm Barrel",
                        ShortName = "UMP45 10mm Barrel",
                        Description = "UMP45 barrel converted to 10x25mm ammunition. Features optimized chamber and bore for 10mm Auto cartridges."
                    }
                }
            },
            OverrideProperties = new TemplateItemProperties
            {
                // Prefab - TBD: Replace with custom 10mm barrel model when ready
                // Prefab = new PrefabDetails
                // {
                //     path = "assets/content/items/mods/barrels/barrel_ump45_10mm.bundle", // CUSTOM MODEL TBD
                //     rcid = ""
                // }
            }
        };

        customItemService.CreateItemFromClone(ump4510mmBarrel);
        logger.Info("Created UMP45 10mm barrel");
    }
}
