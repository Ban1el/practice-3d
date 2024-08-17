using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode holdobjectKey = KeyCode.Mouse0;

    [Header("Movement")]
    #region Movement
    public float movementSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;
    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Rigidbody rb;
    #endregion

    [Header("Hold Object")]
    private RaycastHit hit;
    [SerializeField] Transform holdArea;
    private GameObject heldObj;
    private Rigidbody heldObjRB;
    [SerializeField] private Transform cameraDirection;
    private RaycastHit objectHit;

    [SerializeField] private LayerMask objectLayer;

    [Header("Physics Parameters")]
    [SerializeField] private float objectHoldRange = 5.0f;
    [SerializeField] private float holdupForce = 150.0f;

    private void Start()
    {
        readyToJump = true;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Actions.OnEnableUI?.Invoke(UserInterface.CrossHair); 
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        MyInput();
        SpeedControl();

        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        HoldObject();
        SetCursor();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void SetCursor()
    {
        if (InteractableDetected())
        {
            Actions.OnEnableUI(UserInterface.ItemPickIcon);
            Actions.OnDisableUI(UserInterface.CrossHair);
        }
        else
        {
            Actions.OnEnableUI(UserInterface.CrossHair);
            Actions.OnDisableUI(UserInterface.ItemPickIcon);
        }
    }

    private bool InteractableDetected()
    {
        return Physics.Raycast(cameraDirection.position, cameraDirection.forward, out hit, objectHoldRange, objectLayer); 
    }

    #region Hold Object
    private void HoldObject()
    {
        if (Input.GetKeyDown(holdobjectKey))
        {
            if (heldObj == null)
            {
                if (InteractableDetected())
                {
                    PickupObject(hit.transform.gameObject);
                }
            }
            else
            {
                DropObject();
            }
        }

        if (heldObj != null)
        {
            MoveObject();
        }
    }

    private void MoveObject()
    {
        if (Vector3.Distance(heldObj.transform.position, holdArea.position) > 0.1f)
        {
            Vector3 moveDirection = (holdArea.position - heldObj.transform.position);
            heldObjRB.AddForce(moveDirection * holdupForce);
        }
    }

    private void PickupObject(GameObject pickObj)
    {
        if (pickObj.GetComponent<Rigidbody>())
        {
            heldObjRB = pickObj.GetComponent<Rigidbody>();
            heldObjRB.useGravity = false;
            heldObjRB.drag = 10;
            heldObjRB.constraints = RigidbodyConstraints.FreezeRotation;

            heldObjRB.transform.parent = holdArea;
            heldObj = pickObj;
        }
    }

    private void DropObject()
    {
        heldObjRB.useGravity = true;
        heldObjRB.drag = 1;
        heldObjRB.constraints = RigidbodyConstraints.None;

        heldObjRB.transform.parent = null;
        heldObj = null;
    }
    #endregion

    #region Movement
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded)
            rb.AddForce(moveDirection.normalized * movementSpeed * 10f, ForceMode.Force);
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * movementSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > movementSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * movementSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
    #endregion

    #region Hold Up Object

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 objectHoldDirection = cameraDirection.forward * objectHoldRange;
        Gizmos.DrawRay(cameraDirection.position, objectHoldDirection);
    }
}
