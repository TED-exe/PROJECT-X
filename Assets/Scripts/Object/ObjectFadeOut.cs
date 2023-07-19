using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class ObjectFadeOut : MonoBehaviour
{
    [Header("FADE OUT")]
    [SerializeField][Range(0f,1f)] private float f_fadeOutAmount =.2f;
    [Min(.2f)][SerializeField] private float f_fadeOutSpeed = 10f;
    private float f_originialOpacity;
    public bool b_doFade = false;
    private Material m_objectMaterial;

    private void Awake()
    {
        m_objectMaterial = GetComponent<Renderer>().material;
        f_originialOpacity = m_objectMaterial.GetFloat("_OPACITY");
    }

    private void Update()
    {
        if (!b_doFade)
        {
            FadeIn();
            return;
        }
        else
        {
            FadeOut();
            return;
        }
    }
    private void FadeOut()
    {
        var currentOpacity = m_objectMaterial.GetFloat("_OPACITY");
        if (currentOpacity != f_fadeOutAmount)
            m_objectMaterial.SetFloat("_OPACITY", Mathf.Lerp(currentOpacity, f_fadeOutAmount, f_fadeOutSpeed * Time.deltaTime));
        return;
    }
    private void FadeIn()
    {
        var currentOpacity = m_objectMaterial.GetFloat("_OPACITY");
        if (currentOpacity != f_originialOpacity)
            m_objectMaterial.SetFloat("_OPACITY", Mathf.Lerp(currentOpacity, f_originialOpacity, f_fadeOutSpeed * Time.deltaTime));
        return;
    }

}
