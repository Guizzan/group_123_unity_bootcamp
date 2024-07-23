using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Guizzan.Input.GIM;
using System;
namespace Guizzan
{
    namespace Extensions
    {
        public static class Extensions
        {
            public static Transform GetTopMostParrent(this Transform transform)
            {
                Transform topmostParent = transform;

                while (transform.parent != null)
                {
                    topmostParent = transform.parent;
                    transform = transform.parent;
                }

                return topmostParent;
            }
            public static T NextEnum<T>(this T src) where T : struct
            {
                if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

                T[] Arr = (T[])Enum.GetValues(src.GetType());
                int j = Array.IndexOf<T>(Arr, src) + 1;
                return (Arr.Length == j) ? Arr[0] : Arr[j];
            }
            public static void ShiftRight<T>(this List<T> lst, int shifts)
            {
                for (int i = lst.Count - shifts - 1; i >= 0; i--)
                {
                    lst[i + shifts] = lst[i];
                }

                for (int i = 0; i < shifts; i++)
                {
                    lst[i] = default;
                }
            }
            public static void Swap<T>(this IList<T> list, int indexA, int indexB)
            {
                (list[indexB], list[indexA]) = (list[indexA], list[indexB]);
            }
            public static void DestroyChilderen(this Transform _transform)
            {
                foreach (Transform item in _transform)
                {
                    UnityEngine.Object.Destroy(item.gameObject);
                }
            }
            public static void DestroyChilderenExcept(this Transform _transform, string name)
            {
                foreach (Transform item in _transform)
                {
                    if (item.gameObject.name == name) continue;
                    UnityEngine.Object.Destroy(item.gameObject);
                }
            }
            public static void Shuffle<T>(this IList<T> ts)
            {
                var count = ts.Count;
                var last = count - 1;
                for (var i = 0; i < last; ++i)
                {
                    var r = UnityEngine.Random.Range(i, count);
                    var tmp = ts[i];
                    ts[i] = ts[r];
                    ts[r] = tmp;
                }
            }

            public static void DestroyChilderen(this GameObject go)
            {
                foreach (Transform item in go.transform)
                {
                    UnityEngine.Object.Destroy(item.gameObject);
                }
            }

            public static void SetTransform(this Transform tr, Transform copyFrom)
            {
                tr.SetPositionAndRotation(copyFrom.position, copyFrom.rotation);
                tr.localScale = copyFrom.localScale;
            }
            public static void LocalSetTransform(this Transform tr, Transform copyFrom)
            {
                tr.localPosition = copyFrom.localPosition;
                tr.localRotation = copyFrom.localRotation;
                tr.localScale = copyFrom.localScale;
            }
            public static Vector3 RotatePointWithPivot(this Vector3 point, Vector3 pivot, float angle)
            {
                angle *= Mathf.Deg2Rad;
                var x = Mathf.Cos(angle) * (point.x - pivot.x) - Mathf.Sin(angle) * (point.z - pivot.z) + pivot.x;
                var z = Mathf.Sin(angle) * (point.x - pivot.x) + Mathf.Cos(angle) * (point.z - pivot.z) + pivot.z;
                return new Vector3(x, point.y, z);
            }

            public static Vector3Int RotatePointWithPivot(this Vector3Int point, Vector3Int pivot, float angle)
            {
                angle *= Mathf.Deg2Rad;
                var x = Mathf.Cos(angle) * (point.x - pivot.x) - Mathf.Sin(angle) * (point.z - pivot.z) + pivot.x;
                var z = Mathf.Sin(angle) * (point.x - pivot.x) + Mathf.Cos(angle) * (point.z - pivot.z) + pivot.z;
                return Vector3Int.RoundToInt(new Vector3(x, point.y, z));
            }

            public static Vector3 CeilToInt(this Vector3 vector)
            {
                return new Vector3(Mathf.CeilToInt(vector.x), Mathf.CeilToInt(vector.y), Mathf.CeilToInt(vector.z));
            }
            public static Vector3 Abs(this Vector3 vector)
            {
                return new Vector3(Math.Abs(vector.x), Math.Abs(vector.y), Math.Abs(vector.z));
            }
            public static Vector3Int Abs(this Vector3Int vector)
            {
                return new Vector3Int(Math.Abs(vector.x), Math.Abs(vector.y), Math.Abs(vector.z));
            }
            public static Vector3 RotationRelative(this Transform t, Vector3 pos)
            {
                return t.transform.position + t.right * pos.x + t.up * pos.y + t.forward * pos.z;
            }

            public static T[] AddtoArray<T>(this T[] Org, T New_Value)
            {
                T[] New = new T[Org.Length + 1];
                if (Org != null) Org.CopyTo(New, 0);
                New[Org != null ? Org.Length : 0] = New_Value;
                return New;
            }
            public static bool ContainsLayer(this LayerMask mask, int layer)
            {
                return mask == (mask | (1 << layer));
            }
            public static bool ArrayContains<T>(this T[] Org, T value)
            {
                for (int i = 0; i < Org.Length; i++)
                {
                    if (Org[i].Equals(value)) return true;
                }
                return false;
            }
            public static float Remap(this float value, float from1, float to1, float from2, float to2)
            {
                return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
            }
        }
    }
}
