using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : TransitionShader {

    [Tooltip("How much transition to start with")]
    [Range(0, 1)]
    public float start;

    [Tooltip("How much transition to end with")]
    [Range(0, 1)]
    public float end;

    [Tooltip("How long (in seconds) does the transition take")]
    [Range(0, 60)]
    public float time;

    [Tooltip("If the transition uses a texture")]
    public bool useTransition;

    [Tooltip("If the transition uses a fade")]
    public bool useFade;

    public void forwards()
    {
        this.transition(this.start, this.end);
    }

    public void backwards()
    {
        this.transition(this.end, this.start);
    }

    private void transition(float start, float end)
    {
        if (!this.isAnimating())
        {
            if (!this.useTransition)
            {
                this.setCutoff(1);
            }
            if (!this.useFade)
            {
                this.setFade(1);
            }
            this.startAnimation(start, end, this.time);
        }
    }

    public override void updateShader(float delta)
    {
        if (this.useTransition)
            this.setCutoff(delta);
        if (this.useFade)
            this.setFade(delta);
        base.updateShader(delta);
    }

}
