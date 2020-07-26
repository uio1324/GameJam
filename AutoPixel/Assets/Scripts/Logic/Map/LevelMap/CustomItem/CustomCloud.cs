using System;
using UnityEngine;

namespace Logic.Map.LevelMap.CustomItem
{
    public class CustomCloud : CustomItemMono
    {
        public float beginPushAsideHeight;
        public float endPushAsideHeight;
        private Transform leftGroup;
        private Vector3 leftGroupOrigin;
        private float leftGroupDestinyX;
        private Transform rightGroup;
        private Vector3 rightGroupOrigin;
        private float rightGroupDestinyX;
        private const float m_maxHorizontalOffset = 9f;

        private void Awake()
        {
                var leftTran = transform.Find("left");
                leftGroup = leftTran;
                leftGroupOrigin = leftTran.localPosition;
                leftGroupDestinyX = leftGroupOrigin.x - m_maxHorizontalOffset;

                var rightTran = transform.Find("right");
                rightGroup = rightTran;
                rightGroupOrigin = rightTran.localPosition;
                rightGroupDestinyX = rightGroupOrigin.x + m_maxHorizontalOffset;
        }

        public override void PushParam(CustomItem customItemData)
        {
            beginPushAsideHeight = customItemData.Param.x;
            endPushAsideHeight = customItemData.Param.y;
        }

        private void Update()
        {
            var curHeight = MapLogic.m_instance.GetCameraPos().y;
            if (beginPushAsideHeight < curHeight)
            {
                    var actualLeftY = leftGroup.position.y;
                    if (actualLeftY < endPushAsideHeight)
                    {
                        var leftT = Mathf.Clamp01(((curHeight - beginPushAsideHeight) - leftGroupOrigin.y) / (endPushAsideHeight - beginPushAsideHeight));
                        leftGroup.position = new Vector3(Mathf.Lerp(leftGroupOrigin.x + transform.position.x, leftGroupDestinyX, leftT), actualLeftY, 0);
                    }
                    
                    var actualRightY = rightGroup.position.y;
                    if (actualRightY < endPushAsideHeight)
                    {
                        var rightT = Mathf.Clamp01(((curHeight - beginPushAsideHeight) - rightGroupOrigin.y) / (endPushAsideHeight - beginPushAsideHeight));
                        rightGroup.position = new Vector3(Mathf.Lerp(rightGroupOrigin.x + transform.position.x, rightGroupDestinyX, rightT), actualRightY, 0);
                    }
            }
        }
    }
}