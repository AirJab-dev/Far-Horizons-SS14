using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._FarHorizons.UI.BackgroundTraits;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class BackgroundTraitsComponent : Component
{
    [DataField] public EntProtoId TraitsAction = "ActionOpenVampireTraits";
    [DataField] public Enum TraitsUiKey = BackgroundTraitsUiKey.Key;
    [DataField, AutoNetworkedField] public List<ProtoId<BackgroundTraitPrototype>> Traits = new();
    [DataField] public bool AllowTraitSelection = true;
    [ViewVariables(VVAccess.ReadOnly)] public List<ProtoId<BackgroundTraitPrototype>>? SelectedTraits;
}