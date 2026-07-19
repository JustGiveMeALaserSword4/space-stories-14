using Content.Shared._Stories.Examine.VerbScan;
using Content.Shared.Atmos.Rotting;
using Content.Shared.Inventory;
using Robust.Shared.Utility;

namespace Content.Shared._Stories.Examine.VerbScan;

public sealed class ShowPerishPercentSystem : EntitySystem
{
    [Dependency] private InventorySystem _inventorySystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PerishableComponent, VerbScanEvent>(OnVerbScan);
    }

    public void OnVerbScan(EntityUid uid, PerishableComponent perishable, VerbScanEvent args)
    {
        if (!HasComp<ShowPerishPercentComponent>(args.Examiner) &&
        !_inventorySystem.TryGetInventoryEntity<ShowPerishPercentComponent>(args.Examiner, out _))
            return;
        
        float perishPercent = 0f;

        if (perishable.RotAfter.TotalSeconds > 0)
            perishPercent = (float)(perishable.RotAccumulator.TotalSeconds / perishable.RotAfter.TotalSeconds) * 100f;

        perishPercent = Math.Clamp(perishPercent, 0f, 100f);

        string message = "stories-examine-perish-percent-" + perishable.Stage;
        string localizedMessage = Loc.GetString(message, ("percent", perishPercent.ToString("F0")));
        args.VerbScanMessage.AddMessage(FormattedMessage.FromMarkup(localizedMessage));
        args.VerbScanMessage.PushNewline();
    }
}
// TODO: Сделано не опытным, начинающим кодером, прошу переделать при необходимости или убрать этот комментарий ^w^
