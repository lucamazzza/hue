using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ColoringController : MonoBehaviour
{
    private GameObject coloringTarget;
    private Renderer targetRenderer;
    private Texture2D drawTexture;
    private Color[] brushPixels;
    private int drawColor;
    private float drawTime;

    [Header("Camera Setup")]
    [Tooltip("Trascina qui la ColoringCamera dedicata")]
    public Camera drawingCamera; // Sostituisce Camera.main

    [Header("Configurazione Pennello")]
    public float brushSize = 0.05f; 
    public float colorDrainRate = 0.1f; 

    [Header("Input System References")]
    public InputActionReference drawAction;
    public InputActionReference pointAction;

    [Header("Layer Mask Setup")]
    public LayerMask drawableLayer;
    public LayerMask drawingBaseLayer;

    [Header("UI References")]
    public Button[] colorButtons;
    public Color[] colors;
    public RectTransform redBar;
    public RectTransform blueBar;
    public RectTransform yellowBar;

    [Header("Color Management")]
    public int maxRed = 100;
    public int maxBlue = 100;
    public int maxYellow = 100;
    private int currentRed;
    private int currentBlue;
    private int currentYellow;

    void Start()
    {
        for (int i = 0; i < colorButtons.Length; i++)
        {
            int buttonIndex = i;
            colorButtons[i].onClick.AddListener(() => OnColorButtonClicked(buttonIndex));
        }
    }

    public void Initialize(GameObject target, Texture2D savedTexture = null)
    {
        currentRed = maxRed;
        currentBlue = maxBlue;
        currentYellow = maxYellow;
        drawColor = 0;
        updateColorBars();

        coloringTarget = target;

        Transform drawingTransform = coloringTarget.transform.Find("Drawing");
        Transform cardboardTransform = coloringTarget.transform.Find("Cardboard");

        if (drawingTransform == null)
        {
            Debug.LogError("Errore: Figlio 'Drawing' non trovato nel prefab!");
            return;
        }

        if (!drawingTransform.GetComponent<Collider>()) drawingTransform.gameObject.AddComponent<MeshCollider>();
        if (cardboardTransform != null && !cardboardTransform.GetComponent<Collider>()) cardboardTransform.gameObject.AddComponent<MeshCollider>();

        targetRenderer = drawingTransform.GetComponent<Renderer>();
        
        Texture2D baseTexture = (Texture2D)targetRenderer.material.mainTexture;
        Texture2D source = (savedTexture != null) ? savedTexture : baseTexture;

        drawTexture = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        drawTexture.SetPixels(source.GetPixels());
        drawTexture.Apply();

        targetRenderer.material.mainTexture = drawTexture;

        createBrushPattern();
    }

    void Update()
    {
        if (drawTexture != null && drawAction.action.IsPressed())
        {
            DrawAtMousePosition();
        }
    }

    void DrawAtMousePosition()
    {
        if (drawingCamera == null) return;

        Vector2 mousePos = pointAction.action.ReadValue<Vector2>();
        Ray ray = drawingCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, drawingBaseLayer))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, drawableLayer))
            {
                if (hasEnoughColor())
                {
                    DrawOnTexture(hit.textureCoord);

                    drawTime += Time.deltaTime;
                    if (drawTime >= colorDrainRate)
                    {
                        drainColor();
                        drawTime = 0;
                    }
                }
            }
        }
    }

    void DrawOnTexture(Vector2 uv)
    {
        int x = Mathf.RoundToInt(uv.x * drawTexture.width);
        int y = Mathf.RoundToInt(uv.y * drawTexture.height);

        int brushRadius = Mathf.RoundToInt(brushSize * drawTexture.width);
        
        for (int i = -brushRadius; i <= brushRadius; i++)
        {
            for (int j = -brushRadius; j <= brushRadius; j++)
            {
                if (i * i + j * j <= brushRadius * brushRadius)
                {
                    int px = x + i;
                    int py = y + j;

                    if (px >= 0 && px < drawTexture.width && py >= 0 && py < drawTexture.height)
                    {
                        drawTexture.SetPixel(px, py, colors[drawColor]);
                    }
                }
            }
        }
        drawTexture.Apply();
    }

    public Texture2D GetCurrentTexture() => drawTexture;

    void createBrushPattern()
    {
        int brushDiameter = Mathf.RoundToInt(brushSize * (drawTexture != null ? drawTexture.width : 512));
        brushPixels = new Color[brushDiameter * brushDiameter];
        for (int i = 0; i < brushPixels.Length; i++) brushPixels[i] = colors[drawColor];
    }

    void OnColorButtonClicked(int index)
    {
        if (index < colors.Length)
        {
            drawColor = index;
            createBrushPattern();
        }
    }

    void drainColor()
    {
        switch (drawColor)
        {
            case 0: currentRed -= 2; break;
            case 1: currentYellow -= 2; break;
            case 2: currentBlue -= 2; break;
            case 3: currentRed--; currentYellow--; break;
            case 4: currentYellow--; currentBlue--; break;
            case 5: currentRed--; currentBlue--; break;
        }
        updateColorBars();
    }

    bool hasEnoughColor()
    {
        switch (drawColor)
        {
            case 0: return currentRed >= 2;
            case 1: return currentYellow >= 2;
            case 2: return currentBlue >= 2;
            case 3: return currentRed >= 1 && currentYellow >= 1;
            case 4: return currentYellow >= 1 && currentBlue >= 1;
            case 5: return currentRed >= 1 && currentBlue >= 1;
            default: return false;
        }
    }

    void updateColorBars()
    {
        if (redBar == null || blueBar == null || yellowBar == null) return;
        SetBarScale(redBar, currentRed, maxRed);
        SetBarScale(blueBar, currentBlue, maxBlue);
        SetBarScale(yellowBar, currentYellow, maxYellow);
    }

    void SetBarScale(RectTransform bar, int current, int max)
    {
        if (bar.childCount == 0) return;
        RectTransform fill = (RectTransform)bar.transform.GetChild(0);
        float ratio = Mathf.Clamp01((float)current / (float)max);
        fill.localScale = new Vector3(1, ratio, 1); 
    }
}