using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pincushion.LD55
{
    public class ColliderController : MonoBehaviour, IColliderController
    {
        [field: SerializeField] public string Name {get;set;}

        [field: SerializeField] public ColliderType ColliderType { get; set; }


        [field: SerializeField] public bool IsRoot { get; private set; }
        [field: SerializeField] public IColliderController Root { get; private set; }
        [field: SerializeField] public IColliderController Parent { get; private set; }

        private void Awake()
        {
            IColliderController parent = transform.parent?.GetComponentInParent<IColliderController>();
            Parent = parent;
            IsRoot = parent == null;

            IColliderController root = this;
            while (parent != null)
            {
                root = parent;
                parent = root.gameObject.transform.parent?.GetComponentInParent<IColliderController>();
            }
            Root = root;

            PopulateChildWithRootData();
        }

        private void PopulateChildWithRootData()
        {
            if (!IsRoot)
            {
                Name = Root.Name;
                ColliderType = Root.ColliderType;
            }
        }
    }
}