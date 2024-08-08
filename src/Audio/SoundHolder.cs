using System.Collections.Generic;

using UnityEngine;

using NEP.MonoDirector.Data;
using Il2CppSLZ.Marrow.Pool;

namespace NEP.MonoDirector.Audio
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class SoundHolder : MonoBehaviour
    {
        public SoundHolder(System.IntPtr ptr) : base(ptr) { }

        public static Dictionary<string, AudioClip> LoadedClips;

        private Poolee poolee;

        private AudioClip sound;

        private void Start()
        {
            poolee = GetComponent<Poolee>();
            AssignSound(WarehouseLoader.soundTable[poolee.SpawnableCrate.Description]);
        }

        private void OnDisable()
        {
            poolee.Despawn();
        }

        public void AssignSound(AudioClip sound)
        {
            this.sound = sound;
        }

        public AudioClip GetSound()
        {
            return sound;
        }
    }
}