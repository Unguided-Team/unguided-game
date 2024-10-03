using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private GameObject CinemachineVCamObject;
    public UnityEvent heavyCameraShakeEvent;
    public UnityEvent lightCameraShakeEvent;

    private CinemachineVirtualCamera cvm;
    [SerializeField] private float defaultShakeIntensity = 1f;
    [SerializeField] private float defaultShakeTime = 0.2f;
    [SerializeField] private float heavyShakeIntensity = 0.5f;
    [SerializeField] private float heavyShakeTime = 0.2f;
    [SerializeField] private float lightShakeIntensity = 0.5f;
    [SerializeField] private float lightShakeTime = 0.1f;

    private float timer = 0;
    private CinemachineBasicMultiChannelPerlin _cbmcp;

    private bool cameraShaking = false;

    void Awake()
    {
        cvm = CinemachineVCamObject.GetComponent<CinemachineVirtualCamera>();
        _cbmcp = cvm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        heavyCameraShakeEvent.AddListener(StartHeavyShake);
        lightCameraShakeEvent.AddListener(StartLightShake);
    }

    void Start()
    {
        StopShake();
    }

    void Update()
    {
        if (cameraShaking)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
                StopShake();
        }
        else 
        {
            if (Input.GetKeyDown(KeyCode.P))
                StartShake(defaultShakeIntensity, defaultShakeTime);
        }
    }

    private void StartHeavyShake()
    {
        StartShake(heavyShakeIntensity, heavyShakeTime);
    }

    private void StartLightShake()
    {
        StartShake(lightShakeIntensity, lightShakeTime);
    }

    private void StartShake(float shakeIntensity, float shakeTime)
    {
        _cbmcp.m_AmplitudeGain = shakeIntensity;
        timer = shakeTime;
        cameraShaking = true;
    }

    private void StopShake()
    {
        _cbmcp.m_AmplitudeGain = 0f;
        timer = 0;
        cameraShaking = false;
    }
}
