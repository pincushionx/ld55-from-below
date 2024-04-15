using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pincushion.LD55
{
    public class GameSceneOverlayController : MonoBehaviour
    {
        private GameSceneController _scene;

        [SerializeField] private UIDocument _document;
        private VisualElement _root;
        private VisualElement _messageBox;

        private Action _messageBoxOkClicked;

        public void Init(GameSceneController scene)
        {
            _scene = scene;
        }

        private void Awake()
        {
            _root = _document.rootVisualElement;


            _root.Q<Button>("EndTurnButton").RegisterCallback<ClickEvent>(EndTurnClicked);


            _root.Q<Button>("WinLevelMessageBoxOkButton").RegisterCallback<ClickEvent>(WinLevelMessageBoxOkClicked);
            _root.Q<Button>("LoseLevelMessageBoxRetryButton").RegisterCallback<ClickEvent>(LoseLevelMessageBoxRetryClicked);
            _root.Q<Button>("LoseLevelMessageBoxLevelSelectButton").RegisterCallback<ClickEvent>(LoseLevelMessageBoxOkClicked);
            _root.Q<Button>("WinGameMessageBoxOkButton").RegisterCallback<ClickEvent>(WinGameMessageBoxOkClicked);
            ShowMessageBox("WinLevelMessageBox", false);
            ShowMessageBox("LoseLevelMessageBox", false);
            ShowMessageBox("WinGameMessageBox", false);

            /* _messageBox = _root.Q<VisualElement>("GeneralMessageBox");
             _messageBox.style.display = DisplayStyle.None;
             _messageBox.Q<Button>("GeneralMessageBoxOkButton").RegisterCallback<ClickEvent>(MessageBoxOkClicked);


             _root.Q<Button>("WinLevelMessageBoxOkButton").RegisterCallback<ClickEvent>(WinLevelMessageBoxOkClicked);
             _root.Q<Button>("LoseLevelMessageBoxOkButton").RegisterCallback<ClickEvent>(LoseLevelMessageBoxOkClicked);
             _root.Q<Button>("WinGameMessageBoxOkButton").RegisterCallback<ClickEvent>(WinGameMessageBoxOkClicked);
             ShowMessageBox("WinLevelMessageBox", false);
             ShowMessageBox("LoseLevelMessageBox", false);
             ShowMessageBox("WinGameMessageBox", false);


             SetTextElement("BuffNameText", null);
             SetTextElement("RemainingEffectText", null);

             SetTextElement("MouseTurnsText", null);
             SetTextElement("CatTurnsText", null);

             _root.Q<Button>("HowToPlay").RegisterCallback<ClickEvent>(HowToPlayClicked);
             _root.Q<Button>("HowToPlayOkButton").RegisterCallback<ClickEvent>(e => HideHowToPlay());
             HideHowToPlay();

             _root.Q<Button>("LevelSelect").RegisterCallback<ClickEvent>(LevelSelectClicked);
             _root.Q<Button>("RestartLevel").RegisterCallback<ClickEvent>(RestartLevelClicked);

             _root.Q<Slider>("VolumeSlider").RegisterCallback<ChangeEvent<float>>(VolumeChanged);*/
        }

        public void EndTurnClicked(ClickEvent evt)
        {
            if (_scene.IsPlayerTurn)
            {

                _scene.EndPlayerTurn();
            }
        }


        public void ShowEndTurnButton(bool show)
        {
            _root.Q<Button>("EndTurnButton").style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }










        



        public void ShowWinLevelMessageBox()
        {
            ShowMessageBox("WinLevelMessageBox", true);
        }
        public void ShowLoseLevelMessageBox()
        {
            ShowMessageBox("LoseLevelMessageBox", true);
        }
        public void ShowWinGameMessageBox()
        {
            ShowMessageBox("WinGameMessageBox", true);
        }

        private void ShowMessageBox(string name, bool show)
        {
            VisualElement element = _root.Q<VisualElement>(name);
            element.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void WinGameMessageBoxOkClicked(ClickEvent evt)
        {
            ShowMessageBox("WinGameMessageBox", false);
            LevelSelectClicked(evt);
        }
        
        private void LoseLevelMessageBoxRetryClicked(ClickEvent evt)
        {
            ShowMessageBox("LoseLevelMessageBox", false);
            RestartLevelClicked(evt);
        }

        private void LoseLevelMessageBoxOkClicked(ClickEvent evt)
        {
            ShowMessageBox("LoseLevelMessageBox", false);
            LevelSelectClicked(evt);
        }

        private void WinLevelMessageBoxOkClicked(ClickEvent evt)
        {
            ShowMessageBox("WinLevelMessageBox", false);
            GameManager.Instance.ShowLevelSelectScene();
         }

        private void RestartLevelClicked(ClickEvent evt)
        {
            int level = GameManager.Instance.Level;
            GameManager.Instance.ShowLevel(level);
        }

        private void LevelSelectClicked(ClickEvent evt)
        {
            GameManager.Instance.ShowLevelSelectScene();
        }


        private void VolumeChanged(ChangeEvent<float> evt)
        {
            GameManager.Instance.SetVolume(evt.newValue / 100f);
        }
/*
        private void HowToPlayClicked(ClickEvent evt)
        {
            VisualElement element = _root.Q<VisualElement>("HowToPlayMessageBox");
            element.style.display = DisplayStyle.Flex;
        }

        private void HideHowToPlay()
        {
            VisualElement element = _root.Q<VisualElement>("HowToPlayMessageBox");
            element.style.display = DisplayStyle.None;
        }

        private void RestartLevelClicked(ClickEvent evt)
        {
            int level = GameManager.Instance.Level;
            GameManager.Instance.ShowLevel(level);
        }

        private void LevelSelectClicked(ClickEvent evt)
        {
            GameManager.Instance.ShowLevelSelectScene();
        }

        public void TestClick()
        {
            Debug.Log("Test click");
        }

        public void UpdateBuffs(string buffName, string remainingEffect)
        {
            SetTextElement("BuffNameText", buffName);
            SetTextElement("RemainingEffectText", remainingEffect);
        }
        private void SetTextElement(string name, string text)
        {
            TextElement element = _root.Q<TextElement>(name);

            if (text == null)
            {
                element.style.display = DisplayStyle.None;
            }
            else
            {
                element.style.display = DisplayStyle.Flex;
                element.text = text;
            }
        }

        private void MessageBoxOkClicked(ClickEvent evt)
        {
            _messageBox.style.display = DisplayStyle.None;

            _messageBoxOkClicked?.Invoke();
            _messageBoxOkClicked = null;
        }

        public void ShowMessage(string title, string message, Action closedCallback)
        {
            _messageBoxOkClicked = closedCallback;

            TextElement titleElement = _messageBox.Q<TextElement>("GeneralMessageBoxTitle");
            titleElement.text = title;

            TextElement textElement = _messageBox.Q<TextElement>("GeneralMessageBoxText");
            textElement.text = message;

            _messageBox.style.display = DisplayStyle.Flex;
        }*/

    }
}