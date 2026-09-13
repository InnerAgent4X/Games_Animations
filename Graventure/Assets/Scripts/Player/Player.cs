using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;

public class Player : MonoBehaviour
{

    public int Score = 0;

    public InputActionAsset PlayerActions;

    public Transform[] tracks;

    private int healthyboy = 3;

    private float moveInput;
    private bool moveHold = false;

    private InputAction moveAction;
    private InputAction attackPhysical;
    private InputAction attackMagic;

    void OnEnable()
    {
        PlayerActions.Enable();
    }

    void OnDisable()
    {
        PlayerActions.Disable();
    }

    void Awake()
    {
        moveAction = PlayerActions.FindAction("Move");
        attackPhysical = PlayerActions.FindAction("Atk_P");
        attackMagic = PlayerActions.FindAction("Atk_M");
    }

    void Start()
    {
        transform.position = tracks[1].position;
    }

    void Update()
    {
        moveInput = moveAction.ReadValue<float>();
        movement();
    }

    void movement()
    {
        if (!moveHold)
        {
            StartCoroutine(movementPause());
            if (moveInput > 0)
            {
                Debug.Log("Move Right");
                transform.position = tracks[1].position;
            }
            else if (moveInput < 0) transform.position = tracks[0].position;
        }
    }

    IEnumerator movementPause()
    {
        moveHold = true;
        yield return new WaitUntil(() => moveInput == 0);
        moveHold = false;
    }
}
