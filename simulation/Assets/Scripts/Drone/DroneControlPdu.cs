using hakoniwa.drone.sim;
using hakoniwa.pdu.core;
using hakoniwa.pdu.interfaces;
using hakoniwa.pdu.msgs.hako_msgs;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace hakoniwa.drone
{
    public class DroneControlPdu : MonoBehaviour, IDroneControlOp, IHakoniwaWebObject
    {
        public const int game_ops_arm_button_index = 0;
        public const int game_ops_grab_baggage_button_index = 1;
        public const int game_ops_camera_button_index = 2;
        public const int game_ops_camera_move_up_index = 11;
        public const int game_ops_camera_move_down_index = 12;

        public const int game_ops_stick_turn_lr = 0;
        public const int game_ops_stick_up_down = 1;
        public const int game_ops_stick_move_lr = 2;
        public const int game_ops_stick_move_fb = 3;

        private string robotName = "Drone";
        private string pdu_name = "hako_cmd_game";
        private IPduManager pdu_manager;
        private bool[] button;
        private double[] axis;

        void Awake()
        {
            axis = new double[6];
            button = new bool[15];

        }

        public async Task DeclarePduAsync()
        {
            pdu_manager = WebServerBridge.Instance.Get();
            if (pdu_manager == null)
            {
                throw new Exception("Can not get Pdu Manager");
            }
            var ret = await pdu_manager.DeclarePduForWrite(robotName, pdu_name);
            Debug.Log("declare pdu hako_cmd_game: " + ret);

        }

        public void DoFlush()
        {
            if (pdu_manager == null)
            {
                return;
            }
            var pdu_cmd_game_ctrl = pdu_manager.CreateNamedPdu(robotName, pdu_name);
            if (pdu_cmd_game_ctrl == null)
            {
                return;
            }
            var cmd_game_ctrl = new hakoniwa.pdu.msgs.hako_msgs.GameControllerOperation(pdu_cmd_game_ctrl);
            cmd_game_ctrl.button = button;
            cmd_game_ctrl.axis = axis;

            //Debug.Log("Abutton: button: " + button[game_ops_arm_button_index] + " pdu: " + cmd_game_ctrl.button[game_ops_arm_button_index]);
            pdu_manager.WriteNamedPdu(pdu_cmd_game_ctrl);
            pdu_manager.FlushNamedPdu(pdu_cmd_game_ctrl);
        }

        public void DoInitialize(string robotName)
        {
            this.robotName = robotName;
        }

        public int PutForward(int index, double value)
        {
            axis[game_ops_stick_move_fb] = value;
            return 0;
        }

        public int PutHeading(int index, double value)
        {
            axis[game_ops_stick_turn_lr] = value;
            return 0;
        }

        public int PutHorizontal(int index, double value)
        {

            axis[game_ops_stick_move_lr] = value;
            return 0;
        }
        public int PutVertical(int index, double value)
        {
            axis[game_ops_stick_up_down] = value;
            return 0;
        }

        public int PutRadioControlButton(int index, int value)
        {
            Debug.Log("PutRadioControlButton: value = " + value);
            button[game_ops_arm_button_index] = (value != 0);
            return 0;
        }


    }

}
