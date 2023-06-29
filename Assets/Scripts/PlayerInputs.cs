using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class PlayerInputs : MonoBehaviour
{
    //[SerializeField] private Animator _anim;

    [SerializeField] private float moveX = 0.0f;
    [SerializeField] private float moveY = 0.0f;
    [SerializeField] private bool isMoving = false;


    [SerializeField] private float _minImpactForce = 20;

    // Anim times can be gathered from the state itself, but 
    // for the simplicity of the video...
    [SerializeField] private float _landAnimDuration = 0.1f;
    [SerializeField] private float _attackAnimTime = 0.2f;

    private IPlayerController _player;
    private Animator _anim;
    private SpriteRenderer _renderer;

    private bool _grounded;
    private float _lockedTill;
    private bool _jumpTriggered;
    private bool _attacked;
    private bool _landed;

    private void Awake()
    {
        if (!TryGetComponent(out IPlayerController player))
        {
            Destroy(this);
            return;
        }

        _player = player;
        _anim = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
    }



    // Start is called before the first frame update
    void Start()
    {
        _anim.CrossFade("idle", 0, 0);

        _player.Jumped += () => {
            _jumpTriggered = true;
        };
        _player.Attacked += () => {
            _attacked = true;
        };
        _player.GroundedChanged += (grounded, impactForce) => {
            _grounded = grounded;
            _landed = impactForce >= _minImpactForce;
        };

    }

    // Update is called once per frame
    void Update()
    {
        UpdateMotion();
        if (_player.Input.x != 0) _renderer.flipX = _player.Input.x < 0;

        var state = GetState();

        _jumpTriggered = false;
        _landed = false;
        _attacked = false;

        if (state == _currentState) return;
        _anim.CrossFade(state, 0, 0);
        _currentState = state;
    }

    private int GetState()
    {
        if (Time.time < _lockedTill) return _currentState;

        // Priorities
        if (_attacked) return LockState(Attack, _attackAnimTime);
        if (_player.Crouching) return Crouch;
        if (_landed) return LockState(Land, _landAnimDuration);
        if (_jumpTriggered) return Jump;

        if (_grounded) return _player.Input.x == 0 ? Idle : IsMoving;
        return _player.Speed.y > 0 ? Jump : Fall;

        int LockState(int s, float t)
        {
            _lockedTill = Time.time + t;
            return s;
        }
    }
        private void UpdateMotion()
    {
        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");

        isMoving = !Mathf.Approximately(moveX, 0f);


        _anim.SetFloat("moveX", moveX);
        _anim.SetBool("isMoving", isMoving);


    }



    #region Cached Properties

    private int _currentState;

    private static readonly int Idle = Animator.StringToHash("idle");
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static readonly int Jump = Animator.StringToHash("jumping");
    private static readonly int Fall = Animator.StringToHash("falling");
    private static readonly int Attack = Animator.StringToHash("attack");
    private static readonly int Crouch = Animator.StringToHash("crouch");
    private static readonly int Land = Animator.StringToHash("land");

    #endregion

}

public interface IPlayerController
{
    public Vector2 Input { get; }
    public Vector2 Speed { get; }
    public bool Crouching { get; }

    public event Action<bool, float> GroundedChanged; // Grounded - Impact force
    public event Action Jumped;
    public event Action Attacked;
}
