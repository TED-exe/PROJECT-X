using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectFadeOut : MonoBehaviour, IEquatable<ObjectFadeOut>
{
    public List<Renderer> re_FadeOutRenderers = new List<Renderer>();
    public Vector3 v3_Position;
    public List<Material> m_materials = new List<Material>();
    [HideInInspector] public float f_initialObjectAlfa;
    private void Awake()
    {   
        v3_Position = transform.position;

        if (re_FadeOutRenderers.Count == 0)
        {
            re_FadeOutRenderers.AddRange(GetComponentsInChildren<Renderer>());
        }
        foreach (Renderer renderer in re_FadeOutRenderers)
        {
            m_materials.Add(renderer.material);
        }
        f_initialObjectAlfa = m_materials[0].color.a;
    }
    public bool Equals(ObjectFadeOut other)
    {
        return v3_Position.Equals(other.v3_Position);
    }

    public override int GetHashCode()
    {
        return v3_Position.GetHashCode();
    }

}
