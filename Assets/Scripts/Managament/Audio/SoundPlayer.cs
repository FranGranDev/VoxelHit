using DG.Tweening;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Audio
{
    public class SoundPlayer : MonoBehaviour, ISoundPlayer
    {
        [SerializeField] private SoundsData data;
        [Space]
        [SerializeField] private GameObject soundPrefab;
        [SerializeField] private AudioSource musicSource;

        private List<string> playingSounds = new List<string>();


        private bool musicMuted
        {
            get => musicSource.isPlaying;
            set
            {
                if (value)
                {
                    musicSource.Pause();
                }
                else
                {
                    musicSource.UnPause();
                    float volume = musicSource.volume;
                    musicSource.volume = 0f;
                    DOTween.To(() => musicSource.volume, (x) => musicSource.volume = x, volume, 1f).
                        SetEase(Ease.InOutSine);
                }
            }
        }


        public void Initilize(GameInfo info)
        {
            PlayMusic(info.Music);
        }

        private void PlayMusic(AudioClip clip)
        {
            if (gameObject == null)
                return;
            if (clip == null || clip == musicSource.clip)
                return;

            musicSource.clip = clip;
            musicSource.time = 0;
            musicSource.Play();
        }

        public void PlaySound(string id, float volume = 1f, float tone = 1f, float Delay = 0)
        {
            if (data.Sounds.Exists(item => item.id == id))
            {
                SoundsData.Item soundItem = data.Sounds.Find(item => item.id == id);

                if (soundItem.maxInstance != 0 && playingSounds.FindAll((x) => x == id).Count > soundItem.maxInstance)
                    return;
                GameObject SoundObject = Instantiate(soundPrefab, transform);
                SoundObject.name = $"source: {id}";

                AudioSource soundSource = SoundObject.GetComponent<AudioSource>();
                soundSource.clip = soundItem.Clip;
                soundSource.volume = soundItem.Volume * volume;
                soundSource.pitch = soundItem.Pitch * tone;
                soundSource.spatialBlend = soundItem.SpatialBlend;
                soundSource.maxDistance = soundItem.MaxDistance;

                if (SoundObject == null)
                    return;

                soundSource.PlayDelayed(Delay);
                playingSounds.Add(id);
                DestroySoundOnEnd(soundSource, Delay, () => playingSounds.Remove(id));
            }
            else
            {
                Debug.LogWarning($"No sound with such id: {id}");
            }
        }
        public void PlayMusic(string id, float volume = 1f, float tone = 1f, float Delay = 0)
        {
            if (data.Sounds.Exists(item => item.id == id))
            {
                SoundsData.Item soundItem = data.Sounds.Find(item => item.id == id);

                if (soundItem.maxInstance != 0 && playingSounds.FindAll((x) => x == id).Count > soundItem.maxInstance)
                    return;
                GameObject SoundObject = Instantiate(soundPrefab, transform);
                SoundObject.name = $"source: {id}";

                AudioSource soundSource = SoundObject.GetComponent<AudioSource>();
                soundSource.clip = soundItem.Clip;
                soundSource.volume = soundItem.Volume * volume;
                soundSource.pitch = soundItem.Pitch * tone;
                soundSource.spatialBlend = soundItem.SpatialBlend;
                soundSource.maxDistance = soundItem.MaxDistance;

                if (SoundObject == null)
                    return;

                soundSource.PlayDelayed(Delay);
                playingSounds.Add(id);
                musicMuted = true;
                DestroySoundOnEnd(soundSource, Delay, () =>
                {
                    playingSounds.Remove(id);
                    musicMuted = false;
                });
            }
            else
            {
                Debug.LogWarning($"No sound with such id: {id}");
            }
        }

        private void DestroySoundOnEnd(AudioSource obj, float Delay, System.Action onDestroyed)
        {
            try
            {
                StartCoroutine(DestroySoundOnEndCour(obj, Delay, onDestroyed));
            }
            catch { }
        }
        private IEnumerator DestroySoundOnEndCour(AudioSource obj, float Delay, System.Action onDestroyed)
        {
            yield return new WaitForSeconds(Delay);
            while (obj != null && obj.isPlaying)
            {
                yield return new WaitForFixedUpdate();
            }
            if (obj != null)
            {
                Destroy(obj.gameObject);
            }
            onDestroyed?.Invoke();
            yield break;
        }
    }
}