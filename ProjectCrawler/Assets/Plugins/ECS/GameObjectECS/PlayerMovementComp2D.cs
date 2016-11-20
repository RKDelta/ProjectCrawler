namespace ECS.GameObjectECS
{
    using UnityEngine;

    [ECS.RequireComponent(typeof(GameObjectComp))]
    public class PlayerMovementComp2D : ECS.Component
    {
        GameObject gameObject;

        public float speed = 10;

        public override void SetEntity(Entity entity)
        {
            base.SetEntity(entity);

            var bridge = this.entity.GetComponent<GameObjectComp>().bridge;

            bridge.OnUpdate += this.Update;

            this.gameObject = bridge.gameObject;
        }

        public void Update()
        {
            gameObject.transform.Translate(new Vector2(
                Input.GetAxis("Horizontal") * Time.deltaTime * speed,
                Input.GetAxis("Vertical") * Time.deltaTime * speed));
        }
    }
}
