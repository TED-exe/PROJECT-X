using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPull : MonoBehaviour
{
    private const string Player = "Player";
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(Player))
            return;
        if (!other.transform.parent.TryGetComponent<PlayerPullObjectSystem>(out PlayerPullObjectSystem pullSystem))
            return;
        pullSystem.b_stayInColider = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(Player))
            return;
        if (!other.transform.parent.TryGetComponent<PlayerPullObjectSystem>(out PlayerPullObjectSystem pullSystem))
            return;
        pullSystem.b_stayInColider = false;
    }
}
