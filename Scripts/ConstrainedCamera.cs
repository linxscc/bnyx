/******************************************************************************
 * Spine Runtimes Software License v2.5
 *
 * Copyright (c) 2013-2016, Esoteric Software
 * All rights reserved.
 *
 * You are granted a perpetual, non-exclusive, non-sublicensable, and
 * non-transferable license to use, install, execute, and perform the Spine
 * Runtimes software and derivative works solely for personal or internal
 * use. Without the written permission of Esoteric Software (see Section 2 of
 * the Spine Software License Agreement), you may not (a) modify, translate,
 * adapt, or develop new applications using the Spine Runtimes or otherwise
 * create derivative works or improvements of the Spine Runtimes or (b) remove,
 * delete, alter, or obscure any trademarks or any copyright, trademark, patent,
 * or other intellectual property or proprietary rights notices on or in the
 * Software, including any copy thereof. Redistributions in binary or source
 * form must include this license and terms.
 *
 * THIS SOFTWARE IS PROVIDED BY ESOTERIC SOFTWARE "AS IS" AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
 * EVENT SHALL ESOTERIC SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES, BUSINESS INTERRUPTION, OR LOSS OF
 * USE, DATA, OR PROFITS) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
 * IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

// Contributed by: Mitch Thompson

using UnityEngine;
using DG.Tweening;

namespace Tang
{
    [System.Serializable]
    public class CameraLimit
    {
        public float minX = -9999999;
        public float maxX = 9999999;
        public float minY = -9999999;
        public float maxY = 9999999;
    }

    // namespace Spine.Unity.Examples {
    public class ConstrainedCamera : MyMonoBehaviour
    {
        public Transform Player1;
        public Transform Player2;

        public Vector3 offset;
        // public Vector3 min;
        // public Vector3 max;
        public float smoothing = 5f;

        public Vector4 boxXY = new Vector4(0.3f, 0.7f, 0.3f, 0.7f);
        public Vector4 boxXY2
        {
            get
            {
                return new Vector4(boxXY.x * Camera.pixelWidth, boxXY.y * Camera.pixelWidth, boxXY.z * Camera.pixelHeight, boxXY.w * Camera.pixelHeight);
            }
        }

        public string mode = "move";

        public Vector3 shakeOffset = Vector3.zero;

        private Camera _camera;

        Camera all;
        Camera background;
        Camera frontCamera;

        public CameraLimit cameraLimit = new CameraLimit();
        // public float minX

        private Camera Camera
        {
            get { return _camera ?? (_camera = this.GetComponent<Camera>()); }
            set { _camera = value; }
        }

        void Start()
        {
            //maintf=Camera.main.GetComponent<Transform>();
            all = gameObject.GetChild("All").GetComponent<Camera>();
            background = gameObject.GetChild("BackGround").GetComponent<Camera>();
            frontCamera = gameObject.GetChild("Front").GetComponent<Camera>();
        }

        public Vector3 Lerp(Vector3 src, Vector3 dest)
        {
            float distance = (dest - src).magnitude / smoothing;
            return Vector3.Lerp(src, dest, distance * Time.deltaTime);
        }

        int curIndex = 0;
        // Update is called once per frame
        void LateUpdate()
        {
            if (Player1 == null)
            {
                GameObject player = GameObject.Find("Player1");
                if (player)
                {
                    Player1 = player.transform;
                }
            }

            if (Player2 == null)
            {
                GameObject player = GameObject.Find("Player2");
                if (player)
                {
                    Player2 = player.transform;
                }
            }

            if (Player1)
            {
                Vector3 pos = Player1.position;
                if (Player2 && Player2.gameObject.activeInHierarchy)
                {
                    pos = (pos + Player2.position) / 2;
                }

                if ((transform.position - pos).magnitude < 10)
                {
                    transform.position = Vector3.Lerp(transform.position, pos, smoothing * Time.deltaTime) + shakeOffset;
                }
                else
                {
                    transform.position = pos;
                }

                //Vector3 srcPos = Camera.WorldToScreenPoint(transform.position);
                //Vector3 destPos = Camera.WorldToScreenPoint(Player1.position);


                //if (destPos.x > boxXY2.y)
                //{
                //    Vector3 destPos_ = srcPos + destPos - new Vector3(boxXY2.y, destPos.y, destPos.z);
                //    transform.position = Camera.ScreenToWorldPoint(Lerp(srcPos, destPos_));
                //}

                //if (destPos.x < boxXY2.x)
                //{
                //    Vector3 destPos_ = srcPos + destPos - new Vector3(boxXY2.x, destPos.y, destPos.z);
                //    transform.position = Camera.ScreenToWorldPoint(Lerp(srcPos, destPos_));
                //}

                //if (destPos.y > boxXY2.w)
                //{
                //    Vector3 destPos_ = srcPos + destPos - new Vector3(destPos.x, boxXY2.w, destPos.z);
                //    transform.position = Camera.ScreenToWorldPoint(Lerp(srcPos, destPos_));
                //}

                //if (destPos.y < boxXY2.z)
                //{
                //    Vector3 destPos_ = srcPos + destPos - new Vector3(destPos.x, boxXY2.z, destPos.z);
                //    transform.position = Camera.ScreenToWorldPoint(Lerp(srcPos, destPos_));
                //}





                //if (NeedMove())
                //{
                //Vector3 sPos = transform.position;
                //Vector3 dPos = CalcGoalPositon(target.position + new Vector3(0, 1, 0));

                //Vector3 toDestPos = dPos - sPos;

                //var distance = Vector3.Distance(dPos, sPos);

                //Vector3 viewPos = Camera.WorldToViewportPoint(target.transform.position);
                //Vector3 screenPos = Camera.WorldToScreenPoint(target.transform.position);

                //// 向右移动 add by TangJian 2018/12/5 18:04
                //if (viewPos.x > boxXY.y && toDestPos.x > 0)
                //{
                //    Vector3 dPos_ = dPos;
                //    Vector3 screenP = Camera.ViewportToWorldPoint(new Vector3(boxXY.y, 0, 0));
                //    dPos_.x = screenP.x;

                //    Vector3 newPos = Vector3.Lerp(transform.position, dPos_, smoothing * Time.deltaTime) + shakeOffset;
                //    transform.position = new Vector3(newPos.x, transform.position.y, transform.position.z);
                //}

                //if (viewPos.x < boxXY.x && toDestPos.x < 0)
                //{
                //    transform.position = Vector3.Lerp(sPos, dPos, smoothing * Time.deltaTime) + shakeOffset;
                //}

                //if (viewPos.y > boxXY.w && toDestPos.y > 0)
                //{
                //    transform.position = Vector3.Lerp(sPos, dPos, smoothing * Time.deltaTime) + shakeOffset;
                //}

                //if (viewPos.y < boxXY.z && toDestPos.y < 0)
                //{
                //    transform.position = Vector3.Lerp(sPos, dPos, smoothing * Time.deltaTime) + shakeOffset;
                //}

                //if (distance >= 100)
                //{
                //    transform.position = dPos + shakeOffset;
                //}
            }

            if (mode == "move")
            {
                all.transform.localPosition = Vector3.zero;
                background.transform.localPosition = Vector3.zero;
                frontCamera.transform.localPosition = Vector3.zero;
            }
        }

        //bool NeedMove()
        //{


        //    //Debug.Log("viewPos = " + viewPos);

        //    return viewPos.x < boxXY.x || viewPos.x > boxXY.y || viewPos.y < boxXY.z || viewPos.y > boxXY.w;
        //}



        //将相机x,y,z位移 转换为 只有x,y方向的位移 (保证人物在相机中间)
        Vector3 CalcGoalPositon(Vector3 targetPos)
        {

            Vector3 goalPoint = targetPos + offset;
            // point.x = Mathf.Clamp(point.x, min.x, max.x);
            // point.y = Mathf.Clamp(point.y, min.y, max.y);
            // point.z = Mathf.Clamp(point.z, min.z, max.z);
            Matrix4x4 m = this.Camera.worldToCameraMatrix;
            float y = (m.m00 * m.m12 - m.m02 * m.m10) * goalPoint.z / (m.m00 * m.m11 - m.m01 * m.m10);
            goalPoint.y = y + goalPoint.y;
            goalPoint.x = (m.m02 * goalPoint.z - m.m01 * y) / m.m00 + goalPoint.x;
            goalPoint.z = 0;

            // 限制 add by TangJian 2018/02/02 15:13:33
            // if (cameraLimit != null)
            // {
            //     goalPoint.x = goalPoint.x.Range(cameraLimit.minX, cameraLimit.maxX);
            //     goalPoint.y = goalPoint.y.Range(cameraLimit.minY, cameraLimit.maxY);
            // }

            return goalPoint;
        }

        // 震动 add by TangJian 2017/11/28 16:40:16
        public void Shake(float duration, float strength = 1, int vibrato = 10, float randomness = 90, bool snapping = false, bool fadeOut = true)
        {
            mode = "shake";

            all.transform.DOShakePosition(duration, strength, vibrato, randomness, snapping, fadeOut).OnComplete(() =>
            {
                mode = "move";
            });

            frontCamera.transform.DOShakePosition(duration, strength, vibrato, randomness, snapping, fadeOut).OnComplete(() =>
            {
                mode = "move";
            });

            background.transform.DOShakePosition(duration, strength, vibrato, randomness, snapping, fadeOut).OnComplete(() =>
            {
                mode = "move";
            });

        }
        public void ShakeY(float duration, float strength = 1, int vibrato = 10, float randomness = 90, bool snapping = false, bool fadeOut = true)
        {
            mode = "shake";

            all.transform.DOShakePosition(duration, new Vector3(0, strength, 0), vibrato, randomness, snapping, fadeOut).OnComplete(() =>
              {
                  mode = "move";
              });

            frontCamera.transform.DOShakePosition(duration, new Vector3(0, strength, 0), vibrato, randomness, snapping, fadeOut).OnComplete(() =>
            {
                mode = "move";
            });

            background.transform.DOShakePosition(duration, new Vector3(0, strength, 0), vibrato, randomness, snapping, fadeOut).OnComplete(() =>
            {
                mode = "move";
            });
        }
        public void ShakeX(float duration, float strength = 1, int vibrato = 10, float randomness = 90, bool snapping = false, bool fadeOut = true)
        {
            mode = "shake";

            all.transform.DOShakePosition(duration, new Vector3(strength, 0, 0), vibrato, randomness, snapping, fadeOut).OnComplete(() =>
              {
                  mode = "move";
              });

            frontCamera.transform.DOShakePosition(duration, new Vector3(strength, 0, 0), vibrato, randomness, snapping, fadeOut).OnComplete(() =>
            {
                mode = "move";
            });

            background.transform.DOShakePosition(duration, new Vector3(strength, 0, 0), vibrato, randomness, snapping, fadeOut).OnComplete(() =>
            {
                mode = "move";
            });
        }
        public void ShakeV3(float duration, Vector3 strength = new Vector3(), int vibrato = 10, float randomness = 90, bool snapping = false, bool fadeOut = true)
        {
            mode = "shake";

            all.transform.DOShakePosition(duration, strength, vibrato, randomness, snapping, fadeOut).OnComplete(() =>
           {
               mode = "move";
           });

            frontCamera.transform.DOShakePosition(duration, strength, vibrato, randomness, snapping, fadeOut).OnComplete(() =>
            {
                mode = "move";
            });

            background.transform.DOShakePosition(duration, strength, vibrato, randomness, snapping, fadeOut).OnComplete(() =>
            {
                mode = "move";
            });
        }
        public void ScaleDown(float size)
        {
            AddSize(size);
        }

        public void ScaleUp(float size)
        {
            AddSize(-size);
        }

        void SetSize(float value, float time = 0.05f)
        {
            value = value.Range(7, 9);

            all.Doscalesize(value, time);
            background.Doscalesize(value, time);
            frontCamera.Doscalesize(value, time);

            // delayFunc("SetSize", () =>
            // {
            //     SetSizeToDefault();
            // }, 10);
        }

        void AddSize(float value)
        {
            SetSize(all.orthographicSize + value, 0.1f);
        }

        public void SetSizeToDefault()
        {
            SetSize(9, 0.5f);
        }

        public void Scalesize(float endV, float erwr, float gewa)
        {
            all.Doscalesize(gewa, endV).OnComplete(() =>
             {
                 all.Doscalesize(9f, erwr);
             }
            );
            background.Doscalesize(gewa, endV).OnComplete(() =>
              {
                  background.Doscalesize(9f, erwr);
              }
             );

            frontCamera.Doscalesize(gewa, endV).OnComplete(() =>
            {
                background.Doscalesize(9f, erwr);
            }
             );
        }
    }
}