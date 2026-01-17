using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services.Mod;

namespace _18CustomItemService;

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

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class CustomItemServiceExample(
    ISptLogger<CustomItemServiceExample> logger,
    CustomItemService customItemService) : IOnLoad
{

    public Task OnLoad()
    {

        var ammo10mmTpls = new[]
        {
            ItemTpl.AMMO_10X25_AUTO_FMJ,
            ItemTpl.AMMO_10X25_AUTO_JHP,
            ItemTpl.AMMO_10X25_AUTO_AP,
            ItemTpl.AMMO_10X25_AUTO_SAP,
            ItemTpl.AMMO_10X25_AUTO_FTX,
        };

        //Example of adding new item by cloning an existing item using `createCloneDetails`
        var cloneMP5 = new NewItemFromCloneDetails
        {
            ItemTplToClone = ItemTpl.RECIEVER_MP5_9X19,
            // ParentId refers to the Node item the gun will be under, you can check it in https://db.sp-tarkov.com/search
            ParentId = "55818a304bdc2db5418b457d",
            // The new id of our cloned item - MUST be a valid mongo id, search online for mongo id generators
            NewId = "696bbd26ddc277320902e63f",
            // Flea price of item
            FleaPriceRoubles = 75000,
            // Price of item in handbook
            HandbookPriceRoubles = 55000,
            // Handbook Parent Id refers to the category the gun will be under
            HandbookParentId = "5b5f75b986f77447ec5d7710",
            //you see those side box tab thing that only select gun under specific icon? Handbook parent can be found in Spt_Data\Server\database\templates.
            Locales = new Dictionary<string, LocaleDetails>
            {
                {
                    "en", new LocaleDetails
                    {
                        Name = "MP5 10mm Reciever",
                        ShortName = "MP5 10mm Reciever",
                        Description = "MP5 reciever chambered in 10mm Auto."
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
                        Id = "61f7c9e189e6fb1a5e3ea791",
                        Parent = "696bbd26ddc277320902e63f",
                        Properties = new SlotProperties
                        {
                            Filters =
                            [
                                new SlotFilter
                                {
                                    Filter =
                                    [
                                        // TBD: Add 10mm Auto ammo TPLs here
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

        customItemService.CreateItemFromClone(cloneMP5); // Send our data to the function that creates our item
        
        return Task.CompletedTask;
    }
}
