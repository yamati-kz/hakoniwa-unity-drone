using hakoniwa.drone;
using hakoniwa.drone.sim;
using hakoniwa.objects.core;
using hakoniwa.objects.core.sensors;
using hakoniwa.pdu.interfaces;
using hakoniwa.pdu.msgs.geometry_msgs;
using hakoniwa.pdu.msgs.hako_mavlink_msgs;
using hakoniwa.sim;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DroneAvatarWeb : MonoBehaviour, IHakoniwaWebObject, IDroneBatteryStatus
{
    public string robotName = "Drone";
    public string pdu_name_propeller = "motor";
    public string pdu_name_pos = "pos";
    public GameObject body;
    private DronePropeller drone_propeller;
    private DroneControl drone_control;
    public bool useBattery = true;
    public string pdu_name_battery = "battery";
    private hakoniwa.pdu.msgs.hako_msgs.HakoBatteryStatus battery_status;
    public DroneCameraController camera_controller;
    private DroneConfig droneConfig;
    private ILiDAR3DController[] lidars;

    void Start()
    {
        if (body == null)
        {
            throw new Exception("Body is not assigned");
        }
        drone_propeller = this.GetComponentInChildren<DronePropeller>();
        if (drone_propeller == null)
        {
            throw new Exception("Can not found drone propeller");
        }
        Debug.Log("max rotation : " + drone_propeller.maxRotationSpeed);
        drone_control = this.GetComponentInChildren<DroneControl>();
        if (drone_control == null)
        {
            Debug.Log("not found DroneControl");
        }
        /*
         * Drone Config
         */
        droneConfig = this.GetComponentInChildren<DroneConfig>();
        if (droneConfig)
        {
            droneConfig.LoadDroneConfig(robotName);
        }

    }
    private float[] prev_controls = new float[4];
    private IPduManager pduManager;
    void FixedUpdate()
    {
        if (pduManager == null)
        {
            pduManager = WebServerBridge.Instance.Get();
            if (pduManager == null)
            {
                Debug.LogWarning("Can not find pduManager...");
                return;
            }
        }

        /*
         * Position
         */
        IPdu pdu_pos = pduManager.ReadPdu(robotName, pdu_name_pos);
        if (pdu_pos == null)
        {
            //Debug.Log("Can not get pdu of pos");
        }
        else
        {
            Twist pos = new Twist(pdu_pos);
            //Debug.Log($"Twist ({pos.linear.x} {pos.linear.y} {pos.linear.z})");
            UpdatePosition(pos);
        }

        /*
         * Propeller
         */
        IPdu pdu_propeller = pduManager.ReadPdu(robotName, pdu_name_propeller);
        if (pdu_propeller == null)
        {
            //Debug.Log("Can not get pdu of propeller");
            drone_propeller.Rotate(prev_controls[0], prev_controls[1], prev_controls[2], prev_controls[3]);
        }
        else
        {
            HakoHilActuatorControls propeller = new HakoHilActuatorControls(pdu_propeller);
            for (int i = 0; i < 4; i++)
            {
                prev_controls[i] = propeller.controls[i];
            }
            //Debug.Log($"c1: {propeller.controls[0]} c2: {propeller.controls[1]} c3: {propeller.controls[2]} c4: {propeller.controls[3]}");
            drone_propeller.Rotate((float)propeller.controls[0], (float)propeller.controls[1], (float)propeller.controls[2], (float)propeller.controls[3]);
        }
        /*
         * Battery
         */
        if (useBattery)
        {
            IPdu pdu_battery = pduManager.ReadPdu(robotName, pdu_name_battery);
            if (pdu_battery != null)
            {
                battery_status = new hakoniwa.pdu.msgs.hako_msgs.HakoBatteryStatus(pdu_battery);
            }
        }
        /*
         * Camera
         */
        if (camera_controller != null)
        {
            camera_controller.DoControl(pduManager);
        }
        /*
         * LiDAR
         */
        if (lidars != null)
        {
            foreach (var lidar in lidars)
            {
                lidar.DoControl(pduManager);
            }
        }

    }
    void Update()
    {
        /*
         * DroneControl
         */
        if (drone_control != null)
        {
            //Debug.Log("Do Drone Control..");
            drone_control.HandleInput();
            if ((pduManager != null) && (camera_controller != null))
            {
                drone_control.HandleCameraControl(camera_controller.GetCameraController(), pduManager);
            }
        }

    }

    private void UpdatePosition(Twist pos)
    {
        UnityEngine.Vector3 unity_pos = new UnityEngine.Vector3();
        unity_pos.z = (float)pos.linear.x;
        unity_pos.x = -(float)pos.linear.y;
        unity_pos.y = (float)pos.linear.z;
        body.transform.position = unity_pos;
        //Debug.Log("pos: " + body.transform.position);

        float rollDegrees = Mathf.Rad2Deg * (float)pos.angular.x * 1;
        float pitchDegrees = Mathf.Rad2Deg * (float)pos.angular.y * 1;
        float yawDegrees = Mathf.Rad2Deg * (float)pos.angular.z;

        UnityEngine.Quaternion rotation = UnityEngine.Quaternion.Euler(pitchDegrees, -yawDegrees, -rollDegrees);
        body.transform.rotation = rotation;
    }

    public async Task DeclarePduAsync()
    {
        var pdu_manager = WebServerBridge.Instance.Get();
        if (pdu_manager == null)
        {
            throw new Exception("Can not get Pdu Manager");
        }
        //this.robotName = robot_name;
        var ret = await pdu_manager.DeclarePduForRead(robotName, pdu_name_pos);
        Debug.Log("declare pdu pos: " + ret);
        ret = await pdu_manager.DeclarePduForRead(robotName, pdu_name_propeller);
        Debug.Log("declare pdu propeller: " + ret);
        /*
         * Battery
         */
        if (useBattery)
        {
            ret = await pdu_manager.DeclarePduForRead(robotName, pdu_name_battery);
            if (ret == false)
            {
                throw new ArgumentException($"Can not declare pdu for read: {robotName} {pdu_name_battery}");
            }
        }

        foreach (UnityEngine.Transform child in this.transform)
        {
            var subObjects = child.GetComponentsInChildren<IHakoniwaWebObject>();
            foreach (var obj in subObjects)
            {
                if ((UnityEngine.Object)obj != (UnityEngine.Object)this
                    && obj is MonoBehaviour mb
                    && mb.enabled
                    && mb.gameObject.activeInHierarchy)
                {
                    await obj.DeclarePduAsync();
                }

            }
        }
        /*
         * Camera
         */
        if (camera_controller)
        {
            camera_controller.GetCameraController().DelclarePdu(robotName, pdu_manager);
        }
        /*
         * LiDAR
         */
        lidars = this.GetComponentsInChildren<ILiDAR3DController>();
        if (lidars != null)
        {
            if (droneConfig)
            {
                droneConfig.SetLidarPosition(robotName);
            }
            foreach (var lidar in lidars)
            {
                lidar.DoInitialize(robotName, pdu_manager);
            }
        }

    }

    public double get_full_voltage()
    {
        if (battery_status != null)
        {
            return battery_status.full_voltage;
        }
        return 0;
    }

    public double get_curr_voltage()
    {
        if (battery_status != null)
        {
            return battery_status.curr_voltage;
        }
        return 0;
    }

    public uint get_status()
    {
        if (battery_status != null)
        {
            return battery_status.status;
        }
        return 0;
    }

    public uint get_cycles()
    {
        if (battery_status != null)
        {
            return battery_status.cycles;
        }
        return 0;

    }

    public double get_temperature()
    {
        if (battery_status != null)
        {
            return battery_status.curr_temp;
        }
        return 0;
    }
}
