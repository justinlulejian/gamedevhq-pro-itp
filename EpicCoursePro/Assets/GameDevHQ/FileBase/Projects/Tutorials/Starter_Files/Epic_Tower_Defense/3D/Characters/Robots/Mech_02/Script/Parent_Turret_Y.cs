using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Parent_Turret_Y : MonoBehaviour
{
    public GameObject Parent_Point;
    
    [SerializeField]
    private List<MeshRenderer> _dissolveMeshRenderers = new List<MeshRenderer>();

    private void Awake()
    {
        _dissolveMeshRenderers = GetComponentsInChildren<MeshRenderer>().ToList();
        
        if (_dissolveMeshRenderers.Count < 1)
        {
            Debug.LogError($"Mesh renderer from parent turret script is non-existent on" +
                           $" enemy {name}");
        }
    }

    public List<MeshRenderer> GetDissolveMeshRenderers()
    {
        return _dissolveMeshRenderers;
    }


    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x, Parent_Point.transform.position.y, Parent_Point.transform.position.z);
    }
}
