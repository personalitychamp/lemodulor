﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGenerator : MonoBehaviour {

	public float scale = 0.01f;	// A scale to be applied accross all measures
	public float min = 1f; 	// Their min width or height
	float Min {get{return min * scale;}}	
	public float max = 10f; // Their max width or height
	float Max {get{return max * scale;}}	
	public float radius = 10f; // How far from zero their a transform can be placed
	float Radius {get{return radius * scale;}} 
	public int count = 10;// The maximum number of cubes to be placed
	public List<BonedCube> cubes = new List<BonedCube>();	// A list to retrieve the cubes from
	
	public static Vector3 RandomVector(float min, float max){
		return new Vector3(Random.Range(min,max),Random.Range(min,max), Random.Range(min,max));
	}

	public void Start(){
				
		count = Random.Range(1, count);	

		while(cubes.Count < count){
			var bc = BonedCube.Make();	
			bc.transform.SetParent(this.transform);	// The cubes are children of this transform so they can be placed on a ground plane in AR
			bc.a.transform.position = this.transform.position + RandomVector(Radius * -1, Radius);
			bc.Set(Random.Range(Min, Max),Random.Range(Min, Max),Random.Range(Min, Max));
			cubes.Add(bc);
		}

		foreach(var bc in cubes){
			if(bc == null)
				continue;

			if(bc.a.position.y < 0){
				// if i'm 10 meters tall and located at -1 then the visible section of me is going to be 4
				// if 10 meters tall and locate at +1 then the visible section of me is going to be 6
				bc.GetComponentInChildren<Renderer>().material.color = Color.gray;
				bc.a.position -= new Vector3(0, bc.a.position.y, 0);
				bc.b.position -= new Vector3(0, bc.b.position.y, 0);
			}
		}
	}

	public void Reset(){
		foreach(var bc in cubes){
			if(bc == null)
				continue;
			bc.Stop();
		}
		cubes.Clear();
	}
}