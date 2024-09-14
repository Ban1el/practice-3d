using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode holdobjectKey = KeyCode.Mouse0;
    public KeyCode inventoryKey = KeyCode.E;

    [Header("Movement")]
    #region Movement
    [SerializeField] private bool canMove = true;
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

    #region Hold Object
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
    #endregion

    #region Interact Door
    [SerializeField] Camera cam;
    Transform selectedDoor;
    GameObject dragPointGameobject;
    int leftDoor = 0;
    [SerializeField] LayerMask doorLayer;
    [SerializeField] float doorMoveRange;
    #endregion

    private bool inventoryIsOpen = false;

    private void Start()
    {
        readyToJump = true;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Actions.OnEnableUI?.Invoke(UserInterface.CrossHair);
    }

    private void Update()
    {
        Inventory();
        
        if (canMove)
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

            InteractDoor();

            PickItem();
        }
    }

    private void FixedUpdate()
    {
        if(canMove)
            MovePlayer();
    }

    private void SetCursor()
    {
        if (InteractableDetected())
        {
            Actions.OnDisableUI(UserInterface.CrossHair);
        }
        else
        {
            Actions.OnEnableUI(UserInterface.CrossHair);
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
                    if (hit.collider.CompareTag("HoldableObject"))
                    {
                        PickupObject(hit.transform.gameObject);
                    }
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

    #region Pick Item
    private void PickItem()
    {
        if (Input.GetKeyDown(holdobjectKey))
        {
            if (InteractableDetected())
            {
                if (hit.collider.CompareTag("PickableObject"))
                {
                    hit.collider.GetComponent<WorldItem>().Pick();
                }
            }
        }
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

    #region Door
    private void InteractDoor()
    {
        //Raycast
        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, doorMoveRange, doorLayer))
        {
            if (Input.GetMouseButtonDown(0))
            {
                selectedDoor = hit.collider.gameObject.transform;
            }
        }

        if (selectedDoor != null)
        {
            HingeJoint joint = selectedDoor.GetComponent<HingeJoint>();
            JointMotor motor = joint.motor;

            //Create drag point object for reference where players mouse is pointing
            if (dragPointGameobject == null)
            {
                dragPointGameobject = new GameObject("Ray door");
                dragPointGameobject.transform.parent = selectedDoor;
            }

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            dragPointGameobject.transform.position = ray.GetPoint(Vector3.Distance(selectedDoor.position, transform.position));
            dragPointGameobject.transform.rotation = selectedDoor.rotation;


            float delta = Mathf.Pow(Vector3.Distance(dragPointGameobject.transform.position, selectedDoor.position), 3);

            //Deciding if it is left or right door
            if (selectedDoor.GetComponent<MeshRenderer>().localBounds.center.x > selectedDoor.localPosition.x)
            {
                leftDoor = 1;
            }
            else
            {
                leftDoor = -1;
            }

            //Applying velocity to door motor
            float speedMultiplier = 80000;

            Debug.Log("Selected door z: " + Mathf.Abs(selectedDoor.parent.forward.z) + " | 0.5f");

            if (Mathf.Abs(selectedDoor.parent.forward.z) > 0.5f)
            {
                Debug.Log("Drag Position x: " + dragPointGameobject.transform.position.x);
                Debug.Log("Selected door Position x: " + selectedDoor.position.x);
                if (dragPointGameobject.transform.position.x > selectedDoor.position.x)
                {
                    motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor;
                }
                else
                {
                    motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor;
                }
            }
            else
            {
                Debug.Log("Drag Position z: " + dragPointGameobject.transform.position.z);
                Debug.Log("Selected door Position z: " + selectedDoor.position.z);
                if (dragPointGameobject.transform.position.z > selectedDoor.position.z)
                {
                    motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor;
                }
                else
                {
                    motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor;
                }
            }
            joint.motor = motor;

            if (Input.GetMouseButtonUp(0))
            {
                selectedDoor = null;
                motor.targetVelocity = 0;
                joint.motor = motor;
                Destroy(dragPointGameobject);
            }
        }
    }
    #endregion

    #region Inventory
    private void Inventory()
    {
        if (Input.GetKeyDown(inventoryKey))
        {

            if (!inventoryIsOpen)
            {
                Actions.SetPlayerControl?.Invoke(false);
                Actions.OnDisableUI?.Invoke(UserInterface.CrossHair);
                Actions.OnEnableUI?.Invoke(UserInterface.ItemPickIcon);

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                inventoryIsOpen = true;
            }
            else
            {
                Actions.OnDisableUI?.Invoke(UserInterface.ItemPickIcon);
                Actions.OnEnableUI?.Invoke(UserInterface.CrossHair);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Actions.SetPlayerControl?.Invoke(true);

                inventoryIsOpen = false;
            }
        }
    }
    #endregion

    private void SetControl(bool _canMove)
    {
        canMove = _canMove;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 doorInteractDirection = cameraDirection.forward * doorMoveRange;
        Gizmos.DrawRay(cameraDirection.position, doorInteractDirection);
    }

    private void OnEnable()
    {
        Actions.SetPlayerControl += SetControl;
    }

    private void OnDisable()
    {
        Actions.SetPlayerControl -= SetControl;
    }
}
