using UnityEngine;

public class HouseMaterialApplier : MonoBehaviour
{
    [Header("House Material Settings")]
    public Material houseMaterial;
    public bool applyOnStart = true;
    
    [Header("Texture Settings")]
    public Texture2D brickTexture;
    public Texture2D woodTexture;
    public Texture2D mossTexture;
    public Texture2D weatheringTexture;
    public Texture2D blendMask;
    
    [Header("Material Properties")]
    [Range(0.1f, 10f)]
    public float brickScale = 2f;
    [Range(0.1f, 10f)]
    public float woodScale = 1.5f;
    [Range(0f, 1f)]
    public float weathering = 0.2f;
    [Range(0f, 1f)]
    public float mossAmount = 0.1f;
    [Range(0f, 1f)]
    public float materialBlend = 0.5f;
    
    [Header("Colors")]
    public Color brickColor = new Color(0.7f, 0.3f, 0.2f, 1f);
    public Color woodColor = new Color(0.6f, 0.4f, 0.2f, 1f);
    public Color mossColor = new Color(0.2f, 0.4f, 0.1f, 1f);
    
    void Start()
    {
        if (applyOnStart)
        {
            ApplyHouseMaterial();
        }
    }
    
    [ContextMenu("Apply House Material")]
    public void ApplyHouseMaterial()
    {
        if (houseMaterial == null)
        {
            Debug.LogError("House material is not assigned!");
            return;
        }
        
        // Get all renderers in this object and children
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        
        foreach (Renderer renderer in renderers)
        {
            // Apply the house material
            renderer.material = houseMaterial;
            
            // Set up the material properties
            SetupMaterialProperties(renderer.material);
        }
        
        Debug.Log($"Applied house material to {renderers.Length} renderers");
    }
    
    private void SetupMaterialProperties(Material material)
    {
        // Set textures if provided
        if (brickTexture != null)
            material.SetTexture("_BrickTexture", brickTexture);
        if (woodTexture != null)
            material.SetTexture("_WoodTexture", woodTexture);
        if (mossTexture != null)
            material.SetTexture("_MossTexture", mossTexture);
        if (weatheringTexture != null)
            material.SetTexture("_WeatheringTexture", weatheringTexture);
        if (blendMask != null)
            material.SetTexture("_BlendMask", blendMask);
        
        // Set colors
        material.SetColor("_BrickColor", brickColor);
        material.SetColor("_WoodColor", woodColor);
        material.SetColor("_MossColor", mossColor);
        
        // Set scales and amounts
        material.SetFloat("_BrickScale", brickScale);
        material.SetFloat("_WoodScale", woodScale);
        material.SetFloat("_Weathering", weathering);
        material.SetFloat("_MossAmount", mossAmount);
        material.SetFloat("_MaterialBlend", materialBlend);
        
        // Set surface properties for convincing house appearance
        material.SetFloat("_Smoothness", 0.3f);
        material.SetFloat("_Metallic", 0f);
        material.SetFloat("_BrickRoughness", 0.8f);
        material.SetFloat("_WoodRoughness", 0.6f);
    }
    
    [ContextMenu("Create Default House Material")]
    public void CreateDefaultHouseMaterial()
    {
        // Create a new material with the house shader
        Material newMaterial = new Material(Shader.Find("Custom/House Material"));
        
        // Set default house-like properties
        newMaterial.SetColor("_BaseColor", new Color(0.8f, 0.6f, 0.4f, 1f));
        newMaterial.SetColor("_BrickColor", brickColor);
        newMaterial.SetColor("_WoodColor", woodColor);
        newMaterial.SetColor("_MossColor", mossColor);
        
        newMaterial.SetFloat("_BrickScale", brickScale);
        newMaterial.SetFloat("_WoodScale", woodScale);
        newMaterial.SetFloat("_Weathering", weathering);
        newMaterial.SetFloat("_MossAmount", mossAmount);
        newMaterial.SetFloat("_MaterialBlend", materialBlend);
        newMaterial.SetFloat("_Smoothness", 0.3f);
        newMaterial.SetFloat("_Metallic", 0f);
        newMaterial.SetFloat("_BrickRoughness", 0.8f);
        newMaterial.SetFloat("_WoodRoughness", 0.6f);
        
        houseMaterial = newMaterial;
        
        Debug.Log("Created default house material");
    }
    
    [ContextMenu("Generate Procedural House Textures")]
    public void GenerateProceduralTextures()
    {
        // Generate a simple brick pattern
        brickTexture = GenerateBrickTexture(256, 256);
        
        // Generate a wood grain pattern
        woodTexture = GenerateWoodTexture(256, 256);
        
        // Generate a moss pattern
        mossTexture = GenerateMossTexture(256, 256);
        
        // Generate a weathering pattern
        weatheringTexture = GenerateWeatheringTexture(256, 256);
        
        // Generate a blend mask
        blendMask = GenerateBlendMask(256, 256);
        
        Debug.Log("Generated procedural house textures");
    }
    
    private Texture2D GenerateBrickTexture(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        
        int brickWidth = width / 8;
        int brickHeight = height / 4;
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int brickX = x / brickWidth;
                int brickY = y / brickHeight;
                
                // Create brick pattern
                float brickValue = 0.8f;
                if (brickX % 2 == 1)
                {
                    brickY += 1;
                }
                
                // Add mortar lines
                if (x % brickWidth < 2 || y % brickHeight < 2)
                {
                    brickValue = 0.3f;
                }
                
                // Add some variation
                brickValue += Random.Range(-0.1f, 0.1f);
                brickValue = Mathf.Clamp01(brickValue);
                
                Color color = new Color(brickValue, brickValue * 0.8f, brickValue * 0.6f, 1f);
                texture.SetPixel(x, y, color);
            }
        }
        
        texture.Apply();
        return texture;
    }
    
    private Texture2D GenerateWoodTexture(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Create wood grain pattern
                float grain = Mathf.PerlinNoise(x * 0.01f, y * 0.01f);
                float rings = Mathf.PerlinNoise(x * 0.02f, 0);
                
                float woodValue = (grain + rings) * 0.5f + 0.3f;
                woodValue += Random.Range(-0.05f, 0.05f);
                woodValue = Mathf.Clamp01(woodValue);
                
                Color color = new Color(woodValue, woodValue * 0.7f, woodValue * 0.4f, 1f);
                texture.SetPixel(x, y, color);
            }
        }
        
        texture.Apply();
        return texture;
    }
    
    private Texture2D GenerateMossTexture(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Create moss pattern using Perlin noise
                float moss = Mathf.PerlinNoise(x * 0.02f, y * 0.02f);
                moss = Mathf.Pow(moss, 2); // Make it more clustered
                
                Color color = new Color(0.2f, 0.4f, 0.1f, moss);
                texture.SetPixel(x, y, color);
            }
        }
        
        texture.Apply();
        return texture;
    }
    
    private Texture2D GenerateWeatheringTexture(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Create weathering pattern
                float weathering = Mathf.PerlinNoise(x * 0.01f, y * 0.01f);
                weathering += Mathf.PerlinNoise(x * 0.005f, y * 0.005f) * 0.5f;
                weathering = Mathf.Clamp01(weathering);
                
                Color color = new Color(weathering, weathering, weathering, 1f);
                texture.SetPixel(x, y, color);
            }
        }
        
        texture.Apply();
        return texture;
    }
    
    private Texture2D GenerateBlendMask(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Create a blend mask that separates brick and wood areas
                float blend = Mathf.PerlinNoise(x * 0.01f, y * 0.01f);
                
                // Make it more binary (brick vs wood)
                blend = blend > 0.5f ? 1f : 0f;
                
                Color color = new Color(blend, blend, blend, 1f);
                texture.SetPixel(x, y, color);
            }
        }
        
        texture.Apply();
        return texture;
    }
} 