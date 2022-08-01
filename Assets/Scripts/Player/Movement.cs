using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float ThrusterPower = 1000;
    public float TurningAcceleration = 10;
    public float TurningSpeed = 180;
    public float TurningDrag = 30;

    private Rigidbody2D body;

    float currentTurningSpeed = 0;

    bool IsPressingLeft = false;
    bool IsPressingRight = false;
    Vector3 leftThrusterPosition;
    Vector3 rightThrusterPosition;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckKeyboard();
        Move();
        Turn();
    }

    // void OnDrawGizmos()
    // {
    //     CheckThrustersPosition();
    //     if (IsPressingLeft)
    //     {
    //         Gizmos.color = Color.green;
    //         Gizmos.DrawSphere(leftThrusterPosition, 0.1f);
    //     }
    //     if (IsPressingRight)
    //     {
    //         Gizmos.color = Color.green;
    //         Gizmos.DrawSphere(rightThrusterPosition, 0.1f);
    //     }
    // }

    void CheckKeyboard()
    {
        if (Input.GetKey(KeyCode.LeftShift))  
        {
            IsPressingLeft = true;
        }  
        else
        {
            IsPressingLeft = false;
        }

        if (Input.GetKey(KeyCode.RightShift))  
        {
            IsPressingRight = true;
        }   
        else
        {
            IsPressingRight = false;
        }
    }

    void CheckThrustersPosition()
    {
        leftThrusterPosition = transform.position + (transform.right * -0.5f) + (transform.up * -0.5f);
        rightThrusterPosition = transform.position + (transform.right * 0.5f) + (transform.up * -0.5f);
    }

    void Turn()
    {
        if (IsPressingLeft && !IsPressingRight)
        {
            currentTurningSpeed -= TurningAcceleration * Time.deltaTime;
        }
        else if (IsPressingRight && !IsPressingLeft)
        {
            currentTurningSpeed += TurningAcceleration * Time.deltaTime;
        }
        else
        {
            if (Math.Abs(currentTurningSpeed) - (TurningDrag * Time.deltaTime) > 0)
            {
                currentTurningSpeed -= TurningDrag * Math.Sign(currentTurningSpeed) * Time.deltaTime;
            }
            else
            {
                currentTurningSpeed = 0;
            }
        }
        if (Math.Abs(currentTurningSpeed) > TurningSpeed)
        {
            currentTurningSpeed = TurningSpeed * Math.Sign(currentTurningSpeed);
        }
        body.angularVelocity = currentTurningSpeed;
    }

    void Move()
    {
        if (IsPressingLeft && IsPressingRight)
        {
            body.AddRelativeForce(new Vector2(0, ThrusterPower * Time.deltaTime));
        }
    }
}
