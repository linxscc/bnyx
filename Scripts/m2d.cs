using UnityEngine;

#pragma warning disable 0414
public class m2d : MonoBehaviour
{
	private Vector3 lastPosition;
	private Vector3 offset;
    CharacterController controller;

    [Header("Moving")]
    public float walkSpeed = 4;
    public float gravity = 65;
    public Vector3 velocity = Vector3.zero;

    Quaternion flippedRotation = Quaternion.Euler(0, 180, 0);

    public MeshRenderer myMeshRenderer;
    // Use this for initialization
    void Start()
	{
        //myMeshRenderer = this.GetComponent<MeshRenderer>();
        //myMeshRenderer.sortingLayerName = "player";
        //myMeshRenderer.sortingOrder = 0;
        controller = this.GetComponent<CharacterController>();
        lastPosition = transform.position;
		//offset = this.transform.position - Camera.main.transform.position;
	}
	
	// Update is called once per frame
	void Update()
	{
		Vector3 curPosition = transform.position;
		// Camera.main.transform.Translate( curPosition - lastPosition );
		lastPosition = curPosition;

		float x = Input.GetAxis("Horizontal");
		float z = Input.GetAxis("Vertical");
        // float y = Input.GetAxis("Jump");

        if (x != 0)
        {
            Debug.Log("x = " + x);
            velocity.x = walkSpeed * Mathf.Sign(x);

            //更改方向
            //Vector3 lastScale = transform.localScale;
            //lastScale.x *= Mathf.Sign(x);
            //transform.localScale = lastScale;
            if (x > 0)
                transform.localRotation = Quaternion.identity;
            else if (x < 0)
                transform.localRotation = flippedRotation;
        }
        else
        {
            velocity.x = 0;
        }

        if (z!=0)
        {
            
            
            Debug.Log("z = " + z);
            velocity.z = walkSpeed * Mathf.Sign(z);
        }
        else
        {
            velocity.z = 0;
        }
        
        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        if (controller.isGrounded)
        {
            //cancel out Y velocity if on ground
            velocity.y = -gravity * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.J))
		{
			//transform.Translate( x * 0.01f , 0 , y * 0.01f );

			//this.GetComponent<CharacterController>().AddForce( 0 , 270 , 0 );

        }
	}
}
