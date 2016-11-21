#pragma strict

var cam1 : Camera;
var cam2 : Camera;
 
function Start() {
    cam1.enabled = true;
    cam2.enabled = false;
}
 
function Update() {
 
    if (Input.GetKeyDown(KeyCode.C)) {
        cam1.enabled = !cam1.enabled;
        cam2.enabled = !cam2.enabled;
    }
 
}