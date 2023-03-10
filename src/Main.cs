﻿using System.Reflection;
using System.IO;

using MelonLoader;
using UnityEngine;

using BoneLib;
using BoneLib.BoneMenu;

using NEP.MonoDirector.Audio;
using NEP.MonoDirector.Cameras;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.State;
using NEP.MonoDirector.UI;
using NEP.MonoDirector.UI.Interface;
using UnityEngine.Splines;
using Unity.Mathematics;

namespace NEP.MonoDirector
{
    public static class BuildInfo
    {
        public const string Name = "MonoDirector"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "Camera dolly system for Boneworks. Take cool shots and whatnot!"; // Description for the Mod.  (Set as null if none)
        public const string Author = "Not Enough Photons"; // Author of the Mod.  (MUST BE SET)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "0.0.1"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class Main : MelonMod
    {
        internal static MelonLogger.Instance Logger;

        public static Main instance;

        public static Director director;

        public static FreeCameraRig camera;

        public static FeedbackSFX feedbackSFX;

        public static AssetBundle bundle;

        public override void OnInitializeMelon()
        {
            Logger = new MelonLogger.Instance("MonoDirector", System.ConsoleColor.Magenta);

            instance = this;

            bundle = GetEmbeddedBundle();

            BoneLib.Hooking.OnLevelInitialized += (info) => MonoDirectorInitialize();

            BuildMenu();
        }

        private void MonoDirectorInitialize()
        {
            ResetInstances();
            CreateDirector();
            CreateCamera();
            CreateUI();
            CreateSFX();
        }

        private void ResetInstances()
        {
            Events.FlushActions();
            director = null;
            feedbackSFX = null;
        }

        private void CreateCamera()
        {
            SLZ.Rig.RigManager rigManager = BoneLib.Player.rigManager;
            GameObject gameObject = rigManager.transform.Find("Spectator Camera").gameObject;
            camera = gameObject.AddComponent<FreeCameraRig>();
        }

        private void CreateDirector()
        {
            GameObject directorObject = new GameObject("Director");
            director = directorObject.AddComponent<Director>();
            director.SetCamera(camera);
        }

        private void CreateSFX()
        {
            GameObject feedback = new GameObject("Feedback SFX");
            feedbackSFX = feedback.AddComponent<FeedbackSFX>();
        }

        private void CreateUI()
        {
            //UIManager.Construct();
            var test = GameObject.Instantiate(bundle.LoadAsset("md_main_menu")).Cast<GameObject>();
            test.AddComponent<RootPanel>();
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

        private void BuildMenu()
        {
            var root = MenuManager.CreateCategory("Not Enough Photons", Color.white);
            var mdMenu = root.CreateCategory("MonoDirector", "#db35b2");
            var playbackCategory = mdMenu.CreateCategory("Playback", Color.white);
            var actorCategory = mdMenu.CreateCategory("Actors", Color.white);
            var settingsCategory = mdMenu.CreateCategory("Settings", Color.white);

            //playbackCategory.CreateEnumElement<CaptureState>("Capture Type", Color.white, (type) => director.captureState = type);
            playbackCategory.CreateFunctionElement("Record", Color.red, () => director.Record());
            playbackCategory.CreateFunctionElement("Play", Color.green, () => director.Play());
            playbackCategory.CreateFunctionElement("Pause", Color.yellow, () => director.Pause());
            playbackCategory.CreateFunctionElement("Stop", Color.red, () => director.Stop());

            actorCategory.CreateFunctionElement("Remove All Actors", Color.red, () => director.RemoveAllActors(), "Are you sure? This cannot be undone.");
            actorCategory.CreateFunctionElement("Clear Scene", Color.red, () => director.ClearScene(), "Are you sure? This cannot be undone.");

            settingsCategory.CreateBoolElement("Spawn Gun Sets Props", Color.white, false, (value) => Settings.World.spawnGunProps = value);
            settingsCategory.CreateBoolElement("Spawn Gun Sets NPCs", Color.white, false, (value) => Settings.World.spawnGunNPCs = value);
            settingsCategory.CreateFunctionElement("Spectator Head Mode", Color.white, () => Director.instance.Camera.TrackHeadCamera());

            var debug = settingsCategory.CreateCategory("Debug", Color.green);

            debug.CreateBoolElement("Debug Mode", Color.white, false, (value) => Settings.Debug.debugEnabled = value);
            debug.CreateBoolElement("Use Debug Keys", Color.white, false, (value) => Settings.Debug.useKeys = value);
        }
    }
}