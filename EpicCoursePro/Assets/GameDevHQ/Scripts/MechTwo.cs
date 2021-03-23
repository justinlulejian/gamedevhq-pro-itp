using UnityEngine;

namespace GameDevHQ.Scripts
{
    public class MechTwo : Enemy
    {
        private Parent_Turret_Y _parentTurretY;

        // Note: This currently needs to be Start vs Awake so that _parentTurretY can init it's mesh
        // renderers.
        protected override void Start()
        {
            base.Start();
            _parentTurretY = GetComponentInChildren<Parent_Turret_Y>();
            if (_parentTurretY == null)
            {
                Debug.LogError($"Parent turret Y is is non-existent on enemy {name}");
            }

            int previousCountRenderers = _dissolveMeshRenderers.Count;
            _dissolveMeshRenderers.AddRange(_parentTurretY.GetDissolveMeshRenderers());
            if (previousCountRenderers >= _dissolveMeshRenderers.Count)
            {
                Debug.LogError("MechTwo did not get it's dissolve renderers from parent" +
                               "turret Y.");
            }
        }
    }
}