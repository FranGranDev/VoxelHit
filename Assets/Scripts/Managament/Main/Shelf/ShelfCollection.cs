using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Managament.Shelfs
{
    public class ShelfCollection : MonoBehaviour, IEnumerable<Shelf>
    {
        public const int ShelfCount = 3;

        [SerializeField] private List<Shelf> shelfs;
        [SerializeField] private List<ParticleSystem> confetties;

        public List<VoxelModelsData.Group> Groups { get; private set; }
        public int Index { get; private set; }
        public IEnumerator<Shelf> GetEnumerator()
        {
            return shelfs.GetEnumerator();
        }


        public void Initialize(Dictionary<string, List<ModelId>> dictionary, List<VoxelModelsData.Group> groups, int index, bool async = false)
        {
            Index = index;
            Groups = groups;

            for(int i = 0; i < shelfs.Count; i++)
            {
                if (i < groups.Count)
                {
                    shelfs[i].Initialize(dictionary[groups[i].Id], groups[i], async);
                }
                else
                {
                    shelfs[i].Hide();
                }
            }
        }

        public void ConfettiBoom()
        {
            foreach(ParticleSystem particleSystem in confetties)
            {
                particleSystem.transform.parent = null;
                particleSystem.Play();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return shelfs.GetEnumerator();
        }
    }
}
