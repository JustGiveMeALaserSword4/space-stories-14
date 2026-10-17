using Robust.Shared.Audio;

namespace Content.Server._Stories.BioCode;

[RegisterComponent]
public sealed partial class BioCodeComponent : Component
{
    [DataField("beepInterval")]
    public float BeepInterval = 1;

    [DataField("beepSound")]
    public SoundSpecifier? BeepSound = new SoundPathSpecifier("/Audio/Machines/Nuke/general_beep.ogg");

    [DataField("delay")]
    public float Delay = 3f;

    [DataField("dnaNotMatchMessage")]
    public LocId DNANotMatchMessage = "stories-biocode-dna-not-match";

    [DataField("emaggedMessage")]
    public LocId EmaggedMessage = "stories-biocode-defense-emagged";

    [DataField("hardDefense")]
    public bool HardDefense = false;

    [DataField("hardDefenseMessage")]
    public LocId HardDefenseMessage = "stories-biocode-hard-defense";

    [DataField("initialBeepDelay")]
    public float? InitialBeepDelay = 0;

    [DataField("recordedDNA")]
    public string? RecordedDNA = null;

    [DataField("successRecordMessage")]
    public LocId SuccessRecordMessage = "stories-biocode-successful-recording";
}
