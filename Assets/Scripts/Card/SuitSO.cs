using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pincushion.LD55
{
    [CreateAssetMenu(fileName = "Suit", menuName = "ScriptableObjects/Suit", order = 1)]
    public class SuitSO : ScriptableObject
    {
        public string Name = "";
        public int SuitRequiresNumCardsToSummon = 2; // Includes the summon card
    }
}