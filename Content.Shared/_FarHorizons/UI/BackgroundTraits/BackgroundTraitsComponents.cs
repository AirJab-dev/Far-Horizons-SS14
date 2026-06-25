using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._FarHorizons.UI.BackgroundTraits;

[RegisterComponent, NetworkedComponent]
public sealed partial class BackgroundTraitsComponent : Component
{
    [DataField] public EntProtoId TraitsAction = "ActionOpenVampireTraits";
    [DataField] public Enum TraitsUiKey = BackgroundTraitsUiKey.Key;
    [DataField] public List<BackgroundTraitPrototype> Traits = [];
    [DataField] public bool AllowTraitSelection = true;
    [ViewVariables(VVAccess.ReadOnly)] public List<ProtoId<BackgroundTraitPrototype>>? SelectedTraits;
}