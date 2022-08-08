using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
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
        if (Input.GetKeyDown(KeyCode.Space) && characterPosition != CharacterPosition.Center)
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
}