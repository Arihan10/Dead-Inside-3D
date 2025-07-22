using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float sprintSpeed, walkSpeed, jumpUpForce, smoothTime, downForce = 40f;

    bool grounded;
    Vector3 smoothMoveVelocity, moveAmount;

    Rigidbody rb;

    PhotonView PV; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>(); 
    }


    // Update is called once per frame
    void Update()
    {
        if (PV && !PV.IsMine) return;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Move();
        Jump();
    }

    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpUpForce);
        }
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    public bool GetGroundedState()
    {
        return grounded;
    }

    private void FixedUpdate()
    {
        if (PV && !PV.IsMine) return;

        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount * Time.fixedDeltaTime));

        if (!grounded)
        {
            rb.AddForce(Vector3.down * downForce);
        }
    }
}
