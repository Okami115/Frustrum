using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frustrum : MonoBehaviour
{
    private const int maxPlanes = 6;

    Plane[] planes = new Plane[maxPlanes];

    // Las esquinas del frustrum cercano
    [SerializeField] Vector3 nearTopLeft;
    [SerializeField] Vector3 nearTopRight;
    [SerializeField] Vector3 nearBottomLeft;
    [SerializeField] Vector3 nearBottomRight;

    // Las esquinas del frustrum lejano
    [SerializeField] Vector3 farTopLeft;
    [SerializeField] Vector3 farTopRight;
    [SerializeField] Vector3 farBottomLeft;
    [SerializeField] Vector3 farBottomRight;
    
    Camera cam;
    void Start()
    {
        cam = Camera.main;
        for (int i = 0; i < maxPlanes; i++)
        {
            planes[i] = new Plane();
        }
    }

    // Update is called once per frame
    void Update()
    {
        SetPlanePoint();

        planes[0].Set3Points(cam.transform.position, farBottomLeft, farTopLeft);    //Superior
        planes[1].Set3Points(cam.transform.position, farBottomLeft, farTopLeft);    //Inferior
        planes[2].Set3Points(cam.transform.position, farBottomLeft, farTopLeft);    //Izquierda
        planes[3].Set3Points(cam.transform.position, farTopRight, farBottomRight);  //Derecha
        planes[4].Set3Points(cam.transform.position, farTopLeft, farTopRight);      //Frontal
        planes[5].Set3Points(cam.transform.position, farBottomRight, farBottomLeft);//Posterior
    }


    public void SetPlanePoint()
    {
        // Setea los puntos en las esquinas del plano cercano
        float halfCameraHeightNear = Mathf.Tan((cam.fieldOfView / 2) * Mathf.Deg2Rad) * cam.nearClipPlane;
        float CameraHalfWidthNear = (cam.aspect * halfCameraHeightNear);

        Vector3 nearPlaneDistance = cam.transform.position + (cam.transform.forward * cam.nearClipPlane);
        nearTopLeft = nearPlaneDistance + (cam.transform.up * halfCameraHeightNear) - (cam.transform.right * CameraHalfWidthNear);
        nearTopRight = nearPlaneDistance + (cam.transform.up * halfCameraHeightNear) + (cam.transform.right * CameraHalfWidthNear);
        nearBottomLeft = nearPlaneDistance - (cam.transform.up * halfCameraHeightNear) - (cam.transform.right * CameraHalfWidthNear);
        nearBottomRight = nearPlaneDistance - (cam.transform.up * halfCameraHeightNear) + (cam.transform.right * CameraHalfWidthNear);

        // Setea los puntos en las esquinas del plano lejano
        float halfCameraHeightfar = Mathf.Tan((cam.fieldOfView / 2) * Mathf.Deg2Rad) * cam.farClipPlane;
        float CameraHalfWidthFar = (cam.aspect * halfCameraHeightfar);

        Vector3 farPlaneDistance = cam.transform.position + (cam.transform.forward * cam.farClipPlane);
        farTopLeft = farPlaneDistance + (cam.transform.up * halfCameraHeightfar) - (cam.transform.right * CameraHalfWidthFar);
        farTopRight = farPlaneDistance + (cam.transform.up * halfCameraHeightfar) + (cam.transform.right * CameraHalfWidthFar);
        farBottomLeft = farPlaneDistance - (cam.transform.up * halfCameraHeightfar) - (cam.transform.right * CameraHalfWidthFar);
        farBottomRight = farPlaneDistance - (cam.transform.up * halfCameraHeightfar) + (cam.transform.right * CameraHalfWidthFar);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        //Plano Cercano
        DrawPlane(nearTopRight, nearBottomRight, nearBottomLeft, nearTopLeft);
        //Plano Lejano
        DrawPlane(farTopRight, farBottomRight, farBottomLeft, farTopLeft);
        // Plano Derecho
        DrawPlane(nearTopRight, farTopRight, farBottomRight, nearBottomRight);
        // Plano Izquierdo
        DrawPlane(nearTopLeft, farTopLeft, farBottomLeft, nearBottomLeft);
        // Plano Superior
        DrawPlane(nearTopLeft, farTopLeft, farTopRight, nearTopRight);
        //Plano Inferior
        DrawPlane(nearBottomLeft, farBottomLeft, farBottomRight, nearBottomRight);
    }

    public void DrawPlane(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);
    }
}
