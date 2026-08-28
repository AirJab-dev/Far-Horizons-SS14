using Content.Shared._FarHorizons.Fluids;
using Content.Shared.GameTicking;

public sealed partial class FluidFootprintSystem : SharedFluidFootprintSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RoundRestartCleanupEvent>(_ => ClearCache());
    }
}