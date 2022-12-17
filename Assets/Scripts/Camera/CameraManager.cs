using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Camera _cam ;
    public float CameraZoomSpeed = 10f;
    public float CameraSpeed = 10f;
    public float CameraMinZoom = 6;
    public float CameraMaxZoom = 1;

    public float EdgeDetectionThicknessPercentage = 5f;
    private Vector2 YThicknessBoundaries;
    private Vector2 XThicknessBoundaries;
    public float CameraBoundX;
    public float CameraBoundY;
    private void Awake() {
        _cam = Camera.main;
        
    }
    private void Update()
    {
        CheckZoom(Input.mouseScrollDelta.y);
        CheckMousePos(Input.mousePosition);
        CheckKeyboardInputs();
    }
    public void CheckZoom(float scroll)
    {
        if(scroll == 0)
        {
            return;
        }
        else if(scroll > 0)
        {
            if(_cam.orthographicSize > CameraMaxZoom - CameraZoomSpeed*Time.deltaTime)
                _cam.orthographicSize -= CameraZoomSpeed*Time.deltaTime;
        }else if(scroll < 0)
        {
            if(_cam.orthographicSize < CameraMinZoom +CameraZoomSpeed*Time.deltaTime )
                _cam.orthographicSize += CameraZoomSpeed*Time.deltaTime;
        }
    }

    void CheckKeyboardInputs()
    {
        if(Input.GetAxis("Horizontal") > 0f)
        {
            MoveCamera(CameraSpeed*Time.deltaTime*Input.GetAxis("Horizontal") , 0f);
        }
        else if(Input.GetAxis("Horizontal") < 0f)
        {
            MoveCamera(-CameraSpeed*Time.deltaTime* Mathf.Abs(Input.GetAxis("Horizontal")) , 0f);
        }
        
        if(Input.GetAxis("Vertical") > 0f)
        {
            MoveCamera(0f , CameraSpeed*Time.deltaTime*Input.GetAxis("Vertical"));        
        }
        else if(Input.GetAxis("Vertical") < 0f)
        {
            MoveCamera(0f , -CameraSpeed*Time.deltaTime*Mathf.Abs(Input.GetAxis("Vertical")));
        }
    }
    void CheckMousePos(Vector3 MousePos)
    {
        XThicknessBoundaries = new Vector2(Screen.width* 1f-(EdgeDetectionThicknessPercentage / 100f) , Screen.width* (EdgeDetectionThicknessPercentage / 100f));
        YThicknessBoundaries = new Vector2(Screen.height* 1f-(EdgeDetectionThicknessPercentage / 100f) , Screen.height* (EdgeDetectionThicknessPercentage / 100f));
        if(MousePos.x > XThicknessBoundaries.x)
        {
            MoveCamera(CameraSpeed*Time.deltaTime , 0f);
        }
        else if(MousePos.x < XThicknessBoundaries.y)
        {
            MoveCamera(-CameraSpeed*Time.deltaTime , 0f);
        }
        
        if(MousePos.y > YThicknessBoundaries.x )
        {
            MoveCamera(0f , CameraSpeed*Time.deltaTime);        
        }
        else if(MousePos.y < YThicknessBoundaries.y)
        {
            MoveCamera(0f , -CameraSpeed*Time.deltaTime);
        }
    }

    public void MoveCamera(float x , float y)
    {
        bool checkCamBoundsX = (_cam.transform.position.x + x > 0 && _cam.transform.position.x + x < CameraBoundX) ;
        bool checkCamBoundsY =(_cam.transform.position.y + y > 0 && _cam.transform.position.y + y < CameraBoundY);
        if(!checkCamBoundsX && y == 0f)
            return;
        if(!checkCamBoundsY && x == 0f)
        return;
        _cam.transform.position += new Vector3(x,y,0f);
    }
}
