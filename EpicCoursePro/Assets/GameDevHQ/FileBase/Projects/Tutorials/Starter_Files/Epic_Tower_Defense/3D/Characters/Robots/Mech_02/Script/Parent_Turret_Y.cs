using System.Collections.Generic;
using System.Linq;
using GameDevHQ.Scripts;
using UnityEngine;

namespace GameDevHQ.FileBase.Projects.Tutorials.Starter_Files.Epic_Tower_Defense._3D.Characters.Robots.Mech_02.Script
{
    public class Parent_Turret_Y : MonoBehaviour
    {
        public GameObject Parent_Point;
    
        [SerializeField]
        private List<Material> _dissolveMeshRendererMaterials = new List<Material>();

        private void Awake()
        {
            _dissolveMeshRendererMaterials = Enemy.GetMaterialsFromRenderers(
                GetComponentsInChildren<Renderer>().ToList());
            if (_dissolveMeshRendererMaterials.Count < 1)
            {
                Debug.LogError($"Mesh renderer from parent turret script is non-existent on" +
                               $" enemy {name}");
            }
        }

        public List<Material> GetDissolveMeshRendererMaterials()
        {
            return _dissolveMeshRendererMaterials;
        }


        // Update is called once per frame
        void LateUpdate()
        {
            transform.position = new Vector3(transform.position.x, Parent_Point.transform.position.y, Parent_Point.transform.position.z);
        }
    }
}
