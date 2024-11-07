using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Inventory;
using Content.Shared.Silicons.Borgs;
using Content.Shared.Verbs;
using Robust.Shared.Utility;

namespace Content.Shared._SECTOR.Armor;

public abstract class SharedEnergyShieldSystem : EntitySystem
{
    [Dependency] private readonly ExamineSystemShared _examine = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<EnergyShieldComponent, InventoryRelayedEvent<DamageModifyEvent>>(OnDamageModifyRelayed);
        SubscribeLocalEvent<EnergyShieldComponent, DamageModifyEvent>(OnDamageModify);
        SubscribeLocalEvent<EnergyShieldComponent, GetVerbsEvent<ExamineVerb>>(OnArmorVerbExamine);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<EnergyShieldComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            if (component.TimeForRecharge > 0f)
            {
                component.TimeForRecharge -= frameTime;
                continue;
            }

            component.Capacity += component.RechargeRate * component.MaxCapacity * frameTime;
            if (component.Capacity >= component.MaxCapacity)
                component.Capacity = component.MaxCapacity;
        }
    }

    private DamageModifierSet GetModifierSet(EnergyShieldComponent component)
    {
        var set = new DamageModifierSet();
        if (component.Nullfiers is null)
            return set;

        foreach (var nullfy in component.Nullfiers)
        {
            set.Coefficients.Add(nullfy, 0.0f);
        }

        return set;
    }

    private void OnDamageModify(EntityUid uid, EnergyShieldComponent component, ref DamageModifyEvent args)
    {
        if (!CanNegateDamage(component))
            return;

        component.Capacity -= args.Damage.GetTotal();
        component.TimeForRecharge = component.TimeUntilRechargeAvailable;

        args.Damage = DamageSpecifier.ApplyModifierSet(args.Damage, GetModifierSet(component));
    }

    private void OnDamageModifyRelayed(EntityUid uid, EnergyShieldComponent component, InventoryRelayedEvent<DamageModifyEvent> args)
    {
        if (!CanNegateDamage(component))
            return;

        component.Capacity -= args.Args.Damage.GetTotal();
        component.TimeForRecharge = component.TimeUntilRechargeAvailable;

        args.Args.Damage = DamageSpecifier.ApplyModifierSet(args.Args.Damage, GetModifierSet(component));
    }

    private void OnArmorVerbExamine(EntityUid uid, EnergyShieldComponent component, GetVerbsEvent<ExamineVerb> args)
    {
        if (!args.CanInteract || !args.CanAccess)
            return;

        var examineMessage = GetShieldExamine(component);

        var ev = new EnergyShieldExamineEvent(examineMessage);
        RaiseLocalEvent(uid, ref ev);

        _examine.AddDetailedExamineVerb(args, component, examineMessage,
            Loc.GetString("energyshield-examinable-verb-text"), "/Textures/Interface/VerbIcons/dot.svg.192dpi.png",
            Loc.GetString("energyshield-examinable-verb-message"));
    }

    private FormattedMessage GetShieldExamine(EnergyShieldComponent component)
    {
        var msg = new FormattedMessage();
        msg.AddMarkupOrThrow(Loc.GetString("energyshield-examine"));

        if (component.Nullfiers is null)
        {
            msg.AddMarkupOrThrow("[no nullfier error]");
            return msg;
        }

        foreach (var nullfy in component.Nullfiers)
        {
            msg.PushNewline();
            msg.AddMarkupOrThrow(Loc.GetString("energyshield-nullfy-type", ("type", nullfy)));
        }
        return msg;
    }

    private bool CanNegateDamage(EnergyShieldComponent component) => component.Capacity > 0f;
}
