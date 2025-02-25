using System;
using hakoniwa.pdu.interfaces;
using hakoniwa.pdu.msgs.geometry_msgs;
using hakoniwa.pdu.msgs.hako_mavlink_msgs;
using hakoniwa.pdu.msgs.hako_msgs;
using hakoniwa.sim;
using hakoniwa.sim.core;
using UnityEngine;

public class DroneAvatar : MonoBehaviour, IHakoObject
{
    IHakoPdu hakoPdu;
    public string robotName = "Drone";
    public string pdu_name_propeller = "motor";
    public string pdu_name_pos = "pos";
    public GameObject body;
    public Rigidbody rd;

    private DronePropeller drone_propeller;

    public void EventInitialize()
    {
        Debug.Log("Event Initialize");
        if (body == null)
        {
            throw new Exception("Body is not assigned");
        }
        if (rd == null)
        {
            throw new Exception("Can not find rigidbody on " + this.gameObject.name);
        }
        drone_propeller = this.GetComponentInChildren<DronePropeller>();
        if (drone_propeller == null)
        {
            Debug.Log("Can not found drone propeller");
        }


        hakoPdu = HakoAsset.GetHakoPdu();
        /*
         * Position
         */
        var ret = hakoPdu.DeclarePduForRead(robotName, pdu_name_pos);
        if (ret == false)
        {
            throw new ArgumentException($"Can not declare pdu for read: {robotName} {pdu_name_pos}");
        }
        /*
         * Propeller
         */
        ret = hakoPdu.DeclarePduForRead(robotName, pdu_name_propeller);
        if (ret == false)
        {
            throw new ArgumentException($"Can not declare pdu for read: {robotName} {pdu_name_propeller}");
        }
    }

    public void EventReset()
    {
        //nothing to do
    }

    public void EventStart()
    {
        //nothing to do
    }

    public void EventStop()
    {
        //nothing to do
    }

    public void EventTick()
    {
        var pduManager = hakoPdu.GetPduManager();
        if (pduManager == null)
        {
            return;
        }

        /*
         * Position
         */
        IPdu pdu_pos = pduManager.ReadPdu(robotName, pdu_name_pos);
        if (pdu_pos == null)
        {
            Debug.Log("Can not get pdu of pos");
        }
        else
        {
            Twist pos = new Twist(pdu_pos);
            //Debug.Log($"Twist ({pos.linear.x} {pos.linear.y} {pos.linear.z})");
            UpdatePosition(pos);
        }

        if (drone_propeller) {
            /*
             * Propeller
             */
            IPdu pdu_propeller = pduManager.ReadPdu(robotName, pdu_name_propeller);
            if (pdu_propeller == null)
            {
                //Debug.Log("Can not get pdu of propeller");
            }
            else
            {
                HakoHilActuatorControls propeller = new HakoHilActuatorControls(pdu_propeller);
                Debug.Log("c1: " + propeller.controls[0]);
                drone_propeller.Rotate((float)propeller.controls[0], (float)propeller.controls[1], (float)propeller.controls[2], (float)propeller.controls[3]);
            }
        }

    }
    public bool enableLerp = false;
    private void UpdatePosition(Twist pos)
    {
        UnityEngine.Vector3 unity_pos = new UnityEngine.Vector3();
        unity_pos.z = (float)pos.linear.x;
        unity_pos.x = -(float)pos.linear.y;
        unity_pos.y = (float)pos.linear.z;
        //body.transform.position = unity_pos;
        //Debug.Log("pos: " + body.transform.position);

        float rollDegrees = Mathf.Rad2Deg * (float)pos.angular.x;
        float pitchDegrees = Mathf.Rad2Deg * (float)pos.angular.y;
        float yawDegrees = Mathf.Rad2Deg * (float)pos.angular.z;

        UnityEngine.Quaternion targetRotation = UnityEngine.Quaternion.Euler(pitchDegrees, -yawDegrees, -rollDegrees);
        //body.transform.rotation = rotation;

        if (enableLerp)
        {
            float speed = 8.0f;
            float step = speed * Time.deltaTime;

            // 位置をLerp
            UnityEngine.Vector3 startPosition = this.rd.position;
            UnityEngine.Vector3 endPosition = unity_pos;
            this.rd.MovePosition(UnityEngine.Vector3.Lerp(startPosition, endPosition, step));

            // 回転をLerp
            UnityEngine.Quaternion startRotation = this.rd.rotation;
            this.rd.MoveRotation(UnityEngine.Quaternion.Lerp(startRotation, targetRotation, step));
        }
        else
        {
            this.rd.MovePosition(unity_pos);
            this.rd.MoveRotation(targetRotation);
        }


    }
}
