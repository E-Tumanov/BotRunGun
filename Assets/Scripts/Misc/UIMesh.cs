//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



/*



    Сейчас работает только если меш нормализован.
    Но тогда херня получается со скейлом. Точнее со стречем. Надо думать.

    Большинство плагов генерят меши

    Может какойто универсальный механизм  для "уголки"/"тени"/"bevel" /"outline"
    Даже не реализацию. А просто подход. Т.е. по необходимости берём и прогаем
    нужный элемент.

    и т.д. только всё это векторное

    + А вот градиенты, надо считать уже в шейдере. !!!! Тогда будет бачинг рваться.
        ну решение конечно банальное - держи интерфес простым. Хотя линейный градиент
        можно бы и впилить по умолчанию. 

*/



// For a discussion of the code, see: https://www.hallgrimgames.com/blog/2018/11/25/custom-unity-ui-meshes

public class UIMesh : MaskableGraphic
{
    public float GridCellSize = 40f;

    [SerializeField] Mesh mesh;
    [SerializeField] bool flipTexU = false;
    [SerializeField] bool flipTexV = false;
    

    [SerializeField] Texture m_Texture;

    public float SC = 1;
    // make it such that unity will trigger our ui element to redraw whenever we change the texture in the inspector
    public Texture texture
    {
        get
        {
            return m_Texture;
        }
        set
        {
            if (m_Texture == value)
                return;

            m_Texture = value;
            SetVerticesDirty();
            SetMaterialDirty();
        }
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        SetVerticesDirty();
        SetMaterialDirty();
    }

    // if no texture is configured, use the default white texture as mainTexture
    public override Texture mainTexture
    {
        get
        {
            return m_Texture == null ? s_WhiteTexture : m_Texture;
        }
    }

    // helper to easily create quads for our ui mesh. You could make any triangle-based geometry other than quads, too!
    void AddQuad(VertexHelper vh, Vector2 corner1, Vector2 corner2, Vector2 uvCorner1, Vector2 uvCorner2)
    {
        var i = vh.currentVertCount;

        UIVertex vert = new UIVertex();
        vert.color = this.color;  // Do not forget to set this, otherwise 

        vert.position = corner1;
        vert.uv0 = uvCorner1;
        vh.AddVert(vert);

        vert.position = new Vector2(corner2.x, corner1.y);
        vert.uv0 = new Vector2(uvCorner2.x, uvCorner1.y);
        vh.AddVert(vert);

        vert.position = corner2;
        vert.uv0 = uvCorner2;
        vh.AddVert(vert);

        vert.position = new Vector2(corner1.x, corner2.y);
        vert.uv0 = new Vector2(uvCorner1.x, uvCorner2.y);
        vh.AddVert(vert);

        vh.AddTriangle(i + 0, i + 2, i + 1);
        vh.AddTriangle(i + 3, i + 2, i + 0);
    }


    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        // Bottom left corner of the full RectTransform of our UI element
        
        var blc = new Vector2(0, 0) - rectTransform.pivot;
        blc.x *= rectTransform.rect.width;
        blc.y *= rectTransform.rect.height;

        foreach (var v in mesh.vertices)
        {
            UIVertex vert = new UIVertex();
            vert.color = this.color;  // Do not forget to set this, otherwise 

            vert.position = new Vector2(blc.x + Random.value * rectTransform.rect.width, 
                blc.y + Random.value * rectTransform.rect.height);

            vert.position = new Vector2(blc.x - v.x * rectTransform.rect.width, blc.y + v.y * rectTransform.rect.height);

            vert.uv0 = new Vector2(-v.x, v.y);
            if (flipTexV)
                vert.uv0.y = 1 - vert.uv0.y;
            if (flipTexU)
                vert.uv0.x = 1 - vert.uv0.x;
            vh.AddVert(vert);
        }

        for (int i=0; i< mesh.triangles.Length / 3; i++)
        {
            vh.AddTriangle(mesh.triangles[i*3+0], mesh.triangles[i * 3 + 1], mesh.triangles[i * 3 + 2]);
        }

        //Debug.Log("Mesh was redrawn!");
    }
        /*
        // actually update our mesh
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            // Let's make sure we don't enter infinite loops
            if (GridCellSize <= 0)
            {
                GridCellSize = 1f;
                Debug.LogWarning("GridCellSize must be positive number. Setting to 1 to avoid problems.");
            }

            // Clear vertex helper to reset vertices, indices etc.
            vh.Clear();

            // Bottom left corner of the full RectTransform of our UI element
            var bottomLeftCorner = new Vector2(0, 0) - rectTransform.pivot;
            bottomLeftCorner.x *= rectTransform.rect.width;
            bottomLeftCorner.y *= rectTransform.rect.height;

            // Place as many square grid tiles as fit inside our UI RectTransform, at any given GridCellSize
            for (float x = 0; x < rectTransform.rect.width - GridCellSize; x += GridCellSize)
            {
                for (float y = 0; y < rectTransform.rect.height - GridCellSize; y += GridCellSize)
                {
                    AddQuad(vh,
                        bottomLeftCorner + x * Vector2.right + y * Vector2.up,
                        bottomLeftCorner + (x + GridCellSize) * Vector2.right + (y + GridCellSize) * Vector2.up,
                        Vector2.zero, Vector2.one); // UVs
                }
            }

            Debug.Log("Mesh was redrawn!");
        }*/
    }


