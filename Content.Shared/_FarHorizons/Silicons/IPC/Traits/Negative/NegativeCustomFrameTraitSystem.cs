using Content.Shared._FarHorizons.IPC.Traits;
using Content.Shared._FarHorizons.Silicons.IPC.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Tag;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;

namespace Content.Shared._FarHorizons.Silicons.IPC.Traits.Negative;

public sealed class BloodPoweredTraitTraitSystem : IPCTraitSystem<BloodPoweredTraitComponent>
{
    [Dependency] private readonly ItemSlotsSystem _itemSlots = default!;
    protected override void TraitInit(Entity<IPCBrainHolderComponent, BloodPoweredTraitComponent> ent)
    {
        if(!TryComp<IPCBatteryComponent>(ent.Owner, out var ipcBattery))
            return;

        ipcBattery.DrainAllowedTargets.Clear();
        Dirty(ent.Owner, ipcBattery);
        
        if(!_itemSlots.TryGetSlot(ent.Owner, ipcBattery.PowerCellSlot.CellSlotId, out var cellSlot))
            return;

        _itemSlots.SetDisableEject(ent.Owner, cellSlot, true);
        _itemSlots.SetSwap(ent.Owner, cellSlot, false);
    }
}

public sealed class MicroreactorIncompatibilityTraitSystem : IPCTraitSystem<MicroreactorIncompatibilityComponent>
{
    [Dependency] private readonly ItemSlotsSystem _itemSlots = default!;
    protected override void TraitInit(Entity<IPCBrainHolderComponent, MicroreactorIncompatibilityComponent> ent)
    {
        if(!TryComp<IPCBatteryComponent>(ent.Owner, out var ipcBattery) || 
            !_itemSlots.TryGetSlot(ent.Owner, ipcBattery.PowerCellSlot.CellSlotId, out var cellSlot))
                return;

        _itemSlots.SetDisableEject(ent.Owner, cellSlot, true);
        var blackList = new EntityWhitelist
        {
            Tags = new List<ProtoId<TagPrototype>>()
        };
        blackList.Tags.Add("PowerCellMicroreactor");
        _itemSlots.SetBlacklist(ent.Owner, cellSlot, blackList);
    }
}