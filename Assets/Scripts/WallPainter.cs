using UnityEngine;

public class WallPainter : MonoBehaviour
{
    [Header("Painting")]
    [SerializeField]
    private int brushRadius = 16; // Brush size in pixels
    public Camera cam;
    public Transform CamTransform;

    private Renderer renderer;
    private Texture2D texture;
    private float totalPixelCount = 16384f; // width * height (128x128)
    private float paintedPixelCount = 0f;

    private Vector3 _maxBound;
    private Vector3 _minBound;
    private float _distance;
    private void Start()
    {
        renderer = GetComponent<Renderer>();
        texture = Instantiate(renderer.material.mainTexture) as Texture2D;
        renderer.material.mainTexture = texture;

        var bounds = renderer.bounds;
        _minBound = bounds.min;
        _maxBound = bounds.max;
        _distance = Vector3.Dot(transform.position - CamTransform.position, CamTransform.forward);

    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            var clickPos = Input.mousePosition;
            
            clickPos.z = _distance;
            var pos = cam.ScreenToWorldPoint(clickPos);

            float normalizedX = (pos.x - _minBound.x) / (_maxBound.x - _minBound.x); // Normalize x to [0, 1]
            float normalizedY = (pos.y - _minBound.y) / (_maxBound.y - _minBound.y); // Normalize y to [0, 1]

            int x = Mathf.FloorToInt(normalizedX * texture.width);
            int y = Mathf.FloorToInt(normalizedY * texture.height);

            int xLimit = Mathf.Min(x + brushRadius + 1, texture.width);
            int yLimit = Mathf.Min(y + brushRadius + 1, texture.height);

            for (int u = Mathf.Max(0, x - brushRadius); u < xLimit; u++)
            {
                for (int v = Mathf.Max(0, y - brushRadius); v < yLimit; v++)
                {
                    if ((x - u) * (x - u) + (y - v) * (y - v) < brushRadius * brushRadius)
                    {
                        if (texture.GetPixel(u, v).Equals(Color.white)) // Change this condition if needed
                        {
                            texture.SetPixel(u, v, Color.red);
                            paintedPixelCount += 1.0f;
                        }
                    }
                }
            }

            texture.Apply(); // Apply changes to the texture
        }

        if (paintedPixelCount >= totalPixelCount)
        {
            Debug.Log("yes");
            ClearTexture();
        }
    }

    private void ClearTexture()
    {
        var clearColors = new Color[texture.width * texture.height];
        for (int i = 0; i < clearColors.Length; i++)
        {
            clearColors[i] = Color.white; // Set to white or any color you prefer
        }
        
        texture.SetPixels(clearColors);
        texture.Apply();
        paintedPixelCount = 0;
    }
}