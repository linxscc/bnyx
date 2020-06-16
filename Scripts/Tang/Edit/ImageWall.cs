








using UnityEngine;


public class ImageWall : MonoBehaviour
{
    [SerializeField] float rawWidth;
    [SerializeField] float rawHeight;

    public float width;
    public float height;

    public Vector2 offsetScale = new Vector2(0.3f, 0.3f);

    Material material;

    Transform lookAtTransform;
    Transform LookAtTransform
    {
        get
        {
            if (lookAtTransform == null)
            {
                lookAtTransform = Camera.main.transform;
            }
            return lookAtTransform;
        }
    }

    Vector2 uvOffset;
    Vector2 UVOffset
    {
        get
        {
            uvOffset.x = (transform.position.x - LookAtTransform.position.x) / rawWidth;


            uvOffset.y = 0;//LookAtTransform.position.y - transform.position.y / rawHeight;

            uvOffset.x *= offsetScale.x;
            uvOffset.y *= offsetScale.y;
            return uvOffset;
        }
    }

    void Start()
    {
        Init();
    }

    public void Init()
    {
        material = gameObject.GetComponent<MeshRenderer>().sharedMaterial;

        // rawWidth = material.mainTexture.width / 100;
        // rawHeight = material.mainTexture.height / 100;

        // if (width <= 0)
        // {
        //     width = rawWidth;
        // }

        // if (height <= 0)
        // {
        //     height = rawHeight;
        // }

        // UpdateScale();
    }

    public void Reset()
    {
        material = gameObject.GetComponent<MeshRenderer>().sharedMaterial;

        rawWidth = material.mainTexture.width / 100;
        rawHeight = material.mainTexture.height / 100;
    }

    public void UpdateScale()
    {
        transform.localScale = new Vector3(width, height, 1);
    }

    public void Update()
    {
        UpdateScale();

        material.SetFloat("rawWidth", rawWidth);
        material.SetFloat("rawHeight", rawHeight);

        material.SetFloat("width", width);
        material.SetFloat("height", height);

        material.SetFloat("uvOffsetX", UVOffset.x);
        material.SetFloat("uvOffsetY", UVOffset.y);

        // if (Application.isPlaying)
        // {
        //     lookTransform = Camera.main.transform;// GameObject.Find("Player1").transform;
        // }

        // float minX = (0.5f - viewPortSize / 2.0f);
        // float maxX = (0.5f + viewPortSize / 2.0f);

        // float minY = (0.5f - viewPortSize / 2.0f);
        // float maxY = (0.5f + viewPortSize / 2.0f);


        // var width = (wallHeight * rawWidth / rawHeight);
        // var height = wallHeight;
        // transform.localScale = new Vector3(width - (minX - (1 - maxX)) * width, height - (minY - (1 - maxY)) * height, 1);
        // // transform.localScale = new Vector3(visblePortX.y - visblePortX.x, visblePortY.y - visblePortY.x, 1);

        // Vector2 offset = Vector2.zero;
        // if (lookTransform != null)
        // {
        //     offset = transform.position - lookTransform.position;
        // }
        // offset += lookAtPosOffset;
        // Vector2 offsetMax = new Vector2(factorX, factorY);

        // offset.x = offset.x.Range(-offsetMax.x, offsetMax.x);
        // offset.y = offset.y.Range(-offsetMax.y, offsetMax.y);



        // // float factor = (Time.time / 100f) % 1;



        // material.SetFloat("uvxmin", minX + (offset.x / factorX) * minX);
        // material.SetFloat("uvxmax", maxX + (offset.x / factorX) * minX);

        // material.SetFloat("uvymin", minY + (offset.y / factorY) * minY);
        // material.SetFloat("uvymax", maxY + (offset.y / factorY) * minY);
    }
}