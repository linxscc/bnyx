
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SpriteFrameAnim : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public List<Sprite> spriteFrameList;

    public float interval = 0.1f;
    public bool loop = false;

    IEnumerator ienumeratorPlayFrame;

    private void OnEnable()
    {
        ienumeratorPlayFrame = PlayFrame();
    }

    private void Update()
    {
        if (ienumeratorPlayFrame.MoveNext())
        {

        }
    }

    IEnumerator PlayFrame()
    {
    PlayFrameBegin:

        if (spriteFrameList != null && spriteFrameList.Count > 0)
        {
            for (int i = 0; i < spriteFrameList.Count; i++)
            {
                spriteRenderer.sprite = spriteFrameList[i];

                float currTime = Time.time;
                while (Time.time - currTime <= interval)
                {
                    yield return 0;
                }
            }
        }

        if (loop)
        {
            goto PlayFrameBegin;
        }
    }
}