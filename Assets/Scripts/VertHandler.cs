#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scenes.MainHall
{
    [AddComponentMenu("Mesh/Vert Handler")]
    [ExecuteInEditMode]
    public class VertHandler : MonoBehaviour
    {
        private VertHandleGizmo[] _handles;

        private Mesh _mesh;
        private Vector3 _vertPos;
        private Vector3[] _verts;
        public new bool Destroy;
        public List<VertGroup> Groups; //list of vertex groups
        public bool GroupSelected; //group the selected set of verticies, add them to the groups list
        public bool RemoveFromAll; // removes the selected objects from all groups
        public bool RemoveFromGroup; // removes the selected objects from a given group


        public VertGroup SelectedHandles;

        private void OnEnable()
        {
            _mesh = GetComponent<MeshFilter>().sharedMesh;
            _verts = GetUniqueVerticies(_mesh.vertices);
            if (_handles != null) return;
            foreach (var vert in _verts)
            {
                _vertPos = transform.TransformPoint(vert);
                var handle = new GameObject("VertHandle");
                handle.transform.position = _vertPos;
                handle.transform.parent = transform;
                var gizmo = handle.AddComponent<VertHandleGizmo>();
                gizmo.Parent = this;
            }
        }

        private void OnDrawGizmos()
        {
            //display a list of the currently selected vertex handles
            _handles = gameObject.GetComponentsInChildren<VertHandleGizmo>();
            if (SelectedHandles == null)
                return;

            if (SelectedHandles.VertHandles == null)
                return;
            SelectedHandles.VertHandles.Clear();
            foreach (var handle in _handles)
                if (handle.Selected)
                    SelectedHandles.VertHandles.Add(handle.gameObject);
        }

        private void Update()
        {
            if (Destroy)
            {
                Destroy = false;
                DestroyImmediate(this);
                return;
            }


            if (GroupSelected)
            {
                GroupSelected = false;
                var newGroup = true;
                //for all existing groups
                foreach (var group in Groups)
                    //if this group has the same name as what's in the selected group
                    if (group.Name == SelectedHandles.Name)
                    {
                        //this group already exists
                        newGroup = false;
                        //add any new edges to the group
                        foreach (var vertHandle in SelectedHandles.VertHandles)
                        {
                            if (!group.VertHandles.Contains(vertHandle))
                                group.VertHandles.Add(vertHandle);
                            //make the edge know about the group it now belongs to
                            var gizmo = vertHandle.GetComponent<VertHandleGizmo>();
                            if (!gizmo.HasGroup(group))
                                gizmo.Groups.Add(group);
                        }
                        //set the group color
                        group.Color = SelectedHandles.Color;
                    }
                if (newGroup)
                {
                    var g = new VertGroup(
                        SelectedHandles); // selectedHandles.name, selectedHandles.edges, selectedHandles.color);
                    Groups.Add(g);
                }
            }

            if (RemoveFromGroup)
            {
                RemoveFromGroup = false;
                foreach (var group in Groups)
                    if (group.Name == SelectedHandles.Name)
                        foreach (var vertHandle in SelectedHandles.VertHandles)
                            if (group.VertHandles.Contains(vertHandle))
                            {
                                group.VertHandles.Remove(vertHandle);
                                var gizmo = vertHandle.GetComponent<VertHandleGizmo>();
                                if (gizmo.Groups.Contains(group))
                                    gizmo.Groups.Remove(group);
                            }
                CleanGroups();
            }

            if (RemoveFromAll)
            {
                RemoveFromAll = false;
                foreach (var group in Groups)
                foreach (var vertHandle in SelectedHandles.VertHandles)
                    if (group.VertHandles.Contains(vertHandle))
                    {
                        group.VertHandles.Remove(vertHandle);
                        var gizmo = vertHandle.GetComponent<VertHandleGizmo>();
                        if (gizmo.Groups.Contains(group))
                            gizmo.Groups.Remove(group);
                    }
                CleanGroups();
            }
        }


        //clean out groups that have no edges in them
        private void CleanGroups()
        {
            var toRemove = new List<VertGroup>();
            foreach (var group in Groups)
                if (group.VertHandles.Count == 0)
                    toRemove.Add(group);
            foreach (var group in toRemove)
                Groups.Remove(group);
        }

        private Vector3[] GetUniqueVerticies(Vector3[] verticies)
        {
            var uniqueVerts = new List<Vector3>();

            foreach (var vert in verticies)
                if (!uniqueVerts.Contains(vert))
                    uniqueVerts.Add(vert);


            return uniqueVerts.ToArray();
        }
    }

    [Serializable]
    public class VertGroup
    {
        public Color Color;

        public string Name;
        public List<GameObject> VertHandles;


        public VertGroup(VertGroup selectedGroup)
        {
            Name = selectedGroup.Name;
            VertHandles = new List<GameObject>();
            Color = selectedGroup.Color;
            foreach (var vertHandle in selectedGroup.VertHandles)
            {
                VertHandles.Add(vertHandle);
                var gizmo = vertHandle.GetComponent<VertHandleGizmo>();
                gizmo.Groups.Add(this);
            }
        }
    }

    [ExecuteInEditMode]
    public class VertHandleGizmo : MonoBehaviour
    {
        private const double Tolerance = .01;
        private static float _currentSize = 0.1f;
        private readonly Color _color = Color.white;
        private float _lastKnownSize = _currentSize;
        public new bool Destroy;
        public List<VertGroup> Groups = new List<VertGroup>(); // the groups this gizmo belongs to
        public VertHandler Parent;
        public bool Selected;

        public float Size = _currentSize;

        private void Update()
        {
            // Change the size if the user requests it

            if (Math.Abs(_lastKnownSize - Size) > Tolerance)
            {
                _lastKnownSize = Size;
                _currentSize = Size;
            }

            // Ensure the rest of the gizmos know the size has changed...
            if (Math.Abs(_currentSize - _lastKnownSize) > Tolerance)
            {
                _lastKnownSize = _currentSize;
                Size = _lastKnownSize;
            }

            if (Destroy)
                DestroyImmediate(Parent);
        }

        private void OnDrawGizmos()
        {
            var toCenter = (Parent.transform.position - transform.position).normalized;
            var up = Vector3.Cross(Vector3.right, toCenter).normalized;
            var right = Vector3.Cross(toCenter, up).normalized;
            var radius = _currentSize;

            //draw an icon for each group this vertex belongs to
            var angleStep = 360f / Groups.Count;
            for (var i = 0; i < Groups.Count; i++)
            {
                var group = Groups[i];
                Gizmos.color = group.Color;
                var angle = angleStep * i;
                var rad = Mathf.Deg2Rad * angle;
                var x = Mathf.Cos(rad) * radius;
                var y = Mathf.Sin(rad) * radius;
                var position = transform.position + right * x + up * y;
                Gizmos.DrawCube(position, Vector3.one * _currentSize * .5f);
            }

            //draw the selectable cube
            Gizmos.color = _color;
            Selected = Selection.Contains(gameObject); //IsSelected();
            if (Selected)
                Gizmos.color = Color.black;
            Gizmos.DrawCube(transform.position, Vector3.one * _currentSize);
        }

        public bool HasGroup(VertGroup g)
        {
            foreach (var group in Groups)
                if (group.Name == g.Name)
                    return true;
            return false;
        }
    }
}

#endif
