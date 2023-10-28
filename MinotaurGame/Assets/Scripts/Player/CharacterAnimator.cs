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
        {CharacterAnimation.FALL, "fall"}
    };

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        PlayRandom();
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

    public void Animate(CharacterAnimation animation) {
        if (animation != currentAnimation) {
            currentAnimation = animation;
            anim.Play(animToState[animation]);
        }
    }
}

public enum CharacterAnimation {
    IDLE,
    WALK,
    JUMP,
    FALL
}
