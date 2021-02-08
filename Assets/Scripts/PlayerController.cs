﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class PlayerController : MonoBehaviour
{
    private Coroutine currentCorutine = null;
    public GameObject postProcess;
    private Volume volume;
    private Vignette vignette;
    private Bloom bloom;
    public JoyStick joystick;
    public float speed = 0, gravity = -9.8f, height, vy = 0;
    private float yeetSpeed = 1, fixedDeltaTime, jumpStartX = 0;
    public float vignetteIntensity;
    public bool jumping, kickOff;
    private Animator animator;
    private Rigidbody rb = null;

    // Start is called before the first frame update
    void Start()
    {
        volume = postProcess.GetComponent<Volume>();
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out bloom);
        rb = GetComponent<Rigidbody>();
        jumping = false;
        vignetteIntensity = 0;
        vy = gravity;
        animator = GetComponentInChildren<Animator>();
        this.fixedDeltaTime = Time.fixedDeltaTime;
        kickOff = false;
    }

    // Update is called once per frame
    void Update()
    {


        Vector2 input = joystick.getInputFromJoystick3(150);

        bool grounded = isGrounded(.01f);
        if (!grounded)
        {
            vy += gravity * Time.deltaTime;
        }
        else if(!jumping)
        {
            animator.SetBool("Jumping", false);
            yeetSpeed = 1;
            vy = 0;
        }
        if(vy < 0)
        {
            jumping = false;
        }


        float horizontal = !kickOff ? input.x * speed * yeetSpeed : jumpStartX;
        kickOff = false;
        float vertical = input.y * speed * yeetSpeed;

        if(Input.touchCount == 4)
        {
            this.transform.position = new Vector3(0, 10, this.transform.position.z);
            vy = 0;
        }

        Vector3 velocity = new Vector3(horizontal, vy, 0f);

        rb.velocity = velocity;
    }

    private bool isGrounded(float distToGround)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, distToGround*2))
        {
            if(hit.distance <= distToGround)
            {
                return true;
            }
        }
        return false;
    }
    public void jumpStart()
    {
        if (!jumping && vy >= 0)
        {
            jumpStartX = rb.velocity.x;
            yeetSpeed = 0f;
            jumping = true;
            animator.SetBool("Jumping", true);
        }
    }
    public void jump()
    {
        kickOff = true;
        yeetSpeed = 1f;
        vy = Mathf.Sqrt(2 * -gravity * height);
    }



    public void slowTime(float scale)
    {
        if(currentCorutine != null)
        {
            StopCoroutine(currentCorutine);
        }
        currentCorutine = StartCoroutine(SmoothVignette(vignetteIntensity, .45f, 0.2f*scale));
        Debug.Log("DOWN");
        Time.timeScale = scale;
        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
    }
    public void resetTime()
    {
        if (currentCorutine != null)
        {
            StopCoroutine(currentCorutine);
        }
        currentCorutine = StartCoroutine(SmoothVignette(vignetteIntensity, 0f, 0.2f));
        Debug.Log("UP");
        Time.timeScale = 1;
        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
    }
    IEnumerator SmoothVignette(float v_start, float v_end, float duration)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            vignetteIntensity = Mathf.Lerp(v_start, v_end, elapsed / duration);
            vignette.intensity.value = vignetteIntensity;
            bloom.intensity.value = vignetteIntensity * (3f / .45f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        vignetteIntensity = v_end;
        vignette.intensity.value = vignetteIntensity;
        currentCorutine = null;
    }

    public void setSpeed(System.Single s)
    {
        speed = s;
    }

}
