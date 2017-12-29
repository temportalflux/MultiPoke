using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionShader : MonoBehaviour
{

    [Tooltip("The material to use to transition with (Expects ShaderDynamicOverlay)")]
    public Material transitionMaterial;

    Coroutine currentTransition;
    private float percentDone;

    void Start()
    {
        this.currentTransition = null;    
    }

    public bool isAnimating()
    {
        return this.currentTransition != null;
    }

    protected void startAnimation(float start, float end, float time)
    {
        this.setCutoff(0);
        this.setFade(1);
        this.currentTransition = StartCoroutine(this.runLoop(start, end, time));
    }

    public float GetPercentDone()
    {
        return this.percentDone;
    }

    IEnumerator runLoop(float start, float end, float time)
    {
        // Func => (end - start)x + start;
        // for start = 0, end = 1 => +1x + 0 => x
        // for start = 1, end = 0 => -1x + 1 => -x + 1
        // ? > 0 (while loop) => (end - start)(end - Func)
        // for start = 0, end = 1 => 1(1 - x) = 1 - x => -x + 1
        // for start = 1, end = 0 => -1(0 - (-x + 1)) => -(x - 1) => -x + 1
        
        float direction = end - start;

        float delta = start; // the start component (offset function by start)
        do
        {
            // Update the function with the starting delta
            // or update with each incremental delta
            this.updateShader(delta);

            // Add (end-start)x to delta
            delta += (Time.deltaTime * direction) / time;

            this.percentDone = delta / (end - start);
            //Debug.Log(delta);
            yield return null;
        }
        while (direction * (end - delta) > 0);
        // Assign max value
        delta = end;

        // Update shader with max value
        this.updateShader(delta);

        // Stop shader animation
        this.currentTransition = null;
    }

    virtual public void updateShader(float delta)
    {
        CameraBlit.setMaterial(this.transitionMaterial);
    }

    protected void setCutoff(float value)
    {
        if (this.transitionMaterial != null)
        {
            this.transitionMaterial.SetFloat("_Cutoff", value);
        }
    }

    protected void setFade(float value)
    {
        if (this.transitionMaterial != null)
        {
            this.transitionMaterial.SetFloat("_Fade", value);
        }
    }

}
