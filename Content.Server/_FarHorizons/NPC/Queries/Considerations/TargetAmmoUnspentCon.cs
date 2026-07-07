using Content.Server.NPC;
using Content.Server.NPC.Queries.Considerations;
using Content.Shared.Weapons.Ranged.Components;

namespace Content.Server._FarHorizons.NPC.Queries.Considerations;

/// <summary>
/// Returns 1f where the specified target is valid for the active hand's whitelist.
/// </summary>
public sealed partial class TargetAmmoUnspentCon : ExternalConsideration
{
    public override float GetScore(NPCBlackboard blackboard, EntityUid targetUid, IEntityManager entMan) => 
        !entMan.TryGetComponent<CartridgeAmmoComponent>(targetUid, out var cartridgeAmmo) ? 0f : cartridgeAmmo.Spent ? 0f : 1f;
}
