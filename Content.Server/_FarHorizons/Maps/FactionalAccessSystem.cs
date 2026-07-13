using System.Linq;
using Content.Server.Station.Components;
using Content.Server.Station.Events;
using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Station.Components;
using Robust.Shared.Prototypes;

namespace Content.Server._FarHorizons.Maps.FactionalAccess;

public sealed partial class FactionalAccessSystem : EntitySystem
{
    [Dependency] private readonly AccessReaderSystem _access = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<FactionalAccessComponent, StationPostInitEvent>(OnStationPostInit);
    }

    private void OnStationPostInit(Entity<FactionalAccessComponent> ent, ref StationPostInitEvent args)
    {
        if (!TryComp<StationDataComponent>(args.Station, out var stationData))
            return;

        foreach (var grid in stationData.Grids)
        {
            var readers = _lookup.GetEntitiesIntersecting(grid, LookupFlags.Uncontained | LookupFlags.Static);

            foreach (var uid in readers)
            {
                if (!TryComp<AccessReaderComponent>(uid, out var accessComp))
                    continue;

                var oldAccessList = accessComp.AccessLists
                    .Select(set => new HashSet<ProtoId<AccessLevelPrototype>>(set))
                    .ToList();

                if (oldAccessList.Count == 0)
                    continue;

                var newAccessList = new List<HashSet<ProtoId<AccessLevelPrototype>>>();

                foreach (var accessSet in oldAccessList)
                {
                    var newSet = new HashSet<ProtoId<AccessLevelPrototype>>();

                    foreach (var access in accessSet)
                    {
                        newSet.Add(ent.Comp.EquivalentAccessList.TryGetValue(access, out var equivalent)
                            ? equivalent
                            : access);
                    }

                    newAccessList.Add(newSet);
                }

                _access.TryRemoveAccesses((uid, accessComp), oldAccessList);
                _access.TryAddAccesses((uid, accessComp), newAccessList);
            }
        }
    }
}