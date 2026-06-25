using Content.Shared._FarHorizons.UI.BackgroundTraits;
using Content.Shared._FarHorizons.Vampire;
using Robust.Client.UserInterface;
using Robust.Shared.Prototypes;

namespace Content.Client._FarHorizons.UI.BackgroundTraits;

public sealed partial class BackgroundTraitsBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    private VampireTraitsWindow? _window;

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<VampireTraitsWindow>();
        _window.SelectTraitsCallback += SubmitTraits;
    }

    private void SubmitTraits(List<ProtoId<BackgroundTraitPrototype>> traits) =>
        SendMessage(new SubmitVampireTraitSelectionMessage(traits));
}