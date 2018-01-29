/****************************************
	RagdollController.js
	Disable and enable rigidbodies and colliders on the ragdoll
	
	Copyright 2013 Unluck Software	
 	www.chemicalbliss.com											
*****************************************/
#pragma strict
#pragma downcast 
private var boneRig : Component[];		// Contains the ragdoll bones
private var mass:float = .1;	// Mass of each bone
var root:Transform;				// Assign the root bone to position the shadow projector
var _model:GameObject;
var _bodyMesh:Mesh;

var _headBone:Transform;

var _bodyPS:ParticleSystem;


//Blinking
private var colorOriginal:Color;
private var color:Color;
private var _R:float = 2500;
private var _G:float = 2500;
private var _B:float = 2500;

private var _randomColor:boolean;
private var _blinkCounter:int;
private var _stopBlink:int;

function Start () {
    if(!root)
        root = transform.Find("Root");
    if(!_model)
        _model = transform.Find("MicroMale").gameObject;
    if(!_headBone)
        _headBone = transform.Find("Head");
    boneRig = gameObject.GetComponentsInChildren (Rigidbody); 
    disableRagdoll();
    //Blinking
    colorOriginal = _model.GetComponent.<Renderer>().material.color;
}

function Blink(times:int, speed:float, red:float, green:float , blue:float){
    CancelInvoke();
    _randomColor= false;
    _R = red;
    _G = green;
    _B = blue;
    _stopBlink = times;
    InvokeRepeating("BlinkInvoke", speed, speed);
}

    function Blink(times:int, speed:float){
        CancelInvoke();
        _randomColor = true;
        _stopBlink = times;
        InvokeRepeating("BlinkInvoke", speed, speed);
    }

        function BlinkInvoke () {
            if(_blinkCounter < _stopBlink){
                if(_randomColor){
                    color = new Color(Random.Range(1, 5) ,Random.Range(1, 5),Random.Range(1, 5),1);
                }else{
                    color = new Color(_R , _G , _B ,1);
                }
		
                if(_model.GetComponent.<Renderer>().material.color == colorOriginal){
                    _model.GetComponent.<Renderer>().material.color = color;
                }else{
                    _model.GetComponent.<Renderer>().material.color = colorOriginal;
                }
                _blinkCounter++;
            }else{
                _model.GetComponent.<Renderer>().material.color = colorOriginal;
                _blinkCounter = 0;
                CancelInvoke();
            }
        }

        function disableRagdoll () {
            for (var ragdoll : Rigidbody in boneRig) {
                if(ragdoll.GetComponent.<Collider>() && ragdoll.GetComponent.<Collider>()!=this.GetComponent.<Collider>()){
                ragdoll.GetComponent.<Collider>().enabled = false;
                ragdoll.isKinematic = true;
                ragdoll.mass = 0.01;
            }
    }
    GetComponent.<Collider>().enabled = true;
}
 
function enableRagdoll (delay:float, force:Vector3) {
    yield(WaitForSeconds(delay));
    for (var ragdoll : Rigidbody in boneRig) {
		if(ragdoll.GetComponent.<Collider>())
		ragdoll.GetComponent.<Collider>().enabled = true;
		ragdoll.isKinematic = false; 
		ragdoll.mass = mass;
		if(force.magnitude > 0)
		ragdoll.AddForce(force*Random.value);
    }
    GetComponent(Animator).enabled=false;
    GetComponent.<Collider>().enabled = false;
    Destroy(GetComponent("BotControlScript"));
    GetComponent.<Rigidbody>().isKinematic = true;
    GetComponent.<Rigidbody>().useGravity = false;
}

function EnableCollisions(c1:Collider, c2:Collider){
    yield(WaitForSeconds(1));
    if(c2 && c1.enabled)
        Physics.IgnoreCollision(c1,c2, false);
}