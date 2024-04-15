using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace Pincushion.LD55
{
    public class SelectionWindowController : MonoBehaviour
    {
        public GameObject SummonCardOuterContainer;
        public GameObject SummonCardInnerContainer;
        private GameSceneController _scene;

        public TMP_Text MessageText;
        private Queue<string> _messages = new Queue<string>();
        private int _maxMessages = 7;


        private AvatarController _selectedAvatar = null;
        private CardController _selectedAvatarCard = null;

        private void Awake()
        {
            MessageText.text = string.Empty;
            UpdateMessages();
        }

        public void Init(GameSceneController scene)
        {
            _scene = scene;
        }

        public void Update()
        {
            if (_selectedAvatar != null)
            {
                UpdateSelectedAvatar();
            }
        }


        public void SetSelectedAvatar(AvatarController avatar)
        {

            if (_scene.Deck.CardPrefabsByType.ContainsKey(avatar.Card))
            {
                ClearSelectedAvatar();

                CardController prefab = _scene.Deck.CardPrefabsByType[avatar.Card];
                CardController card = Instantiate(prefab);

                
                card.TextSetExternally = true; // So it doesn't populate on start
                SummonCardOuterContainer.SetActive(true);
                card.transform.SetParent(SummonCardInnerContainer.transform, false);


                _selectedAvatar = avatar;
                _selectedAvatarCard = card;
                _selectedAvatarCard.DamageText.text = _selectedAvatar.Damage.ToString();
                _selectedAvatarCard.DefenseText.text = _selectedAvatar.Defense.ToString();
                UpdateSelectedAvatar();
            }
        }
        private void UpdateSelectedAvatar()
        {
            _selectedAvatarCard.HealthText.text = _selectedAvatar.RemainingHealth + "/" + _selectedAvatar.MaxHealth;
            _selectedAvatarCard.SpeedText.text = (_selectedAvatar.RemainingDistanceThisTurn - 1) + "/" + (_selectedAvatar.DistancePerTurn - 1);
        }

        public void ClearSelectedAvatar()
        {
            for (int i = SummonCardInnerContainer.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(SummonCardInnerContainer.transform.GetChild(i).gameObject);
            }
            SummonCardOuterContainer.SetActive(false);
            _selectedAvatar = null;
            _selectedAvatarCard = null;
        }

        public void AddErrorMessage(string message)
        {
            if (_messages.Count >= _maxMessages)
            {
                _messages.Dequeue();
            }
            _messages.Enqueue("<color=#b28305>" + message + "</color>");
            UpdateMessages();
        }
        public void AddMessage(string message)
        {
            if (_messages.Count >= _maxMessages)
            {
                _messages.Dequeue();
            }
            _messages.Enqueue(message);
            UpdateMessages();
        }
        private void UpdateMessages()
        {
            string messages = string.Empty;

            foreach (string message in _messages)
            {
                messages += message + "\n";
            }

            MessageText.text = messages;
        }
    }
}