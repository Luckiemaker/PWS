using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StumpBehaviour : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(stumpDisseapearence());
    }

    IEnumerator stumpDisseapearence()
    {
        yield return new WaitForSeconds(70 + Random.Range(0, 30));
        Destroy(this.gameObject);
    }
}
