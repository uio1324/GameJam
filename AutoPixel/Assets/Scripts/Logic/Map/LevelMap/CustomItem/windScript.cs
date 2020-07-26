using UnityEngine;

namespace Logic.Map.LevelMap.CustomItem
{
    public class windScript : CustomItemMono
    {
        public Vector2 windDirection;  //矢量类型
        public float windStrength;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        public override void PushParam(CustomItem customItemData)
        {
            windDirection = new Vector2(customItemData.Param.x, customItemData.Param.y);
            windStrength = customItemData.Param.z;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if(collision.attachedRigidbody)
            {
                var velocity = new Vector2(collision.attachedRigidbody.velocity.x, windStrength);
                collision.attachedRigidbody.velocity = velocity;
            }
        }
    }
}
