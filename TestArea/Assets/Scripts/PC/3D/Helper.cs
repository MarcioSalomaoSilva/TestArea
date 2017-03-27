using UnityEngine;


//-TP_Motor and TP_Animato depend on TP_Controller. 
//-TP_Controller handles input and motor is recieves input and processes motion. 
//-TP_Animator holds the various different states of animation. 
//-TP_camera handles everything to do with the camera and needs a few objects attached to the PC.
//--three targeLookAts, one for each side to wich the camaera moves and one in the center of the player.
//--Create them and put them into place most don't need to be assigned and debugs are in place
//--TP_camera is currently incomplete in terms of inputs but both mouse and controller can be used
//

public static class Helper {


	public struct ClipPlanePoints {

		public Vector3 UpperLeft;
		public Vector3 UpperRight;
		public Vector3 LowerLeft;
		public Vector3 LowerRight;

		public Vector3 MidUpperLeft;
		public Vector3 MidUpperRight;
		public Vector3 MidLowerLeft;
		public Vector3 MidLowerRight;

		public Vector3 FarUpperLeft;
		public Vector3 FarUpperRight;
		public Vector3 FarLowerLeft;
		public Vector3 FarLowerRight;

	}
	
	public static float ClampAngle(float angle, float min, float max){

		do {

			if (angle < -360)
				angle += 360;
			if (angle >360)
				angle-=360;

		} while(angle<-360 || angle > 360);

		return Mathf.Clamp (angle, min, max);

	}

	public static ClipPlanePoints ClipPlaneAtNear(Vector3 pos){

		var clipPlanePoints = new ClipPlanePoints ();

		if(Camera.main == null){
			return clipPlanePoints;
		}

		//cameras transform
		var transform = Camera.main.transform;
		//find the feild of view of the main camera and divide by 2, convert to radians becuase the tangent operation recieves radians
		var halfFOV = (Camera.main.fieldOfView / 2f) * Mathf.Deg2Rad;
		//store aspect
		var aspect = Camera.main.aspect;

		//distance from camera to near clip plane
		var distance = Camera.main.nearClipPlane;
		//height of the the clip plane
		var height = distance * Mathf.Tan (halfFOV);
		//obtain width based of height
		var width = height * aspect;

		//distance from camera to mid clip plane
		var middistance = TP_Camera.Instance.Distance;
		//height of the the clip plane
		var midheight = middistance * Mathf.Tan (halfFOV);
		//obtain width based of height
		var midwidth = midheight * (aspect);

		//distance from camera to mid clip plane
		var fardistance = TP_Camera.Instance.Distance *3f;
		//height of the the clip plane
		var farheight = fardistance * Mathf.Tan (halfFOV);
		//obtain width based of height
		var farwidth = farheight * (aspect);



		//near plane clip points
		clipPlanePoints.LowerRight = pos + transform.right*width;
		clipPlanePoints.LowerRight -= transform.up*height;
		clipPlanePoints.LowerRight += transform.forward * distance;

		clipPlanePoints.LowerLeft = pos - transform.right*width;
		clipPlanePoints.LowerLeft -= transform.up*height;
		clipPlanePoints.LowerLeft += transform.forward * distance;

		clipPlanePoints.UpperRight = pos + transform.right*width;
		clipPlanePoints.UpperRight += transform.up*height;
		clipPlanePoints.UpperRight += transform.forward * distance;

		clipPlanePoints.UpperLeft = pos - transform.right*width;
		clipPlanePoints.UpperLeft += transform.up*height;
		clipPlanePoints.UpperLeft += transform.forward * distance;

		//mid plane clip points
		clipPlanePoints.MidLowerRight = pos + transform.right*midwidth;
		clipPlanePoints.MidLowerRight -= transform.up*midheight;
		clipPlanePoints.MidLowerRight += transform.forward * middistance;
		
		clipPlanePoints.MidLowerLeft = pos - transform.right*midwidth;
		clipPlanePoints.MidLowerLeft -= transform.up*midheight;
		clipPlanePoints.MidLowerLeft += transform.forward * middistance;
		
		clipPlanePoints.MidUpperRight = pos + transform.right*midwidth;
		clipPlanePoints.MidUpperRight += transform.up*midheight;
		clipPlanePoints.MidUpperRight += transform.forward * middistance;
		
		clipPlanePoints.MidUpperLeft = pos - transform.right*midwidth;
		clipPlanePoints.MidUpperLeft += transform.up*midheight;
		clipPlanePoints.MidUpperLeft += transform.forward * middistance;

		//far plane clip points
		clipPlanePoints.FarLowerRight = pos + transform.right*farwidth;
		clipPlanePoints.FarLowerRight -= transform.up*farheight;
		clipPlanePoints.FarLowerRight += transform.forward * fardistance;
		
		clipPlanePoints.FarLowerLeft = pos - transform.right*farwidth;
		clipPlanePoints.FarLowerLeft -= transform.up*farheight;
		clipPlanePoints.FarLowerLeft += transform.forward * fardistance;
		
		clipPlanePoints.FarUpperRight = pos + transform.right*farwidth;
		clipPlanePoints.FarUpperRight += transform.up*farheight;
		clipPlanePoints.FarUpperRight += transform.forward * fardistance;
		
		clipPlanePoints.FarUpperLeft = pos - transform.right*farwidth;
		clipPlanePoints.FarUpperLeft += transform.up*farheight;
		clipPlanePoints.FarUpperLeft += transform.forward * fardistance;

		return clipPlanePoints;
	}
}