using Data;
using Services;

namespace Vibration
{
    public class Haptic : IHaptic
    {
        static Haptic()
        {
            Vibration.Init();
        }

        public void VibrateHaptic()
        {
            if (!SavedData.VibroEnabled)
                return;
#if UNITY_ANDROID
            Vibration.Vibrate(5L);
#else
            Vibration.VibratePop();
#endif
        }
        public void VibratePop()
        {
            if (!SavedData.VibroEnabled)
                return;
            Vibration.VibratePop();
        }
        public void VibratePeek()
        {
            if (!SavedData.VibroEnabled)
                return;
            Vibration.VibratePeek();
        }
        public void VibrateNope()
        {
            if (!SavedData.VibroEnabled)
                return;
            Vibration.VibrateNope();
        }
    }
}