using System.Linq;
using Content.Shared._Stories.Examine.VerbScan;
using Content.Shared.Atmos.Rotting;
using Content.Shared.Inventory.Events;
//using Content.Shared.Overlays;
using Content.Shared.StatusIcon.Components;
//using Content.Client.Overlays;
//using Robust.Client.Player;
//using Robust.Shared.Prototypes;

namespace Content.Client.Overlays;

public sealed partial class ShowPerishWarnIconsSystem : EquipmentHudSystem<ShowPerishPercentComponent>
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PerishableComponent, GetStatusIconsEvent>(OnGetStatusIconsEvent);
    }

    private ShowPerishPercentComponent? _viewerComp;

    protected override void UpdateInternal(RefreshEquipmentHudEvent<ShowPerishPercentComponent> args)
    {
        var uid = args.Components.FirstOrDefault()?.Owner ?? EntityUid.Invalid;
        if (uid.IsValid() && TryComp<ShowPerishPercentComponent>(uid, out var comp))
            _viewerComp = comp;
        else
            _viewerComp = null;
    }

    protected override void DeactivateInternal()
    {
        _viewerComp = null;
    }

    private void OnGetStatusIconsEvent(EntityUid uid, PerishableComponent component, ref GetStatusIconsEvent ev)
    {
        if (!IsActive || _viewerComp == null)
            return;

        if (_viewerComp.IsEnabled && ProtoMan.Resolve(_viewerComp.PerishWarnStatusIcon, out var iconPrototype))
            ev.StatusIcons.Add(iconPrototype);
    }
}
