using System;
using hakoniwa.drone.sim;
using hakoniwa.pdu.interfaces;
using hakoniwa.pdu.unity;
using hakoniwa.sim;
using UnityEngine;
using UnityEngine.UI;

namespace hakoniwa.drone.sim
{
    public class CameraController : MonoBehaviour
    {
        public string sensor_name = "hakoniwa_camera";
        public string pdu_name_cmd_camera = "hako_cmd_camera";
        public string pdu_name_camera_data = "hako_camera_data";
        public string pdu_name_cmd_camera_move = "hako_cmd_camera_move";
        public string pdu_name_camera_info = "hako_cmd_camera_info";

        public float camera_move_up_deg = -15.0f;
        public float camera_move_down_deg = 90.0f;
        private RenderTexture RenderTextureRef;
        private Texture2D tex;
        public int width = 640;
        public int height = 480;
        private byte[] raw_bytes;
        private byte[] compressed_bytes;
        private Camera my_camera;
        public RawImage displayImage; // 映像を表示するUIのRawImage

        /*
         * Camera Image
         */
        public int current_id = -1;
        public int request_id = 0;
        public int encode_type = 0;
        /*
         * Camera Move
         */
        public int move_current_id = -1;
        public int move_request_id = 0;
        public float move_step = 1.0f;  // 一回の動きのステップ量
        private float camera_move_button_time_duration = 0f;
        public float camera_move_button_threshold_speedup = 1.0f;

        private string robotName;

        private void Awake()
        {
            this.my_camera = this.GetComponentInChildren<Camera>();
            var texture = new Texture2D(this.width, this.height, TextureFormat.RGB24, false);
            this.RenderTextureRef = new RenderTexture(texture.width, texture.height, 32);
            this.my_camera.targetTexture = this.RenderTextureRef;
        }

        void LateUpdate()
        {
            Vector3 parentEulerAngles = my_camera.transform.parent.eulerAngles;
            my_camera.transform.localEulerAngles = new Vector3(manual_rotation_deg - parentEulerAngles.x, 0, -parentEulerAngles.z);
        }

        public void DoInitialize(string robot_name, IHakoPdu hakoPdu)
        {
            this.robotName = robot_name;
            var ret = hakoPdu.DeclarePduForRead(robotName, pdu_name_cmd_camera);
            if (ret == false)
            {
                throw new ArgumentException($"Can not declare pdu for read: {robotName} {pdu_name_cmd_camera}");
            }
            ret = hakoPdu.DeclarePduForRead(robotName, pdu_name_cmd_camera_move);
            if (ret == false)
            {
                throw new ArgumentException($"Can not declare pdu for read: {robotName} {pdu_name_cmd_camera_move}");
            }
            ret = hakoPdu.DeclarePduForWrite(robotName, pdu_name_camera_data);
            if (ret == false)
            {
                throw new ArgumentException($"Can not declare pdu for write: {robotName} {pdu_name_camera_data}");
            }
            ret = hakoPdu.DeclarePduForWrite(robotName, pdu_name_camera_info);
            if (ret == false)
            {
                throw new ArgumentException($"Can not declare pdu for write: {robotName} {pdu_name_camera_info}");
            }
        }
        private void WriteCameraDataPdu(IPduManager pduManager)
        {
            INamedPdu pdu = pduManager.CreateNamedPdu(robotName, pdu_name_camera_data);
            if (pdu == null)
            {
                throw new ArgumentException($"Can not create pdu for write: {robotName} {pdu_name_camera_data}");
            }
            var camera_data = new hakoniwa.pdu.msgs.hako_msgs.HakoCameraData(pdu);
            camera_data.request_id = current_id;
            TimeStamp.Set(camera_data.image.header);
            camera_data.image.header.frame_id = this.sensor_name;
            if (encode_type == 0)
            {
                camera_data.image.format = "png";
            }
            else
            {
                camera_data.image.format = "jpeg";
            }
            camera_data.image.data = compressed_bytes;
#if false
        // ファイルに書き出す
        string filePath = Path.Combine("./", $"{sensor_name}.{camera_data.image.format}");
        try
        {
            File.WriteAllBytes(filePath, camera_data.image.data);
            Debug.Log($"Image saved: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save image: {e.Message}");
        }
#endif
            pduManager.WriteNamedPdu(pdu);
            pduManager.FlushNamedPdu(pdu);
            //Debug.Log("Write Camera Data done!!: size of compressed_bytes = " + compressed_bytes.Length);
        }
        private void WriteCameraInfo(IPduManager pduManager)
        {
            INamedPdu pdu = pduManager.CreateNamedPdu(robotName, pdu_name_camera_info);
            if (pdu == null)
            {
                throw new ArgumentException($"Can not create pdu for write: {robotName} {pdu_name_camera_info}");
            }
            var camera_info = new hakoniwa.pdu.msgs.hako_msgs.HakoCameraInfo(pdu);

            camera_info.request_id = move_current_id;
            camera_info.angle.x = 0;
            camera_info.angle.y = this.manual_rotation_deg;
            camera_info.angle.z = 0;

            pduManager.WriteNamedPdu(pdu);
            pduManager.FlushNamedPdu(pdu);
            //Debug.Log("Write done");
        }

        public void DoControl(IPduManager pduManager)
        {
            /*
             * Camera Image Request
             */
            IPdu pdu_cmd_camera = pduManager.ReadPdu(robotName, pdu_name_cmd_camera);
            if (pdu_cmd_camera != null)
            {
                var cmd_camera = new hakoniwa.pdu.msgs.hako_msgs.HakoCmdCamera(pdu_cmd_camera);
                if (cmd_camera.header.request)
                {
                    request_id = cmd_camera.request_id;
                    encode_type = cmd_camera.encode_type;

                    //Debug.Log("Camera shot request: request_id = " + request_id);
                    if (current_id != request_id)
                    {
                        current_id = request_id;
                        this.Scan();
                        this.WriteCameraDataPdu(pduManager);
                    }
                    else
                    {
                        //Debug.Log("request id is invalid");
                    }
                }
            }
            /*
             * Camera Move Request
             */
            IPdu pdu_camera_move = pduManager.ReadPdu(robotName, pdu_name_cmd_camera_move);
            var camera_move = new hakoniwa.pdu.msgs.hako_msgs.HakoCmdCameraMove(pdu_camera_move);
            if (camera_move.header.request)
            {
                move_request_id = camera_move.request_id;
                if (move_current_id != move_request_id)
                {
                    move_current_id = move_request_id;
                    var target_degree = (float)camera_move.angle.y;
                    SetCameraAngle(-target_degree);
                    Debug.Log("reqest move: " + target_degree + " current deg: " + this.manual_rotation_deg);
                    this.WriteCameraInfo(pduManager);
                }
            }

            var controller = GameController.Instance;
            if (controller && controller.GetRadioControlOn())
            {
                /*
                 * Camera Image Rc request
                 */
                if (controller.GetCameraShotOn())
                {
                    Debug.Log("SHOT!!");
                    this.Scan();
                    this.WriteCameraDataPdu(pduManager);
                }

                if (controller.GetCameraMoveUp())
                {
                    camera_move_button_time_duration += Time.fixedDeltaTime;
                    if (camera_move_button_time_duration > camera_move_button_threshold_speedup)
                    {
                        RotateCamera(-move_step * 3f);
                    }
                    else
                    {
                        RotateCamera(-move_step);
                    }

                }

                if (controller.GetCameraMoveDown())
                {
                    camera_move_button_time_duration += Time.fixedDeltaTime;
                    if (camera_move_button_time_duration > camera_move_button_threshold_speedup)
                    {
                        RotateCamera(move_step * 3f);
                    }
                    else
                    {
                        RotateCamera(move_step);
                    }
                }
                if (!controller.GetCameraMoveDown() && !controller.GetCameraMoveUp())
                {
                    camera_move_button_time_duration = 0f;
                }
            }

            if (displayImage != null)
            {
                displayImage.texture = this.RenderTextureRef;
            }
        }
        private float manual_rotation_deg = 0;
        private void RotateCamera(float step)
        {
            float newPitch = manual_rotation_deg + step;

            // ピッチを-90度から15度の間に制限
            if (newPitch > 180) newPitch -= 360; // Convert angles greater than 180 to negative values
            newPitch = Mathf.Clamp(newPitch, this.camera_move_up_deg, this.camera_move_down_deg);
            manual_rotation_deg = newPitch;
        }
        private void SetCameraAngle(float angle)
        {
            float newPitch = angle;

            // ピッチを-90度から15度の間に制限
            if (newPitch > 180) newPitch -= 360; // Convert angles greater than 180 to negative values
            newPitch = Mathf.Clamp(newPitch, this.camera_move_up_deg, this.camera_move_down_deg);
            manual_rotation_deg = newPitch;
        }
        private void Scan()
        {
            tex = new Texture2D(RenderTextureRef.width, RenderTextureRef.height, TextureFormat.RGB24, false);
            RenderTexture.active = RenderTextureRef;
            int width = RenderTextureRef.width;
            int height = RenderTextureRef.height;
            int step = width * 3;
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();
            byte[] _byte = tex.GetRawTextureData();
            raw_bytes = new byte[_byte.Length];
            for (int i = 0; i < height; i++)
            {
                System.Array.Copy(_byte, i * step, raw_bytes, (height - i - 1) * step, step);
            }

            // Encode texture
            if (encode_type == 0)
            {
                compressed_bytes = tex.EncodeToPNG();
            }
            else
            {
                compressed_bytes = tex.EncodeToJPG();
            }
            UnityEngine.Object.Destroy(tex);
        }
    }
}
