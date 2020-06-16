using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BehaviorDesigner.Runtime.Tasks.Basic.UnityGameObject;
using DG.Tweening;
using UnityEngine;
using Random = System.Random;

namespace Tang
{
    public class AudioManager
    {
        private static AudioManager s_instance;

        public static AudioManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new AudioManager();
                    LoadJson();
                }

                return s_instance;
            }
        }
        static Dictionary<string,string> AudioIDDic = new Dictionary<string, string>();
        static Dictionary<string,AudioAttackName> AudioNameDic = new Dictionary<string, AudioAttackName>();
        private static async void LoadJson()
        {
            var AudioIDTask = AssetManager.LoadAssetAsync<TextAsset>("AttackAudio");
            var AudioIDTextAsset = await AudioIDTask;
            AudioIDDic = Tools.Json2Obj<Dictionary<string, string>>(AudioIDTextAsset.text);
            
            var AudioNameTask = AssetManager.LoadAssetAsync<TextAsset>("AudioName");
            var AudioNameTextAsset = await AudioNameTask;
            AudioNameDic = Tools.Json2Obj<Dictionary<string, AudioAttackName>>(AudioNameTextAsset.text);
        }
        
        public async void PlayEffect(string key, Vector3 worldPos)
        {
            GameObject musicPlayer = await AssetManager.SpawnAsync("Music/MusicPlayer.prefab");
            musicPlayer.transform.position = worldPos;
             
            AudioSource audioSource = musicPlayer.GetComponent<AudioSource>();
            var clip = await AssetManager.LoadAssetAsync<AudioClip>("Music/" + key+ ".WAV");
            audioSource.PlayOneShot(clip);

            musicPlayer.DoDelay(clip.length).OnComplete(() =>
            {
                AssetManager.DeSpawn(musicPlayer);
            });
        }

        public void PlayEffect(HitEffectType hitEffectType,MatType matType,Vector3 worldPos)
        {
            try
            {
                string key = hitEffectType.ToString() + "_" + matType.ToString();
                string AudioID = AudioIDDic[key.ToLower()];
                AudioAttackName audioAttackName = AudioNameDic[AudioID];

                string AudioKey = "";
                int temp = UnityEngine.Random.Range(0, 3);
                switch (temp)
                {
                    case 0:
                        AudioKey = audioAttackName.AudioName1;
                        break;
                    case 1:
                        AudioKey = audioAttackName.AudioName2;
                        break;
                    case 2:
                        AudioKey = audioAttackName.AudioName3;
                        break;
                }
                PlayEffect(AudioKey,worldPos);                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                
            }
            
            

        }
    }
    public class AudioAttackName
    {
        public string AudioName1;
        public string AudioName2;
        public string AudioName3;

        public AudioAttackName(string audioName1,string audioName2,string audioName3)
        {
            AudioName1 = audioName1;
            AudioName2 = audioName2;
            AudioName3 = audioName3;
        }
    }
}