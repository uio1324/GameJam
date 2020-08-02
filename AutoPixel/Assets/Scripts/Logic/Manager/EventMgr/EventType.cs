namespace Logic.Manager.EventMgr
{
    public static class EventType
    {
        public static int ConvertToValue(string fieldName)
        {
            var type = typeof(EventType);
            var fields = type.GetFields();
            foreach (var fieldInfo in fields)
            {
                if (fieldInfo.Name == fieldName)
                {
                    return (int)fieldInfo.GetValue(null);
                }
            }

            return 0;
        }
    }
}