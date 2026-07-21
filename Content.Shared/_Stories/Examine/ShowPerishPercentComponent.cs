//using Content.Shared._Stories.Examine.VerbScan;
using Content.Shared.Inventory;
using Content.Shared.StatusIcon;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Stories.Examine.VerbScan;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(ShowPerishPercentSystem))]
public sealed partial class ShowPerishPercentComponent : Component, IClothingSlots
{
    [DataField("isEnabled"), AutoNetworkedField]
    public bool IsEnabled { get; set; } = true;

    [DataField("perishWarnStatusIcon"), AutoNetworkedField]
    public ProtoId<HealthIconPrototype> PerishWarnStatusIcon = "STPerishWarnIcon";

    public SlotFlags Slots { get; set; } = ~SlotFlags.POCKET;
}
