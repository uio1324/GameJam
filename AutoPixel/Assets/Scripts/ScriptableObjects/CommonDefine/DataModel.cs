using ScriptableObjects.ScriptableObjectsAttribute;

namespace ScriptableObjects.CommonDefine
{
    public abstract class DataModel
    {
        [SpecifyFieldType(typeof(int))]
        public int Id;
    }
}