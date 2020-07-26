using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Logic.Map.LevelMap.CustomItem
{
    [Serializable]
    public class CustomItem
    {
        public string LevelName;
        public Vector2 Pos;
        public float Rotation;
        public Vector3 Scale = Vector3.one;
        public Vector3 Param;
        public GameObject Prefab;
        [NonSerialized]
        public int Key;
        public GameObject Generate()
        {
            var go = Object.Instantiate(Prefab);
            var mono = go.GetComponent<CustomItemMono>();
            if (mono)
            {
                mono.PushParam(this);
            }
            var tran = go.transform;
            tran.position = Pos;
            tran.rotation = Quaternion.Euler(new Vector3(0, 0, Rotation));
            tran.localScale = Scale;
            return go;
        }

        public void GenerateKey()
        {
            var length = Mathf.Sqrt(Scale.x * Scale.x + Scale.y * Scale.y);
            Key = (int) (Pos.y - length);
        }

        public int GetKey()
        {
            return Key;
        }
    }
}