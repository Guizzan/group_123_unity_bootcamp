using Guizzan.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseSoundEvent : MonoBehaviour
{
    [System.Serializable]
    public struct SurfaceSound
    {
        public string[] Sounds;
        public LayerMask Surfaces;
    }
    public abstract void SoundEvent(string type);
    public virtual void MakeSound(ref Transform soundPos, LayerMask surfaceLayer, ref List<SurfaceSound> sounds, ref Dictionary<int, List<int>> usedList, ref float minInterval, ref float lastTime, float volume)
    {
        float _timer = Time.timeSinceLevelLoad;
        int soundsIndex = sounds.IndexOf(sounds.Where(t => t.Surfaces.ContainsLayer(surfaceLayer)).First());

        if (_timer < lastTime + minInterval) return;
        lastTime = _timer;

        if (!usedList.ContainsKey(soundsIndex)) usedList.Add(soundsIndex, new List<int>());

        if (usedList[soundsIndex].Count == sounds[soundsIndex].Sounds.Length)
        {
            int OverrideIndex = usedList[soundsIndex][usedList[soundsIndex].Count - 1];
            usedList[soundsIndex].Clear();
            usedList[soundsIndex].Add(OverrideIndex);
        }
        int index = Random.Range(0, sounds[soundsIndex].Sounds.Length);
        while (usedList[soundsIndex].Contains(index))
        {
            index = Random.Range(0, sounds[soundsIndex].Sounds.Length);
        }
        usedList[soundsIndex].Add(index);

        SoundManager.PlaySound(sounds[soundsIndex].Sounds[index], soundPos, false, volume);
    }
}
