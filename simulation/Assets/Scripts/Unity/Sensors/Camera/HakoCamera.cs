using UnityEngine;

namespace hakoniwa.objects.core.sensors
{
    public class HakoCamera : MonoBehaviour
    {
        private Camera _camera;
        private RenderTexture _renderTexture;

        public void ConfigureCamera(string cameraId, string cameraType, Vector3 position, Vector3 rotation, float fov, int width, int height)
        {
            _camera = GetComponent<Camera>();
            if (_camera == null)
            {
                Debug.LogError("HakoCamera: Camera component not found!");
            }
            Debug.Log("Camera is found");
            // カメラID（オブジェクト名として設定）
            this.gameObject.name = cameraId;

            // 座標と回転の適用
            transform.position = position;
            transform.rotation = Quaternion.Euler(rotation);

            // FOV（視野角）の適用
            if (_camera != null)
            {
                _camera.fieldOfView = fov;
            }

            // RenderTextureの作成 & 解像度設定
            if (_camera != null)
            {
                _renderTexture = new RenderTexture(width, height, 24); // 24はDepth Buffer
                _camera.targetTexture = _renderTexture;
            }

            Debug.Log($"Configured Camera: {cameraId} - Type: {cameraType}, FOV: {fov}, Position: {position}, Rotation: {rotation}, Resolution: {width}x{height}");
        }

        public RenderTexture GetRenderTexture()
        {
            return _camera.targetTexture;
        }
        public float GetFov()
        {
            return _camera.fieldOfView;
        }

    }
}
