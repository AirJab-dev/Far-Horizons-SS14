using Content.Shared._FarHorizons.IPC.Traits;
using Robust.Shared.GameStates;

namespace Content.Shared._FarHorizons.Silicons.IPC.Traits.Positive;

[RegisterComponent]
public sealed partial class CyborgModuleTraitComponent : IPCTraitComponent;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class OverclockingTraitComponent : IPCToggleActionComponent
{
    [DataField] public float drawRateMultiplier = 2f;
    [DataField] public float speedModifier = 1.35f;
}