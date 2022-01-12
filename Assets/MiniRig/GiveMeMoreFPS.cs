
using UnityEngine;
using TMPro;
using UnityEngine.XR;
using System.Collections.Generic;
using URP = UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GiveMeMoreFPS : MonoBehaviour
{
    // on screen debug log variables 
    List<string> logLines = new List<string>();
    public Camera mainCamera;
    public TextMeshPro logField;

    public TextMeshPro fpsLabel;


    // quality parameters 

    
    // 0 = Off, 1 = Low, 2 = Medium, 3 = High, 4 = High Top
    // Read more at : https://developer.oculus.com/documentation/unity/unity-fixed-foveated-rendering/
    [Range(0,4)] int FoveationLevel = 0;
    [Range(0.2f,1f)] float resolutionScale = 1f;
    float DisplayFrequency = 72;
    bool spaceWrap = false;
    MsaaQuality MSAA; // 1,2,4,8
    float renderViewportScale;
    // [Range(0,4)] float fovZoomFactor;
    bool spaceWrapState = false;
    URP AssetURP => (URP) GraphicsSettings.currentRenderPipeline ;
    DepthTextureMode dpethOriginal;
    const DepthTextureMode MotionVectors = DepthTextureMode.MotionVectors | DepthTextureMode.Depth;


    // Button click event handle 


    public void OnButtonPress( MiniButton button )
    {
        switch( button.type )
        {
            case "FFR" : FoveationLevel = button.data;              break;
            case "RES" : resolutionScale = button.data / 100f;      break;
            case "HZ"  : DisplayFrequency = button.data;            break;
            case "ASW" : spaceWrap = ! spaceWrap;                   break;
            case "MSA" : MSAA = (MsaaQuality) button.data;          break; 
         // case "FOV" : fovZoomFactor = button.data / 100f;        break;
            case "RVP" : renderViewportScale = button.data / 100f;  break;
        }

        if( Application.platform != RuntimePlatform.Android )
        {
            switch( button.type )
            {
                case "FFR" : case "ASW" : 
                Debug.LogWarning("Operation only supported on android devices");
                break;
            }
        }
    }

    void Awake()
    {
        // Show debug log message on the wall 

        Application.logMessageReceived += ( a, b, c ) => {
            if( c == LogType.Warning ) a = $"<color=\"yellow\">{a}</color>";
            else if( c != LogType.Log )a = $"<color=\"red\">{a}</color>";
            logLines.Add( a );
            logField.text = string.Join("\n", logLines);logField.ForceMeshUpdate();
            if( logField.isTextTruncated )
            for( var i = 0; i < 5 ; ++i ) {
                logField.text = string.Join("\n", logLines);logField.ForceMeshUpdate();
                if( logField.isTextTruncated && logLines.Count > 0 )
                    logLines.RemoveAt( 0 );
            }
        };
    }

    void Start()
    {
        Debug.Log("V 1.0.1");

        // Get default values 
        
        resolutionScale = XRSettings.eyeTextureResolutionScale;
        renderViewportScale = XRSettings.renderViewportScale;
        // fovZoomFactor = XRDevice.fovZoomFactor;
        dpethOriginal = mainCamera.depthTextureMode;
        // cam = Camera.main; if( cam ) cam.stereoTargetEye = StereoTargetEyeMask.None;
        MSAA = (MsaaQuality) AssetURP.msaaSampleCount;

        // Quest values 

        if( OculusNative.IsSupported )
        {
            if( OculusNative.TryGetFFR( out OculusNative.FFR ffr ) )
                FoveationLevel = ( int ) ffr;

            if( Unity.XR.Oculus.Performance.TryGetDisplayRefreshRate( out float rate ) )
                    DisplayFrequency = rate;
        }
    }

    int cooldown = 0;

    void Update()
    {
        if( cooldown-- > 0 ) return; cooldown = 10; // run update once per 10 frames 

        if( renderViewportScale != XRSettings.renderViewportScale ) //! RVP
        {
            Debug.Log($"Render View Port Scale: {XRSettings.renderViewportScale:N3} -> {renderViewportScale:N3}");
            XRSettings.renderViewportScale = renderViewportScale;
            renderViewportScale = XRSettings.renderViewportScale;
        }

        // if( fovZoomFactor != XRDevice.fovZoomFactor )
        // {
        //     // if( cam ) cam.fieldOfView = 60 * fovZoomFactor;

        //     XRDevice.fovZoomFactor = fovZoomFactor;
        //     Debug.Log("FOV Zoom Factor set = " + fovZoomFactor.ToString("N2") );
        // }

        if( XRSettings.eyeTextureResolutionScale != resolutionScale ) //! RES 
        {
            XRSettings.eyeTextureResolutionScale = resolutionScale;
            AssetURP.renderScale = resolutionScale;
            Debug.Log("Resolution Scale set = " + AssetURP.renderScale.ToString("N2") );
        }

        if( (int) MSAA != AssetURP.msaaSampleCount ) //! MSSA
        {
            AssetURP.msaaSampleCount = (int) MSAA;
            Debug.Log("MSAA set = " + (MsaaQuality) AssetURP.msaaSampleCount );
        }

        if( OculusNative.IsSupported ) //! FFR
        {
            var currentFFR = ( OculusNative.FFR ) FoveationLevel;   

            if( OculusNative.TryGetFFR( out OculusNative.FFR ffr1 ) && ffr1 != currentFFR )
            {
                // this doesn't work .. 
                // Unity.XR.Oculus.Utils.EnableDynamicFFR(FoveationLevel > 0);
                // Unity.XR.Oculus.Utils.SetFoveationLevel(FoveationLevel);
                // Debug.Log("GetFoveationLevel set = " + Unity.XR.Oculus.Utils.GetFoveationLevel() );

                if( OculusNative.TrySetFFR( ( OculusNative.FFR ) FoveationLevel ) )
                {
                    if( OculusNative.TryGetFFR( out OculusNative.FFR ffr2 ) )
                    {
                        Debug.Log("GetFoveationLevel set = " + ffr2 );
                    }
                }

                // this seems to work , also checkout : OVRManager.fixedFoveatedRenderingSupported
                // OVRManager.fixedFoveatedRenderingLevel = ( OVRManager.FixedFoveatedRenderingLevel ) FoveationLevel;
                // Debug.Log("GetFoveationLevel set = " + OVRManager.fixedFoveatedRenderingLevel );
            }
        }


        if( OculusNative.IsSupported ) //! HZ
        {
            if( Unity.XR.Oculus.Performance.TryGetDisplayRefreshRate( out float rate ) )
            {
                if( rate != DisplayFrequency )
                {
                    if( Unity.XR.Oculus.Performance.TryGetAvailableDisplayRefreshRates( out float[] rates ) )
                        Debug.Log("Available refresh rates: " + string.Join( ", " , rates ) );

                    if( ! Unity.XR.Oculus.Performance.TrySetDisplayRefreshRate( DisplayFrequency ) )
                    {
                        Debug.LogWarning($"failed to set freq to {DisplayFrequency.ToString("N1")} current {rate.ToString("N1")}");
                        DisplayFrequency = rate;
                    }
                    else 
                    {
                        // if( Oculus.Performance.TryGetDisplayRefreshRate( out float rateNew ) )    
                        //     Debug.Log("DisplayRefreshRate set = " + rateNew.ToString("N1") );
                        // else 
                            Debug.Log("DisplayRefreshRate set = " + DisplayFrequency.ToString("N1") );
                    }

                } 
            }
        }
        else //! FPS
        {
            if( Application.targetFrameRate != (int) DisplayFrequency )
            {
                Application.targetFrameRate = ( int ) DisplayFrequency;
                Debug.Log("Frame Rate set = " + Application.targetFrameRate );
            }
        }

        if( OculusNative.IsSupported ) //! ASW
        {
            if( spaceWrap != spaceWrapState )
            {
                spaceWrapState = spaceWrap;
                OculusNative.SetSpaceWrap( spaceWrapState );
                // OculusXRPlugin.SetSpaceWarp( spaceWrapState ? OVRPlugin.Bool.True : OVRPlugin.Bool.False );
                // OVRManager.SetSpaceWarp( spaceWrapState );
                Debug.Log("SpaceWrap set = " + ( spaceWrapState ? "T" : "F" ) );

                Debug.Log("Current camera depth texture mode: " + mainCamera.depthTextureMode );

                mainCamera.depthTextureMode = spaceWrap ? dpethOriginal | MotionVectors : dpethOriginal;

                Debug.Log("New camera depth texture mode: " + mainCamera.depthTextureMode );
            }
        }
    }

    float fps_time = 0;
    float fps_value = 0;

    int fps_count = 0;

    void LateUpdate()
    {
        fps_count ++ ;

        var dt = Time.deltaTime;

        fps_time += dt;

        if( fps_time >= 1 )
        {
            fps_value = ( fps_count / fps_time );

            fps_time = fps_count = 0;
        }

        fpsLabel.text = fps_value.ToString("N1") + " FPS"
            + "\n" + ( spaceWrap ? fps_value * 2 : fps_value ).ToString("N0") + " Hz"
            + "\n" + ( (int) ( dt * 1000 ) ) + " ms";
    }
}
