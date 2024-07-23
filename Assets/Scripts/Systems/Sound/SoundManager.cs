using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.Audio;

[System.Serializable]
public class SoundInfo
{
    public string Name;
    public AudioClip Sound;
    public AudioMixerGroup Mixer;
}

public class SoundManager : MonoBehaviour
{
    #region Setup
    public List<SoundInfo> SoundList = new();
    public GameObject Prefab2D;
    public GameObject Prefab3D;

    #region Secret
    private static SoundManager StaticInstance;
    private void Awake()
    {
        if (StaticInstance != null) Destroy(this);
        StaticInstance = this;
    }
    #endregion

    public static GameObject Prefab2DStatic { get { return StaticInstance.Prefab2D; } }
    public static GameObject Prefab3DStatic { get { return StaticInstance.Prefab3D; } }
    public static List<SoundInfo> StaticSoundList { get { return StaticInstance.SoundList; } }
    #endregion

    private static void BasePlaySound(out GameObject Instance, string name, bool loop = false, float volume = 1, bool positional = false)
    {
        SoundInfo soundInfo = GetSound(name);
        Instance = Instantiate(positional ? Prefab3DStatic : Prefab2DStatic, StaticInstance.transform);
        AudioSource instanceAudioSource = Instance.GetComponent<AudioSource>();
        instanceAudioSource.outputAudioMixerGroup = soundInfo.Mixer;
        instanceAudioSource.loop = loop;
        instanceAudioSource.clip = soundInfo.Sound;
        instanceAudioSource.volume = volume;

        instanceAudioSource.Play();

        if (!loop) Destroy(Instance, soundInfo.Sound.length);
    }

    public static void PlaySound(string name, bool loop = false, float volume = 1)
    {
        BasePlaySound(out GameObject Instance, name, loop, volume);
    }

    public static void PlaySound(string name, Transform transform, bool loop = false, float volume = 1)
    {
        if (transform == null) return;
        BasePlaySound(out GameObject Instance, name, loop, volume, true);
        Instance.transform.parent = transform;
        Instance.transform.localPosition = Vector3.zero;
    }

    public static void PlaySound(string name, Vector3 position, bool loop = false, float volume = 1)
    {
        BasePlaySound(out GameObject Instance, name, loop, volume, true);
        Instance.transform.position = position;
    }
    public static void PlaySoundOnCollision(GameObject collisionObject, string name, string tag = null, bool loop = false, float volume = 1, float destroyAfter = 10)
    {
        SoundOnCollision script = collisionObject.AddComponent<SoundOnCollision>();
        destroyAfter += GetSound(name).Sound.length;
        script.Sound = name;
        script.TagFilter = tag;
        script.DestroyAfter = destroyAfter;
    }

    #region Really private functions
    private static SoundInfo GetSound(string name)
    {
        SoundInfo info = StaticSoundList.Where(t => t.Name == name).First();
        return info;
    }
    #endregion

}
