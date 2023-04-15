using Unity.Mathematics;

namespace Triangulation.Demo
{
    public struct MovingPoint
    {
        float3 position;
        float3 direction;
        float speed;
        float deltaTime;
        float dirChangeTimer;
        private float3 target;

        public float3 Position => position;

        public MovingPoint(in float3 startPos, in float2 bounds, in float dirChangeTimer)
        {
            speed = UnityEngine.Random.Range(.1f, .5f);
            position = startPos;
            var tX = UnityEngine.Random.Range(-bounds.x, bounds.x);
            var tZ = UnityEngine.Random.Range(-bounds.y, bounds.y);
            direction = new(tX - position.x, 0, tZ - position.z);
            direction = math.normalize(direction);
            target = new(tX, 0, tZ);
            deltaTime = 0;
            this.dirChangeTimer = dirChangeTimer;
        }
        public void Update(in float delta, in float2 bounds)
        {
            deltaTime += delta;

            if (deltaTime >= dirChangeTimer)
            {
                deltaTime = 0;
                NewSpeed();
            }

            MoveToTarget(delta);
            if (math.distancesq(position, target) <= .001f)
                NewTarget(bounds);

            position.x *= position.x > bounds.x || position.x < -bounds.x ? -1 : 1;
            position.z *= position.z > bounds.y || position.z < -bounds.y ? -1 : 1;

            position.x = math.clamp(position.x, -bounds.x, bounds.x);
            position.z = math.clamp(position.z, -bounds.y, bounds.y);
        }

        void MoveToTarget(in float delta) => position += direction * speed * delta;
        private void NewSpeed() => speed = UnityEngine.Random.Range(.1f, .5f);
        private void NewTarget(in float2 bounds)
        {
            var tX = UnityEngine.Random.Range(-bounds.x, bounds.x);
            var tZ = UnityEngine.Random.Range(-bounds.y, bounds.y);
            target = new(tX, 0, tZ);
            direction = target - position;
            direction = math.normalize(direction);
        }
    }
}