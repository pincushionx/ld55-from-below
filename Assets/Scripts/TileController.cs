using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pincushion.LD55
{
    [RequireComponent(typeof(IColliderController))]
    public class TileController : MonoBehaviour
    {
        public IColliderController Collider;
        public Tile Tile;

        public GameSceneController Scene;

        public void Init(GameSceneController scene)
        {
            Scene = scene;
        }
    }
}