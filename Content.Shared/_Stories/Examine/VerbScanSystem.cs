using Content.Shared._Stories.Examine.VerbScan;
using Content.Shared.Examine;
using Content.Shared.Verbs;
using Robust.Shared.Utility;

namespace Content.Shared._Stories.Examine.VerbScan;

public sealed class VerbScanSystem : EntitySystem
{
    [Dependency] private ExamineSystemShared _examineSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MetaDataComponent, GetVerbsEvent<ExamineVerb>>(OnExamined);
    }

    public void OnExamined(EntityUid uid, MetaDataComponent component, GetVerbsEvent<ExamineVerb> args)
    {
        var scanEvent = new VerbScanEvent{ Examiner = args.User };
        RaiseLocalEvent(args.Target, scanEvent);

        if (scanEvent.VerbScanMessage.Count == 0)
            return;

        //if (!args.CanInteract || !args.CanAccess) // Проверка можно ли вообще совершить действие в тек ситуации?
        //    return;

        var message = scanEvent.VerbScanMessage;
        var detailsRange = _examineSystem.IsInDetailsRange(args.User, uid);
        var verbScan = new ExamineVerb()
        {
            Act = () =>
            {
                _examineSystem.SendExamineTooltip(args.User, args.Target, message, false, false);
            },
            Text = Loc.GetString("stories-scan-options-verb-text"),
            Category = VerbCategory.Examine,
            Disabled = !detailsRange,
            Message = detailsRange ? null : Loc.GetString("stories-scan-options-verb-disabled"),
            Icon = new SpriteSpecifier.Texture(new("/Textures/_Stories/Interface/Examine/scan.png")),
        };

        args.Verbs.Add(verbScan);
    }
}

public sealed class VerbScanEvent : EntityEventArgs
{
    public FormattedMessage VerbScanMessage = new();
    public EntityUid Examiner;
}
