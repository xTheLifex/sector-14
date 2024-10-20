using Robust.Shared.GameStates;
using Robust.Shared.Utility;

namespace Content.Shared._SECTOR.Armor;

[RegisterComponent, NetworkedComponent, Access(typeof(EnergyShieldComponent))]
public sealed partial class EnergyShieldComponent : Component
{
    [DataField("color")]
    public Color SpriteColor = Color.White;

    //TODO: Energy shield sounds and appearence.
}

[ByRefEvent]
public record struct EnergyShieldExamineEvent(FormattedMessage Msg);
