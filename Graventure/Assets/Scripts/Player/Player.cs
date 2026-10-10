using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public InputActionAsset PlayerActions;

    public GameObject P_ATK;
    public GameObject M_ATK;

    public Transform[] player_positions;
    public Transform[] enemy_spawn;
    public Slider moveSlider;
    public ParticleSystem moveParticle_right;
    public ParticleSystem moveParticle_left;

    private float moveInput;
    private bool moveHold = false;
    private bool physicalHold = false;
    private bool magicHold = false;
    private bool canAttack = true;

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
        if (!GameManager.Instance.isGameOver && !GameManager.Instance.isYouWin)
        {
            //alternative input method for movement using a slider UI element (also changed the movement() method to accommodate this)
            //moveInput = moveSlider.value;
            moveInput = moveAction.ReadValue<float>();
            movement();
            if (attackPhysical.WasPressedThisFrame() && canAttack) PhysicalAttack();
            if (attackMagic.WasPressedThisFrame() && canAttack) MagicAttack();
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
                moveParticle_right.Play();
                transform.LookAt(enemy_spawn[1].position);
            }
            //change to <= if using the slider UI element for movement
            else if (moveInput < 0)
            {
                transform.position = player_positions[0].position;
                moveParticle_left.Play();
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
    
    public void cooldown()
    {
        StartCoroutine(AttackPenalty());
    }

    IEnumerator AttackPenalty()
    {
        canAttack = false;
        yield return new WaitForSeconds(GameManager.Instance.levelSettings.PenaltyDuration);
        canAttack = true;
    }

    void PhysicalAttack()
    {
        if (!physicalHold)
        {
            StartCoroutine(PhysicalAttackPause());
            Instantiate(P_ATK, transform.position, transform.rotation);
        }
    }

    IEnumerator PhysicalAttackPause()
    {
        physicalHold = true;
        yield return new WaitUntil(() => !attackPhysical.IsPressed());
        physicalHold = false;
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
        yield return new WaitUntil(() => !attackMagic.IsPressed());
        magicHold = false;
    }

}
