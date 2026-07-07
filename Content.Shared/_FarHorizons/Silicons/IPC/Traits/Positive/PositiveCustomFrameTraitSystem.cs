using Content.Shared._FarHorizons.IPC.Traits;
using Content.Shared._FarHorizons.Silicons.IPC.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Damage.Components;
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
        if(!TryComp<PowerCellDrawComponent>(ent.Owner, out var pcdComp))
            return;

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

public sealed class RepairNanitesTraitSystem : IPCToggleActionTraitSystem<RepairNanitesTraitComponent, RepairNanitesTraitEvent>
{
    [Dependency] private readonly PowerCellSystem _power = default!;
    [Dependency] private readonly StatusEffectsSystem _status = default!;
    protected override void TraitInit(Entity<IPCBrainHolderComponent, RepairNanitesTraitComponent> ent)
    {
        base.TraitInit(ent);

        if(!TryComp<PassiveDamageComponent>(ent.Owner, out var psdComp)) return;

        ent.Comp2.oldDamage = psdComp.Damage;
        ent.Comp2.oldDamageCap = psdComp.DamageCap;
        ent.Comp2.oldAllowedStates = psdComp.AllowedStates;
    }

    protected override void OnToggled(Entity<IPCBrainHolderComponent, RepairNanitesTraitComponent> ent, bool toggle)
    {
        if(!TryComp<PowerCellDrawComponent>(ent.Owner, out var pcdComp) || !TryComp<PassiveDamageComponent>(ent.Owner, out var psdComp))
            return;
            
        if(toggle)
        {
            _power.SetDrawRate( ent.Owner, pcdComp.DrawRate * ent.Comp2.drawRateMultiplier);
            _status.TrySetStatusEffectDuration(ent.Owner, "StatusEffectIPCFanDisabled");
            psdComp.Damage = ent.Comp2.Damage;
            psdComp.DamageCap = ent.Comp2.DamageCap;
            psdComp.AllowedStates = ent.Comp2.AllowedStates;
            Dirty(ent.Owner, psdComp);
        }
        else if(!toggle)
        {
            _power.SetDrawRate( ent.Owner, pcdComp.DrawRate / ent.Comp2.drawRateMultiplier);
            _status.TryRemoveStatusEffect(ent.Owner, "StatusEffectIPCFanDisabled");
            psdComp.Damage = ent.Comp2.oldDamage;
            psdComp.DamageCap = ent.Comp2.oldDamageCap;
            psdComp.AllowedStates = ent.Comp2.oldAllowedStates;
            Dirty(ent.Owner, psdComp);
        }
    }
}

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