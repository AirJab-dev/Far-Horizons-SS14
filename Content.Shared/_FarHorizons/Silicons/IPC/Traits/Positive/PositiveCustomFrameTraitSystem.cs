using Content.Shared._FarHorizons.IPC.Traits;
using Content.Shared._FarHorizons.Silicons.IPC.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Tag;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;

namespace Content.Shared._FarHorizons.Silicons.IPC.Traits.Positive;

public sealed class CyborgModuleTraitTraitSystem : IPCTraitSystem<CyborgModuleTraitComponent>
{
    [Dependency] private readonly ItemSlotsSystem _itemSlots = default!;
    protected override void TraitInit(Entity<IPCBrainHolderComponent, CyborgModuleTraitComponent> ent)
    {
        var borgSlot = new ItemSlot()
        {
            Name = "Borg Module",
            Whitelist = new EntityWhitelist()
            {
                Tags = new List<ProtoId<TagPrototype>>()
                {
                    "BorgModuleIPCCompatible"
                }
            },
        };

        _itemSlots.AddItemSlot(ent.Owner, "borg_module", borgSlot);

        _itemSlots.SetBlacklist(ent.Owner, borgSlot, new EntityWhitelist()
        {
            Tags = new List<ProtoId<TagPrototype>>()
            {
                "BorgModuleIPCIncompatible"
            }
        }, replaceExisting: true);
    }
}