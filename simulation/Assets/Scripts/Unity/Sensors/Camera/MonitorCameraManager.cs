using System.Collections.Generic;
using hakoniwa.objects.core.frame;
using UnityEngine;

namespace hakoniwa.objects.core.sensors
{
    public class MonitorCameraManager : MonoBehaviour
    {
        public string monitorCameraConfigPath = "./monitor_camera_config.json";
        public GameObject cameraPrefab;
        private List<HakoCamera> hakoCameras;
        private bool isReady = false;
        public bool IsReady()
        {
            return isReady;
        }
        public List<string> GetCameraNames()
        {
            if (isReady)
            {
                List<string> cameraNames = new List<string>();
                foreach (var camera in hakoCameras)
                {
                    cameraNames.Add(camera.name);
                }
                return cameraNames;
            }
            else
            {
                return null;
            }
        }
        public RenderTexture GetCameraRenderTexture(string cameraName)
        {
            if (isReady)
            {
                foreach (var camera in hakoCameras)
                {
                    if (camera.name == cameraName)
                    {
                        return camera.GetRenderTexture();
                    }
                }
            }
            return null;
        }
        public (Vector3 position, Vector3 rotation, float fov) GetCameraInfo(string cameraName)
        {
            if (isReady)
            {
                foreach (var camera in hakoCameras)
                {
                    if (camera.name == cameraName)
                    {
                        return (camera.transform.position, camera.transform.eulerAngles, camera.GetFov());
                    }
                }
            }
            return (Vector3.zero, Vector3.zero, 0);
        }

        void Start()
        {
            SetCameras();
            isReady = true;
        }
        public void ReloadCameras()
        {
            RemoveCamers();
            SetCameras();
        }
        private void RemoveCamers()
        {
            if (hakoCameras != null)
            {
                foreach (var hakoCamera in hakoCameras)
                {
                    if (hakoCamera != null)
                    {
                        Destroy(hakoCamera.gameObject);
                    }
                }
                hakoCameras.Clear();
            }
        }
        private void SetCameras()
        {
            var cameraConfig = CameraConfigLoader.LoadConfig(monitorCameraConfigPath);

            if (cameraConfig == null)
            {
                Debug.LogError("Failed to load camera configuration.");
                return;
            }
            hakoCameras = new List<HakoCamera>();
            foreach (var camData in cameraConfig.monitor_cameras)
            {
                GameObject newCamera = Instantiate(cameraPrefab);
                HakoCamera hakoCamera = newCamera.GetComponent<HakoCamera>();

                if (hakoCamera != null)
                {
                    newCamera.transform.SetParent(this.transform);
                    newCamera.transform.name = camData.pdu_info.robot_name;
                    Vector3 position = new Vector3(camData.coordinate_system.position.x, camData.coordinate_system.position.y, camData.coordinate_system.position.z);
                    Vector3 rotation = new Vector3(camData.coordinate_system.orientation.roll, camData.coordinate_system.orientation.pitch, camData.coordinate_system.orientation.yaw);

                    position = FrameConvertor.PosRos2Unity(position);
                    rotation = FrameConvertor.EulerRosDeg2UnityDeg(rotation);
                    hakoCamera.ConfigureCamera(
                        camData.pdu_info.robot_name,
                        camData.camera_type,
                        position,
                        rotation,
                        camData.fov.horizontal,
                        camData.resolution.width,
                        camData.resolution.height
                    );
                    hakoCameras.Add(hakoCamera);
                }
            }
        }
    }
}