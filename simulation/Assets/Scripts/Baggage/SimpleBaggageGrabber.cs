using UnityEngine;

namespace hakoniwa.objects.core
{
    public class SimpleBaggageGrabber : MonoBehaviour, IBaggageGrabber
    {
        public Magnet magnet;

        public void Grab()
        {
            magnet.TurnOn();
        }

        public void Release()
        {
            magnet.TurnOff();
        }
    }
}
