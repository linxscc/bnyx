using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// using FairyGUI;
// using Spine.Unity;

namespace Tang
{
    public static class DoTweenExtend
    {
        // 延时方法 add by TangJian 2018/01/16 15:42:34
        public static Tweener DoDelay(this object target, float duration)
        {
            return DOTween.To(() =>
                {
                    return 0;
                },
                (float val) =>
                {
                }, 0f, duration);
        }

        public static void DoDelay(this object target, float duration, TweenCallback action)
        {
            target.DoDelay(duration).OnComplete(action);
        }

        public static Tweener DoMove(this RoleController target, Vector3 endValue, float duration)
        {
            return DOTween.To(() =>
                {
                    return target.gameObject.transform.position;
                },
                (Vector3 pos) =>
                {
                    target.CharacterController.Move(pos - target.gameObject.transform.position);
                }, endValue, duration);
        }

        public static Tweener DoMoveBy(this RoleController target, Vector3 endValue, float duration, Ease ease = Ease.Linear)
        {
            Vector3 targetPos = Vector3.zero;
            return DOTween.To(() =>
                {
                    return targetPos;
                },
                (Vector3 pos) =>
                {
                    target.CharacterController.Move(pos - targetPos);
                    targetPos = pos;
                }, endValue, duration).SetEase(ease);
        }

        public static Tweener DoTransform(this GameObject target, Vector3 endValue, float duration)
        {
            return DOTween.To(() =>
                {
                    return target.transform.position;
                },
                (Vector3 pos) =>
                {
                    //target.transform.position.Move(pos - target.transform.position);
                    target.transform.position = pos - target.transform.position;
                }, endValue, duration);
        }

        public static Tweener DoValue(this FairyGUI.GProgressBar target, float endValue, float duration)
        {
            return DOTween.To(() =>
                {
                    return (float)target.value;
                },
                (float val) =>
                {
                    //target.transform.position.Move(pos - target.transform.position);
                    target.value = (double)val;
                }, endValue, duration);
        }
        public static Tweener DoMax(this FairyGUI.GProgressBar target, float endValue, float duration)
        {
            return DOTween.To(() =>
                {
                    return (float)target.max;
                },
                (float val) =>
                {
                    //target.transform.position.Move(pos - target.transform.position);
                    target.max = (double)val;
                }, endValue, duration);
        }

        public static Tweener DoSetsize(this FairyGUI.GObject target, Vector2 endValue, float duration)
        {
            return DOTween.To(() =>
                {
                    return target.size;
                },
                (Vector2 val) =>
                {
                    target.SetSize(val.x, val.y);
                }, endValue, duration);
        }

        public static Tweener Doscalesize(this Camera target, float endValue, float duration)
        {
            return DOTween.To(() =>
                {
                    return target.orthographicSize;
                },
                (float val) =>
                {
                    target.orthographicSize = val;
                }, endValue, duration);
        }

        public static Tweener DOFade(this FairyGUI.GObject target, float endValue, float duration)
        {
            return DOTween.To(() =>
                {
                    return target.alpha;
                },
                (float alpha) =>
                {
                    target.alpha = alpha;
                }, endValue, duration);
        }

        public static Tweener DOColor(this FairyGUI.GTextField target, Color endValue, float duration)
        {
            return DOTween.To(() =>
                {
                    return target.color;
                },
                (Color alpha) =>
                {
                    target.color = alpha;
                }, endValue, duration);
        }

        public static Tweener DOAnimFloat(this Animator target, string name, float endValue, float duration)
        {
            return DOTween.To(() =>
                {
                    return target.GetFloat(name);
                },
                (float alpha) =>
                {
                    target.SetFloat(name, alpha);
                }, endValue, duration);
        }
        
        // 动画颜色渐变效果 add by TangJian 2019/3/14 21:24
//        public static Tweener DOFade(this SkeletonRenderer target, float endValue, float duration,
//            Ease ease = Ease.Linear)
//        {
//            return DOTween.To(() =>
//                {
//                    MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
//                    target.GetPropertyBlock(materialPropertyBlock);
//
//                    return materialPropertyBlock.GetVector("_Color").w;
//                },
//                (float value) =>
//                {
//                    MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
//                    target.GetPropertyBlock(materialPropertyBlock);
//
//                    var oldColor = materialPropertyBlock.GetVector("_Color");
//                    oldColor.w = value;
//                    materialPropertyBlock.SetVector("_Color", oldColor);
//                    target.SetPropertyBlock(materialPropertyBlock);
//
//                }, endValue, duration).SetEase(ease);
//        }

        // gameObject 透明度渐变 add by TangJian 2017/10/27 17:01:40
        public static Tweener DOFade(this GameObject target, float endValue, float duration, Ease ease = Ease.Linear)
        {
            Tweener ret = null;
            target.transform.Recursive((Transform tr, int de) =>
            {
                foreach (var item in tr.GetComponents<SpriteRenderer>())
                {
                    ret = item.DOFadeWithMaterialPropertyBlock(endValue, duration).SetEase(ease);
                }
                foreach (var item in tr.GetComponents<Renderer>())
                {
                    ret = item.material.DOFade(endValue, duration).SetEase(ease);
                }
                foreach (var item in tr.GetComponents<Image>())
                {
                    ret = item.DOFade(endValue, duration).SetEase(ease);
                }
                foreach (var item in tr.GetComponents<Text>())
                {
                    ret = item.DOFade(endValue, duration).SetEase(ease);
                }
                foreach (var item in tr.GetComponents<UnityEngine.Light>())
                {
                    ret = item.DOIntensity(endValue, duration).SetEase(ease);
                }
                foreach (var item in tr.GetComponents<CanvasGroup>())
                {
                    ret = item.DOFade(endValue, duration).SetEase(ease);
                }
                foreach (var item in tr.GetComponents<Projector>())
                {
                    ret = item.material.DOFade(endValue, duration).SetEase(ease);
                }
            }, 1, 99);
            return ret;
        }
        
        public static Tweener DOFadeWithMaterialPropertyBlock(this Renderer target, float endValue, float duration, Ease ease = Ease.Linear)
        {
            return DOTween.To(() =>
                {
                    MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
                    target.GetPropertyBlock(materialPropertyBlock);

                    return materialPropertyBlock.GetVector("_Color").w;
                },
                (float value) =>
                {
                    MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
                    target.GetPropertyBlock(materialPropertyBlock);

                    var oldColor = materialPropertyBlock.GetVector("_Color");
                    oldColor.w = value;
                    materialPropertyBlock.SetVector("_Color", oldColor);
                    target.SetPropertyBlock(materialPropertyBlock);

                }, endValue, duration).SetEase(ease);
        }

        public static Tweener DoMaterialVector4To(this Material material, string name, Vector4 endValue, float duration, Ease ease = Ease.Linear)
        {
            return DOTween.To(() =>
                {
                    return material.GetVector(name);
                },
                (Vector4 value) =>
                {
                    material.SetVector(name, value);
                }, endValue, duration).SetEase(ease);
        }
        public static Tweener DoMaterialVector4To(this Renderer target, string name, Vector4 endValue, float duration, Ease ease = Ease.Linear)
        {
            return DOTween.To(() =>
                {
                    MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
                    target.GetPropertyBlock(materialPropertyBlock);

                    return materialPropertyBlock.GetVector(name);
                },
                (Vector4 value) =>
                {
                    MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
                    target.GetPropertyBlock(materialPropertyBlock);

                    var oldColor = materialPropertyBlock.GetVector(name);
                    oldColor = value;
                    materialPropertyBlock.SetVector(name, oldColor);
                    target.SetPropertyBlock(materialPropertyBlock);

                }, endValue, duration).SetEase(ease);
        }

        public static Tweener DoLocalMove(this Transform target, Vector3 endLocalPos, float duration, Ease ease = Ease.Linear)
        {
            return DOTween.To(() =>
            {
                return target.localPosition;
            }, (Vector3 value) =>
            {
                target.localPosition = value;
            }, endLocalPos, duration).SetEase(ease);
        }

        public static Tweener Dobezier(this GameObject gameobject, Vector3 p0, Vector3 p1, Vector3 p2, float endValue, float firsttime)
        {
            return DOTween.To(() =>
                {
                    return Time.fixedDeltaTime;
                },
                (float time) =>
                {//1-firsttime-time/firsttime
                    gameobject.transform.position = Tools.BezierCurve(p0, p1, p2, time);
                }, endValue, firsttime);
        }

        public static Tweener DoFloat(this float target, float Difference, float duration)
        {
            return DOTween.To(() =>
                {
                    return target;
                },
                (float time) =>
                {
                    target = target + Tween.tweenChange(time, TweenType.Quad_EaseOut) * Difference;
                }, 1, duration);
        }

        public static Tweener DoFloatTo(this float target, float to, float duration)
        {
            return DOTween.To(() =>
                {
                    return target;
                },
                (float value) =>
                {
                    target = value;
                }, to, duration);
        }
        
        public static Tweener DoAnimatorSpeedTo(this Animator target, float to, float duration)
        {
            float beginSpeed = target.speed;
            return DOTween.To(() =>
                {
                    return beginSpeed;
                },
                (float value) =>
                {
                    target.speed = value;
                }, to, duration);
        }

        public static Tweener DoColorTo(this Spine.Skeleton target, Color endValue, float duration)
        {
            return DOTween.To(() =>
                {
                    return new Color(target.r, target.g, target.b, target.a);
                },
                (Color color) =>
                {
                    // target.SetColor(color);
                    target.r = color.r;
                    target.g = color.g;
                    target.b = color.b;
                    target.a = color.a;
                }, endValue, duration);
        }

        // 加颜色动画 add by tianjinpeng 2018/07/11 18:53:32
        public static Tweener DoAddColorTo(this Renderer target, Color endValue, float duration, MaterialPropertyBlockColor mas)
        {
            // MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            // Color color1 = Color.white;

            return DOTween.To(() =>
                {
                    return mas.GetColor();
                },
                (Color color) =>
                {
                    mas.SetColor("_FullColor", color);
                    target.SetPropertyBlock(mas.mpb);
                }, endValue, duration);
        }

        // // add by TangJian 2018/9/27 20:21
        // public static Tweener DORotateWithRotate(this Rigidbody target, Vector3 endValue, float duration, Vector3 to, RotateMode mode = RotateMode.Fast)
        // {
        //     return target.DORotate(endValue, duration, mode);
        // }
    }

    public class MaterialPropertyBlockColor
    {
        public MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        public Color color1 = Color.white;

        public void SetColor(string name, Color color)
        {
            mpb.SetColor(name, color);
            color1 = color;
        }

        public Color GetColor()
        {
            return color1;
        }
    }
}