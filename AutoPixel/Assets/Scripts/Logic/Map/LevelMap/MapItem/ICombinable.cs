namespace Logic.Map.LevelMap.MapItem
{
    /// <summary>
    /// 如果有类型需要监听被combine的消息，可以实现这个接口
    /// 会不会被聚合，与实不实现此接口无关
    /// </summary>
    public interface ICombinable
    {
        void BeCombined();
    }
}