using Content.Shared._FarHorizons.IPC.Traits;
using Content.Shared._FarHorizons.Silicons.IPC.Components;

namespace Content.Shared._FarHorizons.Silicons.IPC.Traits.Negative;

public sealed class SetDamageModifierTraitSystem : IPCTraitSystem<BloodPoweredTraitComponent>
{
    protected override void TraitInit(Entity<IPCBrainHolderComponent, BloodPoweredTraitComponent> ent)
    {
        if(!TryComp<IPCBatteryComponent>(ent.Owner, out var ipcBattery))
            return;

        ipcBattery.DrainAllowedTargets.Clear();
        Dirty(ent.Owner, ipcBattery);
    }
}