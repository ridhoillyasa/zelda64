using UnityEngine;
using System.Collections;
using System;

public class LinkCharacterController : MonoBehaviour
{

    #region Constants

    private string SPEED_NAME = "Speed";

    #endregion

    #region Variables

    private Animator m_animator;
    private CapsuleCollider m_collider;
    private float m_prevSpeed;

    #endregion

    #region Properties

    public float DeadZone = 0.05f;
    public ThirdPersonCamera ThirdPersonCamera = null;
    public float SpeedLerpT = 2;

    #endregion

    #region Methods

    private float ApplyDeadZone(float stickValue, float deadZone)
    {
        float appliedStickValue = stickValue;

        if (Mathf.Abs(appliedStickValue) <= deadZone)
        {
            appliedStickValue = 0;
        }

        return appliedStickValue;
    }

    private float ComputePythagoreanTheorem(float a, float b)
    {
        return Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2));
    }

    private float GetAngle(float horizontalAxis, float verticalAxis)
    {
        // Get Camera Angle
        float cameraAngle = this.ThirdPersonCamera.ParentRig.rotation.eulerAngles.y;
        // Get Character Angle
        float characterAngle = this.transform.rotation.eulerAngles.y;

        float relativeTransformAngle = characterAngle;// - cameraAngle;

        float rotateByAngle = 0;
        float targetAngle = 0;

        float hypotenuse = this.Normalize(this.ComputePythagoreanTheorem(horizontalAxis, verticalAxis));

        if (hypotenuse != 0)
        {
            float sine = horizontalAxis / hypotenuse;

            targetAngle = Mathf.Asin(sine) * Mathf.Rad2Deg + 90;

            if (verticalAxis < 0)
            {
                targetAngle *= -1;
            }

            if (targetAngle < 0)
            {
                targetAngle += 360;
            }
            else if (targetAngle >= 360)
            {
                targetAngle -= 360;
            }

            targetAngle = Mathf.Clamp(targetAngle, 0, 360);

            rotateByAngle = targetAngle - relativeTransformAngle;
        }

        return rotateByAngle;
    }

    private float GetSpeed(float horizontalAxis, float verticalAxis)
    {
        float speed = Mathf.Abs(Mathf.Sqrt(Mathf.Pow(horizontalAxis, 2) + Mathf.Pow(verticalAxis, 2)));

        speed = Mathf.Lerp(this.m_prevSpeed, speed, this.SpeedLerpT);

        this.m_prevSpeed = speed;

        return speed;
    }

    private float Normalize(float value)
    {
        return Mathf.Clamp(value, 0, 1);
    }

    // Use this for initialization
    void Start()
    {
        this.m_animator = GetComponent<Animator>();
        this.m_collider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.m_animator != null && this.m_collider != null)
        {
            float deadZone = this.DeadZone; // Defaults to .05

            float horizontalAxis = this.ApplyDeadZone(Input.GetAxis("Horizontal"), deadZone);
            float verticalAxis = this.ApplyDeadZone(Input.GetAxis("Vertical"), deadZone);

            float angle = this.GetAngle(horizontalAxis, verticalAxis);
            float speed = this.GetSpeed(horizontalAxis, verticalAxis);

            this.transform.Rotate(new Vector3(0, angle, 0));
            this.m_animator.SetFloat(SPEED_NAME, speed);
        }
    }

    #endregion

}
