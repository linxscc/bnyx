using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

namespace Tang.Anim
{
    public abstract class AnimEffectvirtualClass : MyMonoBehaviour
    {
        public AnimEffectData animEffectData;

        bool flip = false;
        public bool Flip
        {
            set
            {
                flip = value;
            }

            get
            {
                return flip != animEffectData.moveFlip;
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
                // if (direction == Direction.Right)
                return orientation;

                // return new Vector3(-orientation.x, orientation.y, orientation.z);
            }
        }

        public virtual void PlayAnim(AnimEffectData animEffectData)
        {
            switch (animEffectData.animEffectType)
            {
                case AnimEffectData.AnimEffectType.Image:
                    ////AnimGameObject.SetActive(false);
                    //Texture texture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(animEffectData.imagePath);
                    ////meshRenderer.material.SetTexture("_MainTex", texture);

                    //// 初始大小 add by TangJian 2018/9/12 10:42
                    //{
                    //    rendererSize.x = texture.width / 100f;
                    //    rendererSize.y = texture.height / 100f;
                    //    rendererSize.z = 1;

                    //    float factor = 1.0f / rendererSize.y;

                    //    rendererSize.x *= factor;
                    //    rendererSize.y *= factor;
                    //}

                    //// 锚点 add by TangJian 2018/9/12 10:42
                    //{
                    //    //imageGameObject.transform.localPosition = -animEffectData.anchor;
                    //}
                    break;
                case AnimEffectData.AnimEffectType.Anim:
                    //AnimGameObject.AddComponentUnique<SortRenderer>();
                    //Spine.Unity.SkeletonDataAsset skeletonDataAsset= (Spine.Unity.SkeletonDataAsset)UnityEditor.AssetDatabase.LoadAssetAtPath(animEffectData.animPath, typeof(Spine.Unity.SkeletonDataAsset));
                    //Spine.Unity.SkeletonAnimation skeletonAnimation = AnimGameObject.GetComponent<Spine.Unity.SkeletonAnimation>();
                    //skeletonAnimation.skeletonDataAsset = (Spine.Unity.SkeletonDataAsset)UnityEditor.AssetDatabase.LoadAssetAtPath(animEffectData.animPath, typeof(Spine.Unity.SkeletonDataAsset));
                    //rendererGameObject.SetActive(true);
                    //AnimGameObject.SetActive(true);
                    //imageGameObject.SetActive(false);

                    //Spine.TrackEntry TrackEntry = skeletonAnimation.state.SetAnimation(0, animEffectData.animName, true);
                    //TrackEntry.TimeScale = 1;

                    //skeletonAnimation.state.End += (entry) =>
                    //{
                    //    Destroy(gameObject);
                    //};
                    break;

            }

            {
                transform.localPosition += animEffectData.pos;
                transform.localScale = animEffectData.scale;

                gameObject.RecursiveComponent<Renderer>((Renderer rd, int depth) =>
                {
                    if (rd.material.HasProperty("AddColor"))
                    {
                        if (rd.gameObject.GetComponent<Spine.Unity.SkeletonRenderer>() != null)
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

                gameObject.RecursiveComponent<Renderer>((Renderer rd, int depth) =>
                {
                    if (rd.material.HasProperty("MulColor"))
                    {
                        if (rd.gameObject.GetComponent<Spine.Unity.SkeletonRenderer>() != null)
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

                gameObject.RecursiveComponent<Renderer>((Renderer rd, int depth) =>
                {
                    if (rd.gameObject.GetComponent<Spine.Unity.SkeletonRenderer>() != null)
                    {
                        Set_Color(rd, "_Color", new Color(1, 1, 1, 1));

                        Color color = Get_Color(rd, "_Color");
                        color.a = animEffectData.alpha;
                        Set_Color(rd, "_Color", color);
                    }
                    else
                    {
                        Color color = rd.material.color;
                        color.a = animEffectData.alpha;
                        rd.material.color = color;
                    }
                    // Color color = rd.material.color;
                    // color.a = animEffectData.alpha;
                    // rd.material.color = color;



                    // rd.material.SetColor("_Color", new Color(1, 1, 1, 0.1f));

                    // MaterialPropertyBlock mabb = new MaterialPropertyBlock();
                    // mabb.SetColor("_Color", new Color(1, 1, 1, 0.1f));
                    // rd.SetPropertyBlock(mabb);
                }, 1, 99);
            }

            RunAction(animEffectData.actionData, () =>
            {
                // GameObjectManager.Instance.Despawn(gameObject);
                Tools.Destroy(gameObject);
            });
        }


        public virtual void Set_Color(Renderer target, string ColorName, Color color)
        {
            MaterialPropertyBlock mabb = new MaterialPropertyBlock();
            target.GetPropertyBlock(mabb);

            mabb.SetColor(ColorName, color);
            target.SetPropertyBlock(mabb);
        }

        public virtual Color Get_Color(Renderer target, string ColorName)
        {
            MaterialPropertyBlock mabb = new MaterialPropertyBlock();
            target.GetPropertyBlock(mabb);
            return mabb.GetVector(ColorName);
        }

        public virtual void RunAction(ActionData actionData, System.Action onComplete)
        {
            switch (actionData.actionType)
            {
                case ActionType.MoveBy:
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

                    transform.DoLocalMove(transform.localPosition + (actionData.pos + randomPos).RotateFrontTo(Orientation).Flip(Flip), actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo), actionData.ease).OnComplete(() =>
                   {
                       if (onComplete != null)
                           onComplete();
                   });
                    break;
                case ActionType.AddForce:
                    {
                        var bloodRigidbody = gameObject.GetComponentInChildren<Rigidbody>();
                        if (bloodRigidbody != null)
                        {
                            if (actionData.RandomType == RandomType.Fix)
                            {
                                bloodRigidbody.AddForce(actionData.Force.RotateFrontTo(Orientation).Flip(Flip), actionData.ForceMode);
                            }
                            else
                            {
                                Vector3 force = new Vector3(
                                    Random.Range(actionData.randomForceMin.x, actionData.randomForceMax.x),
                                    Random.Range(actionData.randomForceMin.y, actionData.randomForceMax.y),
                                    Random.Range(actionData.randomForceMin.z, actionData.randomForceMax.z));

                                bloodRigidbody.AddForce(force.RotateFrontTo(Orientation).Flip(Flip), actionData.ForceMode);
                            }
                        }

                        gameObject.DoDelay(actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo)).OnComplete(() => { onComplete(); });
                    }
                    break;
                case ActionType.AddRotation:
                    {
                        AddRotation(actionData, onComplete);
                    }
                    break;
                case ActionType.AddTorque:
                    {
                        var bloodRigidbody = gameObject.GetComponentInChildren<Rigidbody>();
                        if (bloodRigidbody != null)
                        {
                            if (actionData.RandomType == RandomType.Fix)
                            {
                                bloodRigidbody.AddTorque(actionData.Force.RotateFrontTo(Orientation).Flip(Flip), actionData.ForceMode);
                            }
                            else
                            {
                                Vector3 force = new Vector3(
                                Random.Range(actionData.randomForceMin.x, actionData.randomForceMax.x),
                                Random.Range(actionData.randomForceMin.y, actionData.randomForceMax.y),
                                Random.Range(actionData.randomForceMin.z, actionData.randomForceMax.z));

                                bloodRigidbody.AddTorque(force.RotateFrontTo(Orientation).Flip(Flip), actionData.ForceMode);
                            }

                        }

                        gameObject.DoDelay(actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo)).OnComplete(() => { onComplete(); });
                    }
                    break;
                case ActionType.FadeTo:
                    FadeTo(actionData, onComplete);
                    break;

                case ActionType.ScaleTo:
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
                    toScale.x = toScale.x * animEffectData.scale.x;
                    toScale.y = toScale.y * animEffectData.scale.y;
                    toScale.z = toScale.z * animEffectData.scale.z;

                    transform.DOScale(toScale, actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo)).SetEase(actionData.ease).OnComplete(() =>
                    {
                        onComplete();
                    });

                    break;
                case ActionType.Curve:
                    {
                        var bloodRigidbody = gameObject.GetComponentInChildren<Rigidbody>();
                        if (bloodRigidbody != null && bloodRigidbody.useGravity)
                        {
                            onComplete();
                        }
                        else
                        {
                            Curve(actionData, onComplete);
                        }
                    }
                    break;
                case ActionType.Path:
                    {
                        var bloodRigidbody = gameObject.GetComponentInChildren<Rigidbody>();
                        if (bloodRigidbody != null && bloodRigidbody.useGravity)
                        {
                            onComplete();
                        }
                        else
                        {
                            Dopath(actionData, onComplete);
                        }
                    }
                    break;

                case ActionType.AddColorTo:

                    AddColorTo(actionData, onComplete);

                    break;

                case ActionType.MulColorTo:

                    MulColorTo(actionData, onComplete);

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

        public void Curve(ActionData actionData, System.Action onComplete)
        {
            Vector3 P0 = gameObject.transform.position;
            Vector3 P2 = new Vector3(P0.x + actionData.width, P0.y, P0.z);
            Vector3 P1 = new Vector3(P0.x + actionData.width / 2, P0.y + actionData.height, P0.z);
            if (Flip)
            {
                P2 = new Vector3(P0.x - actionData.width, P0.y, P0.z);
                P1 = new Vector3(P0.x - actionData.width / 2, P0.y + actionData.height, P0.z);
            }
            gameObject.Dobezier(P0, P1, P2, actionData.t, actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo)).OnComplete(() =>
            {
                onComplete();
            });
        }
        public void Dopath(ActionData actionData, System.Action onComplete)
        {

            Vector3[] vector3s = actionData.vector3List.ToArray();
            for (int intsd = 0; intsd < vector3s.Length; intsd++)
            {
                Vector3 nw = actionData.vector3List[intsd];
                if (Flip)
                {
                    nw.x = -nw.x;
                }
                vector3s[intsd] = nw + gameObject.transform.localPosition;
            }
            gameObject.transform.DOLocalPath(vector3s, actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo), actionData.pathType, actionData.pathMode, actionData.resolution).OnComplete(() =>
            {
                onComplete();
            });
        }
        public virtual void FadeTo(ActionData actionData, System.Action onComplete)
        {
            gameObject.RecursiveComponent<Renderer>((Renderer rd, int depth) =>
            {
                // if (rd.gameObject.GetComponent<Spine.Unity.SkeletonRenderer>() == null)
                // {
                //     rd.gameObject.DOFade(actionData.alpha, actionData.duration, actionData.ease).OnComplete(() =>
                //     {
                //         onComplete();
                //     });
                // }
                // else
                // {
                rd.DOFadeWithMaterialPropertyBlock(actionData.alpha, actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo), actionData.ease).OnComplete(() =>
                {
                    onComplete();
                });
                // }
            }, 1, 999);



        }

        public virtual void AddColorTo(ActionData actionData, System.Action onComplete)
        {
            gameObject.RecursiveComponent<Renderer>((Renderer rd, int depth) =>
            {
                if (rd.material.HasProperty("AddColor"))
                {
                    if (rd.gameObject.GetComponent<Spine.Unity.SkeletonRenderer>() == null)
                    {
                        rd.material.DoMaterialVector4To("AddColor", actionData.addColor, actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo), actionData.ease).OnComplete(() =>
                        {
                            onComplete();
                        });
                    }
                    else
                    {
                        rd.DoMaterialVector4To("AddColor", actionData.addColor, actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo), actionData.ease).OnComplete(() =>
                        {
                            onComplete();
                        });
                    }

                }
                else
                {
                    onComplete();
                }

            }, 1, 999);

        }
        public virtual void MulColorTo(ActionData actionData, System.Action onComplete)
        {
            gameObject.RecursiveComponent<Renderer>((Renderer rd, int depth) =>
            {
                if (rd.material.HasProperty("MulColor"))
                {
                    if (rd.gameObject.GetComponent<Spine.Unity.SkeletonRenderer>() == null)
                    {
                        rd.material.DoMaterialVector4To("MulColor", actionData.addColor, actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo), actionData.ease).OnComplete(() =>
                        {
                            onComplete();
                        });
                    }
                    else
                    {
                        rd.DoMaterialVector4To("MulColor", actionData.addColor, actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo), actionData.ease).OnComplete(() =>
                        {
                            onComplete();
                        });
                    }

                }
                else
                {
                    onComplete();
                }

            }, 1, 999);

        }
        public virtual void AddRotation(ActionData actionData, System.Action onComplete)
        {
            var bloodRigidbody = gameObject.GetComponentInChildren<Rigidbody>();
            if (bloodRigidbody != null && bloodRigidbody.useGravity)
            {
                if (actionData.RandomType == RandomType.Fix)
                {
                    bloodRigidbody.DORotate(actionData.rotation.RotateFrontTo(Orientation).Flip(Flip), actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo), actionData.rotateMode).OnComplete(() =>
                    {
                        onComplete();
                    });
                }
                else
                {
                    Vector3 rotation = new Vector3(Random.Range(actionData.randomRotationFrom.x, actionData.randomRotationTo.x),
                    Random.Range(actionData.randomRotationFrom.y, actionData.randomRotationTo.y),
                    Random.Range(actionData.randomRotationFrom.z, actionData.randomRotationTo.z));

                    bloodRigidbody.DORotate(rotation.RotateFrontTo(Orientation).Flip(Flip), actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo), actionData.rotateMode).OnComplete(() =>
                    {
                        onComplete();
                    });
                }
            }
            else
            {
                if (actionData.RandomType == RandomType.Fix)
                {
                    transform.DORotate(actionData.rotation.RotateFrontTo(Orientation), actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo), actionData.rotateMode).OnComplete(() =>
                    {
                        onComplete();
                    });
                }
                else
                {
                    var rotation = new Vector3(Random.Range(actionData.randomRotationFrom.x, actionData.randomRotationTo.x),
                        Random.Range(actionData.randomRotationFrom.y, actionData.randomRotationTo.y),
                        Random.Range(actionData.randomRotationFrom.z, actionData.randomRotationTo.z));

                    transform.DORotate(rotation.RotateFrontTo(Orientation), actionData.duration + Random.Range(actionData.randomDurationFrom, actionData.randomDurationTo), actionData.rotateMode).OnComplete(() =>
                    {
                        onComplete();
                    });
                }
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