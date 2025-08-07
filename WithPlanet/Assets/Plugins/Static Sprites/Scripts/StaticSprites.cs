//------------------------------------
//          Static Sprites
//     Copyright© 2025 OmniShade     
//------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/**
 * Combines static sprites into a singular mesh to improve performance by reducing 
 * dynamic batching. All sprites and child sprites of this object have their renderers
 * disabled on start, and a new single StaticSprite object is generated in their place.
 * Supports sprites with differing textures, materials, colors, sorting orders, and flip XY.
 **/
namespace StaticSprites {
    
    interface IStaticSprites {
        // Statically batch sprites
        void BatchSprites();

        // Revert static sprites back to individual sprites
        void UnbatchSprites();

        // Save out combined sprites. Only available in Editor.
        void Export();

        bool IsBatched { get; }
    }

    [ExecuteInEditMode]
    class StaticSpriteBatch {
        public List<SpriteRendererInfo> rendererInfos = new();
        public Material material;
        public int sortingLayerID;
        public int sortingOrder;
        public GameObject batchedObj;
    }

    struct SpriteRendererInfo {
        public SpriteRenderer spriteRenderer;
        public CombineInstance combineInstance;
    };

    public class StaticSprites : MonoBehaviour, IStaticSprites {
        const string StaticSpritesName = "StaticSprites";
        const string ExportFolder = "Static Sprites Export";

        [Tooltip("Automatically batch sprites on start")]
        public bool BatchOnStart = true;

        public bool IsBatched => IsBatched;
        bool isBatched = false;

        readonly Dictionary<string, StaticSpriteBatch> batches = new();
        readonly Dictionary<string, Mesh> spriteMeshCache = new();
        
        void Start() {
            if (Application.isPlaying && BatchOnStart)
                BatchSprites();
        }

        // Convert sprites to static sprites
        public void BatchSprites() {
            if (!isBatched) {
                FetchSpriteInfoInChildren();
                GenerateStaticSprites();
                isBatched = true;
            }
            else
                Debug.LogWarning("Sprites are already batched, skipping BatchSprites");
        }

        void FetchSpriteInfoInChildren() {
            var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in spriteRenderers) {
                var sprite = spriteRenderer.sprite;
                if (sprite == null) {
                    Debug.LogWarning("No sprite on SpriteRenderer: " + spriteRenderer.name);
                    continue;
                }
                var tex = sprite.texture;
                if (tex == null) {
                    Debug.LogWarning("No texture on Sprite: " + sprite.name);
                    continue;

                }
                var mat = spriteRenderer.sharedMaterial;
                if (mat == null) {
                    Debug.LogWarning("No material on SpriteRenderer: " + spriteRenderer.name);
                    continue;
                }

                FetchSpriteInfo(spriteRenderer);
            }
        }

        void FetchSpriteInfo(SpriteRenderer spriteRenderer) {
            string batchId = GetBatchId(spriteRenderer);

            if (!batches.ContainsKey(batchId))
                batches.Add(batchId, new StaticSpriteBatch());
            var batch = batches[batchId];

            SetBatchMaterial(batch, spriteRenderer);
            SetBatchSpriteInfo(batch, spriteRenderer);
        }

        string GetBatchId(SpriteRenderer spriteRenderer) {
            var mat = spriteRenderer.sharedMaterial;
            var tex = spriteRenderer.sprite.texture;
            string texName = tex == null ? string.Empty : tex.name;
            return texName + " " + spriteRenderer.sortingLayerID + " " + spriteRenderer.sortingOrder + " " + mat.name;
        }

        void SetBatchMaterial(StaticSpriteBatch batch, SpriteRenderer spriteRenderer) {
            if (batch.material == null) {
                var spriteMat = spriteRenderer.sharedMaterial;
                var tex = spriteRenderer.sprite.texture;
                batch.material = new Material(spriteMat) {
                    name = spriteMat.name + "(" + tex.name + ")",
                    mainTexture = tex,
                };
            }
        }

        void SetBatchSpriteInfo(StaticSpriteBatch batch, SpriteRenderer spriteRenderer) {
            SpriteRendererInfo info;
            info.spriteRenderer = spriteRenderer;
            info.combineInstance = new CombineInstance {
                mesh = CreateSpriteMesh(spriteRenderer),
                transform = spriteRenderer.transform.localToWorldMatrix
            };
            batch.sortingLayerID = spriteRenderer.sortingLayerID;
            batch.sortingOrder = spriteRenderer.sortingOrder;
            batch.rendererInfos.Add(info);
        }

        Mesh CreateSpriteMesh(SpriteRenderer spriteRenderer) {
            var sprite = spriteRenderer.sprite;
            string spriteId = GetSpriteId(spriteRenderer);
            if (spriteMeshCache.ContainsKey(spriteId))
                return spriteMeshCache[spriteId];

            // Create mesh with copied vertices
            var mesh = new Mesh();
            var vertices = new List<Vector3>();
            // If FlipXY, need to reverse vertex sign
            if (spriteRenderer.flipX || spriteRenderer.flipY) {
                foreach (Vector2 vert in sprite.vertices) {
                    Vector2 vert2 = vert;
                    if (spriteRenderer.flipX)
                        vert2.x = -vert2.x;
                    if (spriteRenderer.flipY)
                        vert2.y = -vert2.y;
                    vertices.Add(vert2);
                }
            }
            else {
                foreach (Vector2 vert in sprite.vertices)
                    vertices.Add(vert);
            }
            mesh.SetVertices(vertices);
            mesh.SetTriangles(sprite.triangles, 0);
            mesh.SetUVs(0, sprite.uv);

            // Set color
            var colors = new Color[vertices.Count];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = spriteRenderer.color;
            mesh.SetColors(colors);

            spriteMeshCache.Add(spriteId, mesh);
            return mesh;
        }

        string GetSpriteId(SpriteRenderer spriteRenderer) {
            var sprite = spriteRenderer.sprite;
            string spriteId = sprite == null ? string.Empty : sprite.GetHashCode().ToString();
            string color = GetColorId(spriteRenderer.color);
            return spriteId + " " + color + " " + spriteRenderer.flipX + " " + spriteRenderer.flipY;
        }


        string GetColorId(Color color) {
            return "RGBA(" + color.r.ToString("0.###") + " " + 
                color.g.ToString("0.###") + " " + 
                color.b.ToString("0.###") + " " + 
                color.a.ToString("0.###") + ")";
        }

        void GenerateStaticSprites() {
            var staticSpritesGO = new GameObject(StaticSpritesName);
            staticSpritesGO.transform.SetParent(transform);

            foreach (var kvp in batches) {
                string batchId = kvp.Key;
                var batch = kvp.Value;

                // Create StaticSprites game object
                var go = new GameObject(batchId);
                batch.batchedObj = go;
                go.transform.SetParent(staticSpritesGO.transform);

                // Add mesh filter
                var meshFilter = go.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = new Mesh() {
                    name = batchId
                };

                // Combine meshes and disable the sprite renderer
                var combineInstances = new List<CombineInstance>();
                foreach (var info in batch.rendererInfos) {
                    combineInstances.Add(info.combineInstance);
                    info.spriteRenderer.enabled = false;
                }
                meshFilter.sharedMesh.CombineMeshes(combineInstances.ToArray());
                meshFilter.sharedMesh.UploadMeshData(true);

                // Add mesh renderer with material
                var meshRenderer = go.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = batch.material;

                // Set sorting order and layer
                meshRenderer.sortingLayerID = batch.sortingLayerID;
                meshRenderer.sortingOrder = batch.sortingOrder;
            }
        }

        // Revert static sprites back to individual sprites
        public void UnbatchSprites() {
            if (isBatched) {
                var staticSpritesGO = GameObject.Find(StaticSpritesName);
                if (staticSpritesGO != null) {
                    if (Application.isPlaying)
                        Destroy(staticSpritesGO);
                    else
                        DestroyImmediate(staticSpritesGO);
                }
                    
                // Re-enable sprite renderers
                var spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
                foreach (var spriteRenderer in spriteRenderers)
                    spriteRenderer.enabled = true;

                isBatched = false;
            }
            else
                Debug.LogWarning("Sprites are not batched, skipping UnbatchSprites.");
        }

        public void Export() {
#if UNITY_EDITOR
            const string AssetsFolder = "Assets";
            const string exportFolder = AssetsFolder + "/" + ExportFolder + "/";
            if (!AssetDatabase.IsValidFolder(exportFolder))
                AssetDatabase.CreateFolder(AssetsFolder, ExportFolder);

            // Batch if needed
            bool isBatchedPrev = isBatched;
            if (!isBatched)
                BatchSprites();

            bool isError = false;
            var atlasCache = new Dictionary<string, Texture>();
            try {
                foreach (var kvp in batches) {
                    string batchName = kvp.Key;
                    var batch = kvp.Value;
                    var batchedObj = batch.batchedObj;

                    // Check if sprite atlas, sanitize name if it is
                    bool isSpriteAtlas = batchName.Contains("sactx");
                    if (isSpriteAtlas)
                        batchName = batchName.Replace("|", " ");
                    string basePath = exportFolder + batchName;

                    // Get material
                    string matPath = basePath + ".mat";
                    var renderer = batchedObj.GetComponent<MeshRenderer>();
                    var mat = renderer.sharedMaterial;

                    // Save out atlas texture for atlases
                    if (isSpriteAtlas) {
                        // Shorten atlas name
                        string texPath = basePath + ".png";
                        var match = Regex.Match(texPath, "(.*) \\d \\d ");
                        if (match.Success)
                            texPath = match.Groups[1].Value + ".png";

                        // Create atlas texture if not in cache
                        if (!atlasCache.TryGetValue(texPath, out Texture atlasTex))
                            atlasTex = SaveSpriteAtlas(texPath, mat.mainTexture);
                        mat.mainTexture = atlasTex;
                    }

                    // Save out material
                    AssetDatabase.CreateAsset(mat, matPath);
                    renderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(matPath);

                    // Save out mesh
                    string meshPath = basePath + ".mesh";
                    var meshFilter = batchedObj.GetComponent<MeshFilter>();
                    var mesh = meshFilter.sharedMesh;
                    AssetDatabase.CreateAsset(mesh, meshPath);
                    meshFilter.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);

                    string prefabPath = basePath + ".prefab";
                    PrefabUtility.SaveAsPrefabAsset(batchedObj, prefabPath);
                }
            }
            catch (Exception e) {
                EditorUtility.DisplayDialog(StaticSpritesName, "Export failed:\n" + e.Message, "Close");
                isError = true;
            }

            // Unbatch if needed
            if (!isBatchedPrev)
                UnbatchSprites();

            if (!isError)
                EditorUtility.DisplayDialog(StaticSpritesName, "Exported to\n" + exportFolder, "Close");
#endif
        }

#if UNITY_EDITOR
        Texture SaveSpriteAtlas(string texPath, Texture tex) {
            // Save out a Unity sprite atlas
            // Since atlases are not readable, this method blits it to a render target before saving

            // Blit tex to render texture
            RenderTexture renderTex = RenderTexture.GetTemporary(tex.width, tex.height);
            Graphics.Blit(tex, renderTex);

            // Read render texture to texture
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableTex = new(tex.width, tex.height);
            readableTex.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableTex.Apply();

            // Restore previous state
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);

            // Save as sprite
            File.WriteAllBytes(texPath, readableTex.EncodeToPNG());
            AssetDatabase.Refresh();
            TextureImporter ti = AssetImporter.GetAtPath(texPath) as TextureImporter;
            if (ti != null)
                ti.textureType = TextureImporterType.Sprite;

            return AssetDatabase.LoadAssetAtPath<Texture>(texPath);
        }
#endif        
    }
}
