using System;
using UnityEngine.SceneManagement;

namespace Util
{
    public static class Util
    {
        public static bool IsEditorScene()
        {
            return SceneManager.GetActiveScene().name == "MapEditor";
        }
    }
}