using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Windows;
public class PlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float force = 10f;
    public float jumpPower = 50f;

    [SerializeField] private float gravityMultiplier = 3.0f;
    private float gravity = -9.81f;

    private Rigidbody rb;
    private Vector2 movementVector;
    private int maximumJump = 1;
    private int remainingJump = 1;
    private Vector3 _direction;
    private float _velocity;
    public float maxDistance;

    [SerializeField] private float rotationSpeed = 500f;
    private Camera _mainCamera;

    public Transform groundCheckPos;
    public Vector3 groundCheckSize = new Vector3(0.5f, 0.05f, 0.05f);
    public LayerMask groundLayer;
    
    private CharacterController characterController;
    PlayerInput input;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        _mainCamera = Camera.main;
        //rb.linearDamping = 10;
        //rb.angularDamping = 0;

    }

    // Update is called once per frame
    void Update()
    {
        ApplyRotation();
        ApplyGravity();
    }

    private void FixedUpdate()
    {
        rb.AddForce(_direction * force, ForceMode.Acceleration);

    }

    public void Move(InputAction.CallbackContext context)
    {
        movementVector = context.ReadValue<Vector2>();
        _direction = new Vector3(movementVector.x, 0.0f, movementVector.y);
    }

    public void Jump(InputAction.CallbackContext context)
    {

        if (remainingJump > 0 && isGrounded())
        {
                rb.AddForce(0, jumpPower, 0);
                remainingJump--;
        }

    }

    private bool isGrounded()
    {
        if (Physics.Raycast(transform.position, Vector2.down, maxDistance, groundLayer))
        {
            Debug.DrawRay(transform.position, Vector2.down * maxDistance, Color.red);
            remainingJump = maximumJump;
            return true;
        }

        else return false;
    }
    
    private void ApplyGravity()
    {
        if (isGrounded() && _velocity < 0.0f)
        {
            _velocity = -1.0f;
        }
        else
        {
            _velocity += gravity * gravityMultiplier * Time.deltaTime;
        }

        _direction.y = _velocity;
    }
    private void ApplyRotation()
    {
        if (movementVector.sqrMagnitude == 0) return;

        _direction = Quaternion.Euler(0.0f, _mainCamera.transform.eulerAngles.y, 0.0f) * new Vector3(movementVector.x, 0.0f, movementVector.y);
        var targetRotation = Quaternion.LookRotation(_direction, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
        }
    }

}
