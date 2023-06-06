using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using Animations;


namespace UI.Items
{
    public abstract class BarBase : MonoBehaviour, IBindable<IFillEvent>
    {
        public void Bind(IFillEvent fillEvent)
        {
            fillEvent.OnFilled += Fill;

            Fill(0);
        }

        protected abstract void Fill(float value);
    }
}
