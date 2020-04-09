using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDrawer : MonoBehaviour {

    List<DRAWMESH> Que;

	// Use this for initialization
	void Start ()
    {
        Que = new List<DRAWMESH>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(Que.Count>0)
        {
            foreach(DRAWMESH dm in Que)
            {
                Graphics.DrawMesh(dm.mesh, Vector3.zero, Quaternion.identity, dm.material, 9);
            }
            Que.Clear();
        }
	}

    class DRAWMESH
    {
        public Mesh mesh;
        public Material material;
    }

    public void AddQue(Mesh m,Material t)
    {
        DRAWMESH dm = new DRAWMESH();
        dm.mesh = m;
        dm.material = t;
        Que.Add(dm);
    }
}
