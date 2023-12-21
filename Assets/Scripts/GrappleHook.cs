using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour, IWeaponBase
{
    [SerializeField] PlayerControlsPhys _player;
    [SerializeField] LayerMask _grappleableLayers;
    [SerializeField] float _range;
    [SerializeField] float spherecastRadius = .5f;
    [SerializeField] GameObject _crosshairThingies;
    [SerializeField] Camera mainCam;

    [SerializeField] private LineRenderer _lineRenderer;
    public void Attack(bool pressed)
    {
        _lineRenderer.enabled = false;
        if (!pressed)
        {
            _player.DropHook();
            return;
        }

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, _range, _grappleableLayers))
        {
            _player.HookToRB(hit.rigidbody);
            _lineRenderer.enabled = true;
            _lineRenderer.SetPosition(0, hit.rigidbody.position);
        }
        else if(Physics.SphereCast(transform.position,0.5f, transform.forward, out hit, _range, _grappleableLayers))
        {
            _player.HookToRB(hit.rigidbody);
            _lineRenderer.enabled = true;
            _lineRenderer.SetPosition(0, hit.rigidbody.position);
        }
    }
    private void Awake()
    {
        GameManager.Instance.PlayerRespawned.AddListener(()=>Attack(false));
    }
    public void Update()
    {
        RaycastHit hit;
        _crosshairThingies.SetActive(Physics.Raycast(transform.position, transform.forward, out hit, _range, _grappleableLayers) ||
            Physics.SphereCast(transform.position, 0.5f, transform.forward, out hit, _range, _grappleableLayers));
        _lineRenderer.SetPosition(1, mainCam.ScreenToWorldPoint(new Vector3(Screen.width+2,0,0.04f)));
    }
}
