﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingPoints {

	public BoundingPoints(Vector3 center, Vector3 size){

		yPositive.Add(center + new Vector3(size.x/2, size.y/2, size.z/2));
		yPositive.Add(center + new Vector3(-size.x/2, size.y/2, size.z/2));
		yPositive.Add(center + new Vector3(size.x/2, size.y/2, -size.z/2));
		yPositive.Add(center + new Vector3(-size.x/2, size.y/2, -size.z/2));
		yNegative.Add(center + new Vector3(size.x/2, -size.y/2, size.z/2));
		yNegative.Add(center + new Vector3(-size.x/2, -size.y/2, size.z/2));
		yNegative.Add(center + new Vector3(size.x/2, -size.y/2, -size.z/2));
		yNegative.Add(center + new Vector3(-size.x/2, -size.y/2, -size.z/2));		
		xPositive.Add(center + new Vector3(size.x/2, size.y/2, -size.z/2));
		xPositive.Add(center + new Vector3(size.x/2, size.y/2, size.z/2));
		xPositive.Add(center + new Vector3(size.x/2, -size.y/2, -size.z/2));
		xPositive.Add(center + new Vector3(size.x/2, -size.y/2, size.z/2));
		xNegative.Add(center + new Vector3(-size.x/2, size.y/2, -size.z/2));
		xNegative.Add(center + new Vector3(-size.x/2, size.y/2, size.z/2));
		xNegative.Add(center + new Vector3(-size.x/2, -size.y/2, -size.z/2));
		xNegative.Add(center + new Vector3(-size.x/2, -size.y/2, size.z/2));
		zPositive.Add(center + new Vector3(-size.x/2, size.y/2, size.z/2));
		zPositive.Add(center + new Vector3(-size.x/2, -size.y/2, size.z/2));
		zPositive.Add(center + new Vector3(size.x/2, size.y/2, size.z/2));
		zPositive.Add(center + new Vector3(size.x/2, -size.y/2, size.z/2));
		zNegative.Add(center + new Vector3(-size.x/2, size.y/2, -size.z/2));
		zNegative.Add(center + new Vector3(-size.x/2, -size.y/2, -size.z/2));
		zNegative.Add(center + new Vector3(size.x/2, size.y/2, -size.z/2));
		zNegative.Add(center + new Vector3(size.x/2, -size.y/2, -size.z/2));
		// all.AddRange(tops);
		// all.AddRange(bottoms);
		all.AddRange(xPositive);
		all.AddRange(xNegative);
		all.AddRange(zPositive);
		all.AddRange(zNegative);
		
	}
	public List<Vector3> all = new List<Vector3>();

	public List<Vector3> yPositive = new List<Vector3>();
	public List<Vector3> yNegative = new List<Vector3>();
	public List<Vector3> xPositive = new List<Vector3>();
	public List<Vector3> xNegative = new List<Vector3>();
	public List<Vector3> zPositive = new List<Vector3>();
	public List<Vector3> zNegative = new List<Vector3>();
}

public class ModulorAgent : MonoBehaviour {

	public string id;
	public BonedCube bc;
	public Vector3 center = Vector3.zero;
	public bool offGround;
	public float height;
	public List<float> storeys = new List<float>();
	public BoxCollider boxCollider;
	public Rigidbody rb;
	public Bounds bounds;
	public SkinnedMeshRenderer skinnedMesh;
	public BoundingPoints boundingPoints;
	public List<ModulorAgent> modulorAgents = new List<ModulorAgent>();
	public bool showPoints = false;
	public Dictionary<ModulorAgent, List<Vector3>> intersectingPoints = new Dictionary<ModulorAgent, List<Vector3>>();
	public bool showIntersectingPoints = false;

	// Use this for initialization
	IEnumerator Start () {

		this.gameObject.name = id;
		
		// showPoints = true;
		showIntersectingPoints = true;

		// Set the size of the volume to the closest measurement contained in the red series
		float length = LeModular.GetClosest(bc.length * 100) / 100;
		float width = LeModular.GetClosest(bc.width * 100) / 100;
		float height = LeModular.GetClosest(bc.height * 100) / 100;
		bc.Set(length,width,height);
		
		// Find all the bounding points
		skinnedMesh = GetComponentInChildren<SkinnedMeshRenderer>();
		skinnedMesh.updateWhenOffscreen = true;
		bounds = skinnedMesh.bounds;
		center = bounds.center;
		boundingPoints = new BoundingPoints(center, new Vector3(width, height, length));
		// Add box collider and rigidbody to generate collisions
		boxCollider = bc.gameObject.AddComponent<BoxCollider>();
		boxCollider.enabled = true;
		boxCollider.isTrigger = true;
		boxCollider.center = center;
		boxCollider.size = new Vector3(bc.width, bc.height, bc.length);
		rb = bc.gameObject.AddComponent<Rigidbody>();
		rb.isKinematic = true;
		yield return new WaitUntil(()=> modulorAgents.Count > 0);
		// Iterate through our bounding points to see if any of them are within the bounds of another agent
		foreach(Vector3 point in boundingPoints.all){
			foreach (ModulorAgent agent in modulorAgents)
			{
				if(agent.bounds.Contains(point)){
					if(intersectingPoints.ContainsKey(agent)){
						intersectingPoints[agent].Add(point);
						Debug.Log("Added");
					}else{
						List<Vector3> points = new List<Vector3>();
						points.Add(point);
						intersectingPoints.Add(agent, points);
						Debug.Log("Created");

					}
				}else{
					Debug.Log("Does not contain");
				}
			}
		}
		// TODO: refactor what's below
		var refactor = false;
		if(refactor){
			// Instantiate piloti if the volume is off the ground
		if(offGround){
			var piloti = Resources.Load<GameObject>("piloti");
			var pos = center;
			pos.y -= bc.height/2;
			piloti = Instantiate(piloti, pos, Quaternion.identity, this.transform);
			var pilotiScript = piloti.GetComponent<Pilotis>();
			// get the shortest side
			pilotiScript.length = bc.width;
			pilotiScript.width = bc.length;
			var a = pilotiScript.length > pilotiScript.width ? pilotiScript.width : pilotiScript.length;	// get the shortet side
			a *= 0.1f;
			a = Mathf.Clamp(a, 0.1f, 0.75f);
			pilotiScript.pilotiWidth = a;
			pilotiScript.width -= a;
			pilotiScript.length -= a;
			var ground = bc.a.position;
			ground.y = 0;
			height = Vector3.Distance(bc.a.position, ground);
		}

		List<float> floors = LeModular.Divisions(bc.height);
		var floorBC = BonedCube.Make();
		floorBC.transform.SetParent(this.transform);
		floorBC.a.transform.position = this.bc.a.transform.position;
		floorBC.Set(bc.length, bc.width, 0.1f);
		var _floors = BuildFloors(floors);
		if(_floors != null){
			foreach(var floor in _floors){
				floorBC = BonedCube.Make();
				floorBC.transform.SetParent(this.transform);
				floorBC.a.transform.position = this.bc.a.transform.position + new Vector3(0, floor, 0);
				floorBC.Set(bc.length, bc.width, 0.1f);
			}
		}

		// bc.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
		}
		
	}

	List<float> BuildFloors(List<float> floors){
		// get random number that is the =< the size of the list
		int a = Random.Range(0, floors.Count);
		int b = Random.Range(floors.Count - a, floors.Count);
		Debug.Log(string.Format("{0} a {1} b {2}", id, a, b));
		if(a + b > floors.Count){
			Debug.Log(string.Format("! a {0} b {1}", a, b));
			return BuildFloors(floors);
		}
		return floors.GetRange(a,b);;
	}
	private void OnTriggerEnter(Collider other)
	{
		var go = other.gameObject;
		var modulorAgent = go.GetComponent<ModulorAgent>();
		if(modulorAgent){
			this.modulorAgents.Add(modulorAgent);
		}else{
			Debug.LogWarning(string.Format("No ModulorAgent script attached to gameObject {0} that collided with gameObject {1}.", go.name, this.id));
		}
	}

	private void OnDrawGizmos() {
		
		if(showPoints){
			foreach(var vec in boundingPoints.all){
				Gizmos.DrawCube(this.transform.TransformPoint(vec), Vector3.one * 0.01f);
			}
		}

		if(showIntersectingPoints){
			foreach(List<Vector3> value in intersectingPoints.Values){
				foreach(Vector3 point in value){
					Gizmos.DrawCube(this.transform.TransformPoint(point), Vector3.one * 0.01f);
				}	
			}
		}
	}
}
