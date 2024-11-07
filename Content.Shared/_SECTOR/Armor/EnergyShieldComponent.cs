using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;
using Robust.Shared.Utility;

namespace Content.Shared._SECTOR.Armor;

[RegisterComponent, NetworkedComponent, Access(typeof(SharedEnergyShieldSystem))]
[AutoGenerateComponentState]
public sealed partial class EnergyShieldComponent : Component
{
    [DataField("color")]
    public Color SpriteColor = Color.White;

    [DataField("nullfiers")]
    public List<string>? Nullfiers { get; private set; }

    [DataField("capacity")]
    public FixedPoint2 MaxCapacity = 2000f;

    [DataField("rechargeRate")]
    public FixedPoint2 RechargeRate = 0.2f;

    /// <summary>
    /// Time out of combat before this energy shield can recharge
    /// </summary>
    [DataField("timeUntilRechargeAvailable")]
    public FixedPoint2 TimeUntilRechargeAvailable = 10f;

    [ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public FixedPoint2 Capacity = 1000f;

    [ViewVariables(VVAccess.ReadWrite)]
    public FixedPoint2 TimeForRecharge = 0f;

    //TODO: Energy shield sounds and appearance.
}

[ByRefEvent]
public record struct EnergyShieldExamineEvent(FormattedMessage Msg);
