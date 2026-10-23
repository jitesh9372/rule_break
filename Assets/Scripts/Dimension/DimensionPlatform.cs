// =============================================================================
// DimensionPlatform.cs — RULE//BREAK
// A platform that only exists (is solid) in one reality.
// Uses DimensionObject internally.
// =============================================================================
using UnityEngine;

namespace RuleBreak.Dimension
{
    [RequireComponent(typeof(DimensionObject))]
    public class DimensionPlatform : MonoBehaviour
    {
        // DimensionObject handles all the logic.
        // This class is just a semantic marker for the editor builder.
    }
}
