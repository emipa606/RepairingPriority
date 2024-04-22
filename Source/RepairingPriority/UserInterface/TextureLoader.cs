using UnityEngine;
using Verse;

namespace RepairingPriority.UserInterface;

[StaticConstructorOnStartup]
internal class TextureLoader
{
    public static readonly Texture2D priorityWindowButton = ContentFinder<Texture2D>.Get("repairPrioritiesIcon");
    public static Texture2D plusSign = ContentFinder<Texture2D>.Get("grayscalePlus");
    public static Texture2D addIcon = SolidColorMaterials.NewSolidColorTexture(Color.grey);

    public static readonly Texture2D dragHash = ContentFinder<Texture2D>.Get("UI/Buttons/DragHash");
    public static readonly Texture2D delete = ContentFinder<Texture2D>.Get("UI/Buttons/Delete");
    public static readonly Texture2D repair = ContentFinder<Texture2D>.Get("wrenchRepairPriority");
}