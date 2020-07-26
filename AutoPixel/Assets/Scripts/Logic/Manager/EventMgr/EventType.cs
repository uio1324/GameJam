namespace Logic.Manager.EventMgr
{
    public static class EventType
    {
        public const int DefaultEvent = 0;
        public const int SavePoint = 1000;
        public const int BreakStone = 1001;
        public const int TimelineTrigger = 1002;
        public const int BeginScroll = 1003;
        public const int FillLight = 1004;
        public const int FinishLevel = 1005;

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