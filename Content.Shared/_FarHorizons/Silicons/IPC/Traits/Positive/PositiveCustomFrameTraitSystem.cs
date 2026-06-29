using Content.Shared._FarHorizons.UI.BackgroundTraits;
using Content.Shared.Body.Systems;

namespace Content.Shared._FarHorizons.Silicons.IPC.Traits.Positive;

public sealed class DeluxeOilFilterTraitSystem : BackgroundTraitSystem<DeluxeOilFilterTraitComponent>
{
    [Dependency] private readonly SharedBloodstreamSystem _bloodstream = default!;
    protected override void TraitInit(Entity<BackgroundTraitComponent, DeluxeOilFilterTraitComponent> ent)
    {
        _bloodstream.SetBloodRefreshRate(ent.Owner, 2.0);
        _bloodstream.SetBloodReductionAmount(ent.Owner, 0.33f);
    }
}