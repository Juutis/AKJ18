using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public static PlayerCharacter main;
    void Awake()
    {
        main = this;
    }

    [SerializeField]
    private LayerMask enemyTargetMask;
    [SerializeField]
    private LayerMask enemyProjectileTargetMask;

    private float radius = 0.3f;

    private bool isInvulnerable = true;

    private float invulnerabilityDuration = 2f;
    private float invulnerabilityTimer = 0f;

    private float flashDuration = 0.2f;
    private float flashTimer = 0f;

    private Color originalColor;

    private Color flashColor;
    private Color previousColor;
    private Color targetColor;

    [SerializeField]
    private SpriteRenderer flashSprite;

    private CharacterAnimator charAnim;

    void Start()
    {
        originalColor = flashSprite.color;
        flashColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.05f);
        previousColor = originalColor;
        targetColor = flashColor;
        charAnim = GetComponentInChildren<CharacterAnimator>();
        charAnim.Animate(CharacterAnimation.SPAWN, true);
    }

    void Update()
    {

        if (isInvulnerable)
        {
            invulnerabilityTimer += Time.deltaTime;
            flashTimer += Time.deltaTime;
            flashSprite.color = Color.Lerp(previousColor, targetColor, flashTimer / flashDuration);
            if (flashTimer >= flashDuration)
            {
                flashTimer = 0f;
                targetColor = previousColor;
                previousColor = flashSprite.color;
            }
            if (invulnerabilityTimer >= invulnerabilityDuration)
            {
                flashSprite.color = originalColor;
                isInvulnerable = false;
            }
            return;
        }

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, radius, Vector2.zero, 0f, enemyTargetMask);
        if (hit.collider != null)
        {
            Damageable damageableEntity = hit.collider.GetComponent<Damageable>();

            Debug.Log($"Killed player because we hit {damageableEntity}!");
            GameManager.main.PlayerDie();
        }

        RaycastHit2D hitProjectile = Physics2D.CircleCast(transform.position, radius, Vector2.zero, 0f, enemyProjectileTargetMask);
        if (hitProjectile.collider != null)
        {
            Debug.Log($"Killed player because we hit enemy projectile!");
            GameManager.main.PlayerDie();
        }
    }

    public void Die()
    {
        SoundManager.main.PlaySound(GameSoundType.HeroDie);
        charAnim.Animate(CharacterAnimation.DIE, true);
        Invoke("Destroy", 0.5f);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

}
