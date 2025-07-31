using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ScannerController : MonoBehaviour{

    const int MAXDISTORSION = 76;
    const int MINDISTORSION = 1;
    const float MAXDISTANCE = 15;
    const float MINDISTANCE = 0.1f;

    public Transform origin;
    public Material scannerMat;
    public float initialScanDistance = 0;
    public float maxScanDistance = 20;
    public float scanThickness;
    public GridGenerator gridGenerator;
    
    [ColorUsage(true, true)]
    public List<Color> colors;

    Camera cam;
    float scanDistance;
    float scanDistance1;
    float scanDistance2;

    IEnumerator scanHandler = null;

    LensDistortion lensDistortion = null;
    DepthOfField dof = null;

    Vector3 toRight;
    Vector3 toTop ;
    Vector3 topLeft ;
    Vector3 topRight;
    Vector3 bottomRight ;
    Vector3 bottomLeft;


    void Start()
    {
        PostProcessVolume ppv = gameObject.GetComponent<PostProcessVolume>();
        ppv.profile.TryGetSettings(out lensDistortion);
        ppv.profile.TryGetSettings(out dof);

        float camFar = cam.farClipPlane;
        float camFov = cam.fieldOfView;
        float camAspect = cam.aspect;

        float fovWHalf = camFov * 0.5f;

        toRight = cam.transform.right * Mathf.Tan(fovWHalf * Mathf.Deg2Rad) * camAspect;
        toTop = cam.transform.up * Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

        topLeft = (cam.transform.forward - toRight + toTop);
        float camScale = topLeft.magnitude * camFar;

        topLeft.Normalize();
        topLeft *= camScale;

        topRight = (cam.transform.forward + toRight + toTop);
        topRight.Normalize();
        topRight *= camScale;

        bottomRight = (cam.transform.forward + toRight - toTop);
        bottomRight.Normalize();
        bottomRight *= camScale;

        bottomLeft = (cam.transform.forward - toRight - toTop);
        bottomLeft.Normalize();
        bottomLeft *= camScale;
    }

    void OnEnable()
    {
        cam = GetComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.Depth;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        scannerMat.SetVector("_WorldSpaceScannerPos", origin.position);
        scannerMat.SetFloat("_ScanDistance1", scanDistance1);
        scannerMat.SetFloat("_ScanDistance2", scanDistance2);
        scannerMat.SetFloat("_ScanWidth", scanThickness);

        if (colors.Count >= 3)
        {
            scannerMat.SetColor("_FirstColor", colors[0]);
            scannerMat.SetColor("_SecondColor", colors[1]);
            scannerMat.SetColor("_FillColor", colors[2]);
        }

        scannerMat.SetTexture("_MainTex", src);

        Graphics.Blit(src, dst, scannerMat);
        raycastCornerBlit();
    }

    void raycastCornerBlit()
    {
        GL.PushMatrix();
        GL.LoadOrtho();

        scannerMat.SetPass(0);

        GL.Begin(GL.QUADS);
        GL.Color(Color.red);

        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.MultiTexCoord(1, bottomLeft);
        GL.Vertex3(0.0f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.MultiTexCoord(1, bottomRight);
        GL.Vertex3(1f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1f, 1f);
        GL.MultiTexCoord(1, topRight);
        GL.Vertex3(1f, 1f, 0.0f);

        GL.MultiTexCoord2(0, 0.0f, 1f);
        GL.MultiTexCoord(1, topLeft);
        GL.Vertex3(0.0f, 1f, 0.0f);

        GL.End();
        GL.PopMatrix();
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Return))
        {
            if(scanDistance <= 0)
            {
                scan();
            }
            else
            {
                scanBack();
            }
        }
    }

    void checkAndBlock()
    {
        if(scanHandler != null)
        {
            StopCoroutine(scanHandler);
        }
    }

    void scan()
    {
        checkAndBlock();
        scanHandler = scanCoroutine();
        StartCoroutine(scanHandler);
        GameManager.Instance.PauseGame();
    }

    void scanBack()
    {
        checkAndBlock();
        scanHandler = scanBackCoroutine();
        StartCoroutine(scanHandler);
    }

    [SerializeField]
    float scanSpeed = 2f;
    IEnumerator scanCoroutine()
    {
        distorsion();
        scanDistance = initialScanDistance;
        bool startShow = false;   
        while(scanDistance <= maxScanDistance)
        {
            if(startShow ==false && maxScanDistance/2 < scanDistance)
            {
                startShow = true;
                gridGenerator.show();
                GameManager.Instance.SeeHideObjects();
            }
            scanDistance1 = scanDistance+ scanSpeed;
            yield return new WaitForSecondsRealtime(.01f);
            scanDistance2 = scanDistance1;
            yield return new WaitForSecondsRealtime(.01f);
            scanDistance2 = scanDistance1+ scanSpeed;
            yield return new WaitForSecondsRealtime(.01f);
            scanDistance += scanSpeed;

        }
        scanDistance = maxScanDistance;  
        scanHandler = null;
    }

    IEnumerator scanBackCoroutine()
    {
        bool startHide = false;   
        while(scanDistance > 0 )
        {
            if(scanDistance < maxScanDistance*0.8f && !startHide)
            {
                gridGenerator.hide();
                startHide = true;
                distorsion();
                GameManager.Instance.NotSeeHideObjects();
            }
            scanDistance1 = scanDistance- scanSpeed;
            yield return new WaitForSecondsRealtime(.01f);
            scanDistance2 = scanDistance1;
            yield return new WaitForSecondsRealtime(.01f);
            scanDistance2 = scanDistance1- scanSpeed;
            yield return new WaitForSecondsRealtime(.01f);
            scanDistance -= scanSpeed;

        }
        scanDistance = initialScanDistance; 
        scanHandler = null;
        GameManager.Instance.PauseGame();    
    }

    void distorsion()
    {
        StartCoroutine(distorsionCoroutine());
    }

    IEnumerator distorsionCoroutine()
    {
        if(dof != null && lensDistortion != null)
        {
            int distAmount = MAXDISTORSION;
            float distance = MINDISTANCE;

            while(MINDISTORSION <= distAmount)
            {
                lensDistortion.intensity.value = distAmount;
                dof.focusDistance.value = distance;
                distAmount -=5 ;
                distance += 0.1f;
                yield return new WaitForSecondsRealtime(.01f);
            }

            lensDistortion.intensity.value = MINDISTORSION;
            dof.focusDistance.value = MAXDISTANCE;
        }
    }
}
