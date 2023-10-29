using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    private Animator anim;
    private CharacterAnimation currentAnimation = CharacterAnimation.IDLE;
    private Dictionary<CharacterAnimation, String> animToState = new Dictionary<CharacterAnimation, string> {
        {CharacterAnimation.IDLE, "idle"},
        {CharacterAnimation.WALK, "walk"},
        {CharacterAnimation.JUMP, "jump"},
        {CharacterAnimation.FALL, "fall"},
        {CharacterAnimation.DIE, "die"},
        {CharacterAnimation.SPAWN, "spawn"},
        {CharacterAnimation.DESPAWN, "despawn"}
    };

    [SerializeField]
    private ParticleSystem jumpEffect;

    private bool canPlayAnimation = true;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private int current = 0;

    public void PlayRandom() {
        Array values = Enum.GetValues(typeof(CharacterAnimation));
        var animation = (CharacterAnimation)values.GetValue(current++ % values.Length);
        Animate(animation);
        Invoke("PlayRandom", 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Animate(CharacterAnimation animation, bool blockUntilDone = false) {
        if (!canPlayAnimation) return;

        if (anim == null) {
            anim = GetComponent<Animator>();
        }

        if (animation != currentAnimation) {
            if (jumpEffect != null) {
                if (currentAnimation == CharacterAnimation.FALL) {
                    jumpEffect.Play();
                } else if (animation == CharacterAnimation.JUMP) {
                    jumpEffect.Play();
                }
            }

            currentAnimation = animation;
            anim.Play(animToState[animation]);
        }
        if (blockUntilDone) {
            canPlayAnimation = false;
        }
    }

    public void AnimationFinished() {
        canPlayAnimation = true;
    }
}

public enum CharacterAnimation {
    IDLE,
    WALK,
    JUMP,
    FALL,
    DIE,
    SPAWN,
    DESPAWN
}
