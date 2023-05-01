using System;
using UnityEngine;
using BepInEx;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Utilla;

namespace MonkeSoundboard
{
    [BepInPlugin("net.zenon.monkesoundboard", "Monke Soundboard", "1.0.0")]
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.6.7")]
    public class Class1 : BaseUnityPlugin
    {
        List<AudioClip> soundList;
        public string filePath;
        bool inRoom;
        public void Awake() 
        {
            var harmony = new Harmony("net.zenon.monkesoundboard");
            harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());

        }

        public void Start() 
        {
            var MusicFolder = Directory.CreateDirectory(BepInEx.Paths.BepInExRootPath + "/MonkeSoundboard");
            filePath = BepInEx.Paths.BepInExRootPath + "/MonkeSoundboard/";
           
            if (soundList == null)
            {
                soundList = new List<AudioClip>();
            }
            soundList.Clear();
            DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
            FileInfo[] oggFiles = directoryInfo.GetFiles("*.ogg");
            foreach (FileInfo file in oggFiles)
            {
                StartCoroutine(LoadSound(file.Name));

            }
            FileInfo[] wavFiles = directoryInfo.GetFiles("*.wav");
            foreach (FileInfo file in wavFiles)
            {
                StartCoroutine(LoadSound(file.Name));

            }
        }

        public void OnGUI() 
        {
            if (inRoom) 
            {
                foreach (AudioClip clip in soundList)
                {
                    bool cool = GUILayout.Button(clip.name, Array.Empty<GUILayoutOption>());
                    if (cool)
                    {
                        GorillaNetworking.PhotonNetworkController.Instance.GetComponent<Photon.Voice.Unity.Recorder>().AudioClip = clip;
                        GorillaNetworking.PhotonNetworkController.Instance.GetComponent<Photon.Voice.Unity.Recorder>().SourceType = Photon.Voice.Unity.Recorder.InputSourceType.AudioClip;
                        GorillaNetworking.PhotonNetworkController.Instance.GetComponent<Photon.Voice.Unity.Recorder>().LoopAudioClip = false;
                        GorillaNetworking.PhotonNetworkController.Instance.GetComponent<Photon.Voice.Unity.Recorder>().RestartRecording(true);
                    }
                }

                if (GUILayout.Button("Stop/Fix Mic", Array.Empty<GUILayoutOption>()))
                {
                    GorillaNetworking.PhotonNetworkController.Instance.GetComponent<Photon.Voice.Unity.Recorder>().AudioClip = null;
                    GorillaNetworking.PhotonNetworkController.Instance.GetComponent<Photon.Voice.Unity.Recorder>().SourceType = Photon.Voice.Unity.Recorder.InputSourceType.Microphone;
                    GorillaNetworking.PhotonNetworkController.Instance.GetComponent<Photon.Voice.Unity.Recorder>().RestartRecording(true);
                }
            }
        }

        IEnumerator LoadSound(string fileName)
        {
            WWW request = GetAudioFromFile(filePath, fileName);
            yield return request;
            AudioClip sound = request.GetAudioClip();
            sound.name = fileName;
            soundList.Add(sound);
        }

        WWW GetAudioFromFile(string path, string fileName)
        {
            string loadthisthing = path + fileName;
            WWW request = new WWW(loadthisthing);
            return request;
        }

        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {

            inRoom = true;
        }
        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            inRoom = false;
        }
    }
}
