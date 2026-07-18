using Content.Shared.Inventory;
using Robust.Shared.GameStates;

namespace Content.Shared._Stories.Examine.ShowRotProgress;

/// <summary>
/// This component allows you to see percentage before the onset of decay
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(ShowRotProgressSystem))]
public sealed partial class ShowRotProgressComponent : Component, IClothingSlots
{
    /// <summary>
    /// Determines from which equipment slots this entity can provide its benefits.
    /// </summary>
    public SlotFlags Slots { get; set; } = ~SlotFlags.POCKET;
}
