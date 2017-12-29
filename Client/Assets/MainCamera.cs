using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// \addtogroup client
/// @{

/// <summary>
/// Main attachment to the main camera for handling open-world rendering.
/// Specifically used for handling the split texture (2 players, one screen, think Lego).
/// </summary>
public class MainCamera : MonoBehaviour
{

    /// <summary>
    /// The material shader used to splice the two textures.
    /// </summary>
    public Material spliceTextures;
    /// <summary>
    /// The render texture of the "first" camera.
    /// </summary>
    public RenderTexture textureA;
    /// <summary>
    /// The render texture of the "second" camrea.
    /// </summary>
    public RenderTexture textureB;
    /// <summary>
    /// The position of the "first" camera.
    /// </summary>
    public Vector3 positionA;
    /// <summary>
    /// The position of the "second" camera.
    /// </summary>
    public Vector3 positionB;
    /// <summary>
    /// The square magnitide of the distance between the two cameras.
    /// Mathematically, (a.pos - b.pos).x^2 + (a.pos - b.pos).y^2
    /// </summary>
    private float dist;
    /// <summary>
    /// The square magnitude at which the camera splicing is stopped.
    /// If <code><see cref="dist"/> < value</code>, this camera is used to render the screen.
    /// Else, <see cref="textureA"/> and <see cref="textureB"/> are spliced by the <see cref="spliceTextures"/> material.
    /// </summary>
    public float distTarget = 5f;

    private void Start()
    {
        GameManager.INSTANCE.mainCamera = this;
    }

    /// <summary>
    /// Set the render texture at the index.
    /// </summary>
    /// <param name="index">The index, even is <see cref="textureA"/>, odd is <see cref="textureB"/>.</param>
    /// <param name="texture">The camera's render texture</param>
    public void SetTexture(int index, RenderTexture texture)
    {
        switch (index % 2)
        {
            case 0: this.textureA = texture; break;
            case 1: this.textureB = texture; break;
            default: break;
        }
        // Set in material shader
        this.spliceTextures.SetTexture("_Tex1", this.textureA);
        this.spliceTextures.SetTexture("_Tex2", this.textureB);
    }

    /// <summary>
    /// Updates the position of cameras.
    /// Returns the unit vector of the camera relative to this camera (which is at the avgerage position of both cameras).
    /// </summary>
    /// <param name="index">The index of the camera correlating with the <see cref="SetTexture(int, RenderTexture)"/>.</param>
    /// <param name="position">The position of the camera.</param>
    /// <param name="doUseIndividualCamera">If both cameras have been set.</param>
    /// <returns><see cref="Vector3"/></returns>
    public Vector3 SetPosition(int index, Vector3 position, out bool doUseIndividualCamera)
    {
        doUseIndividualCamera = this.textureA != null && this.textureB != null;

        // Update the position
        switch (index % 2)
        {
            case 0: this.positionA = position; break;
            case 1: this.positionB = position; break;
            default: break;
        }
        // Update the position in the shader
        this.spliceTextures.SetVector("_PosA", this.positionA);
        this.spliceTextures.SetVector("_PosB", this.positionB);
        
        // Get the Z position of the cameras
        float camZ = this.transform.position.z;
        // Assume the position should at least be where it is now
        Vector3 posOut = position;
        // If both textures are active
        if (this.textureA != null && this.textureB != null)
        {
            // Get the vector difference of the two positions
            Vector3 diff = this.positionA - this.positionB;
            // Set this position to the average position
            this.transform.position = (diff * 0.5f) + this.positionB;
            // Get the sqaured distance between them
            this.dist = diff.sqrMagnitude;
            // Return the average position - the input position
            posOut = (this.transform.position - position);
        }
        else
        {
            // Set this position to the only active position
            this.transform.position = this.positionA;
        }
        // Set the Z of this position
        this.transform.position += Vector3.forward * camZ;

        return posOut;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // If both textures are active, then splice as necessary
        if (this.textureA != null && this.textureB != null && this.dist >= this.distTarget * this.distTarget)
        {
            Graphics.Blit(source, destination, this.spliceTextures);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

}
/// @}
