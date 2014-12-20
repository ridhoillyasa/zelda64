using UnityEngine;
using System.Collections;
using System;

public class LinkCharacterController : MonoBehaviour
{
    private string SPEED_NAME = "Speed";

    private Animator m_animator;
    private CapsuleCollider m_collider;
    private float m_prevSpeed;

    public float AngleLerpT = 2;
    public float DeadZone = .05f;
    public ThirdPersonCamera ThirdPersonCamera = null;
    public float SpeedLerpT = 2;

    private float ApplyDeadZone(float stickValue, float deadZone)
    {
        float appliedStickValue = stickValue;

        if (Mathf.Abs(appliedStickValue) <= deadZone)
        {
            appliedStickValue = 0;
        }

        return appliedStickValue;
    }

    private float GetAngle(float horizontalAxis, float verticalAxis)
    {
        float cameraAngle = 0;
        Vector3 cameraAxis = Vector3.zero;
        this.ThirdPersonCamera.transform.rotation.ToAngleAxis(out cameraAngle, out cameraAxis);
        cameraAngle *= Mathf.Deg2Rad;

        float characterAngle = 0;
        Vector3 characterAxis = Vector3.zero;
        this.transform.rotation.ToAngleAxis(out characterAngle, out characterAxis);
        characterAngle *= Mathf.Deg2Rad;

        float transformAngle = characterAngle - cameraAngle;

        float adjustAngle = 0;
        float finishAngle = 0;

        float hypotenuse = Mathf.Sqrt(Mathf.Pow(horizontalAxis, 2) + Mathf.Pow(verticalAxis, 2));

        if (hypotenuse != 0)
        {
            float sine = horizontalAxis / hypotenuse;

            finishAngle = Mathf.Asin(sine) + cameraAngle - (Mathf.PI / 2);

            finishAngle = Mathf.Lerp(transformAngle, finishAngle, AngleLerpT);
            adjustAngle = finishAngle - transformAngle;

            if (verticalAxis < 0)
            {
                adjustAngle *= -1;
            }
        }

        return adjustAngle;
    }

    private float GetSpeed(float horizontalAxis, float verticalAxis)
    {
        float speed = Mathf.Abs(Mathf.Sqrt(Mathf.Pow(horizontalAxis, 2) + Mathf.Pow(verticalAxis, 2)));

        speed = Mathf.Lerp(this.m_prevSpeed, speed, this.SpeedLerpT);

        this.m_prevSpeed = speed;

        return speed;
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
            float deadZone = this.DeadZone;

            float horizontalAxis = this.ApplyDeadZone(Input.GetAxis("Horizontal"), deadZone);
            float verticalAxis = this.ApplyDeadZone(Input.GetAxis("Vertical"), deadZone);

            float angle = this.GetAngle(horizontalAxis, verticalAxis);
            float speed = this.GetSpeed(horizontalAxis, verticalAxis);

            this.transform.Rotate(new Vector3(0, angle * Mathf.Rad2Deg, 0));
            this.m_animator.SetFloat(SPEED_NAME, speed);
        }
    }
}
