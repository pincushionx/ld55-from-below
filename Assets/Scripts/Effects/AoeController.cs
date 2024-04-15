using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pincushion.LD55
{
    public class AoeController : MonoBehaviour
    {

        public GameObject InnerSphere;
        public GameObject OuterSphere;

        private float _aoeTimerStart = 0;

        // These times add up
        private float _rampUpDuration = 1f;
        private float _atMaxDuration = 0.5f;
        private float _rampDownDuration = .25f;

        private Vector3 _originalInnerSphereScale;
        private Vector3 _originalOuterSphereScale;

        private void Awake()
        {
            _originalInnerSphereScale = InnerSphere.transform.localScale;
            _originalOuterSphereScale = OuterSphere.transform.localScale;

            Cleanup();
        }

        private void Cleanup()
        {
            InnerSphere.SetActive(false);
            OuterSphere.SetActive(false);
        }
        public void DoAoE()
        {
            _aoeTimerStart = Time.time;

            InnerSphere.SetActive(true);
            OuterSphere.SetActive(true);

            InnerSphere.transform.localScale = Vector3.zero;
            OuterSphere.transform.localScale = Vector3.zero;
        }

        // Update is called once per frame
        void Update()
        {
            if (_aoeTimerStart > 0)
            {
                float timeElapsed = Time.time - _aoeTimerStart;

                InnerSphere.transform.Rotate(Vector3.up, Time.deltaTime * -500f);
                OuterSphere.transform.Rotate(Vector3.up, Time.deltaTime * 500f);

                if (timeElapsed < _rampUpDuration)
                {
                    InnerSphere.transform.localScale = Vector3.Slerp(Vector3.zero, _originalInnerSphereScale, timeElapsed / _rampUpDuration);
                    OuterSphere.transform.localScale = Vector3.Slerp(Vector3.zero, _originalOuterSphereScale, timeElapsed / _rampUpDuration);
                }
                else if (timeElapsed < _atMaxDuration + _rampUpDuration)
                {
                    // Do nothing. Just keep it here
                }
                else if (timeElapsed < _rampDownDuration + _atMaxDuration + _rampUpDuration)
                {
                    float rampDownTimeElapsed = _aoeTimerStart - (_atMaxDuration + _rampUpDuration);
                    InnerSphere.transform.localScale = Vector3.Slerp(_originalInnerSphereScale, Vector3.zero, rampDownTimeElapsed / _rampDownDuration);
                    OuterSphere.transform.localScale = Vector3.Slerp(_originalOuterSphereScale, Vector3.zero, rampDownTimeElapsed / _rampDownDuration);
                }
                else
                {
                    Cleanup();
                    _aoeTimerStart = 0;
                }
            }
        }
    }
}