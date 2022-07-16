using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float ThrusterPower = 1000;
    public float TurningTorque = 300;

    private Rigidbody2D body;

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
        CheckThrustersPosition();
        Move();
    }

    void OnDrawGizmos()
    {
        if (IsPressingLeft)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(leftThrusterPosition, 0.1f);
        }
        if (IsPressingRight)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(rightThrusterPosition, 0.1f);
        }
    }

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

    void Move()
    {
        if (IsPressingLeft)
        {
            body.AddTorque(-TurningTorque * Time.deltaTime);
        }
        if (IsPressingRight)
        {
            body.AddTorque(TurningTorque * Time.deltaTime);
        }
        if (IsPressingLeft && IsPressingRight)
        {
            body.AddRelativeForce(new Vector2(0, ThrusterPower * Time.deltaTime));
        }
    }
}
