using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pincushion.LD55
{
    public enum ColliderType
    {
        Tile = 0x1,
        Avatar = 0x2,
        SummonTile = 0x5, // Mathches Tile and is unique

        // Keeping card separate to support convenient "ALL" masks
        Card = 0x100,
    }
}