#pragma strict
import System.Collections.Generic;	//Used to sort list
var _animator:Animator;
var _animations:String[];
var _crossFade:boolean;
var maxButtons:int = 10;			//Maximum buttons per page	

var removeTextFromButton:String;	//Unwanted text 
var autoChangeDelay:float;
var image:GUITexture;
var _lastAnim:String;
private var page:int = 0;			//Current page
private var pages:int;				//Number of pages

private var _active:boolean = true;
private var counter:int = -1;


function Start(){
	//Sort list alphabeticly
    _animations.Sort(_animations, function(g1,g2) String.Compare(g1, g2));
	pages = Mathf.Ceil((_animations.length -1 )/ maxButtons);	
}