using UnityEngine;
using System;
using System.Collections;

public class EnemyController : MonoBehaviour
{
	[Header("Stats")]
	public EnemyStats stats;
	public int currentHP;

	public event Action OnEnemyDeath;

	private NexusController nexus;
	private bool isAttacking;
	private bool isDead;
	private Coroutine attackRoutine;

	[Header("Visuals & FX")]
	public GameObject deathFxPrefab;
	public float deathDestroyDelay = 2f;
	public bool disableRendererOnDeath = false;

	[Header("Animation")]
	[SerializeField] private Animator animator;
	[Tooltip("Le transform qui contient le mesh/armature visuelle (facultatif). Si null, on utilise this.transform.")]
	public Transform visualRoot;
	[Tooltip("Coche si ton modèle regarde l'arrière (180°).")]
	public bool invertForward = false;
	[Range(0f, 20f)] public float rotationLerpSpeed = 10f;

	// --- PATCH: attack lock behaviour ---
	[Header("Attack Behaviour")]
	public bool lockPositionOnAttack = true;
	private Vector3 attackLockPos;
	public bool disableRootMotionOnAttack = true;
	// --- end PATCH ---

	private AnimState currentAnimState = AnimState.None;

	private Collider[] cachedColliders;
	private Renderer[] cachedRenderers;

	private enum AnimState { None, Idle, Move, Attack, Die }

	private static readonly int IdleHash = Animator.StringToHash("Idle01");
	private static readonly int MoveHash = Animator.StringToHash("Move01");
	private static readonly int AttackHash = Animator.StringToHash("Attack01");
	private static readonly int DieHash = Animator.StringToHash("Die01");

	private void Awake()
	{
		cachedColliders = GetComponentsInChildren<Collider>();
		cachedRenderers = GetComponentsInChildren<Renderer>();
	}

	private void Start()
	{
		currentHP = stats.maxHP;
		nexus = FindObjectOfType<NexusController>();
		if (nexus == null)
		{
			Debug.LogError($"[EnemyController] Aucune NexusController trouvée. {name} ne pourra pas attaquer.");
		}

		if (animator == null)
		{
			animator = GetComponentInChildren<Animator>();
			if (animator == null)
			{
				Debug.LogWarning($"[EnemyController] Aucun Animator assigné sur {name}.");
			}
		}

		if (visualRoot == null)
		{
			visualRoot = animator != null ? animator.transform : transform;
		}

		PlayIdle();
	}

	private void Update()
	{
		if (isDead) return;

		// PATCH: verrouiller position pendant attaque
		if (isAttacking && lockPositionOnAttack)
		{
			transform.position = attackLockPos;
		}

		if (!isAttacking)
		{
			MoveTowardsNexus();
		}
	}

	private void MoveTowardsNexus()
	{
		if (nexus == null) return;

		Vector3 targetPos = nexus.transform.position;
		Vector3 toTarget = targetPos - transform.position;
		float dist = toTarget.magnitude;

		if (dist > 0.01f)
		{
			Vector3 direction = toTarget.normalized;
			transform.position += direction * stats.speed * Time.deltaTime;
			UpdateVisualFacing(direction);
			PlayMove();
		}

		if (dist < 1f && attackRoutine == null)
		{
			attackRoutine = StartCoroutine(Attack());
		}
	}

	private IEnumerator Attack()
	{
		isAttacking = true;
		bool playAttackAnim = true;

		if (lockPositionOnAttack)
		{
			attackLockPos = transform.position;
		}
		if (disableRootMotionOnAttack && animator != null)
		{
			animator.applyRootMotion = false;
		}

		if (nexus != null)
		{
			Vector3 dir = nexus.transform.position - transform.position;
			UpdateVisualFacing(dir);
		}

		while (!isDead)
		{
			if (playAttackAnim)
			{
				PlayAttack();
			}
			else
			{
				PlayIdle();
			}
			playAttackAnim = !playAttackAnim;

			if (nexus != null)
			{
				nexus.TakeDamage(stats.damage);
				if (nexus.IsDead())
				{
					GameManager.Instance.GameOver();
					yield break;
				}
			}

			yield return new WaitForSeconds(stats.attackInterval);
		}
	}

	public void TakeDamage(int amount)
	{
		if (isDead) return;

		currentHP -= amount;
		if (currentHP <= 0)
		{
			Die();
		}
	}

	private void Die()
	{
		if (isDead) return;
		isDead = true;

		if (attackRoutine != null)
		{
			StopCoroutine(attackRoutine);
			attackRoutine = null;
		}
		isAttacking = false;

		OnEnemyDeath?.Invoke();

		if (cachedColliders != null)
		{
			foreach (var c in cachedColliders) if (c) c.enabled = false;
		}

		PlayDie();

		if (deathFxPrefab != null)
		{
			Instantiate(deathFxPrefab, GetCenter(), Quaternion.identity);
		}

		if (disableRendererOnDeath && cachedRenderers != null)
		{
			foreach (var r in cachedRenderers) if (r) r.enabled = false;
		}

		Destroy(gameObject, deathDestroyDelay);
	}

	public Vector3 GetCenter()
	{
		Collider col = GetComponentInChildren<Collider>();
		return col != null ? col.bounds.center : transform.position;
	}

	public bool IsDead => isDead;

	private void PlayIdle() => SetAnim(AnimState.Idle, IdleHash);
	private void PlayMove() => SetAnim(AnimState.Move, MoveHash);
	private void PlayAttack() => SetAnim(AnimState.Attack, AttackHash);
	private void PlayDie() => SetAnim(AnimState.Die, DieHash);

	private void SetAnim(AnimState newState, int stateHash)
	{
		if (animator == null) return;
		if (currentAnimState == newState) return;
		currentAnimState = newState;
		animator.CrossFade(stateHash, 0.1f);
	}

	private void UpdateVisualFacing(Vector3 worldDir)
	{
		worldDir.y = 0f;
		if (worldDir.sqrMagnitude < 0.0001f) return;

		Quaternion lookRot = Quaternion.LookRotation(worldDir, Vector3.up);
		if (invertForward)
		{
			lookRot *= Quaternion.Euler(0f, 180f, 0f);
		}

		if (visualRoot != null)
			visualRoot.rotation = Quaternion.Slerp(visualRoot.rotation, lookRot, rotationLerpSpeed * Time.deltaTime);
		else
			transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationLerpSpeed * Time.deltaTime);
	}
}
