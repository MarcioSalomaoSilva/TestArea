function Update () {
	GetComponent.<Renderer>().material.color = 
		GetComponent.<CharacterController>().isGrounded ? Color.green : Color.red;
}