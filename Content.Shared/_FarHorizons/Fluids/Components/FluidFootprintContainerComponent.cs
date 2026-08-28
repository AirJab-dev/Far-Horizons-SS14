using System.Numerics;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._FarHorizons.Fluids.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class FluidFootprintContainerComponent : Component
{
    [ViewVariables, AutoNetworkedField] public List<Color> ColorPalette = new();
    [ViewVariables, AutoNetworkedField] public List<ProtoId<FootprintTypePrototype>> ProtoPalette = new();
    [ViewVariables, AutoNetworkedField] public List<FootprintData> Footprints = new();
    [DataField] public EntProtoId? CleanEffect;

    // I compress the data to the best of my ability without losing anything.
    // If I did math right, that's around 75%-85% savings on memory and network data as compared to doing this naively
    public void AddFootprint(
        Vector2 position,
        Angle angle,
        ProtoId<FootprintTypePrototype> footprint,
        float size,
        Color color,
        bool flip,
        float opacity)
    {
        if (!ProtoPalette.Contains(footprint))
            ProtoPalette.Add(footprint);
        
        var protoId = (byte)ProtoPalette.IndexOf(footprint);

        if (!ColorPalette.Contains(color))
            ColorPalette.Add(color);
        
        var colorId = (byte)ColorPalette.IndexOf(color);

        var packedAngle = (ushort)(angle.Theta / MathF.Tau * ushort.MaxValue);

        var packedOpacity = (byte)Math.Clamp((int)(opacity * 255f), 0, 255);

        var result = new FootprintData(position, packedAngle, protoId, colorId, size, packedOpacity, flip);
        Footprints.Add(result);
    }

    public (
        Vector2 Position,
        Angle Angle,
        ProtoId<FootprintTypePrototype> Footprint,
        float Size,
        Color Color,
        bool Flip,
        float Opacity
        )
        Unpack(FootprintData data)
    {
        var angle = new Angle(data.Angle / (float)ushort.MaxValue * MathF.Tau);
        var footprint = ProtoPalette[data.ProtoId];
        var color = ColorPalette[data.ColorId];
        var opacity = data.Opacity / 255f;

        return (data.Position, angle, footprint, data.Size, color, data.Flip, opacity);
    }
}

[Serializable, NetSerializable]
public readonly record struct FootprintData
(
    Vector2 Position,
    ushort Angle,
    byte ProtoId,
    byte ColorId,
    float Size,
    byte Opacity,
    bool Flip
);