using Content.Shared.Access.Components;
using Content.Shared.Silicons.Borgs.Components;

namespace Content.Shared.Silicons.Borgs;

/// <inheritdoc/>
public abstract partial class SharedBorgSystem
{
    private void InitializeAccessModule()
        => SubscribeLocalEvent<BorgChassisComponent, GetAdditionalAccessEvent>(OnAdditionalAccess);
    private void OnAdditionalAccess(Entity<BorgChassisComponent> ent, ref GetAdditionalAccessEvent args)
    {
        foreach(var module in ent.Comp.ModuleContainer.ContainedEntities)
        {
            if(!HasComp<PassiveBorgModuleComponent>(module) || !HasComp<AccessComponent>(module))
                continue;    

            args.Entities.Add(module);
        }
    }
}