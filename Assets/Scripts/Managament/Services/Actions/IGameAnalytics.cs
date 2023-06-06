using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public interface IGameAnalytics
    {
        void TrackLevelStart(int level);
        void TrackLevelFail(int level);
        void TrackLevelCompleate(int level);
    }
}
