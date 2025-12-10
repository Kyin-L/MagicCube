using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class MagicCubeController : MonoBehaviour
{
    public class ClickPoint
    {
        public Vector3 point;
        public Cube cube;

        public ClickPoint(Vector3 p,Cube c)
        {
            point = p;
            cube = c;
        }
    }

    public ClickPoint start;
    public ClickPoint end;

    int magicCubeLayerID;

    Cube[] cubes;
    GameObject center;
    Axis axis;
    List<Cube> rotatingCubes;
    bool isRotating;
    bool isClockwise;

    Vector3 mousePosition;

    void Start()
    {
        magicCubeLayerID = LayerMask.NameToLayer("MagicCube");
        cubes = this.GetComponentsInChildren<Cube>();
        center = new GameObject("Center");
        axis = Axis.None;
        center.transform.position = transform.position;
        rotatingCubes = new List<Cube>();
    }

    
    void Update()
    {
        if(!isRotating)
            CheckMouseMove();
    }

    void FixedUpdate()
    {
        if(isRotating)
        {
            Quaternion rotation = Quaternion.AngleAxis(isClockwise ? 90f : -90f, GetRotateAxisDir());
            Quaternion rot = Quaternion.Slerp(center.transform.rotation, rotation, 0.1f);
            center.transform.rotation = rot;
            if (Quaternion.Angle(center.transform.rotation, rotation) < 0.05f)
            {
                center.transform.rotation = rotation;
                foreach (var item in rotatingCubes)
                {
                    item.transform.SetParent(this.transform);
                    item.RefreshOffset();
                }
                rotatingCubes.Clear();
                center.transform.rotation = Quaternion.identity;
                start = null;
                end = null;
                axis = Axis.None;
                isRotating = false;
            }
        }
    }

    private void CheckMouseMove()
    {
        if(Input.GetMouseButtonDown(1))
        {
            mousePosition = Input.mousePosition;
        }
        if(Input.GetMouseButton(1))
        {
            Vector3 mouseMove = mousePosition - Input.mousePosition;
            this.transform.Rotate(-mouseMove.y/3f, mouseMove.x/3f,mouseMove.z,Space.World);
            mousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit, 100f, 1 << magicCubeLayerID))
            {
                start = new ClickPoint(transform.InverseTransformPoint(hit.point), hit.transform.GetComponent<Cube>());
            }
        }

        if (Input.GetMouseButton(0) && start != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit, 100f, 1 << magicCubeLayerID))
            {
                Cube cube = hit.transform.GetComponent<Cube>();
                if(start.cube != cube)
                {
                    end = new ClickPoint(transform.InverseTransformPoint(hit.point), cube);

                    StartRotate();
                }
            }
        }
    }

    private void StartRotate()
    {
        GetRotateAxis();
        AddRotateCube();
        CheckClockwise();
        isRotating = true;
    }

    private void GetRotateAxis()
    {
        int startZero = 0;
        int endZero = 0;
        if (start.cube.offset.x == 0)
            startZero += 1;
        if (start.cube.offset.y == 0)
            startZero += 1;
        if (start.cube.offset.z == 0)
            startZero += 1;

        if (end.cube.offset.x == 0)
            endZero += 1;
        if (end.cube.offset.y == 0)
            endZero += 1;
        if (end.cube.offset.z == 0)
            endZero += 1;

        if (startZero == 2 || endZero == 2)
        {
            if (start.cube.offset.x == 0 && end.cube.offset.x == 0)
                axis = Axis.X;
            if (start.cube.offset.y == 0 && end.cube.offset.y == 0)
                axis = Axis.Y;
            if (start.cube.offset.z == 0 && end.cube.offset.z == 0)
                axis = Axis.Z;
        }
        else if (startZero == 1)
        {
            if (start.cube.offset.x != 0 && !Mathf.Approximately(Mathf.Abs(start.point.x), 1.5f))
                axis = Axis.X;
            if (start.cube.offset.y != 0 && !Mathf.Approximately(Mathf.Abs(start.point.y), 1.5f))
                axis = Axis.Y;
            if (start.cube.offset.z != 0 && !Mathf.Approximately(Mathf.Abs(start.point.z), 1.5f))
                axis = Axis.Z;
        }
        else if (endZero == 1)
        {
            if (end.cube.offset.x != 0 && !Mathf.Approximately(Mathf.Abs(end.point.x), 1.5f))
                axis = Axis.X;
            if (end.cube.offset.y != 0 && !Mathf.Approximately(Mathf.Abs(end.point.y), 1.5f))
                axis = Axis.Y;
            if (end.cube.offset.z != 0 && !Mathf.Approximately(Mathf.Abs(end.point.z), 1.5f))
                axis = Axis.Z;
        }
    }

    private void AddRotateCube()
    {
        switch(axis)
        {
            case Axis.X:
                foreach(var item in cubes)
                {
                    if(item.offset.x == start.cube.offset.x)
                    {
                        item.gameObject.transform.parent = center.transform;
                        rotatingCubes.Add(item);
                    }
                }
                break;
            case Axis.Y:
                foreach (var item in cubes)
                {
                    if (item.offset.y == start.cube.offset.y)
                    {
                        item.gameObject.transform.parent = center.transform;
                        rotatingCubes.Add(item);
                    }
                }
                break;
            case Axis.Z:
                foreach (var item in cubes)
                {
                    if (item.offset.z == start.cube.offset.z)
                    {
                        item.gameObject.transform.parent = center.transform;
                        rotatingCubes.Add(item);
                    }
                }
                break;
        }
    }

    private void CheckClockwise()
    {
        Vector3 startDir = start.cube.offset;
        Vector3 endDir = end.cube.offset;

        switch (axis)
        {
            case Axis.X:
                startDir.x = 0;
                endDir.x = 0;
                isClockwise = Vector3.Cross(startDir, endDir).x > 0;
                break;
            case Axis.Y:
                startDir.y = 0;
                endDir.y = 0;
                isClockwise = Vector3.Cross(startDir, endDir).y > 0;
                break;
            case Axis.Z:
                startDir.z = 0;
                endDir.z = 0;
                isClockwise = Vector3.Cross(startDir, endDir).z > 0;
                break;
        }
    }

    public Vector3 GetRotateAxisDir()
    {
        Vector3 dir = Vector3.zero;
        switch (axis)
        {
            case Axis.X:
                dir = transform.right;
                break;
            case Axis.Y:
                dir = transform.up;
                break;
            case Axis.Z:
                dir = transform.forward;
                break;
        }
        return dir;
    }
}
