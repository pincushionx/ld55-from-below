using UnityEngine;
using UnityEngine.UIElements;

namespace Pincushion.LD55
{
    public class LevelSelectScreenOverlayController : MonoBehaviour
    {
        [SerializeField] private UIDocument _document;
        private VisualElement _root;
        private void Awake()
        {
            _root = _document.rootVisualElement;
        }

        private void Start()
        {
            ShowLevels();
        }

        private void ShowLevels()
        {
            VisualTreeAsset itemTemplate = Resources.Load<VisualTreeAsset>("LevelSelectItem");
            VisualElement container = _root.Q<VisualElement>("LevelSelectItems");

            container.Clear();

            foreach (LevelData data in GameManager.Instance.LevelData)
            {
                VisualElement item = itemTemplate.Instantiate();

                string statusText = "";
                if (data.CompletedLevel)
                {
                    statusText = " (Completed)";
                }
                else if (data.PlayedLevel)
                {
                    statusText = " (Played)";
                }


                //string levelText = (data.Id + 1).ToString() + ". ";
                string levelText = "Level " + (data.Id + 1).ToString() + statusText + "\n";

                Button button = item.Q<Button>("LevelSelectButton");
                //button.text = levelText + "\"" + data.Name + "\"" + statusText;
                button.text = levelText + "\"" + data.Name + "\"";
                button.RegisterCallback<ClickEvent>(e => LevelClicked(e, data));

                container.Add(item);
            }
        }

        private void LevelClicked(ClickEvent evt, LevelData data)
        {
            GameManager.Instance.ShowLevel(data.Id);
        }
    }
}