using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pincushion.LD55
{
    public interface IColliderController
    {
        public string Name { get; }
        public ColliderType ColliderType { get; }
        public GameObject gameObject { get; }


        public bool IsRoot { get; }
        public IColliderController Root { get; }
        public IColliderController Parent { get; }
    }
}