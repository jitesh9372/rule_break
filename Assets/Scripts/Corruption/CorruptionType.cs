// =============================================================================
// CorruptionType.cs — RULE//BREAK
// Defines the possible transformation behaviours for Rule Corruption.
// =============================================================================
using UnityEngine;

namespace RuleBreak.Corruption
{
    public enum CorruptionType
    {
        None,
        LaserToPlatform,     // Laser becomes a solid platform
        SpikeToBouncePad,    // Spike becomes a bounce pad
        WallToPassable,      // Wall becomes non-solid
        DoorToPortal,        // Door becomes an instant-teleport portal
        MovingToStatic       // Moving platform becomes static
    }
}
