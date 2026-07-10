using System.Numerics;
using Robust.Client.Audio;
using Robust.Client.Physics;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.Map;
using Robust.Shared.Physics;

public sealed class AudioMuffleSystem : EntitySystem
{
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly IConfigurationManager _cfgManager = default!;
    [Dependency] private readonly PhysicsSystem _physics = default!;
    private float _maxRayLength;

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
        
        float occlusion = 0;

        if (distance > 0.1)
        {
            var rayLength = MathF.Min(distance, _maxRayLength);
            var ray = new CollisionRay(listener.Position, delta / distance, _audio.OcclusionCollisionMask);
            var entitiesHit = _physics.IntersectRay(listener.MapId, ray, rayLength, ignoredEnt, returnOnFirstHit:false);
            occlusion = _physics.IntersectRayPenetration(listener.MapId, ray, rayLength, ignoredEnt);
        }

        return occlusion;
    }
}