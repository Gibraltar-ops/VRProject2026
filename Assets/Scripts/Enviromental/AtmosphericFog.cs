using System;
using UnityEngine;

public class AtmosphericFog : MonoBehaviour
{
    [Header("Fog settings")]
    public Color groundFogColor = new Color(0.8f, 0.85f, 0.9f);
    public Color highAltitudeColor = new Color(0.95f, 0.98f, 1.0f);

    public float groundFogEndDistance = 800f;
    public float highAltitudeEndDistance = 150f;

    public Camera playerCamera;

    private void Start()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {
        if (GameManager.instance == null) return;

        float currentHeight = GameManager.instance.Altitude;
        float maxHeight = GameManager.instance.WinAltitude;

        float t = Mathf.InverseLerp(0f, maxHeight, currentHeight);
        
        Color currentFogColor = Color.Lerp(groundFogColor, highAltitudeColor, t);
        float currentFogDistance = Mathf.Lerp(groundFogEndDistance, highAltitudeEndDistance, t);

        RenderSettings.fogColor = currentFogColor;
        RenderSettings.fogEndDistance = currentFogDistance;

        if (playerCamera != null)
        {
            playerCamera.backgroundColor = currentFogColor;
        }
    }



}

