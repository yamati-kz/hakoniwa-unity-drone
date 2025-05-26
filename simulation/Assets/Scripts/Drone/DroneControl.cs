using hakoniwa.objects.core;
using hakoniwa.pdu.interfaces;
using UnityEngine;

namespace hakoniwa.drone
{
    public enum DroneControlInputType
    {
        PS4,
        Xbox,
        Xr
    }
    public interface IDroneControlOp
    {
        public void DoInitialize(string robotName);
        public int PutRadioControlButton(int index, int value);
        public int PutHorizontal(int index, double value);
        public int PutForward(int index, double value);
        public int PutHeading(int index, double value);
        public int PutVertical(int index, double value);
        public void DoFlush();
    }

    public class DroneControl : MonoBehaviour
    {
        public string robotName = "Drone";
        public DroneControlInputType input_type;
        public double stick_strength = 0.1;
        public double stick_yaw_strength = 1.0;
        private IDroneInput controller_input;
        public bool magnet_on = false;
        public GameObject grabberObject;
        private IBaggageGrabber grabber;
        public bool forceGrab = false;
        private IDroneControlOp droneControlOp = null;
        public bool isPlayer = true;

        public bool IsMagnetOn()
        {
            return magnet_on;
        }
        private void Awake()
        {
            if (isPlayer)
            {
                droneControlOp = new DroneControlRc();
            }
            else
            {
                droneControlOp = this.GetComponentInChildren<IDroneControlOp>();
            }
            droneControlOp.DoInitialize(robotName);
        }

        private void Start()
        {
            if (grabberObject != null)
            {
                grabber = grabberObject.GetComponentInChildren<IBaggageGrabber>();
                Debug.Log("grabber: " + grabber);
            }
            else
            {
                Debug.Log("Gabber is not found.");
            }

            if (input_type == DroneControlInputType.PS4)
            {
                controller_input = HakoDroneInputManager.Instance;
            }
            else if (input_type == DroneControlInputType.Xbox)
            {
                //TODO
            }
            else
            {
                controller_input = HakoDroneXrInputManager.Instance;
            }
        }

        public void HandleInput()
        {
            Vector2 leftStick = controller_input.GetLeftStickInput();
            Vector2 rightStick = controller_input.GetRightStickInput();
            float horizontal = rightStick.x;
            float forward = rightStick.y;
            float yaw = leftStick.x;
            float pitch = leftStick.y;

            if (controller_input.IsAButtonPressed())
            {
                droneControlOp.PutRadioControlButton(0, 1);
            }
            else if (controller_input.IsAButtonReleased())
            {
                droneControlOp.PutRadioControlButton(0, 0);
            }
            if (controller_input.IsBButtonReleased())
            {
                magnet_on = IsMagnetOn() ? false : true;
                if (grabber != null)
                {
                    if (magnet_on)
                    {
                        grabber.Grab(forceGrab);
                    }
                    else
                    {
                        grabber.Release();
                    }
                }

            }
            droneControlOp.PutHorizontal(0, horizontal * stick_strength);
            droneControlOp.PutForward(0, -forward * stick_strength);
            droneControlOp.PutHeading(0, yaw * stick_yaw_strength);
            droneControlOp.PutVertical(0, -pitch * stick_strength);

            droneControlOp.DoFlush();
        }

    }
}
