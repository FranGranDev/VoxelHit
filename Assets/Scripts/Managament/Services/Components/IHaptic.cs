using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public interface IHaptic
    {
        ///<summary>
        /// Haptic vibration
        ///</summary>
        void VibrateHaptic();

        ///<summary>
        /// Tiny pop vibration
        ///</summary>
        void VibratePop();
        ///<summary>
        /// Small peek vibration
        ///</summary>
        void VibratePeek();
        ///<summary>
        /// 3 small vibrations
        ///</summary>
        void VibrateNope();
    }
}
