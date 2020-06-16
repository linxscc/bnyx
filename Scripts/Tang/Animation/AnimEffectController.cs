using System;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using Spine.Unity;
using Tang.FrameEvent;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Tang.Anim
{
    public class AnimEffectController : MyMonoBehaviour
    {
        public AnimEffectData animEffectData;

        public bool moveFlip = false;
        public bool MoveFlip
        {
            set
            {
                moveFlip = value;
            }

            get
            {
                return moveFlip != animEffectData.moveFlip;
            }
        }

        public bool RenderFlip
        {
            get
            {
                return animEffectData.renderFlip != MoveFlip;
            }
        }

        Vector3 orientation = new Vector3(1, 0, 0);
        public Vector3 Orientation
        {
            set
            {
                orientation = value;
            }

            get
            {
                return orientation;
            }
        }

        GameObject rendererGameObject;
        GameObject imageGameObject;
        GameObject animGameObject;
        GameObject particleGameObject;
        GameObject otherGameObject;
        
        public GameObject OtherGameObject => otherGameObject;
        
        Vector3 rendererSize = new Vector3(1, 1, 1);

        GameObject moveGameObject;

        private List<Tweener> _tweeners = new List<Tweener>();

        private void Awake()
        {
            rendererGameObject = rendererGameObject ?? gameObject.GetChild("Renderer", true);

            imageGameObject = imageGameObject ?? rendererGameObject.GetChild("Image", true);
            animGameObject = animGameObject ?? rendererGameObject.GetChild("Anim", true);
            particleGameObject = particleGameObject ?? rendererGameObject.GetChild("Particle", true);
            otherGameObject = otherGameObject ?? rendererGameObject.GetChild("Other", true);
            
            imageGameObject.SetActive(false);
            animGameObject.SetActive(false);
            particleGameObject.SetActive(false);
            otherGameObject.SetActive(false);
        }

        private void Initialize()
        {
            imageGameObject.SetActive(false);
            animGameObject.SetActive(false);
            particleGameObject.SetActive(false);
            otherGameObject.SetActive(false);
            
            // 清除没运行完成的Tweener
            foreach (var tweener in _tweeners)
            {
                tweener?.Kill();
            }
            _tweeners.Clear();
            
            // 重置动画
            animGameObject.GetComponent<SkeletonAnimation>().skeletonDataAsset = null;
            
            // 重置other节点
            for (int OtherChildItem = 0; OtherChildItem < otherGameObject.transform.childCount; OtherChildItem++)
            {
                var Obj = otherGameObject.transform.GetChild(0).gameObject;
                Obj.transform.parent = null;
                AssetManager.DeSpawn(Obj);
            }
            
            // 大小
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            imageGameObject.transform.localScale = new Vector3(1, 1, 1);
            animGameObject.transform.localScale = new Vector3(1, 1, 1);
            particleGameObject.transform.localScale = new Vector3(1, 1, 1);
            otherGameObject.transform.localScale = new Vector3(1, 1, 1);
            
            
            gameObject.transform. eulerAngles = Vector3.zero;
            imageGameObject.transform.eulerAngles = Vector3.zero;
            animGameObject.transform.eulerAngles = Vector3.zero;
            particleGameObject.transform.eulerAngles = Vector3.zero;
            otherGameObject.transform.eulerAngles = Vector3.zero;
            
            // 重置节点位置
            imageGameObject.transform.localPosition = Vector3.zero;
            animGameObject.transform.localPosition = Vector3.zero;
            particleGameObject.transform.localPosition = Vector3.zero;
            otherGameObject.transform.localPosition = Vector3.zero;
        }

        private void Init()
        {
            if (otherGameObject.GetComponentInChildren<Rigidbody>() is Rigidbody rigidbody)
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
                rigidbody.inertiaTensorRotation = Quaternion.Euler(Vector3.zero);
                rigidbody.ResetInertiaTensor();
                rigidbody.ResetCenterOfMass();
            }
        }

        public void PlayAnim(AnimEffectData animEffectData)
        {
            Initialize();
            DoPlayAnim(animEffectData);
        }

        
        private async void DoPlayAnim(AnimEffectData animEffectData)
        {
            switch (animEffectData.animEffectType)
            {
                case AnimEffectData.AnimEffectType.Image:
                    Texture texture = await AssetManager.LoadAssetAsync<Texture2D>(animEffectData.imagePath);
                    imageGameObject.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture);

                    imageGameObject.GetComponent<MeshRenderer>().material.SetFloat("ZWrite",
                        animEffectData.zwriteType == FrameEventInfo.ZwriteType.Off ? 0 : 1);

                {
                    switch (animEffectData.renderQueue)
                    {
                        case FrameEventInfo.RenderQueue.Geometry:
                            imageGameObject.GetComponent<MeshRenderer>().material.renderQueue = 2000;
                            break;
                        case FrameEventInfo.RenderQueue.AlphaTest:
                            imageGameObject.GetComponent<MeshRenderer>().material.renderQueue = 2450;
                            break;
                        case FrameEventInfo.RenderQueue.Transparent:
                            imageGameObject.GetComponent<MeshRenderer>().material.renderQueue = 3000;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                    
                    // 初始大小 add by TangJian 2018/9/12 10:42
                {
                    rendererSize.x = texture.width / 100f;
                    rendererSize.y = texture.height / 100f;
                    rendererSize.z = 1;

                    float factor = 1.0f / rendererSize.y;

                    rendererSize.x *= factor;
                    rendererSize.y *= factor;
                }

                    // 锚点 add by TangJian 2018/9/12 10:42
                {
                    imageGameObject.transform.localPosition = -animEffectData.anchor;
                }

                    moveGameObject = gameObject;
                    
                    // 显示图片 add by TangJian 2019/6/14 16:22
                    imageGameObject.SetActive(true);
                    break;
                case AnimEffectData.AnimEffectType.Anim:
                    Spine.Unity.SkeletonAnimation skeletonAnimation = animGameObject.GetComponent<Spine.Unity.SkeletonAnimation>();
                    skeletonAnimation.skeletonDataAsset = await AssetManager.LoadAssetAsync<Spine.Unity.SkeletonDataAsset>(animEffectData.animPath);
                    
                    skeletonAnimation.Initialize(true);
                    Spine.TrackEntry TrackEntry = skeletonAnimation.state.SetAnimation(0, animEffectData.animName, animEffectData.animLoop);
                    TrackEntry.TimeScale = 1;

                    skeletonAnimation.state.TimeScale = animEffectData.animSpeed;
                    animGameObject.transform.localPosition = Vector3.zero;
                    moveGameObject = animGameObject;
                    
                    // 显示动画 add by TangJian 2019/6/14 16:23
                    animGameObject.SetActive(true);
                    break;
                case AnimEffectData.AnimEffectType.Particle:
//                    var particle = await AssetManager.Instantiate(animEffectData.prefabname);

                    var particle = await AssetManager.SpawnAsync(animEffectData.prefabname);
                    particle.transform.parent = particleGameObject.transform;

                    particle.transform.localPosition = Vector3.zero;

                    moveGameObject = particleGameObject;
                    
                    // 显示粒子 add by TangJian 2019/6/14 16:23
                    particleGameObject.SetActive(true);
                    break;
                case AnimEffectData.AnimEffectType.Gameobject:
//                    var other = await AssetManager.Instantiate(animEffectData.prefabpath);
                    var other = await AssetManager.SpawnAsync(animEffectData.prefabpath);

                    other.transform.parent = otherGameObject.transform;

                    other.transform.localPosition = Vector3.zero;
                    
                    other.transform.eulerAngles = Vector3.zero;
                    
                    moveGameObject = otherGameObject;
                    
                    // 显示其他 add by TangJian 2019/6/14 16:23
                    otherGameObject.SetActive(true);
                    break;
                default:
                    Debug.LogError("");
                    break;
            }

            Init();
            
            RunAction(animEffectData.actionData, () =>
            {
                Tang.PoolManager.Instance.Despawn(gameObject);
            });

            {
                gameObject.RecursiveComponent<Renderer>((Renderer rd, int depth) =>
                {
                    SortRenderer sortRenderer = rd.gameObject.AddComponentIfNone<SortRenderer>();
                    sortRenderer.Pos = Vector3.zero;
                }, 1, 99);
            }

            {
                moveGameObject.transform.localPosition += animEffectData.pos.RotateFrontTo(orientation).Flip(MoveFlip);
                if (animEffectData.animEffectType == AnimEffectData.AnimEffectType.Particle)
                {
                    moveGameObject.transform.localScale = animEffectData.scale;
                }
                else
                {
                    moveGameObject.transform.localScale = animEffectData.scale.Flip(RenderFlip);
                }

                if (moveGameObject.transform.localScale.x > 0)
                {
                    moveGameObject.transform.localRotation = Quaternion.Euler((moveGameObject.transform.localRotation.eulerAngles + animEffectData.rotation));
                }
                else
                {
                    moveGameObject.transform.localRotation = Quaternion.Euler((moveGameObject.transform.localRotation.eulerAngles + new Vector3(animEffectData.rotation.x, animEffectData.rotation.y, animEffectData.rotation.z * -1)));
                }


                moveGameObject.RecursiveComponent<Renderer>((Renderer rd, int depth) =>
                {
                    if (rd.material.HasProperty("AddColor"))
                    {
                        if (rd.gameObject.name == "Anim")
                        {
                            Set_Color(rd, "AddColor", new Color(0, 0, 0, 0));
                            Color color = Get_Color(rd, "AddColor");
                            color = animEffectData.addColor;
                            Set_Color(rd, "AddColor", color);
                        }
                        else
                        {
                            rd.material.SetColor("AddColor", animEffectData.addColor);
                        }
                    }
                }, 1, 99);

                moveGameObject.RecursiveComponent<Renderer>((Renderer rd, int depth) =>
                {
                    if (rd.material.HasProperty("MulColor"))
                    {
                        if (rd.gameObject.name == "Anim")
                        {
                            Set_Color(rd, "MulColor", new Color(1, 1, 1, 1));
                            Color color = Get_Color(rd, "MulColor");
                            color = animEffectData.mulColor;
                            Set_Color(rd, "MulColor", color);
                        }
                        else
                        {
                            rd.material.SetColor("MulColor", animEffectData.mulColor);
                        }

                    }
                }, 1, 99);

                moveGameObject.RecursiveComponent<Renderer>((Renderer rd, int depth) =>
                {
                    if (rd.gameObject.name == "Anim" && rd.material.HasProperty("_Color"))
                    {
                        Set_Color(rd, "_Color", new Color(1, 1, 1, 1));

                        Color color = Get_Color(rd, "_Color");
                        color.a = animEffectData.alpha;
                        Set_Color(rd, "_Color", color);
                    }
                    else
                    {
                        if (animEffectData.animEffectType != AnimEffectData.AnimEffectType.Particle)
                        {
                            if (rd.material.HasProperty("_Color"))
                            {
                                Color color = rd.material.color;
                                color.a = animEffectData.alpha;
                                rd.material.color = color;   
                            }
                            else
                            {
                                Debug.Log("没有_Color属性");
                            }
                        }

                    }

                }, 1, 99);
            }
        }
        
        public void Set_Color(Renderer target, string ColorName, Color color)
        {
            MaterialPropertyBlock mabb = new MaterialPropertyBlock();
            target.GetPropertyBlock(mabb);

            mabb.SetColor(ColorName, color);
            target.SetPropertyBlock(mabb);
        }

        public Color Get_Color(Renderer target, string ColorName)
        {
            MaterialPropertyBlock mabb = new MaterialPropertyBlock();
            target.GetPropertyBlock(mabb);
            return mabb.GetVector(ColorName);
        }

        public void RunAction(ActionData actionData, System.Action onComplete)
        {
            switch (actionData.actionType)
            {
                case ActionType.MoveBy:
                    MoveBy(actionData, onComplete);
                    break;

                case ActionType.FadeTo:
                    FadeTo(actionData, onComplete);
                    break;

                case ActionType.ScaleTo:
                    ScaleTo(actionData, onComplete);
                    break;

                case ActionType.AddColorTo:
                    AddColorTo(actionData, onComplete);
                    break;

                case ActionType.MulColorTo:
                    MulColorTo(actionData, onComplete);
                    break;

                case ActionType.AddForce:
                    AddForce(actionData, onComplete);
                    break;

                case ActionType.AddRotation:
                    AddRotation(actionData, onComplete);
                    break;
                case ActionType.AddTorque:
                    AddTorque(actionData, onComplete);
                    break;
                case ActionType.Curve:
                    Curve(actionData, onComplete);
                    break;

                case ActionType.Path:
                    Dopath(actionData, onComplete);
                    break;

                case ActionType.Sequence:
                    RunSequence(actionData.actionList, onComplete, 0);
                    break;

                case ActionType.Parallel:
                    RunParallel(actionData.actionList, onComplete);
                    break;

                default:
                    onComplete();
                    break;
            }
        }

        public void MoveBy(ActionData actionData, System.Action onComplete)
        {
            Vector3 randomPos;
            if (actionData.isNormalizedrandom)
            {
                randomPos = actionData.randomPosFrom + (actionData.randomPosTo - actionData.randomPosFrom).normalized * UnityEngine.Random.Range(0, (actionData.randomPosTo - actionData.randomPosFrom).magnitude);
            }
            else
            {
                randomPos = new Vector3(UnityEngine.Random.Range(actionData.randomPosFrom.x, actionData.randomPosTo.x)
                    , UnityEngine.Random.Range(actionData.randomPosFrom.y, actionData.randomPosTo.y)
                    , UnityEngine.Random.Range(actionData.randomPosFrom.z, actionData.randomPosTo.z));
            }

            Tweener tweener = moveGameObject.transform.DoLocalMove(moveGameObject.transform.localPosition + (actionData.pos + randomPos).RotateFrontTo(Orientation).Flip(MoveFlip), actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo), actionData.ease).OnComplete(() =>
            {
                onComplete();
            });
            _tweeners.Add(tweener);
        }

        public void ScaleTo(ActionData actionData, System.Action onComplete)
        {
            Vector3 randomScale;

            if (actionData.isNormalizedrandom)
            {
                randomScale = actionData.randomScaleFrom + (actionData.randomScaleTo - actionData.randomScaleFrom).normalized * UnityEngine.Random.Range(0, (actionData.randomScaleTo - actionData.randomScaleFrom).magnitude);
            }
            else
            {
                randomScale = new Vector3(UnityEngine.Random.Range(actionData.randomScaleFrom.x, actionData.randomScaleTo.x)
                    , UnityEngine.Random.Range(actionData.randomScaleFrom.y, actionData.randomScaleTo.y)
                    , UnityEngine.Random.Range(actionData.randomScaleFrom.z, actionData.randomScaleTo.z));
            }

            Vector3 toScale = actionData.scale + randomScale;
            toScale.x = toScale.x * animEffectData.scale.x * rendererSize.x;
            toScale.y = toScale.y * animEffectData.scale.y * rendererSize.y;
            toScale.z = toScale.z * animEffectData.scale.z * rendererSize.z;

            Tweener tweener = moveGameObject.transform.DOScale(toScale.Flip(RenderFlip), actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo)).SetEase(actionData.ease).OnComplete(() =>
            {
                onComplete();
            });
            _tweeners.Add(tweener);
        }

        public void AddForce(ActionData actionData, System.Action onComplete)
        {
            var bloodRigidbody = otherGameObject.GetComponentInChildren<Rigidbody>();
            if (bloodRigidbody != null)
            {
                if (actionData.RandomType == RandomType.Fix)
                {
                    bloodRigidbody.AddForce(actionData.Force.RotateFrontTo(Orientation).Flip(MoveFlip), actionData.ForceMode);
                }
                else
                {
                    Vector3 force = new Vector3(
                        Random.Range(actionData.randomForceMin.x, actionData.randomForceMax.x),
                        Random.Range(actionData.randomForceMin.y, actionData.randomForceMax.y),
                        Random.Range(actionData.randomForceMin.z, actionData.randomForceMax.z));

                    bloodRigidbody.AddForce(force.RotateFrontTo(Orientation).Flip(MoveFlip), actionData.ForceMode);
                }
            }

            Tweener tweener = imageGameObject.DoDelay(actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo)).OnComplete(() => { onComplete(); });
            _tweeners.Add(tweener);
        }

        public void AddRotation(ActionData actionData, System.Action onComplete)
        {
            if (actionData.RandomType == RandomType.Fix)
            {
                Vector3 rotation = new Vector3();
                if (animEffectData.moveFlip)
                {
                    if (MoveFlip)
                    {
                        rotation = new Vector3(-actionData.rotation.x, actionData.rotation.y, actionData.rotation.z);
                    }
                    else
                    {
                        rotation = actionData.rotation;
                    }
                }
                else
                {
                    rotation = actionData.rotation;
                }

                Tweener tweener = moveGameObject.transform.DORotate(rotation, actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo), actionData.rotateMode).OnComplete(() =>
                {
                    onComplete();
                });
                _tweeners.Add(tweener);
            }
            else
            {
                Vector3 rotation = new Vector3();
                if (animEffectData.moveFlip)
                {
                    if (MoveFlip)
                    {
                        rotation = new Vector3(Random.Range(-actionData.randomRotationFrom.x, -actionData.randomRotationTo.x),
                            Random.Range(actionData.randomRotationFrom.y, actionData.randomRotationTo.y),
                            Random.Range(actionData.randomRotationFrom.z, actionData.randomRotationTo.z));
                    }
                    else
                    {
                        rotation = new Vector3(Random.Range(actionData.randomRotationFrom.x, actionData.randomRotationTo.x),
                            Random.Range(actionData.randomRotationFrom.y, actionData.randomRotationTo.y),
                            Random.Range(actionData.randomRotationFrom.z, actionData.randomRotationTo.z));
                    }
                }
                else
                {
                    rotation = new Vector3(Random.Range(actionData.randomRotationFrom.x, actionData.randomRotationTo.x),
                        Random.Range(actionData.randomRotationFrom.y, actionData.randomRotationTo.y),
                        Random.Range(actionData.randomRotationFrom.z, actionData.randomRotationTo.z));
                }
                Tweener tweener = moveGameObject.transform.DORotate(rotation, actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo), actionData.rotateMode).OnComplete(() =>
                {
                    onComplete();
                });
                _tweeners.Add(tweener);
            }
        }

        public void AddTorque(ActionData actionData, System.Action onComplete)
        {
            {
                var bloodRigidbody = gameObject.GetComponentInChildren<Rigidbody>();
                if (bloodRigidbody != null)
                {
                    if (actionData.RandomType == RandomType.Fix)
                    {
                        bloodRigidbody.AddTorque(actionData.Force.RotateFrontTo(Orientation).Flip(MoveFlip), actionData.ForceMode);
                    }
                    else
                    {
                        Vector3 force = new Vector3(
                            Random.Range(actionData.randomForceMin.x, actionData.randomForceMax.x),
                            Random.Range(actionData.randomForceMin.y, actionData.randomForceMax.y),
                            Random.Range(actionData.randomForceMin.z, actionData.randomForceMax.z));

                        bloodRigidbody.AddTorque(force.RotateFrontTo(Orientation).Flip(MoveFlip), actionData.ForceMode);
                    }

                }

                Tweener tweener = gameObject.DoDelay(actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo)).OnComplete(() => { onComplete(); });
                _tweeners.Add(tweener);
            }
        }

        public void Curve(ActionData actionData, System.Action onComplete)
        {
            Vector3 P0 = moveGameObject.transform.position;
            Vector3 P2 = new Vector3(P0.x + actionData.width, P0.y, P0.z);
            Vector3 P1 = new Vector3(P0.x + actionData.width / 2, P0.y + actionData.height, P0.z);
            if (MoveFlip)
            {
                P2 = new Vector3(P0.x - actionData.width, P0.y, P0.z);
                P1 = new Vector3(P0.x - actionData.width / 2, P0.y + actionData.height, P0.z);
            }

            Tweener tweener = moveGameObject.Dobezier(P0, P1, P2, actionData.t, actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo)).OnComplete(() =>
            {
                onComplete();
            });
            _tweeners.Add(tweener);
        }
        public void Dopath(ActionData actionData, System.Action onComplete)
        {

            Vector3[] vector3s = actionData.vector3List.ToArray();
            for (int intsd = 0; intsd < vector3s.Length; intsd++)
            {
                Vector3 nw = actionData.vector3List[intsd];
                if (MoveFlip)
                {
                    nw.x = -nw.x;
                }
                vector3s[intsd] = nw + moveGameObject.transform.localPosition;
            }

            Tweener tweener = moveGameObject.transform.DOLocalPath(vector3s, actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo), actionData.pathType, actionData.pathMode, actionData.resolution).OnComplete(() =>
            {
                onComplete();
            });
            _tweeners.Add(tweener);
        }
        public void FadeTo(ActionData actionData, System.Action onComplete)
        {

            if (animEffectData.animEffectType == AnimEffectData.AnimEffectType.Image)
            {
                Tweener tweener = moveGameObject.DOFade(actionData.alpha, actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo), actionData.ease).OnComplete(() =>
                {
                    onComplete();
                });
                _tweeners.Add(tweener);
            }
            else if (animEffectData.animEffectType == AnimEffectData.AnimEffectType.Anim)
            {
                moveGameObject.RecursiveComponent<Renderer>((Renderer rd, int depth) =>
                {

                    Tweener tweener = rd.DOFadeWithMaterialPropertyBlock(actionData.alpha, actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo), actionData.ease).OnComplete(() =>
                    {
                        onComplete();
                    });
                    _tweeners.Add(tweener);

                }, 1, 999);
            }
            else
            {
                Tweener tweener = moveGameObject.DOFade(actionData.alpha, actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo), actionData.ease).OnComplete(() =>
                {
                    onComplete();
                });
                _tweeners.Add(tweener);
            }

        }
        public void AddColorTo(ActionData actionData, System.Action onComplete)
        {

            if (animEffectData.animEffectType == AnimEffectData.AnimEffectType.Image)
            {
                moveGameObject.RecursiveComponent<Renderer>((Renderer rd, int depth) =>
                {
                    if (rd.material.HasProperty("AddColor"))
                    {
                        Tweener tweener = rd.material.DoMaterialVector4To("AddColor", actionData.addColor, actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo), actionData.ease).OnComplete(() =>
                        {
                            onComplete();
                        });
                        _tweeners.Add(tweener);
                    }
                    else
                    {
                        onComplete();
                    }
                }, 1, 999);
            }
            else if (animEffectData.animEffectType == AnimEffectData.AnimEffectType.Anim)
            {
                moveGameObject.RecursiveComponent<Renderer>((Renderer rd, int depth) =>
                {
                    if (rd.material.HasProperty("AddColor"))
                    {
                        Tweener tweener = rd.DoMaterialVector4To("AddColor", actionData.addColor, actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo), actionData.ease).OnComplete(() =>
                        {
                            onComplete();
                        });
                        _tweeners.Add(tweener);
                    }
                    else
                    {
                        onComplete();
                    }

                }, 1, 999);

            }
            else
            {
                moveGameObject.RecursiveComponent<Renderer>((Renderer rd, int depth) =>
                {
                    if (rd.material.HasProperty("AddColor"))
                    {
                        Tweener tweener = rd.material.DoMaterialVector4To("AddColor", actionData.addColor, actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo), actionData.ease).OnComplete(() =>
                        {
                            onComplete();
                        });
                        _tweeners.Add(tweener);
                    }
                    else
                    {
                        onComplete();
                    }
                }, 1, 999);
            }

        }
        public void MulColorTo(ActionData actionData, System.Action onComplete)
        {

            if (animEffectData.animEffectType == AnimEffectData.AnimEffectType.Image)
            {
                moveGameObject.RecursiveComponent<Renderer>((Renderer rd, int depth) =>
                {
                    if (rd.material.HasProperty("MulColor"))
                    {
                        Tweener tweener = rd.material.DoMaterialVector4To("MulColor", actionData.addColor, actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo), actionData.ease).OnComplete(() =>
                        {
                            onComplete();
                        });
                        _tweeners.Add(tweener);
                    }
                    else
                    {
                        onComplete();
                    }
                }, 1, 999);
            }
            else if (animEffectData.animEffectType == AnimEffectData.AnimEffectType.Anim)
            {
                moveGameObject.RecursiveComponent<Renderer>((Renderer rd, int depth) =>
                {
                    if (rd.material.HasProperty("MulColor"))
                    {
                        Tweener tweener = rd.DoMaterialVector4To("MulColor", actionData.addColor, actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo), actionData.ease).OnComplete(() =>
                        {
                            onComplete();
                        });
                        _tweeners.Add(tweener);
                    }
                    else
                    {
                        onComplete();
                    }

                }, 1, 999);

            }
            else
            {
                moveGameObject.RecursiveComponent<Renderer>((Renderer rd, int depth) =>
                {
                    if (rd.material.HasProperty("MulColor"))
                    {
                        Tweener tweener = rd.material.DoMaterialVector4To("MulColor", actionData.addColor, actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo), actionData.ease).OnComplete(() =>
                        {
                            onComplete();
                        });
                        _tweeners.Add(tweener);
                    }
                    else
                    {
                        onComplete();
                    }
                }, 1, 999);
            }

        }

        public void RunSequence(List<ActionData> actionList, System.Action onComplete, int index)
        {
            if (actionList.Count <= 0)
            {
                onComplete();
                return;
            }

            RunAction(actionList[index], () =>
            {
                index++;
                if (index < actionList.Count)
                {
                    RunSequence(actionList, onComplete, index);
                }
                else
                {
                    onComplete();
                }
            });
        }

        public void RunParallel(List<ActionData> actionList, System.Action onComplete)
        {
            if (actionList.Count <= 0)
            {
                onComplete();
                return;
            }

            int completeCount = 0;
            for (int i = 0; i < actionList.Count; i++)
            {
                RunAction(actionList[i], () =>
                {
                    completeCount++;
                    if (completeCount == actionList.Count)
                    {
                        onComplete();
                    }
                });
            }
        }


    }
}