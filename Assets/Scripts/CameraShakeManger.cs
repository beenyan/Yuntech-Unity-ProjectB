using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraShakeManger: MonoBehaviour {
    public static CameraShakeManger Instance { get; private set; }
    private CinemachineVirtualCamera CinemachineVirtualCamera;
    private float ShakeTimer;
    private void Awake() {
        Instance = this;
        CinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void ShakeCamera(float intensity, float time) {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
            CinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        ShakeTimer = time;
    }

    private void Update() {
        if (ShakeTimer > 0) {
            ShakeTimer -= Time.deltaTime;
            if (ShakeTimer <= 0f) {
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
                    CinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
            }
        }
    }
}
