using TMPro;
using UnityEngine;

namespace Pincushion.LD55
{
    public class FloatingTextInstanceController : MonoBehaviour
    {
        private float _ttl = 5f;
        private TextMeshPro _text;

        //private readonly Vector3 velocity = new Vector3(0.025f, 0.01f, 0f);
        private float _speed = 1f;
        private Vector3 _velocity;

        private Transform _lookAt;

        private void Awake()
        {
            _text = GetComponent<TextMeshPro>();
        }

        public void Init(Transform lookAt, Vector3 direction, string s)
        {
            _lookAt = lookAt;

            _velocity = direction * _speed;
            _text.text = s;
        }
        void Update()
        {
            if (_ttl > 0f)
            {
                transform.LookAt(_lookAt);
                transform.Rotate(Vector3.up, 180, Space.Self);

                Vector3 currentPosition = transform.position;
                currentPosition += Time.deltaTime * _velocity;
                transform.position = currentPosition;

                _ttl -= Time.deltaTime;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}