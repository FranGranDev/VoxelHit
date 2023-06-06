using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sound Data", menuName = "Game Data/Sounds")]
public class SoundsData : ScriptableObject
{
    [SerializeField] private List<Item> soundItems = new List<Item>();
    [SerializeField] private List<Item> musicItems = new List<Item>();

    public List<Item> Sounds { get { return soundItems; } }
    public List<Item> Music { get { return soundItems; } }


    [System.Serializable]
    public class Item
    {
        [Header("Название(для удобства)")]
        public string name;
        [Header("ID")]
        public string id;
        [Header("Клип")]
        [SerializeField] private AudioClip[] Clips;
        public AudioClip Clip
        {
            get
            {
                if (Clips.Length == 0)
                    return null;
                return Clips[Random.Range(0, Clips.Length)];
            }
        }
        [Header("Громкость")]
        [Range(0, 1)]
        public float Volume;
        [Header("Тон")]
        [Range(0, 2)]
        public float Pitch;
        [Header("Объемность звука(2D/3D)")]
        [Range(0, 1)]
        public float SpatialBlend;
        [Header("Максимальная дальность звука")]
        public float MaxDistance;
        [Header("Максимальное кол-во звуков одновременно")]
        public int maxInstance;
    }
}


