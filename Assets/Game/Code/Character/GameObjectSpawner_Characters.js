/****************************************
	GameObjectSpawner.js v0.0*
	Copyright 2013 Unluck Software	
 	www.chemicalbliss.com			
 	*script has been hardcoded to work with this the toon characters pack														
*****************************************/
#pragma strict
import System.Collections.Generic;	//Used to sort particle system list
//Visible properties
var particles:GameObject[];			//gameObjects to spawn (used to only be particle systems aka var naming)
var materials:Material[];
var cameraColors:Color[];
var maxButtons:int = 10;			//Maximum buttons per page	
var spawnOnAwake:boolean = true;	//Instantiate the first model on start
var showInfo:boolean;				//Show info text on start
var removeTextFromButton:String;	//Unwanted text 
var removeTextFromMaterialButton:String;//Unwanted text 
var autoChangeDelay:float;
var image:GUITexture;
//Hidden properties
private var page:int = 0;			//Current page
private var pages:int;				//Number of pages
private var currentGOInfo:String;	//Current particle info
var currentGO:GameObject;	//GameObject currently on stage
private var currentColor:Color;
private var isPS:boolean;			//Toggle to check if this is a PS or a GO

private var material:Material;		
private var _active:boolean = true;

private var counter:int = -1;
private var matCounter:int = -1;
private var colorCounter:int;


var bigStyle: GUIStyle;


function Start(){

	
	//Sort particle system list alphabeticly
    particles.Sort(particles, function(g1,g2) String.Compare(g1.name, g2.name));
    materials.Sort(materials, function(g1,g2) String.Compare(g1.name, g2.name));
	//Calculate number of pages
	pages = Mathf.Ceil((particles.length -1 )/ maxButtons);
	//Debug.Log(pages);
	if(spawnOnAwake){
		counter=0;
		ReplaceGO(particles[counter]);
		Info(particles[counter],  counter);
		}
	if(autoChangeDelay > 0){
		InvokeRepeating("NextModel", autoChangeDelay,autoChangeDelay);
	
	}
	
}

function Update () {
	
//	if(Input.GetKeyDown(KeyCode.Space)) {
//    	if(_active){
//    		_active = false;
//    		if(image)
//    		image.enabled = false;
//    	}else{
//    		_active = true;
//    		if(image)
//    		image.enabled = true;
//    	}
//	}
//	if(Input.GetKeyDown(KeyCode.RightArrow)) {
//		NextModel ();
//	}
//	if(Input.GetKeyDown(KeyCode.LeftArrow)) {
//		counter--;
//		if(counter < 0) counter = particles.Length-1;
//		ReplaceGO(particles[counter]);
//		
//		Info(particles[counter],  counter+1);
//		
//	}
//	if(Input.GetKeyDown(KeyCode.UpArrow) && materials.Length>0) {
//		matCounter++;
//		if(matCounter > materials.Length -1) matCounter = 0;
//		material = materials[matCounter];
//		if(currentGO){
//			if(currentGO.renderer){
//			currentGO.renderer.sharedMaterial = material;
//			}else{
//				
//				currentGO.gameObject.Find("MicroMale").renderer.sharedMaterial = material;
//			
//			}
//		}
//	}
//	if(Input.GetKeyDown(KeyCode.DownArrow) && materials.Length>0) {
//		matCounter--;
//		if(matCounter < 0) matCounter = materials.Length-1;
//		material = materials[matCounter];
//		if(currentGO){
//			if(currentGO.renderer){
//			currentGO.renderer.sharedMaterial = material;
//			}else{
//				
//				currentGO.gameObject.Find("MicroMale").renderer.sharedMaterial = material;
//			
//			}
//		}
//		
//	}
//	if(Input.GetKeyDown(KeyCode.B)) {
//		colorCounter++;
//		if(colorCounter > cameraColors.Length -1) colorCounter = 0;
//		
//	}
//	Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, cameraColors[colorCounter], Time.deltaTime*3);
//	
}

function NextModel () {
	
		counter++;
		if(counter > particles.Length -1) counter = 0;
		ReplaceGO(particles[counter]);
		Info(particles[counter],  counter+1);

}


function Info(go:GameObject, i:int) {
	if(go.GetComponent(ParticleSystem)){
			PlayPS(go.GetComponent(ParticleSystem), i );
			InfoPS(go.GetComponent(ParticleSystem), i );
			}else{
			//InfoGO(go, i);
			}

}

function ReplaceGO (_go:GameObject){
		if(currentGO) Destroy(currentGO);
			var go:GameObject = Instantiate(_go);
			currentGO = go;
			if(material)
			go.GetComponent.<Renderer>().sharedMaterial = material;
}

//Play particle system (resets time scale)
function PlayPS (_ps:ParticleSystem, _nr:int){
		Time.timeScale = 1;
		_ps.Play();
		
}

function InfoGO (_ps:GameObject, _nr:int){
		currentGOInfo = "" + "" + _nr + "/" + particles.length +"\n"+
		_ps.gameObject.name +"\n" + _ps.GetComponent(MeshFilter).sharedMesh.triangles.Length/3 + " Tris";
		currentGOInfo = currentGOInfo.Replace("_", " ");
		//Instructions();
		
}

function Instructions() {
		currentGOInfo = currentGOInfo + "\n\nUse mouse wheel to zoom \n"+"Click and hold to rotate\n"+"Press Space to show or hide menu\n"+"Press Up and Down arrows to cycle materials\n"+"Press B to cycle background colors";
			currentGOInfo = currentGOInfo.Replace("(Clone)", "");
}

function InfoPS (_ps:ParticleSystem, _nr:int){
		//Change current particle info text
		currentGOInfo = "System" + ": " + _nr + "/" + particles.length +"\n"+
		"Name: " + _ps.gameObject.name +"\n\n" +
		"Main PS Sub Particles: " + _ps.transform.childCount  +"\n" +
		"Main PS Materials: " + _ps.GetComponent.<Renderer>().materials.length +"\n" +
		"Main PS Shader: " + _ps.GetComponent.<Renderer>().material.shader.name;
		//If plasma(two materials)
		if(_ps.GetComponent.<Renderer>().materials.length >= 2)currentGOInfo = currentGOInfo + "\n\n *Plasma not mobile optimized*";
		//Instructions();
}