using System.Collections.Generic;
using hakoniwa.objects.core.sensors;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace hakoniwa.objects.ui
{
    public class MonitorCameraUI : MonoBehaviour
    {
        public MonitorCameraManager cameraManager;
        public RawImage cameraDisplay;
        public TMP_Dropdown cameraDropdown;
        public Button reloadButton;
        private bool isInitialized = false;

        private void Initialize()
        {
            if (isInitialized == false)
            {
                if (cameraManager.IsReady())
                {
                    UpdateCameraList();
                    cameraDropdown.onValueChanged.AddListener(OnCameraSelected);
                    reloadButton.onClick.AddListener(() => {
                        cameraManager.ReloadCameras();
                        UpdateCameraList();
                    });
                    isInitialized = true;
                }
            }
        }

        void Start()
        {
            Initialize();
        }
        private void Update()
        {
            Initialize();
        }
        void UpdateCameraList()
        {
            cameraDropdown.ClearOptions();
            List<string> cameraNames = cameraManager.GetCameraNames();
            cameraDropdown.AddOptions(cameraNames);
            if (cameraNames.Count > 0)
            {
                cameraDropdown.value = 0;
                OnCameraSelected(0);
            }
        }
        void OnCameraSelected(int index)
        {
            string selectedCamera = cameraDropdown.options[index].text;
            var texture = cameraManager.GetCameraRenderTexture(selectedCamera);
            if (texture != null)
            {
                cameraDisplay.texture = texture;
            }

        }
    }
}
