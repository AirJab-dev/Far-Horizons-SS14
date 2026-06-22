using Content.Shared._FarHorizons.Silicons.IPC.Components;
using Content.Shared.Repairable;
using Content.Shared.Silicons.Borgs.Components;
using Robust.Shared.Containers;

namespace Content.Server._FarHorizons.Silicons.IPC;

public sealed partial class IPCSystem
{
    private void InitializeModules()
    {
        SubscribeLocalEvent<IPCModulesComponent, EntInsertedIntoContainerMessage>(InstallModule);
        SubscribeLocalEvent<IPCModulesComponent, EntRemovedFromContainerMessage>(UninstallModule);
    }

    private void InstallModule(Entity<IPCModulesComponent> ent, ref EntInsertedIntoContainerMessage args) 
    {
        if (!TryComp<BorgModuleComponent>(args.Entity, out var module) || args.Container.ID != ent.Comp.ModuleContainerId)
            return;

        if (module.Installed)
            return;

        module.InstalledEntity = ent.Owner;
        Dirty(args.Entity, module);
        var ev = new BorgModuleInstalledEvent(ent.Owner);
        RaiseLocalEvent(args.Entity, ref ev);
    }

    private void UninstallModule(Entity<IPCModulesComponent> ent, ref EntRemovedFromContainerMessage args) 
    {
        if (!TryComp<BorgModuleComponent>(args.Entity, out var module))
            return;

        if (!module.Installed || args.Container.ID != ent.Comp.ModuleContainerId)
            return;

        module.InstalledEntity = null;
        Dirty(args.Entity, module);
        var ev = new BorgModuleUninstalledEvent(ent.Owner);
        RaiseLocalEvent(args.Entity, ref ev);
    }
}