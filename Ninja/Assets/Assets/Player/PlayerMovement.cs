using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private const float JUMP_TIME_THRESHOLD = 0.1f;
    private const float SWIPE_TIME_THRESHOLD = 0.3f;
    private const float SWIPE_DIST_THRESHOLD = 0.25f; //25% from screen width 
    private const float SWIPE_DURATION_SEC = 0.3f; //0.3sec

    //Swipe attrs
    private Vector2 _fingerDown;
    private DateTime _fingerDownTime;
    private Vector2 _fingerUp;
    private DateTime _fingerUpTime;
    private bool _isMoved;


    [SerializeField] private float jumpHeight = 0.2f;
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private CharacterPosition characterPosition;
    private Animator animator;
    private Rigidbody2D rigidBody;
    private int JumpingHash;
    private void Awake()
    {
        characterPosition = CharacterPosition.RightWall;
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        JumpingHash = Animator.StringToHash("Jumping");
    }



    private void Update()
    {
        HandleMovements();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Contains("Wall"))  // or if(gameObject.CompareTag("YourWallTag"))
        {
            LeanTween.cancel(gameObject);
            characterPosition = transform.position.x < 0 ?
                            CharacterPosition.LeftWall : CharacterPosition.RightWall;
        }
    }

    private void HandleMovements()
    {
        foreach (var touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                _fingerDown = touch.position;
                _fingerDownTime = DateTime.Now;
                _isMoved = false;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                _fingerUp = touch.position;
                _isMoved = true;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                _fingerUp = touch.position;
                _fingerUpTime = DateTime.Now;
                DecideMovement();
            }
        }
    }

    private void DecideMovement()
    {
        var duration = (float)_fingerUpTime.Subtract(_fingerDownTime).TotalSeconds;
        var dirVector = _fingerDown - _fingerUp;
        if (duration > SWIPE_TIME_THRESHOLD) return;
        var direction = dirVector.x > 0 ? MoveDirection.Left : MoveDirection.Right;

        // Jump check
        if (duration < JUMP_TIME_THRESHOLD && !_isMoved)
        {
            DoJump();
        }
        else if (Mathf.Abs(dirVector.x) >= Screen.width * SWIPE_DIST_THRESHOLD) // Swipe
        {
            DoSwipe(direction);
        }

    }


    private void DoSwipe(MoveDirection direction)
    {
        if (characterPosition == CharacterPosition.Center)
            return;
        if ((int)direction == (int)characterPosition)
            return;
        var goTo = direction == MoveDirection.Left ? WallsController.LeftWallX : WallsController.RightWallX;
        characterPosition = CharacterPosition.Center;
        LeanTween.moveX(gameObject, goTo, SWIPE_DURATION_SEC)
                .setOnComplete(() =>
                {
                    animator.SetBool(JumpingHash, false);
                    characterPosition = direction == MoveDirection.Left ?
                            CharacterPosition.LeftWall : CharacterPosition.RightWall;
                }).setOnStart(() =>
                {
                    transform.Rotate(180, 0, 0);
                });
    }

    private void DoJump()
    {
        if (characterPosition != CharacterPosition.Center)
        {
            var previousCharacterPosition = characterPosition;
            characterPosition = CharacterPosition.Center;
            var directionScalar = characterPosition == CharacterPosition.LeftWall ? -1 : 1;
            animator.SetBool(JumpingHash, true);
            LeanTween.moveX(gameObject, directionScalar * jumpHeight, 0.7f)
                .setLoopPingPong(1)
                .setOnComplete(() =>
                {
                    animator.SetBool(JumpingHash, false);
                    characterPosition = previousCharacterPosition;
                });
        }
    }




    public enum CharacterPosition { LeftWall, RightWall, Center }
    private enum MoveDirection { Left, Right }
}