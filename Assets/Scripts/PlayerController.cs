using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float laneDistance = 2.5f; //distance between two lanes
    [SerializeField] private float upForce;

    private int desiredLane = 0; // -1 = left , 0 = middle , 1 = right
    private Vector2 initToutchPosition;
    private Vector2 deltaPosition;
    private bool isSwipeValid = true;
    public bool isGrounded = true;

    private void FixedUpdate()
    {
        Vector3 forwardMove = transform.forward * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + forwardMove);

        if (Physics.Raycast(transform.position, Vector3.down, 0.5f))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            RightArrowMove();
        }

        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            LeftArrowMove();
        }

        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }

        TouchMove();
    }

    private void RightArrowMove()
    {
        desiredLane += 1;
        desiredLane = Mathf.Clamp(desiredLane, -1, 1);
        transform.position = new Vector3(desiredLane * laneDistance, transform.position.y, transform.position.z);
    }

    private void LeftArrowMove()
    {
        desiredLane -= 1;
        desiredLane = Mathf.Clamp(desiredLane, -1, 1);
        transform.position = new Vector3(desiredLane * laneDistance, transform.position.y, transform.position.z);
    }

    private void TouchMove()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                initToutchPosition = Input.GetTouch(0).position;
            }

            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                if (isSwipeValid)
                {
                    isSwipeValid = false;
                    deltaPosition = (Input.GetTouch(0).position - initToutchPosition) / (float)Screen.width;
                    if (deltaPosition.x < 0)
                    {
                        desiredLane -= 1;
                    }
                    else
                    {
                        desiredLane += 1;
                    }

                    desiredLane = Mathf.Clamp(desiredLane, -1, 1);

                    transform.position = new Vector3(desiredLane * laneDistance, transform.position.y, transform.position.z);
                }
            }

            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                isSwipeValid = true;
            }
        }
    }

    public void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(transform.up * upForce);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            PlayerManager.gameOver = true;
        }
    }
}