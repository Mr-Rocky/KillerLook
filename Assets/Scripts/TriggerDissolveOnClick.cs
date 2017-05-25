using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDissolveOnClick : MonoBehaviour {

    Vector3 point;
    bool didHit = false;
    DissolveEffect targetEffect;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
            {
                targetEffect = hitInfo.collider.gameObject.GetComponent<DissolveEffect>();

                if (targetEffect != null)
                {
                    didHit = true;
                    point = hitInfo.point;
                    targetEffect.Reset();
                }
            }
        }

        if (didHit && Input.GetMouseButtonUp(0))
        {
            targetEffect.TriggerDissolve(point);

        }
    }
}
