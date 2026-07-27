using System.Numerics;
using Content.Shared._FarHorizons.Audio;
using Content.Shared.Doors.Components;
using Content.Shared.Physics;
using Robust.Client.Audio;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.Map;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Systems;

namespace Content.Client._FarHorizons.Audio.CustomAudio;

public sealed class CustomAudioSystem : EntitySystem
{
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly IConfigurationManager _cfgManager = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;

    private float _maxRayLength;

    private const float MuffleDecayConstant = 0.3f;  
    private const float MaxOcclusion = 10f;

    public override void Initialize()
    {
        base.Initialize();
        Subs.CVar(_cfgManager, CVars.AudioRaycastLength, OnRaycastLengthChanged, true);
        _audio.GetOcclusionOverride += GetOcclusion;
    }

    public override void Shutdown()
    { 
        base.Shutdown();
        _audio.GetOcclusionOverride -= GetOcclusion;
    }

    private void OnRaycastLengthChanged(float value)
        => _maxRayLength = value;

    private float GetOcclusion(MapCoordinates listener, Vector2 delta, float distance, EntityUid? ignoredEnt)
    {
        if (distance <= 0.1f)
            return 0f;

        var rayLength = MathF.Min(distance, _maxRayLength);
        var ray = new CollisionRay(listener.Position, delta / distance, (int) (CollisionGroup.Opaque | CollisionGroup.Impassable));

        var results = _physics.IntersectRay(listener.MapId, ray, rayLength, ignoredEnt, returnOnFirstHit: false);

        var totaOcclusion = 0f;
        foreach (var result in results)
        {
            var occlusion = 0f; 
            if (!TryComp<StructureOcclusionComponent>(result.HitEntity, out var occlusionComp))
                continue;

            if(!occlusionComp.DoesOcclusionWorkWhenOpen 
                && TryComp<DoorComponent>(result.HitEntity, out var doorComp) 
                && doorComp.State is DoorState.Open or DoorState.Opening)
                    continue;

            occlusion = occlusionComp.OcclusionAmount;
            totaOcclusion += occlusionComp.OcclusionAmount;
        }

        var shaped = MaxOcclusion * (1f - MathF.Exp(-MuffleDecayConstant * totaOcclusion));

        return shaped;
    }
}