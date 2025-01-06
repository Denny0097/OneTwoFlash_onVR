using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wave.Essence.Eye;
using Wave.XR;

public class FocusEyeData
{
    // Combined Eye:
    public Vector3 CombinedEyeOrigin;
    public Vector3 CombindedEyeDirectionNormalized;

    // Left Eye:
    public Vector3 LeftEyeOrigin;
    public Vector3 LeftEyeDirectionNormalized;
    public float LeftEyeOpenness;
    public float LeftEyePupilDiameter;
    public Vector2 LeftEyePupilPositionInSensorArea;

    // Right Eye:
    public Vector3 RightEyeOrigin;
    public Vector3 RightEyeDirectionNormalized;
    public float RightEyeOpenness;
    public float RightEyePupilDiameter;
    public Vector2 RightEyePupilPositionInSensorArea;
}

public class FocusEyeDataManager : MonoBehaviour
{
    public FocusEyeData GetEyeData()
    {
        FocusEyeData data = new FocusEyeData();

        if (EyeManager.Instance == null)
        {
            Debug.LogError("EyeManager.Instance is null");
            return null;
        }

        bool result = true;

        result &= EyeManager.Instance.GetCombinedEyeOrigin(out data.CombinedEyeOrigin);
        result &= EyeManager.Instance.GetCombindedEyeDirectionNormalized(out data.CombindedEyeDirectionNormalized);
        result &= EyeManager.Instance.GetLeftEyeOrigin(out data.LeftEyeOrigin);
        result &= EyeManager.Instance.GetLeftEyeDirectionNormalized(out data.LeftEyeDirectionNormalized);
        result &= EyeManager.Instance.GetLeftEyeOpenness(out data.LeftEyeOpenness);
        result &= EyeManager.Instance.GetLeftEyePupilDiameter(out data.LeftEyePupilDiameter);
        result &= EyeManager.Instance.GetLeftEyePupilPositionInSensorArea(out data.LeftEyePupilPositionInSensorArea);
        result &= EyeManager.Instance.GetRightEyeOrigin(out data.RightEyeOrigin);
        result &= EyeManager.Instance.GetRightEyeDirectionNormalized(out data.RightEyeDirectionNormalized);
        result &= EyeManager.Instance.GetRightEyeOpenness(out data.RightEyeOpenness);
        result &= EyeManager.Instance.GetRightEyePupilDiameter(out data.RightEyePupilDiameter);
        result &= EyeManager.Instance.GetRightEyePupilPositionInSensorArea(out data.RightEyePupilPositionInSensorArea);

        if (!result)
        {
            Debug.LogWarning("Failed to get all eye tracking data");
            return null;
        }

        return data;
    }

    // Check if user's eyes are closed over 5s
    public void CheckEyesOpen(FocusEyeData data)
    {
        if (EyeManager.Instance == null)
        {
            Debug.LogError("EyeManager.Instance is null");
            return;
        }

        bool IsEyesOpen;
        EyeManager.Instance.GetLeftEyeOpenness(out data.LeftEyeOpenness);
        EyeManager.Instance.GetRightEyeOpenness(out data.RightEyeOpenness);

        IsEyesOpen = data.LeftEyeOpenness >= 0.5f || data.RightEyeOpenness >= 0.5f;

        if (!IsEyesOpen)
        {
            Debug.Log("Eyes are closed");
        }
        else
        {
            Debug.Log("Eyes are open");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckEyeManagerReady());
    }

    private IEnumerator CheckEyeManagerReady()
    {
        while (EyeManager.Instance == null || !EyeManager.Instance.EnableEyeTracking)
        {
            Debug.Log("Waiting for EyeManager to be ready...");
            yield return null; // 等待下一帧再检查
        }

        EyeManager.Instance.EnableEyeTracking = true;

        // EyeManager is ready, now you can start your experiment or other logic
        Debug.Log("EyeManager is ready. Starting experiment...");

        // 例如：开始实验
        // StartExperiment();
    }

    // Update is called once per frame
    void Update()
    {
        // Add any necessary update logic here
    }
}
