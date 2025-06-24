using UnityEngine;
using Verse;

namespace RepairingPriority.UserInterface;

[StaticConstructorOnStartup]
internal class TextureLoader
{
    public static readonly Texture2D PriorityWindowButton = ContentFinder<Texture2D>.Get("repairPrioritiesIcon");
    public static readonly Texture2D Delete = ContentFinder<Texture2D>.Get("UI/Buttons/Delete");
    public static readonly Texture2D Repair = ContentFinder<Texture2D>.Get("wrenchRepairPriority");
}