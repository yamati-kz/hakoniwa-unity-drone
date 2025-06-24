using UnityEngine;

namespace hakoniwa.objects.core
{
    public class Wind : MonoBehaviour
    {
        public GameObject stick;
        public GameObject drone;
        public GameObject wind_state;
        public Vector3 wind_direction;

        public float baseRotateSpeed = 2.0f;
        public float maxRotateSpeed = 10.0f;
        public float maxWindStrength = 2.0f;

        public Color colorNoWind = Color.white;
        public Color colorWithWind = Color.red;

        void Update()
        {
            if (stick == null || drone == null || wind_state == null)
                return;

            float windStrength = wind_direction.magnitude;
            float speedRatio = Mathf.Clamp01(windStrength / maxWindStrength);
            float currentRotateSpeed = Mathf.Lerp(baseRotateSpeed, maxRotateSpeed, speedRatio);

            Quaternion targetRotation;

            if (windStrength == 0f)
            {
                targetRotation = Quaternion.identity;
            }
            else
            {
                Transform droneTransform = drone.transform;
                Vector3 localWindDir = droneTransform.InverseTransformDirection(wind_direction.normalized);
                targetRotation = Quaternion.LookRotation(localWindDir, Vector3.up);
            }

            stick.transform.localRotation = Quaternion.Slerp(
                stick.transform.localRotation,
                targetRotation,
                Time.deltaTime * currentRotateSpeed
            );

            // 色を白 or 赤で明確に切り替え
            var renderer = wind_state.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = (windStrength > 0f) ? colorWithWind : colorNoWind;
            }
        }
    }

}
