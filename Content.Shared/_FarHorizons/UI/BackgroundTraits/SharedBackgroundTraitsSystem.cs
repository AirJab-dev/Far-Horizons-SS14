using System.Linq;
using Content.Shared._FarHorizons.UI.BackgroundTraits;
using Content.Shared.Actions;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._FarHorizons.Vampire;

public abstract partial class SharedBackgroundTraitSystem : EntitySystem
{
    [Dependency] protected readonly IGameTiming Timing = default!;
    [Dependency] protected readonly IPrototypeManager ProtoMan = default!;
    [Dependency] protected readonly SharedActionsSystem _actions = default!;
    [Dependency] protected readonly IComponentFactory _compFactory = default!;
    [Dependency] protected readonly ILogManager _log = default!;
    [Dependency] protected readonly SharedUserInterfaceSystem _ui = default!;

    public override void Initialize()
        => base.Initialize();

    public List<ProtoId<BackgroundTraitPrototype>> ValidatedTraits(List<ProtoId<BackgroundTraitPrototype>> traits)
    {
        var result = new List<ProtoId<BackgroundTraitPrototype>>();
        var points = 0;

        foreach (var t in traits)
        {
            var proto = ProtoMan.Index(t);
            if (proto.Incompatible.Intersect(result).Any()) continue;

            points -= proto.Cost;
            result.Add(t);
        }

        return points >= 0 ? result : [];
    }
}