using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallHandler : MonoBehaviour
{
    [SerializeField] GameObject Ball;
    [SerializeField] Rigidbody2D PivotPoint;
    [SerializeField] private float delayDuration = 0.1f;
    [SerializeField] private float respawnTime = 1f;
    
    
    private Rigidbody2D CurrentBallPosition;
    private SpringJoint2D _springJoint2D;
    private bool isDragging;
    private Camera mainCamera;
    
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        SpawnBall();
    }

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentBallPosition == null)
        {
            return;
        }


        if (Touch.activeTouches.Count == 0)
        {
            if (isDragging)
            {
                LaunchBall();
            }

            isDragging = false;
            return;
        }

        isDragging = true;
        CurrentBallPosition.isKinematic = true;

        Vector2 touchscreen = new Vector2();

        foreach (Touch touch in Touch.activeTouches)
        {
            touchscreen += touch.screenPosition;
        }

        touchscreen /= Touch.activeTouches.Count;
        Vector3 gameCoordinate = mainCamera.ScreenToWorldPoint(touchscreen);
        CurrentBallPosition.position = gameCoordinate;
    }
    
    private void LaunchBall()
    {
        CurrentBallPosition.isKinematic = false;
        CurrentBallPosition = null;
        
        Invoke("DetachBall",delayDuration);
        //Invoke(nameof(DetachBall),delayDuration); both of them acceptable.
    }

    private void DetachBall()
    {
        _springJoint2D.enabled = false;
        _springJoint2D = null;
        
        Invoke(nameof(SpawnBall),respawnTime);
    }

    private void SpawnBall()
    {
        GameObject ballInstance = Instantiate(Ball, PivotPoint.position, Quaternion.identity);

        CurrentBallPosition = ballInstance.GetComponent<Rigidbody2D>();
        _springJoint2D = ballInstance.GetComponent<SpringJoint2D>();

        _springJoint2D.connectedBody = PivotPoint;
    }
}
