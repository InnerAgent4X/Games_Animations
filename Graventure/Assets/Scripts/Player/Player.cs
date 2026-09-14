using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using UnityEditor.XR;

public class Player : MonoBehaviour
{

    public InputActionAsset PlayerActions;

    public GameObject P_ATK;
    public GameObject M_ATK;

    public Transform[] player_positions;
    public Transform[] enemy_spawn;


    private float moveInput;
    private bool moveHold = false;
    private bool attackHold = false;
    private bool magicHold = false;

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
        transform.position = player_positions[1].position;
    }

    void Update()
    {
        if (!GameManager.Instance.isGameOver)
        {
            moveInput = moveAction.ReadValue<float>();
            movement();
            if (attackPhysical.WasPressedThisFrame()) PhysicalAttack();
            if (attackMagic.WasPressedThisFrame()) MagicAttack();
        }
    }

    void movement()
    {
        if (!moveHold)
        {
            StartCoroutine(movementPause());
            if (moveInput > 0)
            {
                transform.position = player_positions[1].position;
                transform.LookAt(enemy_spawn[1].position);
            }
            else if (moveInput < 0)
            {
                transform.position = player_positions[0].position;
                transform.LookAt(enemy_spawn[0].position);
            }
        }
    }

    IEnumerator movementPause()
    {
        moveHold = true;
        yield return new WaitUntil(() => moveInput == 0);
        moveHold = false;
    }

    void PhysicalAttack()
    {
        if (!attackHold)
        {
            StartCoroutine(PhysicalAttackPause());
            Instantiate(P_ATK, transform.position, transform.rotation);
        }
    }

    IEnumerator PhysicalAttackPause()
    {
        attackHold = true;
        yield return new WaitUntil(() => attackPhysical.IsPressed() == false);
        attackHold = false;
    }

    void MagicAttack()
    {
        if (!magicHold)
        {
            StartCoroutine(MagicAttackPause());
            Instantiate(M_ATK, transform.position, transform.rotation);
        }
    }

    IEnumerator MagicAttackPause()
    {
        magicHold = true;
        yield return new WaitUntil(() => attackMagic.IsPressed() == false);
        magicHold = false;
    }

}
