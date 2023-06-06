using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public interface ICursor
    {
        public Transform GetTransform { get; }
        Vector3 Position { get; set; }
        bool Disabled { get; set; }
        bool Hidden { get; set; }

        public void Down();
        public void Up();
    }
}
