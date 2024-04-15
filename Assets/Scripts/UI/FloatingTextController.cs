using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pincushion.LD55
{
    public class FloatingTextController : MonoBehaviour
    {
        [SerializeField] private GameObject _floatingTextPrefab;
        [SerializeField] private Transform _lookAt;

        private GameSceneController _scene;

        public void Init(GameSceneController scene)
        {
            _scene = scene;
            _lookAt = _scene.Camera.transform;
        }


        public void SpawnFloatingText(string text)
        {
            GameObject go = Instantiate(_floatingTextPrefab);
            go.transform.position = transform.position + transform.up;

            FloatingTextInstanceController controller = go.GetComponent<FloatingTextInstanceController>();
            controller.Init(_lookAt, transform.up, text);
        }

        public void SpawnFloatingText(Vector3 position, Vector3 direction, string text)
        {
            GameObject go = Instantiate(_floatingTextPrefab);
            go.transform.position = position;

            FloatingTextInstanceController controller = go.GetComponent<FloatingTextInstanceController>();
            controller.Init(_lookAt, direction, text);
            
        }
    }
}