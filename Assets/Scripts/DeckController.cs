using System.Collections.Generic;
using UnityEngine;

namespace Pincushion.LD55
{
    public class DeckController : MonoBehaviour
    {
        private GameSceneController _scene;

        public CardSO[] cardDefinitions;
        public CardController ForksPrefab;
        public CardController PitsPrefab;
        public CardController TailsPrefab;
        public CardController StarsPrefab;

        private CardController[] _cardPrefabs;

        //public Dictionary<SuitSO, CardController> SuitPrefabs= new Dictionary<SuitSO, CardController>();
        public Dictionary<CardSO, CardController> CardPrefabsByType = new Dictionary<CardSO, CardController>();

        private bool isInitialized = false;

        private void Awake()
        {

            InitializeLists();
        }

        public void InitializeLists()
        {
            if (!isInitialized)
            {

                foreach (CardSO cardDefinitions in cardDefinitions)
                {

                    if (cardDefinitions.Suit.Name == "Forks")
                    {
                        CardController prefab = Instantiate(ForksPrefab);
                        prefab.Card = cardDefinitions;
                        CardPrefabsByType.Add(prefab.Card, prefab);
                    }
                    else if (cardDefinitions.Suit.Name == "Tails")
                    {
                        CardController prefab = Instantiate(TailsPrefab);
                        prefab.Card = cardDefinitions;
                        CardPrefabsByType.Add(prefab.Card, prefab);
                    }
                    else if (cardDefinitions.Suit.Name == "Stars")
                    {
                        CardController prefab = Instantiate(StarsPrefab);
                        prefab.Card = cardDefinitions;
                        CardPrefabsByType.Add(prefab.Card, prefab);
                    }
                    else if (cardDefinitions.Suit.Name == "Pits")
                    {
                        CardController prefab = Instantiate(PitsPrefab);
                        prefab.Card = cardDefinitions;
                        CardPrefabsByType.Add(prefab.Card, prefab);
                    }


                }

                _cardPrefabs = new CardController[CardPrefabsByType.Count];
                int i = 0;
                foreach (KeyValuePair<CardSO, CardController> kv in CardPrefabsByType)
                {
                    _cardPrefabs[i++] = kv.Value;
                }
                isInitialized = true;
            }
        }


        public void Init(GameSceneController scene)
        {
            _scene = scene;
        }


        public void DealCardToPlayerBySuitName(string suitName)
        {
            foreach (KeyValuePair<CardSO, CardController> kv in CardPrefabsByType)
            {
                if (kv.Key.Suit.Name == suitName)
                {
                    _scene.PlayerHand.AddCardToHand(Instantiate(kv.Value));
                }
            }
        }
        public void DealCardToPlayer()
        {
            CardController card = GetCard();
            _scene.PlayerHand.AddCardToHand(card);
        }
        public void DealCardToOpponent()
        {
            CardController card = GetCard();
            _scene.OpponentHand.AddCardToHand(card);
        }

        public CardController GetCard()
        {
            int cardIndex = Random.Range(0, _cardPrefabs.Length);
            CardController prefab = _cardPrefabs[cardIndex];
            return Instantiate(prefab);
        }
    }
}