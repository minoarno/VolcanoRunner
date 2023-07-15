using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{

    private static int _nrInstances = 0;

    public int PlayerId;

    private Camera _mainCam;
    private Renderer _renderer;

    private Color _color;


    // Start is called before the first frame update
    void Start()
    {
        PlayerId = _nrInstances++;

        int xPos = -5 + 5 * PlayerId;
        transform.position = new Vector3(xPos, 0, 0);

        _mainCam = Camera.main;



        _color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }

    private void OnDestroy()
    {
        --_nrInstances;
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckRaycast()) GetComponent<Renderer>().material.color = _color;
        else GetComponent<Renderer>().material.color = Color.white;
    }

    private bool CheckRaycast()
    {
        if (!Input.GetMouseButton(0))
            return false;

        Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction * 100f, _color);

        RaycastHit hit;

        Physics.Raycast(ray, out hit, 100);

        var hitPlayer = hit.collider.gameObject.GetComponent<NetworkPlayer>();

        return hitPlayer?.PlayerId == PlayerId;
    }
}
