using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    public int FPS = 60;

    void Start()
    {
        Application.targetFrameRate = FPS;
    }

}
