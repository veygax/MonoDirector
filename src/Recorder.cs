﻿using System.Collections;
using MelonLoader;
using NEP.MonoDirector.Actors;
using NEP.MonoDirector.State;

using UnityEngine;

namespace NEP.MonoDirector.Core
{
    public class Recorder
    {
        public Recorder()
        {
            instance = this;

            Events.OnPreRecord += OnPreRecord;
            Events.OnRecordTick += OnRecordTick;
            Events.OnStopRecording += OnStopRecording;
        }

        public static Recorder instance;

        public float RecordingTime { get => recordingTime; }

        public int RecordTick { get => recordTick; }

        public ActorPlayer ActiveActor { get => activeActor; }

        private ActorPlayer activeActor;

        private Coroutine recordRoutine;

        private float recordingTime;

        private int recordTick;

        public void LateUpdate()
        {
            if (Director.PlayState != PlayState.Recording)
            {
                return;
            }

            Events.OnRecordTick?.Invoke();
        }

        public void BeginRecording()
        {
            if (recordRoutine == null)
            {
                recordRoutine = MelonCoroutines.Start(RecordRoutine()) as Coroutine;
            }
        }

        public void RecordCamera()
        {
            foreach (var castMember in Director.instance.Cast)
            {
                castMember?.Act();
            }
        }

        public void RecordActor()
        {
            activeActor.RecordFrame();

            foreach (var prop in Director.instance.RecordingProps)
            {
                prop.Record(recordTick);
            }

            foreach (var castMember in Director.instance.Cast)
            {
                Playback.instance.AnimateActor(castMember);
            }

            foreach(var prop in Director.instance.WorldProps)
            {
                Playback.instance.AnimateProp(prop);
            }
        }

        public void OnPreRecord()
        {
            if (recordTick > 0)
            {
                recordTick = 0;
            }

            recordingTime = 0f;

            activeActor = new ActorPlayer(Constants.rigManager.avatar);

            foreach (var castMember in Director.instance.Cast)
            {
                if (castMember != null)
                {
                    castMember.Act();
                }
            }
        }

        public void OnRecordTick()
        {
            if (Director.PlayState == PlayState.Paused)
            {
                return;
            }

            recordTick++;

            recordingTime += Time.deltaTime;

            if (Director.CaptureState == CaptureState.CaptureCamera)
            {
                RecordCamera();
            }

            if (Director.CaptureState == CaptureState.CaptureActor)
            {
                RecordActor();
            }

            foreach (var castMember in Director.instance.Cast)
            {
                if (castMember != null)
                {
                    castMember.Act();
                }
            }
        }

        public void OnStopRecording()
        {
            activeActor.CloneAvatar();
            Director.instance.Cast.Add(activeActor);

            activeActor = null;

            Director.instance.WorldProps.AddRange(Director.instance.RecordingProps);
            Director.instance.RecordingProps.Clear();

            if (recordRoutine != null)
            {
                MelonCoroutines.Stop(recordRoutine);
                recordRoutine = null;
            }
        }

        public IEnumerator RecordRoutine()
        {
            Events.OnPreRecord?.Invoke();
            yield return new WaitForSeconds(5f);
            Events.OnStartRecording?.Invoke();

            while (Director.PlayState == PlayState.Recording || Director.PlayState == PlayState.Paused)
            {
                LateUpdate();
                yield return null;
            }

            Events.OnStopRecording?.Invoke();
            yield return null;
        }
    }
}