var magicNumber : float;

private var moveVector : Vector3;
function Update () {
	var controller = GetComponent.<CharacterController>();

	if (controller.isGrounded) moveVector.y = magicNumber;
	else moveVector += Physics.gravity * Time.deltaTime;
		
	controller.Move(moveVector);
}