using Content.Shared.Damage;
using Robust.Shared.Audio;

namespace Content.Server._Stories.BioCode;

[RegisterComponent]
public sealed partial class BioCodeComponent : Component
{
    [DataField("autoRecordDNA")]
    public bool AutoRecordDNA = false;

    [DataField("beepInterval")]
    public float BeepInterval = 1;

    [DataField("beepSound")]
    public SoundSpecifier? BeepSound = new SoundPathSpecifier("/Audio/Machines/Nuke/general_beep.ogg");

    [DataField("damage")]
    public DamageSpecifier Damage = new ()
    {
        DamageDict = new Dictionary<ProtoId<DamageTypePrototype>, FixedPoint2>
        {
            { "Piercing", 5 },
        },
    }

    [DataField("delay")]
    public float Delay = 3f;

    [DataField("hardDefense")]
    public bool HardDefense = false;

    [DataField("initialBeepDelay")]
    public float? InitialBeepDelay = 0;

    [DataField("recordedDNA")]
    public string? RecordedDNA = null;
}
