using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{

    private static int _nrInstances = 0;

    private int _playerId;

    private Camera _mainCam;
    private Renderer _renderer;

    private Color _color;

    private bool _isHit = false;

    // Start is called before the first frame update
    void Start()
    {
        _playerId = _nrInstances++;

        int xPos = -5 + 5 * _playerId;
        transform.position = new Vector3(xPos, 0, 0);

        _mainCam = Camera.main;



        _color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        _renderer = GetComponentInChildren<Renderer>();
    }

    private void OnDestroy()
    {
        --_nrInstances;
    }

    // Update is called once per frame
    void Update()
    {


        if (CheckRaycast()) _renderer.material.color = _color;
        else _renderer.material.color = Color.white;
    }

    private bool CheckRaycast()
    {

        _isHit = false;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;

        mousePos = _mainCam.ScreenToWorldPoint(mousePos);


        if (!Input.GetMouseButton(0))
            return false;

        Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);
            //new Ray(_mainCam.transform.position, mousePos - _mainCam.transform.position);

        Debug.DrawRay(ray.origin, ray.direction * 100f, _color);

        RaycastHit hit;

        return Physics.Raycast(ray, out hit, 100);
    }
}
