using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Troop : MonoBehaviour {

    public const int EXP_PER_LEVEL = 60;
    public const float PLAYER_CONTROL_SPEED_MULT = 0.6f;

    [SerializeField] private bool _playerControlled = false;
    public bool PlayerControlled { get { return _playerControlled; } set { _playerControlled = value; } }
    [SerializeField] private string _troopName = "";
    public string TroopName { get { return _troopName; } }
    [SerializeField] private int _level = 1;
    public int Level { get { return _level; } }
    [SerializeField] private int _experience = 0;
    public int Experience { get { return _experience; } }
    public int ExperienceNeededToLevel { get { return EXP_PER_LEVEL * _level; } }
    [SerializeField] private float _maxHealth;
    public float MaxHealth { get { return _maxHealth; } }
    [SerializeField] private float _health;
    public float Health { get { return _health; } }
    [SerializeField, Tooltip("Attacks per second.")] private float attackSpeed;
    private float currentAttackCharge = 0;
    private float AttackTime { get { return 1.0f / attackSpeed; } }
    [SerializeField] private float attackDamage;
    [SerializeField] private Color woundedColor;
    [SerializeField] private Troop target;

    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider energySlider;

    private Animator animator;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip deathSound;
    private AudioSource audioSource;
    [SerializeField] private SpriteRenderer graphic;
    [SerializeField] private ParticleSystem particles;

    private Vector3 defaultSpriteLocation;
    private Color baseColor;
    [SerializeField] private bool isAlive = true;

    private Coroutine cr_AttackCycle = null;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        baseColor = graphic.color;
        defaultSpriteLocation = graphic.transform.localPosition;
        GameManager.Instance.GameStateChanged += GameManager_GameStateChanged;
    }

    private void OnDestroy()
    {
        target = null;
        GameManager.Instance.GameStateChanged -= GameManager_GameStateChanged;

    }

    private void Update()
    {
        if (PlayerControlled)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                HandlePlayerAttack();
            }
        }
    }

    private void HandlePlayerAttack()
    {
        currentAttackCharge += attackSpeed * PLAYER_CONTROL_SPEED_MULT;
        energySlider.value = currentAttackCharge / AttackTime;
        if(energySlider.value >= 1.0f)
        {
            if (target != null && target.isAlive && isAlive)
            {
                animator.SetTrigger("Attack");
                target.Damage(attackDamage);
                audioSource.clip = attackSound;
                audioSource.Play();
                currentAttackCharge = 0.0f;
                energySlider.value = currentAttackCharge / AttackTime;
            }
        }
    }

    private void GameManager_GameStateChanged(object sender, GameManager.GameStateChangedArgs e)
    {
        if(e.newState == GameStates.SUMMARY_SCREEN || e.newState == GameStates.GAME_OVER_SCREEN)
        {
            if (!isAlive) { Destroy(this.gameObject); }
        }
    }

    public Sprite GetSprite()
    {
        return graphic.sprite;
    }

    public void UpdateFacing(bool facingLeft)
    {
        graphic.flipX = facingLeft;
    }

    public void GiveExperience(int exp)
    {
        exp = exp + _experience;
        int statToIncrease;
        string stat = "";
        float amount = 0;
        float newValue = 0;
        while(exp > ExperienceNeededToLevel)
        {
            exp -= ExperienceNeededToLevel;
            _level++;

            statToIncrease = UnityEngine.Random.Range(0, 3);

            switch (statToIncrease)
            {
                case 0:
                    _maxHealth += 2;
                    stat = "Health";
                    amount = 2.0f;
                    newValue = _maxHealth;
                    break;
                case 1:
                    attackDamage += 0.1f;
                    stat = "Attack Damage";
                    amount = 0.1f;
                    newValue = attackDamage;
                    break;
                case 2:
                    attackSpeed += 0.1f;
                    stat = "Attack Speed";
                    amount = 0.1f;
                    newValue = attackSpeed;
                    break;
                default:
                    break;
            }

            OnLevelUp(new LevelUpArgs(stat, amount, newValue));
        }

        _experience = exp;
    }

    public void ResetTroop()
    {
        _health = MaxHealth;
        healthSlider.value = 1;
        currentAttackCharge = 0;
        energySlider.value = 0;
        target = null;
    }

    public void SetTarget(Troop t)
    {
        target = t;
        target.Death += Target_Death;
        if (!PlayerControlled)
        {
            StartAttackCycle();
        }
        
    }

    private void Target_Death(object sender, OnDeathArgs e)
    {
        target = null;
        OnKilledTarget();
    }

    public void Damage(float amount)
    {
        if (!isAlive) { return; }
        _health -= amount;
        healthSlider.value = _health / MaxHealth;
        particles.Emit(15);
        StartCoroutine(DamageRumble());
        if(_health <= 0)
        {
            OnDeath(new OnDeathArgs(this));
        }
    }

    public void StartAttackCycle()
    {
        StopAttackCycle();
        if(target != null)
        {
            cr_AttackCycle = StartCoroutine(AttackCycle());
        }
    }

    public void StopAttackCycle()
    {
        if(cr_AttackCycle != null)
        {
            StopCoroutine(cr_AttackCycle);
            cr_AttackCycle = null;
        }
    }

    private IEnumerator AttackCycle()
    {
        currentAttackCharge = 0;
        

        while(currentAttackCharge < AttackTime)
        {
            currentAttackCharge += Time.deltaTime;
            energySlider.value = currentAttackCharge / AttackTime;
            yield return null;
        }

        if(target != null && target.isAlive && isAlive)
        {
            animator.SetTrigger("Attack");
            target.Damage(attackDamage);
            audioSource.clip = attackSound;
            audioSource.Play();
            cr_AttackCycle = StartCoroutine(AttackCycle());
        } else
        {
            cr_AttackCycle = null;
        }
    }

    private IEnumerator DamageRumble()
    {
        Transform graphicTransform = graphic.transform;
        float rumbleTime = 0.15f;
        float t = 0f;
        Vector3 rumblePosition = new Vector3(0, 0, graphic.transform.localPosition.z);

        while(t < rumbleTime)
        {
            t += Time.deltaTime;

            graphic.color = Color.Lerp(woundedColor, baseColor, t / rumbleTime);
            rumblePosition.x += UnityEngine.Random.Range(-0.1f, 0.1f);

            graphic.transform.localPosition = defaultSpriteLocation + rumblePosition;

            yield return null;
        }

        graphic.transform.localPosition = defaultSpriteLocation;
        graphic.color = baseColor;
    }

    /**************************************************************************************************************/
    /*************************************************** EVENTS ***************************************************/
    /**************************************************************************************************************/


    public event EventHandler<OnDeathArgs> Death;

    public class OnDeathArgs : EventArgs
    {
        public Troop troop;

        public OnDeathArgs(Troop troop)
        {
            this.troop = troop;
        }
    }

    private void OnDeath(OnDeathArgs e)
    {       
        isAlive = false;
        animator.SetBool("isAlive", isAlive);
        target = null;
        audioSource.clip = deathSound;
        audioSource.Play();
        EventHandler<OnDeathArgs> handler = Death;

        if (handler != null)
        {
            handler(this, e);
        }
    }

    public event EventHandler<KilledTargetArgs> KilledTarget;

    public class KilledTargetArgs : EventArgs
    {
        public Troop troop;

        public KilledTargetArgs(Troop troop)
        {
            this.troop = troop;
        }
    }

    private void OnKilledTarget()
    {
        EventHandler<KilledTargetArgs> handler = KilledTarget;

        if(handler != null)
        {
            handler(this, new KilledTargetArgs(this));
        }
    }

    public event EventHandler<LevelUpArgs> LevelUp;

    public class LevelUpArgs : EventArgs
    {
        public string stat;
        public float statIncrease;
        public float newValue;

        public LevelUpArgs(string stat, float statIncrease, float newValue)
        {
            this.stat = stat;
            this.statIncrease = statIncrease;
            this.newValue = newValue;
        }
    }

    private void OnLevelUp(LevelUpArgs e)
    {
        EventHandler<LevelUpArgs> handler = LevelUp;

        if (handler != null)
        {
            handler(this, e);
        }
    }
}
