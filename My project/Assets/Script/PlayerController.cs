using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float moveForce = 10f;
    public float jumpImpulse = 7f;
    public GameObject powerUp;
    public TMP_Text powerUpText;
    public float poweredMoveForce = 20f;
    public TMP_Text winText;

    private Rigidbody rb;
    private Vector3 movement;
    private bool gameStarted = false;
    private bool isGrounded = false;
    private bool jumpRequested = false;
    private int remainingPickups;
    private bool powerUpActive = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        remainingPickups = GameObject.FindGameObjectsWithTag("Pickup").Length;

        powerUp.SetActive(false);
        powerUpText.gameObject.SetActive(false);

        winText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!gameStarted) return;

        movement = Vector3.zero;

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            movement.z += 1f;

        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            movement.z -= 1f;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            movement.x -= 1f;

        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            movement.x += 1f;

        if (keyboard.spaceKey.wasPressedThisFrame && isGrounded)
        {
            jumpRequested = true;
        }

        movement = movement.normalized;
    }

    void FixedUpdate()
    {
        if (!gameStarted) return;

        rb.AddForce(movement * moveForce);

        if (jumpRequested)
        {
            jumpRequested = false;

            if (isGrounded)
            {
                rb.AddForce(Vector3.up * jumpImpulse, ForceMode.Impulse);
                isGrounded = false;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("BreakableWall")
            && powerUpActive)
        {
            collision.gameObject.SetActive(false);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public void StartGame()
    {
        gameStarted = true;
        rb.useGravity = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            other.gameObject.SetActive(false);
            remainingPickups--;

            if (remainingPickups == 0)
            {
                powerUp.SetActive(true);
            }
        }
        else if (other.CompareTag("PowerUp") && !powerUpActive)
        {
            other.gameObject.SetActive(false);

            powerUpActive = true;
            moveForce = poweredMoveForce;

            GetComponent<Renderer>().material.color = Color.yellow;
            powerUpText.gameObject.SetActive(true);
        }
        else if (other.CompareTag("Goal"))
        {
            winText.gameObject.SetActive(true);
            powerUpText.gameObject.SetActive(false);

            gameObject.SetActive(false);
        }
    }
}
