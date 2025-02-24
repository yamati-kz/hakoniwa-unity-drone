using UnityEngine;
using Hakoniwa.DroneService;
using System;
using hakoniwa.pdu.msgs.geometry_msgs;

public class DronePlayer : MonoBehaviour, IDroneBatteryStatus, ISimTime
{
    public GameObject body;
    public int debuff_duration_msec = 100;
    private DroneCollision my_collision;
    private DroneControl drone_control;
    private DronePropeller drone_propeller;
    public bool enable_debuff = false;

    public bool enable_data_logger = false;
    public string debug_logpath = null;

    public string robotName = "Drone";
    public string pdu_name_propeller = "drone_motor";
    public string pdu_name_pos = "drone_pos";

    private void SetPosition(Twist pos, UnityEngine.Vector3 unity_pos, UnityEngine.Vector3 unity_rot)
    {
        pos.linear.x = unity_pos.z;
        pos.linear.y = -unity_pos.x;
        pos.linear.z = unity_pos.y;

        pos.angular.x = -Mathf.Deg2Rad * unity_rot.z;
        pos.angular.y = Mathf.Deg2Rad * unity_rot.x;
        pos.angular.z = -Mathf.Deg2Rad * unity_rot.y;
    }

    void Start()
    {
        my_collision = this.GetComponentInChildren<DroneCollision>();
        if (my_collision == null) {
            throw new Exception("Can not found collision");
        }
        drone_control = this.GetComponentInChildren<DroneControl>();
        if (drone_control == null)
        {
            throw new Exception("Can not found drone control");
        }
        drone_propeller = this.GetComponentInChildren<DronePropeller>();
        if (drone_propeller == null)
        {
            Debug.Log("Can not found drone propeller");
        }
        my_collision.SetIndex(0);

        //string droneConfigText = LoadTextFromResources("config/drone/mujoco/drone_config_0");
        //string controllerConfigText = LoadTextFromResources("config/controller/param-api-mixer-mujoco");
        string droneConfigText = LoadTextFromResources("config/drone/rc/drone_config_0");
        string filename = "org" + "-param-api-mixer";
        string controllerConfigText = LoadTextFromResources("config/controller/" + filename);

        if (string.IsNullOrEmpty(droneConfigText))
        {
            throw new Exception("Failed to load droneConfigText from Resources.");
        }

        if (string.IsNullOrEmpty(controllerConfigText))
        {
            throw new Exception("Failed to load controllerConfigText from Resources.");
        }
        int ret = -1;
        if (debug_logpath.Length == 0)
        {
            ret = DroneServiceRC.InitSingle(droneConfigText, controllerConfigText, enable_data_logger, null);

        }
        else
        {
            ret = DroneServiceRC.InitSingle(droneConfigText, controllerConfigText, enable_data_logger, debug_logpath);

        }
        if (enable_debuff)
        {
            DroneServiceRC.SetDebuffOnCollision(0, debuff_duration_msec);
            Debug.Log("InitSingle: ret = " + ret);

            if (ret != 0)
            {
                throw new Exception("Can not Initialize DroneService RC with InitSingle: debug_logpath= " + debug_logpath);
            }
        }

        // DroneServiceRC.Startの呼び出し
        ret = DroneServiceRC.Start();
        Debug.Log("Start: ret = " + ret);

        if (ret != 0)
        {
            throw new Exception("Can not Start DroneService RC");
        }
    }

    private string LoadTextFromResources(string resourcePath)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);
        return textAsset != null ? textAsset.text : null;
    }

    // Update is called once per frame
    void Update()
    {
        drone_control.HandleInput();
    }




    private void FixedUpdate()
    {
        // 現在位置を記録
        for (int i = 0; i < 20; i++)
        {
            DroneServiceRC.Run();
        }

        double x, y, z;
        int ret = DroneServiceRC.GetPosition(0, out x, out y, out z);
        if (ret == 0)
        {
            UnityEngine.Vector3 unity_pos = new UnityEngine.Vector3();
            unity_pos.z = (float)x;
            unity_pos.x = -(float)y;
            unity_pos.y = (float)z;
            body.transform.position = unity_pos;
        }
        double roll, pitch, yaw;
        ret = DroneServiceRC.GetAttitude(0, out roll, out pitch, out yaw);
        if (ret == 0)
        {
            float rollDegrees = Mathf.Rad2Deg * (float)roll;
            float pitchDegrees = Mathf.Rad2Deg * (float)pitch;
            float yawDegrees = Mathf.Rad2Deg * (float)yaw;

            UnityEngine.Quaternion rotation = UnityEngine.Quaternion.Euler(pitchDegrees, -yawDegrees, -rollDegrees);
            body.transform.rotation = rotation;
        }

        if (drone_propeller != null)
        {
            double c1, c2, c3, c4, c5, c6, c7, c8;
            ret = DroneServiceRC.GetControls(0, out c1, out c2, out c3, out c4, out c5, out c6, out c7, out c8);
            if (ret == 0)
            {
                drone_propeller.Rotate((float)c1, (float)c2, (float)c3, (float)c4);
            }

        }
        RunBatteryStatus();
    }
    private Hakoniwa.DroneService.DroneServiceRC.BatteryStatus battery_status;
    private void RunBatteryStatus()
    {
        var ret = DroneServiceRC.TryGetBatteryStatus(0, out battery_status);
        if (!ret)
        {
            Debug.LogWarning("Can not read battery status");
        }

    }

    private void OnApplicationQuit()
    {
        int ret = DroneServiceRC.Stop();
        Debug.Log("Stop: ret = " + ret);
    }

    public double get_full_voltage()
    {
        return battery_status.FullVoltage;
    }

    public double get_curr_voltage()
    {
        return battery_status.CurrentVoltage;
    }

    public uint get_status()
    {
        return battery_status.Status;
    }

    public uint get_cycles()
    {
        return battery_status.ChargeCycles;
    }

    public double get_temperature()
    {
        return battery_status.CurrentTemperature;
    }

    public long GetWorldTime()
    {
        return (long)DroneServiceRC.GetTimeUsec(0);
    }
}
