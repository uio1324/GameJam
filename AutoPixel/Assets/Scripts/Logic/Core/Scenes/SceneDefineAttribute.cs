using System;

namespace Logic.Core.Scenes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SceneDefineAttribute : Attribute
    {
        public string m_sceneName;

        public SceneDefineAttribute(string sceneName)
        {
            m_sceneName = sceneName;
        }
    }
}