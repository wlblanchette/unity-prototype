using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Utilities {
  public class Text {
    public static string dump(object any) {
      try {
        return JsonUtility.ToJson(any);
      } catch (System.ArgumentException) {
        return EditorJsonUtility.ToJson(any);
      } catch (System.Exception e) {
        Debug.LogError(e);
        return "<couldn't serialize objects>";
      }
    }
  }
}
