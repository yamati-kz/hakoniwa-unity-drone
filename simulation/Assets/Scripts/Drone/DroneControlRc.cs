using hakoniwa.drone.service;
using JetBrains.Annotations;
using UnityEngine;

namespace hakoniwa.drone
{
    public class DroneControlRc : IDroneControlOp
    {
        public void DoFlush()
        {
            //nothing to do
        }

        public void DoInitialize(string robotName)
        {
            //nothing to do
        }

        public int PutForward(int index, double value)
        {
            return DroneServiceRC.PutForward(index, value);
        }

        public int PutHeading(int index, double value)
        {
            return DroneServiceRC.PutHeading(index, value);
        }

        public int PutHorizontal(int index, double value)
        {
            return DroneServiceRC.PutHorizontal(index, value);
        }

        public int PutRadioControlButton(int index, int value)
        {
            return DroneServiceRC.PutRadioControlButton(index, value);
        }

        public int PutVertical(int index, double value)
        {
            return DroneServiceRC.PutVertical(index, value);
        }
    }
}

