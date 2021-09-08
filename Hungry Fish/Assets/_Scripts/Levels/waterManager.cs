using UnityEngine;

public class waterManager : MonoBehaviour {
	public LineRenderer waterSurface;

	[HideInInspector]
	public float waterLevel = -0.9f;

	float[] xpositions;
	float[] ypositions;
	float[] velocities;
	float[] accelerations;

	//GameObject[] meshobjects;
	//Mesh[] meshes;


	const float springconstant = 0.02f;
	const float damping = 0.08f;
	const float spread = 0.05f;
	const float z = -1f;

	float baseheight;

	void Start()
	{
		persistentData.Instance.waterInstance = null;
		persistentData.Instance.waterInstance = this;
		float camWidth = Vector3.Distance(Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0f, Camera.main.nearClipPlane)), Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, Camera.main.nearClipPlane)));
		SpawnWater(-camWidth, camWidth * 2f, waterLevel);
		waterSurface.sortingLayerName = "Interactivables";
		waterSurface.sortingOrder = 7;
	}

	public void SpawnWater(float Left, float Width, float Top)
	{
		int edgecount = Mathf.RoundToInt(Width) * 5;
		int nodecount = edgecount + 1;
		xpositions = new float[nodecount];
		ypositions = new float[nodecount];
		velocities = new float[nodecount];
		accelerations = new float[nodecount];
		
		//meshobjects = new GameObject[edgecount];
		//meshes = new Mesh[edgecount];
		
		baseheight = Top;

		//waterSurface.material.renderQueue = 10;
		waterSurface.positionCount = nodecount;
		waterSurface.startWidth = 0.03f;
		waterSurface.endWidth = 0.03f;

		for (int i = 0; i < nodecount; i++)
		{
			ypositions[i] = Top;
			xpositions[i] = Left + Width * i / edgecount;
			accelerations[i] = 0;
			velocities[i] = 0;
			waterSurface.SetPosition(i, new Vector3(xpositions[i], ypositions[i], z));
		}

		/*for (int i = 0; i < edgecount; i++)
		{
			meshes[i] = new Mesh();

			Vector3[] Vertices = new Vector3[4];
			Vertices[0] = new Vector3(xpositions[i], ypositions[i], z);
			Vertices[1] = new Vector3(xpositions[i + 1], ypositions[i + 1], z);
			Vertices[2] = new Vector3(xpositions[i], bottom, z);
			Vertices[3] = new Vector3(xpositions[i+1], bottom, z);

			Vector2[] UVs = new Vector2[4];
			UVs[0] = new Vector2(0, 1);
			UVs[1] = new Vector2(1, 1);
			UVs[2] = new Vector2(0, 0);
			UVs[3] = new Vector2(1, 0);

			int[] tris = new int[6] { 0, 1, 3, 3, 2, 0 };

			meshes[i].vertices = Vertices;
			meshes[i].uv = UVs;
			meshes[i].triangles = tris;

			meshobjects[i] = Instantiate(watermesh,Vector3.zero,Quaternion.identity) as GameObject;
			meshobjects[i].GetComponent<MeshFilter>().mesh = meshes[i];
			meshobjects[i].transform.parent = transform;
		}*/
	}

	/*void UpdateMeshes()
    {
        for (int i = 0; i < meshes.Length; i++)
        {
 
            Vector3[] Vertices = new Vector3[4];
            Vertices[0] = new Vector3(xpositions[i], ypositions[i], z);
            Vertices[1] = new Vector3(xpositions[i+1], ypositions[i+1], z);
            Vertices[2] = new Vector3(xpositions[i], bottom, z);
            Vertices[3] = new Vector3(xpositions[i+1], bottom, z);
 
            meshes[i].vertices = Vertices;
        }
    }*/

	void FixedUpdate()
	{
		for (int i = 0; i < xpositions.Length ; i++)
        {
            float force = springconstant * (ypositions[i] - baseheight) + velocities[i]*damping ;
            accelerations[i] = -force;
            ypositions[i] += velocities[i];
            velocities[i] += accelerations[i];
			waterSurface.SetPosition(i, new Vector3(xpositions[i], ypositions[i], z));
        }

		float[] leftDeltas = new float[xpositions.Length];
		float[] rightDeltas = new float[xpositions.Length];

		for (int j = 0; j < 5; j++)
		{
			for (int i = 0; i < xpositions.Length; i++)
			{
				if (i > 0)
				{
					leftDeltas[i] = spread * (ypositions[i] - ypositions[i-1]);
					//ypositions[i-1] += leftDeltas[i];
					velocities[i - 1] += leftDeltas[i];
				}
				if (i < xpositions.Length - 1)
				{
					rightDeltas[i] = spread * (ypositions[i] - ypositions[i + 1]);
					//ypositions[i + 1] += rightDeltas[i];
					velocities[i + 1] += rightDeltas[i];
				}
			}
			for (int i = 0; i < xpositions.Length; i++)
			{
				if (i > 0) 
				{
					ypositions[i-1] += leftDeltas[i];
				}
				if (i < xpositions.Length - 1) 
				{
					ypositions[i + 1] += rightDeltas[i];
				}
			}
		}
		//UpdateMeshes();
	}

	public void Splash(float xpos, float velocity)
	{
		if (xpos >= xpositions[0] && xpos <= xpositions[xpositions.Length-1])
		{
			xpos -= xpositions[0];
			int index = Mathf.RoundToInt((xpositions.Length-1)*(xpos / (xpositions[xpositions.Length-1] - xpositions[0])));
			velocities[index] = velocity;
		}
	}
}
