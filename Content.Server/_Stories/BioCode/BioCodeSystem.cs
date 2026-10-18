using Content.Shared._Stories.BioCode;
using Content.Shared.Damage.Systems;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.DoAfter;
using Content.Shared.Emag.Systems;
using Content.Shared.Forensics.Components;
using Content.Shared.Hands;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Components;
using Content.Shared.Inventory.Events;
using Content.Shared.Popups;
using Content.Shared.Trigger.Components;
using Content.Shared.Trigger.Systems;
using Content.Shared.Verbs;

namespace Content.Server._Stories.BioCode;

public sealed partial class BioCodeSystem : EntitySystem
{
    [Dependency] private DamageableSystem _damageable = default!;
    [Dependency] private ClothingSystem _clothing = default!;
    [Dependency] private SharedDoAfterSystem _doAfter = default!;
    [Dependency] private SharedHandsSystem _hands = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private TriggerSystem _trigger = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BioCodeComponent, GotEquippedEvent>(OnEquipped);
        SubscribeLocalEvent<BioCodeComponent, GotUnequippedEvent>(OnUnequipped);
        SubscribeLocalEvent<BioCodeComponent, GotEquippedHandEvent>(OnHandEquipped);
        SubscribeLocalEvent<BioCodeComponent, GotUnequippedHandEvent>(OnHandUnequipped);
        SubscribeLocalEvent<BioCodeComponent, GetVerbsEvent<Verb>>(OnGetVerbs);
        SubscribeLocalEvent<DnaComponent, BioCodeRecordDnaDoAfterEvent>(OnRecordDnaComplete);
        SubscribeLocalEvent<DnaComponent, BioCodeDeleteDnaDoAfterEvent>(OnDeleteDnaComplete);
        SubscribeLocalEvent<BioCodeComponent, GotEmaggedEvent>(OnEmagged);
    }

    private void OnEquipped(EntityUid uid, BioCodeComponent comp, GotEquippedEvent args)
    {
        if (!TryComp<DnaComponent>(args.EquipTarget, out var dnaComp) || dnaComp.DNA == null)
            return;

        if (comp.AutoRecordDNA && comp.RecordedDNA == null)
        {
            _damageable.TryChangeDamage(args.EquipTarget, comp.DamagePerRecord, true, true);
            comp.RecordedDNA = dnaComp.DNA;
            _popup.PopupEntity(Loc.GetString("stories-biocode-successful-recording"), uid);
            return;
        }

        if (comp.RecordedDNA == null || comp.RecordedDNA == dnaComp.DNA)
            return;

        if (comp.HardDefense)
        {
            EnsureComp<UnremoveableComponent>(uid);
            _popup.PopupEntity(Loc.GetString("stories-biocode-hard-defense"),
                args.EquipTarget,
                args.EquipTarget,
                PopupType.LargeCaution);
        }
        else
        {
            _popup.PopupEntity(Loc.GetString("stories-biocode-dna-not-match"),
                args.EquipTarget,
                args.EquipTarget,
                PopupType.LargeCaution);
        }

        var timer = EnsureComp<TimerTriggerComponent>(uid);

        timer.Delay = TimeSpan.FromSeconds(comp.Delay);
        timer.BeepInterval = TimeSpan.FromSeconds(comp.BeepInterval);

        if (comp.InitialBeepDelay.HasValue)
            timer.InitialBeepDelay = TimeSpan.FromSeconds(comp.InitialBeepDelay.Value);
        else
            timer.InitialBeepDelay = null;

        timer.BeepSound = comp.BeepSound;

        _trigger.ActivateTimerTrigger(uid, args.EquipTarget);
    }

    private void OnUnequipped(EntityUid uid, BioCodeComponent comp, GotUnequippedEvent args)
    {
        _trigger.StopTimerTrigger(uid);
    }

    private void OnHandEquipped(EntityUid uid, BioCodeComponent comp, GotEquippedHandEvent args)
    {
        if (!comp.ItemBioCode)
            return;

        if (!TryComp<DnaComponent>(args.User, out var dnaComp) || dnaComp.DNA == null)
            return;

        if (comp.AutoRecordDNA && comp.RecordedDNA == null)
        {
            _damageable.TryChangeDamage(args.User, comp.DamagePerRecord, true, true);
            comp.RecordedDNA = dnaComp.DNA;
            _popup.PopupEntity(Loc.GetString("stories-biocode-successful-recording"), uid);
            return;
        }

        if (comp.RecordedDNA == null || comp.RecordedDNA == dnaComp.DNA)
            return;

        if (comp.HardDefense)
        {
            EnsureComp<UnremoveableComponent>(uid);
            _popup.PopupEntity(Loc.GetString("stories-biocode-hard-defense"),
                args.User,
                args.User,
                PopupType.LargeCaution);
        }
        else
        {
            _popup.PopupEntity(Loc.GetString("stories-biocode-dna-not-match"),
                args.User,
                args.User,
                PopupType.LargeCaution);
        }

        var timer = EnsureComp<TimerTriggerComponent>(uid);

        timer.Delay = TimeSpan.FromSeconds(comp.Delay);
        timer.BeepInterval = TimeSpan.FromSeconds(comp.BeepInterval);

        if (comp.InitialBeepDelay.HasValue)
            timer.InitialBeepDelay = TimeSpan.FromSeconds(comp.InitialBeepDelay.Value);
        else
            timer.InitialBeepDelay = null;

        timer.BeepSound = comp.BeepSound;

        _trigger.ActivateTimerTrigger(uid, args.User);
    }

    private void OnHandUnequipped(EntityUid uid, BioCodeComponent comp, GotUnequippedHandEvent args)
    {
        _trigger.StopTimerTrigger(uid);
    }

    private void OnGetVerbs(EntityUid uid, BioCodeComponent comp, GetVerbsEvent<Verb> args)
    {
        if (!_clothing.IsEquipped(uid) && !(comp.ItemBioCode && _hands.IsHolding(args.User, uid)))
            return;

        if (comp.RecordedDNA != null && comp.AllowOverwriting)
        {
            var deleteDnaVerb = new Verb
            {
                Text = Loc.GetString("stories-biocode-delete-dna-verb"),
                Act = () => {
                    var doAfterArgs = new DoAfterArgs(EntityManager,
                        args.User,
                        TimeSpan.FromSeconds(comp.Delay + 1),
                        new BioCodeDeleteDnaDoAfterEvent(),
                        args.User,
                        used: uid)
                    {
                        BreakOnMove = true,
                        BreakOnDamage = true,
                        NeedHand = true,
                        DistanceThreshold = 0.1f,
                    };

                    _doAfter.TryStartDoAfter(doAfterArgs);
                }
            };

            args.Verbs.Add(deleteDnaVerb);
            return;
        }

        if (comp.RecordedDNA != null)
            return;

        var recordDnaVerb = new Verb
        {
            Text = Loc.GetString("stories-biocode-record-dna-verb"),
            Act = () => {
                if (!TryComp<DnaComponent>(args.User, out var dnaComp) || dnaComp.DNA == null)
                {
                    _popup.PopupEntity(Loc.GetString("stories-biocode-dna-not-found"), uid);
                    return;
                }

                var doAfterArgs = new DoAfterArgs(EntityManager,
                    args.User,
                    TimeSpan.FromSeconds(comp.Delay + 1),
                    new BioCodeRecordDnaDoAfterEvent(),
                    args.User,
                    used: uid)
                {
                    BreakOnMove = true,
                    BreakOnDamage = true,
                    NeedHand = true,
                    DistanceThreshold = 0.1f,
                };

                _doAfter.TryStartDoAfter(doAfterArgs);
            },
        };
        args.Verbs.Add(recordDnaVerb);
    }

    private void OnDeleteDnaComplete(EntityUid uid, DnaComponent comp, BioCodeDeleteDnaDoAfterEvent args)
    {
        if (args.Cancelled)
            return;

        if (!TryComp<BioCodeComponent>(args.Used, out var bioCodeComp))
            return;

        bioCodeComp.RecordedDNA = null;
        _popup.PopupEntity(Loc.GetString("stories-biocode-successful-deletion"), uid);
    }

    private void OnRecordDnaComplete(EntityUid uid, DnaComponent comp, BioCodeRecordDnaDoAfterEvent args)
    {
        if (args.Cancelled)
            return;

        if (!TryComp<BioCodeComponent>(args.Used, out var bioCodeComp))
            return;

        _damageable.TryChangeDamage(args.User, bioCodeComp.DamagePerRecord, true, true);
        bioCodeComp.RecordedDNA = comp.DNA;
        _popup.PopupEntity(Loc.GetString("stories-biocode-successful-recording"), uid);
    }

    private void OnEmagged(EntityUid uid, BioCodeComponent comp, GotEmaggedEvent args)
    {
        _popup.PopupEntity(Loc.GetString("stories-biocode-defense-emagged"), uid);
        RemComp<BioCodeComponent>(uid);
    }
}
