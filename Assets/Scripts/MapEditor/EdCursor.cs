using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Курсор + сетка для редактора
/// </summary>

public interface IEdCursor
{
    event System.Action<Vector3> onChangePos;
    Vector3 GetSnapPos();
    Vector3 GetRealPos();
    void SetPos(Vector3 pos);
    void SetMinMaxX(float minX, float maxX);

    void SetGridSize(float gridSize);
}


public class EdCursor : MonoBehaviour, IEdCursor
{
    public event System.Action<Vector3> onChangePos = delegate { };

    Vector3 mPrevCursorPos;
    Vector3 mCursorPos;
    Vector3 mRealPos;
    float mGridSize = 1;
    float mMinX = -3.5f;
    float mMaxX = +3.5f;

    public Vector3 GetRealPos()
    {
        return mRealPos;
    }

    public void SetMinMaxX(float minX, float maxX)
    {
        mMinX = minX;
        mMaxX = maxX;
    }

    public void SetGridSize(float gridSize)
    {
        mGridSize = gridSize;
        if (mGridSize <= 0)
            mGridSize = 1;

        SetPos(mCursorPos);
    }

    public Vector3 GetSnapPos()
    {
        return mCursorPos;
    }

    public void SetPos(Vector3 pos)
    {
        mRealPos = pos;
        transform.position = mCursorPos = mPrevCursorPos = LimitAndSnap(mRealPos);
        onChangePos(mCursorPos);
    }

    Vector3 LimitAndSnap(Vector3 pos)
    {
        pos.x = Mathf.Clamp(pos.x, mMinX, mMaxX);
        if (pos.z < 0)
            pos.z = 0;

        // snap
        float gmul = mGridSize * 0.25f;
        if (pos.x > 0)
            pos.x = Mathf.Floor(pos.x / gmul) * gmul;
        else
            pos.x = Mathf.Ceil(pos.x / gmul) * gmul;
        pos.z = Mathf.Floor(pos.z / gmul) * gmul;

        return pos;
    }

    public void OnDragDelegate(BaseEventData data)
    {
        //  https://docs.unity3d.com/2018.2/Documentation/ScriptReference/EventSystems.EventTrigger.OnDrag.html
        var pd = data as PointerEventData; // приведение типов

        if (pd == null)
            return;

        float SC = (2 * Camera.main.orthographicSize) / Screen.height;
        mRealPos.x -= pd.delta.x * SC;
        mRealPos.z -= pd.delta.y * SC;

        mRealPos.x = Mathf.Clamp(mRealPos.x, mMinX - 0.5f, mMaxX + 0.5f);
        mRealPos.z = Mathf.Clamp(mRealPos.z, 0, mRealPos.z + 10);

        mCursorPos = LimitAndSnap(mRealPos);
        if (mPrevCursorPos != mCursorPos)
        {
            mPrevCursorPos = mCursorPos;
            onChangePos(mCursorPos);
        }
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, mCursorPos, 0.6f * 60 * Time.deltaTime);
    }
}
