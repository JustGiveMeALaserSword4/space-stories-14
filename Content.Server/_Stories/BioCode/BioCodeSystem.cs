using Content.Server._Stories.BioCode;
using Content.Shared.Emag.Systems;
using Content.Shared.Forensics.Components;
using Content.Shared.Interaction.Components;
using Content.Shared.Inventory.Events;
using Content.Shared.Popups;
using Content.Shared.Trigger.Components;
using Content.Shared.Trigger.Systems;

namespace Content.Server._Stories.BioCode;

public sealed partial class BioCodeSystem : EntitySystem
{
    [Dependency] private SharedPopupSystem _popupSystem = default!;
    [Dependency] private TriggerSystem _trigger = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BioCodeComponent, GotEquippedEvent>(OnEquipped);
        SubscribeLocalEvent<BioCodeComponent, GotUnequippedEvent>(OnUnequipped);
        SubscribeLocalEvent<BioCodeComponent, GotEmaggedEvent>(OnEmagged);
    }

    private void OnEquipped(EntityUid uid, BioCodeComponent comp, GotEquippedEvent args)
    {
        if (!TryComp<DnaComponent>(args.EquipTarget, out var dnaComp))
            return;

        if (dnaComp.DNA == null)
            return;

        if (comp.RecordedDNA == null)
        {
            comp.RecordedDNA = dnaComp.DNA;
            _popupSystem.PopupEntity(Loc.GetString(comp.SuccessRecordMessage),
                args.EquipTarget,
                args.EquipTarget,
                PopupType.Medium);
            return;
        }

        if (comp.RecordedDNA == dnaComp.DNA)
            return;

        if (comp.HardDefense)
        {
            EnsureComp<UnremoveableComponent>(uid);
            _popupSystem.PopupEntity(Loc.GetString(comp.HardDefenseMessage),
                args.EquipTarget,
                args.EquipTarget,
                PopupType.LargeCaution);
        }
        else
        {
            _popupSystem.PopupEntity(Loc.GetString(comp.DNANotMatchMessage),
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

    private void OnEmagged(EntityUid uid, BioCodeComponent comp, GotEmaggedEvent args)
    {
        _popupSystem.PopupEntity(Loc.GetString(comp.EmaggedMessage), uid);
        RemComp<BioCodeComponent>(uid);
    }
}
