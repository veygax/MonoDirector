using System;
using System.Diagnostics;
using System.Reflection;
using System.IO;

using MelonLoader;

using UnityEngine;

using Il2CppSLZ.Marrow.Warehouse;

using NEP.MonoDirector.Audio;
using NEP.MonoDirector.Cameras;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.UI;
using NEP.MonoDirector.Data;
using BoneLib;


namespace NEP.MonoDirector
{
    public static class BuildInfo
    {
        public const string Name = "MonoDirector"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "A movie/photo making utility for BONELAB!"; // Description for the Mod.  (Set as null if none)
        public const string Author = "Not Enough Photons"; // Author of the Mod.  (MUST BE SET)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.1.5"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class Main : MelonMod
    {
        internal static MelonLogger.Instance Logger;
        internal static AssetBundle _bundle;

        public override void OnInitializeMelon()
        {
            Logger = new MelonLogger.Instance("MonoDirector", System.Drawing.Color.Red);

            Directory.CreateDirectory(Constants.dirBase);
            Directory.CreateDirectory(Constants.dirMod);
            Directory.CreateDirectory(Constants.dirSFX);
            Directory.CreateDirectory(Constants.dirImg);

            _bundle = GetEmbeddedBundle();

            Hooking.OnLevelLoaded += new Action<LevelInfo>((info) => MonoDirectorInitialize());

            AssetWarehouse._onReady += new Action(() =>
            {
                AudioClip[] sounds = WarehouseLoader.GetSounds().ToArray();
                WarehouseLoader.GenerateSpawnablesFromSounds(sounds);
            });

            MDBoneMenu.Initialize();
        }

        private void MonoDirectorInitialize()
        {
            ResetInstances();
            CreateCameraManager();
            CreateDirector();
            CreateSFX();
            CreateUI();

            // Data.AvatarPhotoBuilder.Initialize();
        }

        private void ResetInstances()
        {
            Events.FlushActions();
            Director.Instance?.CleanUp();
            PropMarkerManager.CleanUp();
        }

        private void CreateCameraManager()
        {
            new CameraRigManager();
        }

        private void CreateDirector()
        {
            new Director();
        }

        private void CreateSFX()
        {
            new FeedbackSFX();
        }

        private void CreateUI()
        {
            // PropMarkerManager.Initialize();
            InfoInterfaceManager.Initialize();
            WarehouseLoader.SpawnFromBarcode(WarehouseLoader.mainMenuBarcode);
        }

        private static AssetBundle GetEmbeddedBundle()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            string fileName = "md_resources.pack";

            using (Stream resourceStream = assembly.GetManifestResourceStream("NEP.MonoDirector.Resources." + fileName))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    resourceStream.CopyTo(memoryStream);
                    return AssetBundle.LoadFromMemory(memoryStream.ToArray());
                }
            }
        }
    }
}
