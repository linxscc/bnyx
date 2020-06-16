using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;
using FairyGUI;
using Tang.Anim;
using Random = UnityEngine.Random;

namespace Tang
{
    using FrameEvent;

    [System.Serializable]
    public class AnimEffectData
    {
        public enum AnimEffectType
        {
            Group = 1,
            Image = 2,
            Anim = 3,
            Random = 4,
            Gameobject = 5,
            Particle = 6,
        }
        public enum GameobjectPathType
        {
            Path = 1,
            name = 2
        }

       
        [System.Serializable]
        public class AnimEffectRef
        {
            public string id;
            public int weight = 1;
            public float delayTime = 0;
        }

        public string id;

        public AnimEffectType animEffectType;
        public GameobjectPathType gameobjectPathType;

        // 图片 add by TangJian 2018/9/5 19:14
        public string imagePath;

        // 动画 add by TangJian 2018/9/5 19:14
        public string animPath;
        public string animName;
        public float animSpeed = 1f;
        public bool animLoop = false;

        //ganeobject add by tianjinpeng 2018/09/20 15:54:03
        public string prefabname;
        public string prefabpath;

        // 集合 add by TangJian 2018/9/8 22:16
        public List<AnimEffectRef> animEffectList = new List<AnimEffectRef>();


        public List<AnimEffectData> animEffectDataList = new List<AnimEffectData>();

        // 通用 add by TangJian 2018/9/5 19:15
        public Vector2 anchor = new Vector2(0.5f, 0.5f);


        // 初始值 add by TangJian 2018/9/11 11:01
        public Vector3 pos = Vector3.zero;
        public Vector3 rotation = Vector3.zero;
        public Vector3 orientation = new Vector3(1, 0, 0);
        public Vector3 scale = new Vector3(1, 1, 1);
        public Vector4 mulColor = Color.white;
        public Vector4 addColor = Color.black;
        public float alpha = 1;
        public bool renderFlip = false;
        public bool moveFlip = false;
        public FrameEventInfo.ParentType parentType = FrameEventInfo.ParentType.Parent;

        // AnimAction
        public Anim.ActionData actionData = new Anim.ActionData();

        #region //Zwrite类型 2019.3.28

        public FrameEventInfo.ZwriteType zwriteType = FrameEventInfo.ZwriteType.Off;
        public FrameEventInfo.RenderQueue renderQueue = FrameEventInfo.RenderQueue.Transparent;

        #endregion

    }



    [System.Serializable]
    public class AnimManager : MyMonoBehaviour
    {
        private static AnimManager instance;
        public Vector3 bubblepos;
        public float TypingEffecttime;
        public float waitTypingEffecttime;
        TypingEffect textTE;
        public static AnimManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = MainManager.GetInstance().GetManager<AnimManager>();
                }
                return instance;
            }
        }

        [SerializeField] private List<WeaponMixData> weaponMixDataList = new List<WeaponMixData>();
        public List<GameObject> animGameObjectList = new List<GameObject>();
        public Dictionary<string, GameObject> playingAnimDict = new Dictionary<string, GameObject>();
        public List<WeaponMixData> WeaponMixDataList { get { return weaponMixDataList; } }
        GTweener fadetw;
        GTweener roationtw;
        GTweener fodetw;
        GTweener movetw;

        string animEffectSaveDataFileName
        {
            get
            {
                return Application.dataPath + "/Resources_moved/Configs/AnimEffectSaveData";
                // return Tools.ResourcesPathToAbsolutePath("Configs/AnimEffectSaveData.json");
            }
        }
        private string animEffectSaveDataPath = "Manager/AnimEffectSaveDataAsset.asset";
        Dictionary<string, AnimEffectData> animEffectDataDic = new Dictionary<string, AnimEffectData>();


        private void OnEnable()
        {

        }

        private void OnDisable()
        {

        }

        void Start()
        {
            //private Transform main=Camera.main.GetComponent<Transform>();
            //private Camera all=mainca.GetChild("All").GetComponent<Camera>();
            //private Camera background=mainca.GetChild("BackGround").GetComponent<Camera>();

            InitAnimEffect();
        }

        public async void InitAnimEffect()
        {
            animEffectDataDic.Clear();
            var animEffectSaveDataAsset = await AssetManager.LoadAssetAsync<AnimEffectSaveDataAsset>(animEffectSaveDataPath);
            if (animEffectSaveDataAsset != null)
            {
                if (animEffectSaveDataAsset != null && animEffectSaveDataAsset.animEffectDataList != null)
                {
                    foreach (var item in animEffectSaveDataAsset.animEffectDataList)
                    {
                        if (animEffectDataDic.ContainsKey(item.id))
                            animEffectDataDic.Remove(item.id);
                        animEffectDataDic.Add(item.id.ToLower(), item);
                    }
                }
            }
            else
            {
//                var animEffectSaveData = await AssetManager.LoadJson<AnimEffectSaveData>(animEffectSaveDataFileName);

                var animEffectSaveData = Tools.Load<AnimEffectSaveData>(animEffectSaveDataFileName);
                if (animEffectSaveData != null && animEffectSaveData.animEffectDataList != null)
                {
                    foreach (var item in animEffectSaveData.animEffectDataList)
                    {
                        animEffectDataDic.Add(item.id.ToLower(), item);
                    }
                }
            }
        }

        public void PlayAnimEffect(string id, Vector3 position, float rendererAngle = 0, bool flip = false, Vector3 moveOrientation = default(Vector3), Transform transform_ = null, float delayTime = 0, Action<AnimEffectController> callback = null)
        {
            AnimEffectData animEffectData;
            if (animEffectDataDic.TryGetValue(id.ToLower(), out animEffectData))
            {
                PlayAnimEffect(Tools.DepthClone(animEffectData), position, rendererAngle, flip, moveOrientation, transform_, delayTime, callback);
            }
        }

        public async void PlayAnimEffect(AnimEffectData animEffectData, Vector3 position, float rendererAngle = 0, bool flip = false, Vector3 moveOrientation = default(Vector3), Transform transform_ = null, float delayTime = 0, Action<AnimEffectController> callback = null)
        {
            if (animEffectData.animEffectType == AnimEffectData.AnimEffectType.Group)
            {
                foreach (var item in animEffectData.animEffectList)
                {
                    AnimEffectData aed = animEffectData.animEffectDataList.Find((AnimEffectData aed_) =>
                    {
                        return item.id.ToLower() == aed_.id.ToLower();
                    });

                    if (aed != null)
                    {
                        PlayAnimEffect(aed, position, rendererAngle, flip, moveOrientation, transform_, item.delayTime, callback);
                    }
                    else
                    {
                        PlayAnimEffect(item.id, position, rendererAngle, flip, moveOrientation, transform_, item.delayTime, callback);
                    }
                }
            }
            else if (animEffectData.animEffectType == AnimEffectData.AnimEffectType.Random)
            {
                int index = Tools.RandomWithWeight(animEffectData.animEffectList, (AnimEffectData.AnimEffectRef ar, int i) =>
                {
                    return ar.weight;
                });
                if (index >= 0)
                {
                    AnimEffectData aed = animEffectData.animEffectDataList.Find((AnimEffectData aed_) =>
                    {
                        return animEffectData.animEffectList[index].id.ToLower() == aed_.id.ToLower();
                    });
                    if (aed != null)
                    {
                        PlayAnimEffect(aed, position, rendererAngle, flip, moveOrientation, transform_, animEffectData.animEffectList[index].delayTime, callback);
                    }
                    else
                    {
                        PlayAnimEffect(animEffectData.animEffectList[index].id, position, rendererAngle, flip, moveOrientation, transform_, animEffectData.animEffectList[index].delayTime, callback);
                    }   
                }
                else
                {
                    Debug.Log("找不到随机动画: " + animEffectData.id);
                }
            }
            else
            {
                this.DelayToDo(delayTime, async () => { 
                    GameObject animEffectObject = await AssetManager.SpawnAsync("Effect/AnimEffect.prefab");

                    if (transform_ != null)
                    {
                        switch (animEffectData.parentType)
                        {
                            case FrameEventInfo.ParentType.Parent:
                                animEffectObject.transform.parent = transform_.parent;
                                break;
                            case FrameEventInfo.ParentType.Transform:
                                animEffectObject.transform.parent = transform_;
                                break;
                            default:
                                animEffectObject.transform.parent = transform_.parent;
                                break;
                        }
                    }

                    var animEffectController = animEffectObject.GetComponent<AnimEffectController>();

                    animEffectController.animEffectData = animEffectData;

                    animEffectController.animEffectData.rotation.z += rendererAngle;
                        
                    animEffectController.MoveFlip = flip;

                    animEffectController.Orientation = moveOrientation;

                    animEffectObject.transform.position = position;

                    animEffectController.PlayAnim(animEffectData);
                        
                    // 回调 add by TangJian 2019/5/16 15:51
                    callback?.Invoke(animEffectController);
                });
            }
        }

        private static bool isSpawn;
        public GameObject wordSpaceRightGameObject;
        public GameObject WordSpaceRightGameObject
        {
            get
            {
                if (wordSpaceRightGameObject == null)
                {
                    wordSpaceRightGameObject = GameObject.Find("WordSpaceRightGameObject");
                    if (wordSpaceRightGameObject == null)
                    {
                        wordSpaceRightGameObject = new GameObject();
                        wordSpaceRightGameObject.name = "WordSpaceRightGameObject";
                    }
                }
                return wordSpaceRightGameObject;
            }
        }

        public GameObject wordSpaceLeftGameObject;
        public GameObject WordSpaceLeftGameObject
        {
            get
            {
                if (wordSpaceLeftGameObject == null)
                {
                    wordSpaceLeftGameObject = GameObject.Find("WordSpaceLeftGameObject");
                    if (wordSpaceLeftGameObject == null)
                    {
                        wordSpaceLeftGameObject = new GameObject();
                        wordSpaceLeftGameObject.name = "WordSpaceLeftGameObject";

                        wordSpaceLeftGameObject.transform.localScale = new Vector3(-1, 1, 1);
                    }
                }
                return wordSpaceLeftGameObject;
            }
        }

        public void AddAnim(string animId, GameObject gameObject)
        {
            removeAnim(animId);
            playingAnimDict.Add(animId, gameObject);
        }

        public void removeAnim(string animId)
        {
            GameObject animGameObject;
            if (playingAnimDict.TryGetValue(animId, out animGameObject))
            {
                GameObjectManager.Instance.Despawn(animGameObject);
                playingAnimDict.Remove(animId);
            }
        }

        public GameObject getAnim(string animId)
        {
            GameObject animGameObject;
            if (playingAnimDict.TryGetValue(animId, out animGameObject))
            {
                return animGameObject;
            }
            return null;
        }

        public async Task<GameObject> PlayAnim(string animId, string animPath, string animName, bool loop, Vector3 position, float scale = 1f)
        {
            GameObject InstantiateSpecialEffect = new GameObject();
            InstantiateSpecialEffect.SetActive(false);

            InstantiateSpecialEffect.transform.position = position;
            InstantiateSpecialEffect.AddComponentUnique<SortRenderer>();
            SkeletonAnimation skeletonAnimation = InstantiateSpecialEffect.AddComponentUnique<SkeletonAnimation>();
                
            skeletonAnimation.skeletonDataAsset =
                await AssetManager.LoadAssetAsync<SkeletonDataAsset>(animPath);   
            
            skeletonAnimation.Initialize(false);
            
            skeletonAnimation.state.SetAnimation(0, animName, loop);

            InstantiateSpecialEffect.transform.localScale = new Vector3(scale, scale, scale);

            InstantiateSpecialEffect.SetActive(true);
            return InstantiateSpecialEffect;
        }

        public GameObject PlayAnim(string animId, string animName, bool loop, Vector3 position, float rotation = 0f, float scale = 1f, float delayDestroy = 1.0f, Transform parent = null)
        {
            var animGameObject = getAnim(animId);
            if (animGameObject == null)
            {
                animGameObject = GameObjectManager.Instance.Spawn("EffectAnim", animGameObjectList[0]);

                AddAnim(animId, animGameObject);
            }

            var skeletonAnimation = animGameObject.GetComponent<SkeletonAnimation>();
            skeletonAnimation.skeleton.SetToSetupPose();
            skeletonAnimation.state.SetAnimation(0, animName, loop);

            if (parent != null)
            {
                animGameObject.transform.parent = parent;
            }

            animGameObject.transform.position = position;
            animGameObject.transform.eulerAngles = new Vector3(0, 0, rotation);
            animGameObject.transform.localScale = new Vector3(scale, scale, scale);

            // 延时删除 add by TangJian 2017/10/10 15:11:36
            DelayFunc(animId + "delayDestroy", () =>
            {
                removeAnim(animId);
            }, delayDestroy);

            return animGameObject;
        }

        public void PlayEffect(GameObject parentObject, string name, Direction direction, Vector3 position)
        {
            if (name == "dust")
            {
                PlayParticle(parentObject.transform.position);
            }
            else if (name == "dust1")
            {
                GameObject gameObject = GameObjectManager.Instance.Create("HitParticalSystemYan");
                gameObject.transform.position = parentObject.transform.position + position;
                gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x * direction.GetInt(), gameObject.transform.localScale.y, gameObject.transform.localScale.z);

                Destroy(gameObject, 2);
            }
        }

        public void PlayParticle(Vector3 pos)
        {
            GameObject dust = GameObjectManager.Instance.Create("Dust");
            dust.transform.localScale = new Vector3(1, 1, 1) * Random.Range(2f, 3f);
            dust.transform.position = pos + new Vector3(0, dust.GetRendererBounds().size.y / 2, -0.6f);

            dust.DOFade(0.8f, 0).OnComplete(() =>
            {
                dust.DOFade(0, 2f).OnComplete(() =>
                {
                    Tools.Destroy(dust);
                });
            });
        }

        public void PrintBubble(string text, GameObject parent)
        {    //对话气泡
            GameObject Bubble = GameObjectManager.Instance.Create("bubbleandtext");
            Bubble.transform.parent = parent.transform;
            Bubble.transform.position = new Vector3(parent.transform.position.x - 3.46f + bubblepos.x, parent.transform.position.y + 6.84f + bubblepos.y, parent.transform.position.z + bubblepos.y);
            var ui = Bubble.GetComponent<UIPanel>().ui;
            GImage Bubbleimg = ui.GetChild("n0").asImage;
            GTextField textui = ui.GetChild("bubbletext").asTextField;
            textui.text = text;
            textTE = new TypingEffect(textui);
            textTE.Start();
            //Timers.inst.StartCoroutine(textTE.Print(0.500f));
            //textui.text = text;
            int dfd = text.Length;
            //char[] gfdg=text.ToCharArray();

            Timers.inst.StartCoroutine(textTE.Print(TypingEffecttime));
            DelayFunc("waitTypingEffect", () =>
            {
                ui.DOFade(0, 1f).OnComplete(() =>
                {
                    Tools.Destroy(Bubble);
                });
            }, waitTypingEffecttime);
        }
        GTweener movesizepar;
        GTweener dosettw;
        public void Movesize(GObject parent, GObject gameObject, Vector2 sd, Vector2 pos, float time)
        {
            parent.position = sd;
            parent.visible = true;
            GTween.Kill(dosettw);
            GTween.Kill(movesizepar);
            dosettw = parent.TweenResize(gameObject.size, time);
            movesizepar = parent.TweenMove(pos, time).OnComplete(() =>
            {
                gameObject.visible = true;
                parent.visible = false;
            });
        }

        public void PopText(string text, Vector3 pos, GameObject parent, bool baoji)   //伤害数值实例化和动画
        {
            GameObject textobj;
            //GameObject textobj = trans.GetChild("shanhaishuzhi");
            if (baoji == true)
            {
                textobj = GameObjectManager.Instance.Create("text2");
            }
            else
            {
                textobj = GameObjectManager.Instance.Create("text1");
            }

            textobj.transform.parent = parent.transform;
            textobj.transform.position = new Vector3(pos.x, pos.y, pos.z);
            var ui = textobj.GetComponent<UIPanel>().ui;

            GTextField textui = ui.GetChild("text").asTextField;
            //textui.scale = new Vector2(1, 1);
            textui.text = text;

            //var StageCameraObject = GameObject.Find("Stage Camera");
            //var camera = StageCameraObject.GetComponent<Camera>();




            //textobj.transform.position = Vector3.zero;

            // var p = RectTransformUtility.WorldToScreenPoint(Camera.main, new Vector3(pos.x, pos.y, pos.z));
            //ui.position = pos * textobj.transform.localScale.x;

            CustomMonoBehaviour customMonoBehaviour = textobj.AddComponentUnique<CustomMonoBehaviour>();

            var spos = parent.transform.InverseTransformPoint(new Vector3(pos.x, pos.y + 2, pos.z));

            textui.DOFade(1, 0.3f).OnComplete(() =>
            {
                textobj.transform.DOLocalMove(spos, 0.5f).OnComplete(() =>
                {
                    textui.DOFade(0, 1f).OnComplete(() =>
                    {
                        Tools.Destroy(textobj);
                        // GameObjectManager.Instance.Despawn(textobj);
                    });
                });

            });

            //GameObject canvas = GameObject.Find("Canvas");
            //textobj.transform.parent = canvas.transform;
            // textobj.transform.position=RectTransformUtility.WorldToScreenPoint(Camera.main,new Vector3(pos.x,pos.y,pos.z));
            // CustomMonoBehaviour customMonoBehaviour = textobj.AddComponentUnique<CustomMonoBehaviour>();
            // customMonoBehaviour.OnActionSpawned = () =>
            // {textobj.DOFade(1.0f, 0);};
            // textobj.DOFade(0, 0.5f).OnComplete(() =>{
            //     GameObjectManager.Instance.Despawn(textobj);
            // });
            // GameObject textObject = GameObjectManager.Instance.Spawn("Text");
            // Text textComponent = textObject.GetComponent<Text>();
            // textComponent.text = text;

            // GameObject canvas = GameObject.Find("Canvas");
            // textComponent.transform.parent = canvas.transform;

            // textComponent.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, new Vector3(pos.x, pos.y, pos.z));

            // CustomMonoBehaviour customMonoBehaviour = textObject.AddComponentUnique<CustomMonoBehaviour>();
            // customMonoBehaviour.OnActionSpawned = () =>
            // {
            //     textObject.DOFade(1.0f, 0);
            // };

            // textObject.DOFade(0, 0.5f).OnComplete(() =>
            // {
            //     GameObjectManager.Instance.Despawn(textObject);
            // });
        }
        public void WeaponCremaShake(FrameEventInfo.CameraShakeType cameraShakeType = FrameEventInfo.CameraShakeType.None)
        {
            ConstrainedCamera constrainedCamera = Camera.main.GetComponent<ConstrainedCamera>();

            switch (cameraShakeType)
            {
                case FrameEventInfo.CameraShakeType.CameraShake:
                    constrainedCamera.Shake(1f, 1f, 50, 90f, false, true);
                    break;
                case FrameEventInfo.CameraShakeType.LswdCameraShake:
                    constrainedCamera.Shake(0.3f, 0.4f, 50, 90f, false, true);
                    break;
                case FrameEventInfo.CameraShakeType.LswdCameraShakefierce:
                    constrainedCamera.Shake(0.5f, 0.7f, 50, 90f, false, true);
                    break;
                case FrameEventInfo.CameraShakeType.SwdCameraShake:
                    constrainedCamera.Shake(0.3f, 0.2f, 50, 90f, false, true);
                    break;
                case FrameEventInfo.CameraShakeType.SwdCameraShakefierce:
                    constrainedCamera.Shake(0.5f, 0.5f, 50, 90f, false, true);
                    break;
                case FrameEventInfo.CameraShakeType.None:
                    // constrainedCamera.Shake(0.3f, 0.2f, 50, 90f, false, true);
                    break;
                case FrameEventInfo.CameraShakeType.smallshape:
                    constrainedCamera.Shake(0.2f, 0.5f, 100, 90f, false, true);
                    break;
                case FrameEventInfo.CameraShakeType.mediumshape:
                    constrainedCamera.Shake(0.2f, 1f, 100, 90f, false, true);
                    break;
                case FrameEventInfo.CameraShakeType.bigshape:
                    constrainedCamera.Shake(0.2f, 1.5f, 100, 90f, false, true);
                    break;
                default:
                    break;
            }


        }
        public void Shake(float duration, float strength = 1, int vibrato = 10, float randomness = 90, bool snapping = false, bool fadeOut = true)
        {
            ConstrainedCamera constrainedCamera = Camera.main.GetComponent<ConstrainedCamera>();
            constrainedCamera.Shake(duration, strength, vibrato, randomness, snapping, fadeOut);
        }
        public void ShakeY(float duration, float strength = 1, int vibrato = 10, float randomness = 90, bool snapping = false, bool fadeOut = true)
        {
            ConstrainedCamera constrainedCamera = Camera.main.GetComponent<ConstrainedCamera>();
            constrainedCamera.ShakeY(duration, strength, vibrato, randomness, snapping, fadeOut);
        }
        public void ShakeX(float duration, float strength = 1, int vibrato = 10, float randomness = 90, bool snapping = false, bool fadeOut = true)
        {
            ConstrainedCamera constrainedCamera = Camera.main.GetComponent<ConstrainedCamera>();
            constrainedCamera.ShakeX(duration, strength, vibrato, randomness, snapping, fadeOut);
        }
        public void ShakeV3(float duration, Vector3 strength = new Vector3(), int vibrato = 10, float randomness = 90, bool snapping = false, bool fadeOut = true)
        {
            ConstrainedCamera constrainedCamera = Camera.main.GetComponent<ConstrainedCamera>();
            constrainedCamera.ShakeV3(duration, strength, vibrato, randomness, snapping, fadeOut);
        }
        //public void DoPath(Transform tag,Vector2 weizhi){
        //tag.DOPath(weizhi);}

        public void CameraScaleUp(float Scale)
        {
            Camera.main.GetComponent<ConstrainedCamera>().ScaleUp(Scale);
        }
        public void CameraScaleDown(float Scale)
        {
            Camera.main.GetComponent<ConstrainedCamera>().ScaleDown(Scale);
        }
        public void CameraScaleDelut()
        {
            Camera.main.GetComponent<ConstrainedCamera>().SetSizeToDefault();
        }
        public void camerabaoji()
        {
            Camera.main.GetComponent<ConstrainedCamera>().ScaleUp(0.3f);
            DelayFunc("waitcamerascaleup", () =>
            {
                Camera.main.GetComponent<ConstrainedCamera>().ScaleDown(0.3f);
            }, 0.2f);
        }

        public void ScaleSize(float endV, float erwr, float gewa)
        {
            //Camera maincamera = Camera.main.GetComponent<Camera>();
            //Transform main=Camera.main.GetComponent<Transform>();
            Camera.main.GetComponent<ConstrainedCamera>().Scalesize(endV, erwr, gewa);
            //Camera all=mainca.GetChild("All").GetComponent<Camera>();
            //Camera background=mainca.GetChild("BackGround").GetComponent<Camera>();

            //maincamera.Doscalesize(endValue,duration);
            //all.Doscalesize(6f,endV).OnComplete(()=>
            //{
            //all.Doscalesize(9f,erwr);
            // }
            //);
            // background.Doscalesize(6f,endV).OnComplete(()=>
            //{
            //    background.Doscalesize(9f,erwr);
            //}
            //);
        }
        public void bezierInterrupt(GameObject gameobject, string text, Vector3 P0, float time, float force, bool baoji)
        {
            // 贝塞尔曲线中断版伤害数值动画
            GameObject textobj;
            Vector3 P1;
            Vector3 P2;
            float longlong = 3.4f;
            float height = 1.5f;
            textobj = GameObjectManager.Instance.Create("text3");
            // textobj.transform.parent = gameobject.transform;
            textobj.transform.position = new Vector3(P0.x, P0.y, P0.z);
            var ui = textobj.GetComponent<UIPanel>().ui;
            GTextField textui = ui.GetChild("n0").asTextField;
            textui.text = text;
            TextFormat tf = textui.textFormat;
            // tf.color= Color.red;
            // textui.rotationX=30;
            if (baoji)
            {
                tf.color = Color.red;

                textui.SetScale(1.2f, 1.2f);
                if (force > 0)
                {
                    P2 = new Vector3(P0.x + longlong, P0.y, P0.z);
                    P1 = new Vector3(P0.x + longlong / 2, P0.y + height, P0.z);
                }
                else
                {
                    P2 = new Vector3(P0.x - longlong, P0.y, P0.z);
                    P1 = new Vector3(P0.x - longlong / 2, P0.y + height, P0.z);
                }
                textui.TweenScale(new Vector2(0.8f, 0.8f), time);
                textui.TweenFade(0.1f, time);
                textobj.Dobezier(P0, P1, P2, 0.7f, time).OnComplete(() =>
                {
                    Tools.Destroy(textobj);
                });
            }
            else
            {
                textui.color = Color.white;
                textui.SetScale(0.2f, 0.2f);
                Debug.Log(textui.scale);
                // textui.DOColor(Color.white,time/3);
                // delayFunc(()=>
                // {
                //     textui.color= Color.white;
                // },time/3);
                if (force > 0)
                {
                    P2 = new Vector3(P0.x + longlong, P0.y, P0.z);
                    P1 = new Vector3(P0.x + longlong / 2, P0.y + height, P0.z);
                }
                else
                {
                    P2 = new Vector3(P0.x - longlong, P0.y, P0.z);
                    P1 = new Vector3(P0.x - longlong / 2, P0.y + height, P0.z);
                }
                textui.TweenScale(new Vector2(1.3f, 1.3f), time / 20).OnComplete(() =>
                {
                    textui.color = Color.red;
                    DelayFunc(() =>
                    {
                        textui.DOColor(Color.white, time / 20);
                        textui.TweenScale(new Vector2(1f, 1f), time / 20).OnComplete(() =>
                        {
                            textui.TweenScale(new Vector2(0.5f, 0.5f), (time / 10) * 8.5f);
                            textui.TweenFade(0.1f, (time / 10) * 8.5f);
                            float sdaa = (time / 10) * 8.5f;
                            textobj.Dobezier(P0, P1, P2, 0.7f, sdaa).OnComplete(() =>
                            {
                                Tools.Destroy(textobj);
                            });
                        });
                    }, 0.05f);

                });
                // textui.TweenScale(new Vector2(1f,1f),time/6).OnComplete(()=>
                // {
                //     textui.TweenScale(new Vector2(0.6f,0.6f),(time/6)*5);
                // });


            }



        }
        public void cebezierInterrupt(string text, Vector3 P0, float time, bool baoji, float longlong = 3, float height = 3, float t = 0.45f)
        {
            // 贝塞尔曲线测试伤害数值动画
            GameObject textobj;
            Vector3 P1;
            Vector3 P2;
            // float longlong = 3;
            // float height = 3;
            textobj = GameObjectManager.Instance.Create("text3");
            // textobj.transform.parent = gameobject.transform;
            textobj.transform.position = new Vector3(P0.x, P0.y, P0.z);
            var ui = textobj.GetComponent<UIPanel>().ui;
            GTextField textui = ui.GetChild("n0").asTextField;
            textui.text = text;
            TextFormat tf = textui.textFormat;
            // tf.color= Color.red;
            // textui.rotationX=30;
            if (baoji)
            {
                tf.color = Color.red;

                textui.SetScale(1.2f, 1.2f);

                P2 = new Vector3(P0.x + longlong, P0.y, P0.z);
                P1 = new Vector3(P0.x + longlong / 2, P0.y + height, P0.z);

                textui.TweenScale(new Vector2(1, 1), time);

            }
            else
            {
                textui.color = Color.white;

                textui.SetScale(0.2f, 0.2f);
                // delayFunc(()=>
                // {
                //     textui.color= Color.white;
                // },time/3);

                P2 = new Vector3(P0.x + longlong, P0.y, P0.z);
                P1 = new Vector3(P0.x + longlong / 2, P0.y + height, P0.z);

                textui.TweenScale(new Vector2(1.3f, 1.3f), time / 12).OnComplete(() =>
                {
                    textui.color = Color.red;
                    textui.DOColor(Color.white, time / 12);
                    textui.TweenScale(new Vector2(1f, 1f), time / 12).OnComplete(() =>
                    {
                        textui.TweenScale(new Vector2(0.5f, 0.5f), (time / 12) * 10);
                        textui.TweenFade(0.4f, time);
                        float sdaa = (time / 12) * 10;
                        textobj.Dobezier(P0, P1, P2, t, sdaa).OnComplete(() =>
                        {
                            Tools.Destroy(textobj);
                        });
                    });
                });

                // textui.TweenScale(new Vector2(0.8f,0.8f),time);

            }



        }
        public void bezier(GameObject gameobject, string text, Vector3 P0, float time, bool baoji)
        {//贝塞尔曲线伤害数值动画
            GameObject textobj;
            Vector3 P1;
            Vector3 P2;

            if (baoji == true)
            {
                textobj = GameObjectManager.Instance.Create("text2");
                /*P1.x=Random.Range(P0.x+10, P0.x+20);
                P1.y=Random.Range(P0.y+20,P0.y+50);
                P1.z=P0.z;
                P2.x=Random.Range(P0.x+10, P0.x+15);
                P2.y=P0.y;
                P2.z=P0.z;*/
                if (Random.Range(0, 10) > 5)
                {
                    P2.x = Random.Range(P0.x + 3, P0.x + 6);
                    P1.x = P0.x + ((P2.x - P0.x) / 2);
                }
                else
                {
                    P2.x = Random.Range(P0.x - 3, P0.x - 6);
                    P1.x = P0.x + ((P2.x - P0.x) / 2);
                }
                P1.y = Random.Range(P0.y + 8, P0.y + 15);
                P1.z = P0.z;
                P2.y = Random.Range(1f, 6f);
                P2.z = P0.z;
            }
            else
            {
                textobj = GameObjectManager.Instance.Create("text1");
                if (Random.Range(0, 10) > 5)
                {
                    P2.x = Random.Range(P0.x + 3, P0.x + 6);
                    P1.x = P0.x + ((P2.x - P0.x) / 2);
                }
                else
                {
                    P2.x = Random.Range(P0.x - 3, P0.x - 6);
                    P1.x = P0.x + ((P2.x - P0.x) / 2);
                }
                //Random.Range(P0.y+10, P0.y+15)
                //Random.Range(1f,5f)
                //P0.y+7.5f
                P1.y = Random.Range(P0.y + 8, P0.y + 12);
                P1.z = P0.z;
                P2.y = Random.Range(1f, 3f);
                P2.z = P0.z;
            }
            //Vector3[] waypoints = new[] { new Vector3(P0.x,P0.y,P0.z), new Vector3(P1.x,P1.y,P1.z), new Vector3(P2.x,P2.y,P2.z) };

            textobj.transform.parent = gameobject.transform;
            textobj.transform.position = new Vector3(P0.x, P0.y, P0.z);
            var ui = textobj.GetComponent<UIPanel>().ui;
            GTextField textui = ui.GetChild("text").asTextField;
            textui.text = text;
            //textui.TweenScale(new Vector2(1.5f,1.5f),0.1f);
            //textobj.transform.DOPath(waypoints,1.5f,pathType:PathType.CatmullRom);
            //delayFunc("waitpath",()=>
            //{
            //textui.TweenFade(0,1.6f);
            //},0.5f);
            //delayFunc("waitTypingEffect", () =>
            //{

            //}, 0.5f);

            textobj.Dobezier(P0, P1, P2, 1, 0.75f).OnComplete(() =>
            {

            });
            StartCoroutine(waitMove(0.5f, () =>
                {
                    textui.DOFade(0, 0.25f).OnComplete(() =>
                    {
                        Tools.Destroy(textobj);
                    });
                }
            ));
        }
        
        public void GmoveXFode(GObject parent, float endValue, float fodeendvalue, float time, bool onoff)
        {
            if (onoff)
            {
                parent.visible = true;
                killtweener(movetw, parent.TweenMoveX(endValue, time), parent, onoff);
                killtweener(fodetw, parent.TweenFade(fodeendvalue, time), parent, onoff);
            }
            else
            {

                killtweener(movetw, parent.TweenMoveX(endValue, time), parent, onoff);
                killtweener(fodetw, parent.TweenFade(fodeendvalue, time), parent, onoff);
                //StartCoroutine(waitMove(time, () => { parent.visible = false;}));

            }

        }
        public void GmoveYFode(GObject parent, float endValue, float fodeendvalue, float time, bool onoff)
        {
            if (onoff)
            {
                parent.visible = true;
                killtweener(movetw, parent.TweenMoveY(endValue, time), parent, onoff);
                killtweener(fodetw, parent.TweenFade(fodeendvalue, time), parent, onoff);
            }
            else
            {

                killtweener(movetw, parent.TweenMoveY(endValue, time), parent, onoff);
                killtweener(fodetw, parent.TweenFade(fodeendvalue, time), parent, onoff);
                //StartCoroutine(waitMove(time, () => { parent.visible = false;}));

            }

        }
        public void Grotation(GObject parent, float endValue, float time, float nexttime, bool onoff)
        {

            if (onoff)
            {
                killtweener(roationtw, parent.TweenRotate(endValue, time), parent, onoff);
                killtweener(fadetw, parent.TweenFade(1, time), parent, onoff);

            }
            else
            {
                //killtweener(roationtw,parent.TweenRotate(-30f, nexttime));
                killtweener(fadetw, parent.TweenFade(0, nexttime), parent, onoff);
                StartCoroutine(waitMove(nexttime, () => { parent.visible = false; parent.rotation = -40f; }));
            }

        }
        void killtweener(GTweener kill, GTweener newfsf, GObject parent, bool onoff)
        {
            if (kill != null)
            {
                GTween.Kill(kill);
                kill = newfsf;

            }
            else
            {
                kill = newfsf;
            }
            newfsf.OnComplete(() =>
            {
                if (onoff) { }
                else
                {
                    parent.visible = false;
                }


            });


        }
        IEnumerator waitMove(float time, System.Action action)
        {
            for (float timer = time; timer >= 0; timer -= Time.deltaTime)
            {
                yield return 0;
            }
            action();
        }

    }
    public class AnimEffectSaveData
    {
        public List<AnimEffectData> animEffectDataList = new List<AnimEffectData>();
    }
}