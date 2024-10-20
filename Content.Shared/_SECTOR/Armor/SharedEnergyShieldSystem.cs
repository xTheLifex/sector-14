using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Content.Shared.Silicons.Borgs;
using Content.Shared.Verbs;
using Robust.Shared.Utility;

namespace Content.Shared._SECTOR.Armor;

public abstract class SharedEnergyShieldSystem : EntitySystem
{
    [Dependency] private readonly ExamineSystemShared _examine = default!;

    private DamageModifierSet _damageSetZero = new();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<EnergyShieldComponent, InventoryRelayedEvent<DamageModifyEvent>>(OnDamageModify);
        SubscribeLocalEvent<EnergyShieldComponent, BorgModuleRelayedEvent<DamageModifyEvent>>(OnBorgDamageModify);
        SubscribeLocalEvent<EnergyShieldComponent, GetVerbsEvent<ExamineVerb>>(OnArmorVerbExamine);

        _damageSetZero.Coefficients.Add("Heat", 0f);
        _damageSetZero.Coefficients.Add("Cold", 0f);
        _damageSetZero.Coefficients.Add("Structure", 0f);
    }

    private void OnBorgDamageModify(EntityUid uid, EnergyShieldComponent component, BorgModuleRelayedEvent<DamageModifyEvent> args)
    {

        args.Args.Damage = DamageSpecifier.ApplyModifierSet(args.Args.Damage, _damageSetZero);
    }

    private void OnDamageModify(EntityUid uid, EnergyShieldComponent component, InventoryRelayedEvent<DamageModifyEvent> args)
    {
        args.Args.Damage = DamageSpecifier.ApplyModifierSet(args.Args.Damage, _damageSetZero);
    }

    private void OnArmorVerbExamine(EntityUid uid, EnergyShieldComponent component, GetVerbsEvent<ExamineVerb> args)
    {
        if (!args.CanInteract || !args.CanAccess)
            return;

        var examineMessage = GetShieldExamine();

        var ev = new EnergyShieldExamineEvent(examineMessage);
        RaiseLocalEvent(uid, ref ev);

        _examine.AddDetailedExamineVerb(args, component, examineMessage,
            Loc.GetString("energyshield-examinable-verb-text"), "/Textures/Interface/VerbIcons/dot.svg.192dpi.png",
            Loc.GetString("energyshield-examinable-verb-message"));
    }

    private FormattedMessage GetShieldExamine()
    {
        var msg = new FormattedMessage();
        msg.AddMarkupOrThrow(Loc.GetString("energyshield-examine"));
        return msg;
    }
}
