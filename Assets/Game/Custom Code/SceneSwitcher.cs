using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))]
public class SceneSwitcher : MonoBehaviour
{
    GameObject _Player;
    [SerializeField] string _LoadSceneName;
    string _ThisSceneName;
    [Header("Position when entering or exiting a building")]
    [SerializeField] bool _SetPositionOnEnter;
    [SerializeField] Vector3 _Position;
    [Header("Rotation when entering or exiting a building")]
    [SerializeField] bool _KeepRotationOnEnter;
    [SerializeField] bool _SetRotationOnEnter;
    [SerializeField] Vector3 _Rotation;
    Collider _Collider;
    Quaternion _RotationQuat;

    void Start()
    {
        _Player = GameObject.Find("Player");
        _Collider = GetComponent<Collider>();
        _ThisSceneName = SceneManager.GetActiveScene().name;
    }

    private void OnTriggerEnter()
    {
        if (_KeepRotationOnEnter == true)
        {
            _RotationQuat = _Player.transform.localRotation;
        }

        DontDestroyOnLoad(transform.gameObject);
        _Collider.enabled = false;
        SceneManager.LoadScene(_LoadSceneName);
        if (_SetPositionOnEnter == true)
        {
            _Player = GameObject.Find("Player");
            _Player.transform.position = _Position;
            _Player.transform.rotation = new Quaternion(_Rotation.x, _Rotation.y, _Rotation.z, _Player.transform.rotation.w);
        }
    }

    private void Update()
    {
        if (_ThisSceneName != SceneManager.GetActiveScene().name)
        {
            _Player = GameObject.Find("Player");
            if (_SetPositionOnEnter == true)
            {
                _Player.transform.position = _Position;
            }
            if (_KeepRotationOnEnter == true)
            {
                _Player.transform.rotation = _RotationQuat;
            }
            else if (_SetPositionOnEnter == true)
            {
                _Player.transform.rotation = new Quaternion(_Rotation.x, _Rotation.y, _Rotation.z, _Player.transform.rotation.w);
            }

            Destroy(transform.gameObject);
        }
    }
}
