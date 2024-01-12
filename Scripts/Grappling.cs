using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    private float maxGrappleDistance = 100f;
    private SpringJoint joint;
    private float distanceFromPoint;

    public LayerMask whatIsGrappleable;
    public Transform gunTip, camera, player;

    [Header("Grapple Joint Values")]
    public float Spring, Dampen, massScale;

    [Header("Grapple Settings")]
    public float maxPullSpeed,minPullSpeed;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (joint)
            {
                Destroy(joint);
                DrawRope();
            }
            StartGrapple();
        }else if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space))
            StopGrapple();
    }

    void FixedUpdate()
    {
        if(joint && Input.GetKey(KeyCode.LeftControl))
        {
            joint.maxDistance = distanceFromPoint * maxPullSpeed;
            joint.minDistance = distanceFromPoint * minPullSpeed;
        }
    }

    void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.25f;
            joint.minDistance = distanceFromPoint * 0.8f;

            joint.spring = Spring; // more push and pull
            joint.damper = Dampen;
            joint.massScale = massScale;

            lr.positionCount = 2;
        }

    }

    void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(joint);
    }

    void DrawRope()
    {
        //if not grappling, dont draw rope
        if (!joint)
        {
            lr.positionCount = 0;
            return;
        }

        lr.SetPosition(0,gunTip.position);
        lr.SetPosition(1, grapplePoint);
    }
}
