using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


/// <summary>
/// Камера для редактора
/// </summary>
public interface IEdCamera
{
   void SetCursor(IEdCursor cursor);
}

public class EdCamera : MonoBehaviour, IEdCamera
{
   Vector3 mCurPos;
   IEdCursor cursor;

   public void SetCursor(IEdCursor _cursor)
   {
      cursor = _cursor;
   }

   void Update()
   {
        mCurPos = cursor.GetRealPos () + Vector3.up * 20;// Stage.CHANK_SECTION_LEN;
      transform.position = Vector3.Lerp(transform.position, mCurPos, 0.25f * 60 * Time.deltaTime);
   }
}
