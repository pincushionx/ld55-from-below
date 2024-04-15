using Pincushion.LD55;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Pincushion.LD55 {
    public class CardController : MonoBehaviour
    {
        public CardSO Card;

        public SuitSO Suit { get { return Card.Suit; } } // temp

        //public int SleepTurns { get { return Card.SleepTurns; } }
        public int Damage { get { return Card.Damage; } }
        public int AoeDamage { get { return Card.AoeDamage; } }
        public int MaxHealth { get { return Card.MaxHealth; } }
        // Buffs?
        public int Defense { get { return Card.Defense; } }

        public int DistancePerTurn { get { return Card.DistancePerTurn; } }


        public bool TextSetExternally = false;


        public TMP_Text CardText;
        public TMP_Text HealthText;
        public TMP_Text DamageText;
        public TMP_Text DefenseText;
        public TMP_Text SpeedText;
        //public TMP_Text AoeDamageText;

        private void Start()
        {
            if (!TextSetExternally)
            {
                CardText.text = Card.Name.ToString();
                HealthText.text = MaxHealth.ToString();
                DamageText.text = Damage.ToString();
                DefenseText.text = Defense.ToString();
                SpeedText.text = (DistancePerTurn - 1).ToString();

                //AoeDamageText.text = AoeDamage.ToString();
            }
        }


        private float _moveDuration = 0.25f;
        private Vector3 _desiredLocalPos;
        private float _moveStartTime = 0;
        private float _movEndTime = 0;
        public void MoveToLocalPos(Vector3 desiredLocalPos)
        {
            _desiredLocalPos = desiredLocalPos;

            _moveStartTime = Time.time;
        }

        private void Update()
        {
            if (_moveStartTime > 0)
            {
                float progress = (Time.time - _moveStartTime) / _moveDuration;

                transform.localPosition = Vector3.Slerp(transform.localPosition, _desiredLocalPos, progress);

                if (progress > 1)
                {
                    _moveStartTime = 0;
                    _movEndTime = 0;
                }
            }
        }
    }
}