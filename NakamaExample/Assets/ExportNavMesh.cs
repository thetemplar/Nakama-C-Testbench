
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

// Obj exporter component based on: http://wiki.unity3d.com/index.php?title=ObjExporter

public class ExportNavMeshToObj : MonoBehaviour
{
    /*
    struct TriangleEdgeF
    {
        public class EdgeF
        {
            public PointF p1;
            public PointF p2;
            public int Count;

            public static bool operator ==(EdgeF lhs, EdgeF rhs)
            {
                return (lhs.p1 == rhs.p1 && lhs.p2 == rhs.p2) || (lhs.p2 == rhs.p1 && lhs.p1 == rhs.p2);
            }
            public static bool operator !=(EdgeF lhs, EdgeF rhs)
            {
                return !(lhs.p1 == rhs.p1 && lhs.p2 == rhs.p2);
            }

        }
        public EdgeF e1;
        public EdgeF e2;
        public EdgeF e3;

        public PointF BarycentricMidpoint()
        {
            List<PointF> ps = new List<PointF>();
            ps.Add(e1.p1);
            ps.Add(e1.p2);
            ps.Add(e2.p1);
            ps.Add(e2.p2);
            ps.Add(e3.p1);
            ps.Add(e3.p2);
            ps = ps.Distinct().ToList();

            return new PointF((ps[0].X + ps[1].X + ps[2].X) / 3, (ps[0].Y + ps[1].Y + ps[2].Y) / 3);
        }
    }
    static List<TriangleEdgeF> _t = new List<TriangleEdgeF>();
    static List<TriangleEdgeF.EdgeF> _e = new List<TriangleEdgeF.EdgeF>();
    static List<TriangleEdgeF.EdgeF> _e_single = new List<TriangleEdgeF.EdgeF>();

    //[MenuItem("Custom/Export NavMesh to mesh")]
    static void Export()
    {
        _t.Clear();
        _e.Clear();
        _e_single.Clear();
        NavMeshTriangulation triangulatedNavMesh = NavMesh.CalculateTriangulation();

        Mesh mesh = new Mesh();
        mesh.name = "ExportedNavMesh";
        mesh.vertices = triangulatedNavMesh.vertices;
        mesh.triangles = triangulatedNavMesh.indices;
        string filename = Application.dataPath + "/" + Path.GetFileNameWithoutExtension(EditorApplication.currentScene) + " Exported NavMesh.obj";

        List<Vector3> vecs = mesh.vertices.ToList();
        for (int material = 0; material < mesh.subMeshCount; material++)
        {
            int[] triangles = mesh.GetTriangles(material);
            for (int i = 0; i < triangles.Length; i += 3)
            {
                PointF p1 = new PointF(vecs[triangles[i + 0]].x, vecs[triangles[i + 0]].z);
                PointF p2 = new PointF(vecs[triangles[i + 1]].x, vecs[triangles[i + 1]].z);
                PointF p3 = new PointF(vecs[triangles[i + 2]].x, vecs[triangles[i + 2]].z);
                TriangleEdgeF.EdgeF e1 = new TriangleEdgeF.EdgeF() { p1 = p1, p2 = p2 };
                TriangleEdgeF.EdgeF e2 = new TriangleEdgeF.EdgeF() { p1 = p1, p2 = p3 };
                TriangleEdgeF.EdgeF e3 = new TriangleEdgeF.EdgeF() { p1 = p2, p2 = p3 };
                TriangleEdgeF t1 = new TriangleEdgeF() { e1 = e1, e2 = e2, e3 = e3 };
                _t.Add(t1);

                _e.Add(e1);
                _e.Add(e2);
                _e.Add(e3);
            }
        }
        for (int i = 0; i < _t.Count; i++)
        {
            var t = _t[i];

            var c1 = _e.Count(x => x == t.e1);
            t.e1.Count = c1;
            if (c1 == 1)
                _e_single.Add(t.e1);
            var c2 = _e.Count(x => x == t.e2);
            t.e2.Count = c2;
            if (c2 == 1)
                _e_single.Add(t.e2);
            var c3 = _e.Count(x => x == t.e3);
            t.e3.Count = c3;
            if (c3 == 1)
                _e_single.Add(t.e3);
        }
        string _s = "m.Borders = make([]Edge, " + _e_single.Count + ")" + Environment.NewLine;
        string _ss = "";
        for (int j = 0; j < _e_single.Count; j++)
        {
            var e = _e_single[j];
            _s += "m.Borders[" + j + "] = Edge { A: PublicMatchState_Vector2Df { X: " + e.p1.X + ", Y: " + e.p1.Y + " }, B: PublicMatchState_Vector2Df { X: " + e.p2.X + ", Y: " + e.p2.Y + " } }" + Environment.NewLine;
        }

        _s += Environment.NewLine + Environment.NewLine;
        _s += "m.Triangles = make([]Triangle, " + _t.Count + ")" + Environment.NewLine;
        for (int j = 0; j < _t.Count; j++)
        {
            var t = _t[j];
            _s += "m.Triangles[" + j + "] = Triangle { A: PublicMatchState_Vector2Df { X: " + t.e1.p1.X + ", Y: " + t.e1.p1.Y + " }, B: PublicMatchState_Vector2Df { X: " + t.e2.p2.X + ", Y: " + t.e2.p2.Y + " }, C: PublicMatchState_Vector2Df { X: " + t.e3.p1.X + ", Y: " + t.e3.p1.Y + " }, W: PublicMatchState_Vector2Df { X: " + t.BarycentricMidpoint().X + ", Y: " + t.BarycentricMidpoint().Y + " } }" + Environment.NewLine;
            _ss += "Handles.Label(new Vector3(" + t.BarycentricMidpoint().X + "f, 0.1f, " + t.BarycentricMidpoint().Y + "f), \"" + j + "\");" + Environment.NewLine;
            _ss += "Debug.DrawLine(new Vector3(" + t.e1.p1.X + "f, 0.1f, " + t.e1.p1.Y + "f), new Vector3(" + t.e1.p2.X + "f, 0.1f, " + t.e1.p2.Y + "f), " + ((t.e1.Count > 1) ? "Color.blue" : "Color.red") + "); " + Environment.NewLine;
            _ss += "Debug.DrawLine(new Vector3(" + t.e2.p1.X + "f, 0.1f, " + t.e2.p1.Y + "f), new Vector3(" + t.e2.p2.X + "f, 0.1f, " + t.e2.p2.Y + "f), " + ((t.e2.Count > 1) ? "Color.blue" : "Color.red") + "); " + Environment.NewLine;
            _ss += "Debug.DrawLine(new Vector3(" + t.e3.p1.X + "f, 0.1f, " + t.e3.p1.Y + "f), new Vector3(" + t.e3.p2.X + "f, 0.1f, " + t.e3.p2.Y + "f), " + ((t.e3.Count > 1) ? "Color.blue" : "Color.red") + "); " + Environment.NewLine + Environment.NewLine;
        }
        File.WriteAllText(Application.dataPath + "/" + Path.GetFileNameWithoutExtension(EditorApplication.currentScene) + " Exported NavMesh GoLang.obj", _s);
        File.WriteAllText(Application.dataPath + "/" + Path.GetFileNameWithoutExtension(EditorApplication.currentScene) + " Exported NavMesh Unity.obj", _ss);
        //MeshToFile(mesh, filename);
        print("NavMesh exported in '" + Application.dataPath + "'");
        AssetDatabase.Refresh();
    }
    /*
    static string MeshToString(Mesh mesh)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("g ").Append(mesh.name).Append("\n");
        List<Vector3> vecs = new List<Vector3>();
        foreach (Vector3 v in mesh.vertices)
        {
            sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
            vecs.Add(v);
        }
        sb.Append("\n");
        foreach (Vector3 v in mesh.normals)
        {
            sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
        }
        sb.Append("\n");
        foreach (Vector3 v in mesh.uv)
        {
            sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
        }
        for (int material = 0; material < mesh.subMeshCount; material++)
        {
            sb.Append("\n");
            //sb.Append("usemtl ").Append(mats[material].name).Append("\n");
            //sb.Append("usemap ").Append(mats[material].name).Append("\n");

            int[] triangles = mesh.GetTriangles(material);
            for (int i = 0; i < triangles.Length; i += 3)
            {
                sb.Append(string.Format("f {0}/{1}  {2}/{3}  {4}/{5}\n", vecs[triangles[i]].x, vecs[triangles[i]].z, vecs[triangles[i + 1]].x, vecs[triangles[i + 1]].z, vecs[triangles[i + 2]].x, vecs[triangles[i + 2]].z));
                //sb.Append(string.Format("f {0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8}\n", triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
            }
        }
        return sb.ToString();
    }

    static void MeshToFile(Mesh mesh, string filename)
    {
        using (StreamWriter sw = new StreamWriter(filename))
        {
            sw.Write(MeshToString(mesh));
        }
    }*/
}
