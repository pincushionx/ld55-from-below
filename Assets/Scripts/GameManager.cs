using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pincushion.LD55
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        public int Level = 0;
        public List<LevelData> LevelData = new List<LevelData>(); // list of levels and user data
        public bool WinGamePopupShown = false;


        private void Awake()
        {
            InitializeLevelData();
        }
        private void InitializeLevelData()
        {


            LevelData data = new LevelData();
            data.Id = 0;
            data.Name = "Short and with a little help";
            LevelData.Add(data);

            data = new LevelData();
            data.Id = 1;
            data.Name = "Level 1";
            LevelData.Add(data);

            data = new LevelData();
            data.Id = 2;
            data.Name = "Level 2";
            LevelData.Add(data);

            data = new LevelData();
            data.Id = 3;
            data.Name = "Level 3";
            LevelData.Add(data);

            data = new LevelData();
            data.Id = 4;
            data.Name = "Level 4";
            LevelData.Add(data);
        }


        public LevelData GetLevelData(int level)
        {
            foreach (LevelData data in LevelData)
            {
                if (data.Id == level)
                {
                    return data;
                }
            }
            return null;
        }

        public bool GameComplete()
        {
            foreach (LevelData data in LevelData)
            {
                if (!data.CompletedLevel)
                {
                    return false;
                }
            }
            return true;
        }

        public void ShowStartScreen()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
        }

        public void ShowLevelSelectScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelectScene");
        }

        public void ShowLevel(int level)
        {
            Level = level;
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        }

        public void SetVolume(float volume)
        {
            AudioListener.volume = volume;
        }
    }
}