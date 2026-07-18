using Content.Shared._Stories.Examine.ShowRotProgress;
using Content.Shared.Atmos.Rotting;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

using Content.Shared.Verbs;
using Robust.Shared.Utility;

namespace Content.Shared._Stories.Examine.ShowRotProgress;

//public sealed class ShowRotProgressSystem : EntitySystem
//{
//    [Dependency] private InventorySystem _inventorySystem = default!;
//
//    public override void Initialize()
//    {
//        base.Initialize();
//
//        SubscribeLocalEvent<PerishableComponent, ExaminedEvent>(OnExamined);
//    }
//
//    private void OnExamined(EntityUid uid, PerishableComponent component, ExaminedEvent args)
//    {
//        if (HasComp<ShowRotProgressComponent>(args.Examiner) ||
//           _inventorySystem.TryGetInventoryEntity<ShowRotProgressComponent>(args.Examiner, out _))
//        {
//            if (!TryComp<PerishableComponent>(args.Examined, out var perishable))
//                return;
//            float rotPercent = 0f;
//            if (perishable.RotAfter.TotalSeconds > 0)
//            {
//                rotPercent = (float)(perishable.RotAccumulator.TotalSeconds / perishable.RotAfter.TotalSeconds) * 100f;
//            }
//            rotPercent = Math.Clamp(rotPercent, 0f, 100f);
//
//            string message = "examine-rot-percent-perishable-" + perishable.Stage;
//            args.PushMarkup(Loc.GetString(message, ("percent", rotPercent.ToString("F0"))));
//        }
//    }
//}
// TODO: Сделано не опытным, начинающим кодером, прошу переделать при необходимости или убрать этот комментарий ^w^

public sealed class ShowRotProgressSystem : EntitySystem
{
    [Dependency] private ExamineSystemShared _examineSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MetaDataComponent, GetVerbsEvent<ExamineVerb>>(OnExamined);

        SubscribeLocalEvent<PerishableComponent, HasPerishable>(OnExaminedPerishable);
    }

    public void OnExamined(EntityUid uid, MetaDataComponent component, GetVerbsEvent<ExamineVerb> args)
    {
        Log.Debug("OnExamined вызван успешно");

        if (HasComp<PerishableComponent>(args.Target)) // (TryComp<PerishableComponent>(args.Target, out var perishable)) Нахера
        {
            // var HasPerishable = new HasPerishable();
            // RaiseLocalEvent(uid, HasPerishable);
            Log.Debug("HasPerishable от HasComp вызван успешно");
            RaiseLocalEvent(args.Target, new HasPerishable());
        }

        //if (!args.CanInteract || !args.CanAccess) // Проверка можно ли вообще совершить действие в тек ситуации?
        //    return;

        var detailsRange = _examineSystem.IsInDetailsRange(args.User, uid);
        var verb = new ExamineVerb()
        {
            Act = () =>
            {
                var markup = GetScanOptionsExamine();
                _examineSystem.SendExamineTooltip(args.User, args.Target, markup, false, false);
            },
            Text = Loc.GetString("stories-scan-options-verb-text"),
            Category = VerbCategory.Examine,
            Disabled = !detailsRange,
            Message = detailsRange ? null : Loc.GetString("stories-scan-options-verb-disabled"),
            Icon = new SpriteSpecifier.Texture(new("/Textures/_Stories/Interface/Examine/scan.png")),
        };

        args.Verbs.Add(verb);
    }

    private FormattedMessage GetScanOptionsExamine()
    {
        var msg = new FormattedMessage();
        msg.AddText("TEST_NAHUY");
        return msg;
    }

    private void OnExaminedPerishable(EntityUid uid, PerishableComponent component, HasPerishable args)
    {
        Log.Debug("OnExaminedPerishable вызван успешно");
    }
}

public sealed class HasPerishable : EntityEventArgs { }
