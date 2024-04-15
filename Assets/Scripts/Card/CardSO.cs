using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pincushion.LD55
{
    [CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card", order = 1)]
    public class CardSO : ScriptableObject
    {
        public int DistancePerTurn = 3;

        public int SleepTurns = 1;
        public int Damage = 2;
        public int AoeDamage = 0;
        public int MaxHealth = 10;
        public int Defense = 0;

        public bool HasAoe { get{ return AoeDamage > 0; } }

        public string Name = "";
        public SuitSO Suit;

        // Buffs?
    }
}