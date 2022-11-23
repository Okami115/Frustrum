using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frustrum : MonoBehaviour
{
    private const int maxPlanes = 6;
    private const int maxObj = 4;
    private const int maxVertices = 8;

    Plane[] planes = new Plane[maxPlanes];
    [SerializeField] GameObject[] GameObj = new GameObject[maxObj];

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

    public struct Obj
    {
        public GameObject gameObject;
        public MeshRenderer meshRenderer;
        public MeshFilter meshFilter;
        public Vector3[] aabb;
        public Vector3 v3Extents;
        public Vector3 scale;
    }

    [SerializeField] Obj[] Objs = new Obj[maxObj];
    void Start()
    {
        cam = Camera.main;
        // Inicializar planos
        for (int i = 0; i < maxPlanes; i++)
        {
            planes[i] = new Plane();
        }
        // Definir objetos
        for (int i = 0; i < maxObj; i++)
        {
            Objs[i].gameObject = GameObj[i];
            Objs[i].meshRenderer = GameObj[i].GetComponent<MeshRenderer>();
            Objs[i].meshFilter = GameObj[i].GetComponent<MeshFilter>();
            Objs[i].aabb = new Vector3[maxVertices];
            Objs[i].v3Extents = Objs[i].meshRenderer.bounds.extents;
            Objs[i].scale = Objs[i].meshRenderer.bounds.size;
        }
    }

    // Update is called once per frame
    void Update()
    {
        SetPlanePoint();

        Vector3 nearPlanePos = cam.transform.position + cam.transform.forward * cam.nearClipPlane;
        Vector3 farPlanePos = cam.transform.position + cam.transform.forward * cam.farClipPlane;
        // Setea los planos en base a 3 esquinas de la camara
        planes[0].SetNormalAndPosition(cam.transform.forward, nearPlanePos);            //Superior
        planes[1].SetNormalAndPosition(cam.transform.forward * -1, farPlanePos);             //Inferior

        planes[2].Set3Points(cam.transform.position, farBottomLeft, farTopLeft);//left
        planes[3].Set3Points(cam.transform.position, farTopRight, farBottomRight);//right
        planes[4].Set3Points(cam.transform.position, farTopLeft, farTopRight);//top
        planes[5].Set3Points(cam.transform.position, farBottomRight, farBottomLeft);//bottom

        for (int i = 2; i < maxPlanes; i++)
        {
            planes[i].Flip();
        }
        for (int i = 0; i < maxObj; i++)
        {
            Checkcolition(Objs[i]);
        }
        for (int i = 0; i < maxObj; i++)
        {
            AABB(ref Objs[i]);
        }
    }

    public void AABB(ref Obj currentObject)
    {
        if (currentObject.scale != currentObject.gameObject.transform.localScale)
        {
            Quaternion rotation = currentObject.gameObject.transform.rotation;
            currentObject.gameObject.transform.rotation = Quaternion.identity;
            currentObject.v3Extents = currentObject.meshRenderer.bounds.extents;
            currentObject.scale = currentObject.gameObject.transform.localScale;
            currentObject.gameObject.transform.rotation = rotation;
        }


        Vector3 center = currentObject.meshRenderer.bounds.center;
        Vector3 size = currentObject.v3Extents;

        // Setea la posicion de las esquinas del collider del objeto
        currentObject.aabb[0] = new Vector3(center.x - size.x, center.y + size.y, center.z - size.z);  
        currentObject.aabb[1] = new Vector3(center.x + size.x, center.y + size.y, center.z - size.z);  
        currentObject.aabb[2] = new Vector3(center.x - size.x, center.y - size.y, center.z - size.z);  
        currentObject.aabb[3] = new Vector3(center.x + size.x, center.y - size.y, center.z - size.z);  
        currentObject.aabb[4] = new Vector3(center.x - size.x, center.y + size.y, center.z + size.z);  
        currentObject.aabb[5] = new Vector3(center.x + size.x, center.y + size.y, center.z + size.z);  
        currentObject.aabb[6] = new Vector3(center.x - size.x, center.y - size.y, center.z + size.z);  
        currentObject.aabb[7] = new Vector3(center.x + size.x, center.y - size.y, center.z + size.z);

        // tranformar las Posiciones en puntos en el espacio
        currentObject.aabb[0] = transform.TransformPoint(currentObject.aabb[0]);
        currentObject.aabb[1] = transform.TransformPoint(currentObject.aabb[1]);
        currentObject.aabb[2] = transform.TransformPoint(currentObject.aabb[2]);
        currentObject.aabb[3] = transform.TransformPoint(currentObject.aabb[3]);
        currentObject.aabb[4] = transform.TransformPoint(currentObject.aabb[4]);
        currentObject.aabb[5] = transform.TransformPoint(currentObject.aabb[5]);
        currentObject.aabb[6] = transform.TransformPoint(currentObject.aabb[6]);
        currentObject.aabb[7] = transform.TransformPoint(currentObject.aabb[7]);

        // Roto el punto en la direccion que rota el objeto (Punto a rotar , pivot en el que rota , angulo en cada eje)
        currentObject.aabb[0] = RotatePointAroundPivot(currentObject.aabb[0], currentObject.gameObject.transform.position, currentObject.gameObject.transform.rotation.eulerAngles);
        currentObject.aabb[1] = RotatePointAroundPivot(currentObject.aabb[1], currentObject.gameObject.transform.position, currentObject.gameObject.transform.rotation.eulerAngles);
        currentObject.aabb[2] = RotatePointAroundPivot(currentObject.aabb[2], currentObject.gameObject.transform.position, currentObject.gameObject.transform.rotation.eulerAngles);
        currentObject.aabb[3] = RotatePointAroundPivot(currentObject.aabb[3], currentObject.gameObject.transform.position, currentObject.gameObject.transform.rotation.eulerAngles);
        currentObject.aabb[4] = RotatePointAroundPivot(currentObject.aabb[4], currentObject.gameObject.transform.position, currentObject.gameObject.transform.rotation.eulerAngles);
        currentObject.aabb[5] = RotatePointAroundPivot(currentObject.aabb[5], currentObject.gameObject.transform.position, currentObject.gameObject.transform.rotation.eulerAngles);
        currentObject.aabb[6] = RotatePointAroundPivot(currentObject.aabb[6], currentObject.gameObject.transform.position, currentObject.gameObject.transform.rotation.eulerAngles);
        currentObject.aabb[7] = RotatePointAroundPivot(currentObject.aabb[7], currentObject.gameObject.transform.position, currentObject.gameObject.transform.rotation.eulerAngles);
    }
    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
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
        if (!Application.isPlaying)
        {
            return;
        }

        Gizmos.color = Color.red;

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

        for (int i = 0; i < maxObj; i++)
        {
            Gizmos.color = Color.blue;

            DrawAABB(ref Objs[i]);
        }

    }
    public void DrawAABB(ref Obj currentObject)
    {

        Gizmos.color = Color.blue;


        // Draw the AABB Box 
        Gizmos.DrawLine(currentObject.aabb[0], currentObject.aabb[1]);
        Gizmos.DrawLine(currentObject.aabb[1], currentObject.aabb[3]);
        Gizmos.DrawLine(currentObject.aabb[3], currentObject.aabb[2]);
        Gizmos.DrawLine(currentObject.aabb[2], currentObject.aabb[0]);
        Gizmos.DrawLine(currentObject.aabb[0], currentObject.aabb[4]);
        Gizmos.DrawLine(currentObject.aabb[4], currentObject.aabb[5]);
        Gizmos.DrawLine(currentObject.aabb[5], currentObject.aabb[7]);
        Gizmos.DrawLine(currentObject.aabb[7], currentObject.aabb[6]);
        Gizmos.DrawLine(currentObject.aabb[6], currentObject.aabb[4]);
        Gizmos.DrawLine(currentObject.aabb[7], currentObject.aabb[3]);
        Gizmos.DrawLine(currentObject.aabb[6], currentObject.aabb[2]);
        Gizmos.DrawLine(currentObject.aabb[5], currentObject.aabb[1]);



    }
    public void Checkcolition(Obj currentObject)
    {
        for (int i = 0; i < maxVertices; i++)
        {
            int counter = maxPlanes;

            for (int j = 0; j < maxPlanes; j++)
            {
                if (planes[j].GetSide(currentObject.aabb[i]))
                {
                    counter--;
                }
            }

            if (counter == 0)
            {
                //Debug.Log("Está adentro");
                currentObject.gameObject.SetActive(true);
                break;
            }
            else
            {
                //Debug.Log("Está fuera");
                currentObject.gameObject.SetActive(false);
            }
        }

    }
    public void DrawPlane(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);
    }
}
