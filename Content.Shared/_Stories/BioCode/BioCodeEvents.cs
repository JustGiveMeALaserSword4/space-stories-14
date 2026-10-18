using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._Stories.BioCode;

[Serializable, NetSerializable]
public sealed partial class BioCodeRecordDnaDoAfterEvent : DoAfterEvent
{
    public override DoAfterEvent Clone() => new BioCodeRecordDnaDoAfterEvent();
}

[Serializable, NetSerializable]
public sealed partial class BioCodeDeleteDnaDoAfterEvent : DoAfterEvent
{
    public override DoAfterEvent Clone() => new BioCodeDeleteDnaDoAfterEvent();
}