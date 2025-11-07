using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class PillarPlacement : MonoBehaviour
{
    public GameObject pillarModel;
    public GameObject centerPoint;
    public GameObject spawnFolder;
    public NavMeshSurface navSurface;

    private void Start()
    {
        //CreatePillarsAroundPoint(12, centerPoint.transform.position, 27.2f);
        CreatePillarsAroundPoint(12, centerPoint.transform.position, 27f);
        navSurface.BuildNavMesh();
    }

    public void CreatePillarsAroundPoint (int num, Vector3 point, float radius) {
	
        for (int i = 0; i < num; i++)
        {

            /* Distance around the circle */
            var radians = 2 * Mathf.PI / num * i;

            /* Get the vector direction */
            var vertical = Mathf.Sin(radians);
            var horizontal = Mathf.Cos(radians);

            var spawnDir = new Vector3(horizontal, 0, vertical);

            /* Get the spawn position */
            var spawnPos = point + spawnDir * radius; // Radius is just the distance away from the point

            /* Now spawn */
            var enemy = Instantiate(pillarModel, spawnPos, Quaternion.identity, spawnFolder.transform) as GameObject;

            /* Rotate the enemy to face towards player */
            //enemy.transform.LookAt(point);


            /* Adjust height */
            //enemy.transform.Translate(new Vector3(0, enemy.transform.localScale.y / 2, 0));
            enemy.transform.Translate(new Vector3(0, 0, 0));
            
            enemy.transform.Rotate(-90, 0, 0);
        }
    }


}
