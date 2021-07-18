using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grapple : MonoBehaviour
{
    public Camera cam;
    PlayerControls playerControls;


    public float grappleSpeed = 10;
    public float grappleStrengh = 10;
    public float grappleDistance = 10;

    private Vector3 grapplePos;
    private bool grappling = false;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    public void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
        }

        playerControls.Enable();
    }

    public void OnDisable()
    {
        playerControls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, cam.transform.forward, out hit, grappleDistance))
        {

        }

        if (playerControls.Gameplay.Action.triggered)
        {
            if(hit.transform != null)
            {
                grapplePos = hit.point;
                grappling = true;
                rb.useGravity = false;
            }
        }

        if (grappling)
        {
            Vector3 moveVec = (grapplePos - transform.position).normalized;
            moveVec += cam.transform.forward * grappleSpeed;
            rb.AddForce(moveVec * grappleStrengh * Time.deltaTime, ForceMode.VelocityChange);

            if(Vector3.Distance(transform.position, grapplePos)> grappleDistance)
            {
                StopGrappling();
            }
        }
    }


    private void StopGrappling()
    {
        grappling = false;
        rb.useGravity = true;
    }
}
