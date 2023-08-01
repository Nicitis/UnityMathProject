using System.Collections.Generic;
using UnityEngine;

public class GraphicsTest : MonoBehaviour
{
    public Mesh mesh;
    public Material mat;
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        Graphics.DrawMesh(mesh, transform.position, Quaternion.identity, mat, 0);
    }
}