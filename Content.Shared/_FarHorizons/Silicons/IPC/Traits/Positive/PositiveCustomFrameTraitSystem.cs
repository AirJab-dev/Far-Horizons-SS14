using Content.Shared._FarHorizons.IPC.Traits;
using Content.Shared._FarHorizons.Silicons.IPC.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Movement.Systems;
using Content.Shared.PowerCell;
using Content.Shared.PowerCell.Components;
using Content.Shared.StatusEffectNew;
using Content.Shared.Tag;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;

namespace Content.Shared._FarHorizons.Silicons.IPC.Traits.Positive;

public sealed class CyborgModuleTraitSystem : IPCTraitSystem<CyborgModuleTraitComponent>
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

public sealed class OverclockingTraitSystem : IPCToggleActionTraitSystem<OverclockingTraitComponent, OverclockingTraitEvent>
{
    [Dependency] private readonly MovementModStatusSystem _movementMod = default!;
    [Dependency] private readonly StatusEffectsSystem _status = default!;
    [Dependency] private readonly PowerCellSystem _power = default!;
    protected override void OnToggled(Entity<IPCBrainHolderComponent, OverclockingTraitComponent> ent, bool toggle)
    {
        if(TryComp<PowerCellDrawComponent>(ent.Owner, out var pcdComp))
        {
            if(toggle)
            {
                _power.SetDrawRate( ent.Owner, pcdComp.DrawRate * ent.Comp2.drawRateMultiplier);
                _status.TrySetStatusEffectDuration(ent.Owner, MovementModStatusSystem.ReagentSpeed, out var status);
                _movementMod.TryUpdateMovementStatus(ent.Owner, status!.Value, ent.Comp2.speedModifier, ent.Comp2.speedModifier);
                _status.TrySetStatusEffectDuration(ent.Owner, "StatusEffectIPCFanDisabled");
            }
            else if(!toggle)
            {
                _power.SetDrawRate( ent.Owner, pcdComp.DrawRate / ent.Comp2.drawRateMultiplier);
                _status.TryRemoveStatusEffect(ent.Owner, MovementModStatusSystem.ReagentSpeed);
                _status.TryRemoveStatusEffect(ent.Owner, "StatusEffectIPCFanDisabled");
            }
        }
    }
}