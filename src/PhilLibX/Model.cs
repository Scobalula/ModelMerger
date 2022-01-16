// ------------------------------------------------------------------------
// PhilLibX - My Utility Library
// Copyright(c) 2018 Philip/Scobalula
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ------------------------------------------------------------------------
// File: Model.cs
// Author: Philip/Scobalula
// Description: A class to hold a 3-D Model and perform operations such as export, etc. on it
using PhilLibX.Mathematics;
using PhilLibX.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhilLibX
{
    /// <summary>
    /// A class to hold a 3-D Model
    /// </summary>
    public class Model
    {
        /// <summary>
        /// A class to hold a Bone
        /// </summary>
        public class Bone
        {
            /// <summary>
            /// Gets or Sets the name of the bone
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or Sets the index of the parent bone
            /// </summary>
            public int ParentIndex { get; set; }

            /// <summary>
            /// Gets or Sets the position relative to the parent
            /// </summary>
            public Vector3 LocalPosition { get; set; }

            /// <summary>
            /// Gets or sets the rotation relative to the parent
            /// </summary>
            public Quaternion LocalRotation { get; set; }

            /// <summary>
            /// Gets or Sets the position relative to the parent
            /// </summary>
            public Vector3 GlobalPosition { get; set; }

            /// <summary>
            /// Gets or sets the rotation relative to the parent
            /// </summary>
            public Quaternion GlobalRotation { get; set; }

            /// <summary>
            /// Gets or Sets the scale
            /// </summary>
            public Vector3 Scale { get; set; }

            /// <summary>
            /// Creates a new Bone with the given data
            /// </summary>
            /// <param name="name">Bone Name</param>
            public Bone(string name)
            {
                Name = name;
                ParentIndex = -1;
                LocalPosition = new Vector3(0, 0, 0);
                LocalRotation = new Quaternion(0, 0, 0, 1);
                Scale = new Vector3(1.0f, 1.0f, 1.0f);
            }

            /// <summary>
            /// Creates a new Bone with the given data
            /// </summary>
            /// <param name="name">Bone Name</param>
            /// <param name="parent">Bone Parent</param>
            /// <param name="pos">Bone Position</param>
            /// <param name="rot">Bone Rotation</param>
            public Bone(string name, int parent, Vector3 pos, Quaternion rot)
            {
                Name = name;
                ParentIndex = parent;
                LocalPosition = pos;
                LocalRotation = rot;
                Scale = new Vector3(1.0f, 1.0f, 1.0f);
            }

            /// <summary>
            /// Creates a new Bone with the given data
            /// </summary>
            /// <param name="name">Bone Name</param>
            /// <param name="parent">Bone Parent</param>
            /// <param name="lPos">Local Bone Position</param>
            /// <param name="lRot">Local Bone Rotation</param>
            /// <param name="gPos">Global Bone Position</param>
            /// <param name="gRot">Local Bone Rotation</param>
            public Bone(string name, int parent, Vector3 lPos, Quaternion lRot, Vector3 gPos, Quaternion gRot)
            {
                Name = name;
                ParentIndex = parent;
                LocalPosition = lPos;
                LocalRotation = lRot;
                GlobalPosition = gPos;
                GlobalRotation = gRot;
                Scale = new Vector3(1.0f, 1.0f, 1.0f);
            }

            /// <summary>
            /// 
            /// </summary>
            public void HierarchicalSort(List<Bone> sorted, List<Bone> source, List<Bone> results)
            {
                results.Add(this);

                foreach(var bone in sorted)
                {
                    if(bone.ParentIndex > -1)
                    {
                        if (source[bone.ParentIndex] == this)
                        {
                            bone.HierarchicalSort(sorted, source, results);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// A class to hold a vertex
        /// </summary>
        public class Vertex
        {
            /// <summary>
            /// A class to hold a vertex weight
            /// </summary>
            public class Weight
            {
                /// <summary>
                /// Gets or Sets the bone index
                /// </summary>
                public int BoneIndex { get; set; }

                /// <summary>
                /// Gets or Sets the Influence
                /// </summary>
                public float Influence { get; set; }

                public Weight(int boneIndex)
                {
                    BoneIndex = boneIndex;
                    Influence = 1.0f;
                }

                public Weight(int boneIndex, float influence)
                {
                    BoneIndex = boneIndex;
                    Influence = influence;
                }
            }

            /// <summary>
            /// A class to hold a vertex shape
            /// </summary>
            public class Shape
            {
                /// <summary>
                /// Gets or Sets the shape index
                /// </summary>
                public int ShapeIndex { get; set; }

                /// <summary>
                /// Gets or Sets the delta
                /// </summary>
                public Vector3 Delta { get; set; }

                public Shape(int shapeIndex, Vector3 delta)
                {
                    ShapeIndex = shapeIndex;
                    Delta = delta;
                }
            }

            /// <summary>
            /// Gets or Sets the vertex position
            /// </summary>
            public Vector3 Position { get; set; }

            /// <summary>
            /// Gets or Sets the vertex normal
            /// </summary>
            public Vector3 Normal { get; set; }

            /// <summary>
            /// Gets or Sets the vertex tangent
            /// </summary>
            public Vector3 Tangent { get; set; }

            /// <summary>
            /// Gets or Sets the vertex color
            /// </summary>
            public Vector4 Color { get; set; }

            /// <summary>
            /// Gets or Sets the vertex uv sets
            /// </summary>
            public List<Vector2> UVs { get; set; }

            /// <summary>
            /// Gets or Sets the vertex weights
            /// </summary>
            public List<Weight> Weights { get; set; }

            /// <summary>
            /// Gets or Sets the Shapes
            /// </summary>
            public List<Shape> Shapes { get; set; }

            /// <summary>
            /// Creates a new vertex
            /// </summary>
            public Vertex()
            {
                Position = new Vector3(0.0f, 0.0f, 0.0f);
                Normal   = new Vector3(0.0f, 0.0f, 0.0f);
                Tangent  = new Vector3(0.0f, 0.0f, 0.0f);
                Color    = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
                Weights  = new List<Weight>(8);
                UVs      = new List<Vector2>(4);
                Shapes = new List<Shape>();
            }

            /// <summary>
            /// Creates a new vertex
            /// </summary>
            /// <param name="position">Position</param>
            public Vertex(Vector3 position)
            {
                Position = position;
                Normal   = new Vector3(0.0f, 0.0f, 0.0f);
                Tangent  = new Vector3(0.0f, 0.0f, 0.0f);
                Color    = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
                Weights  = new List<Weight>(8);
                UVs      = new List<Vector2>(4);
                Shapes = new List<Shape>();
            }

            /// <summary>
            /// Creates a new vertex
            /// </summary>
            /// <param name="position">Position</param>
            /// <param name="normal">Normal</param>
            public Vertex(Vector3 position, Vector3 normal)
            {
                Position = position;
                Normal   = normal;
                Tangent  = new Vector3(0.0f, 0.0f, 0.0f);
                Color    = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
                Weights  = new List<Weight>(8);
                UVs      = new List<Vector2>(8);
                Shapes = new List<Shape>();
            }

            /// <summary>
            /// Creates a new vertex
            /// </summary>
            /// <param name="position">Position</param>
            /// <param name="normal">Normal</param>
            /// <param name="tangent">Tangent</param>
            public Vertex(Vector3 position, Vector3 normal, Vector3 tangent)
            {
                Position = position;
                Normal   = normal;
                Tangent  = tangent;
                Color    = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
                Weights  = new List<Weight>(8);
                UVs      = new List<Vector2>(8);
                Shapes = new List<Shape>();
            }

            /// <summary>
            /// Creates a new vertex
            /// </summary>
            /// <param name="position">Position</param>
            /// <param name="normal">Normal</param>
            /// <param name="tangent">Tangent</param>
            public Vertex(Vector3 position, Vector3 normal, Vector3 tangent, Vector2 uv)
            {
                Position = position;
                Normal = normal;
                Tangent = tangent;
                Color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
                Weights = new List<Weight>(8);
                UVs = new List<Vector2>(8);
                Shapes = new List<Shape>();

                UVs.Add(uv);
            }

            public void NormalizeWeights()
            {
                var weightSum = 0.0f;

                foreach (var weight in Weights)
                    weightSum += weight.Influence;

                var multiplier = 1.0f / weightSum;

                foreach (var weight in Weights)
                    weightSum *= multiplier;
            }
        }

        /// <summary>
        /// A class to hold a face with multiple vertex indices
        /// </summary>
        public class Face
        {
            /// <summary>
            /// Gets or Sets the face indices
            /// </summary>
            public int[] Indices { get; set; }

            /// <summary>
            /// Creates a new face with the given number of vertices
            /// </summary>
            /// <param name="vertexCount">Vertex count</param>
            public Face(int vertexCount)
            {
                Indices = new int[vertexCount];
            }

            /// <summary>
            /// Creates a new polygon face with the given vertex indices
            /// </summary>
            /// <param name="v1">Index 1</param>
            /// <param name="v2">Index 2</param>
            /// <param name="v3">Index 3</param>
            public Face(int v1, int v2, int v3)
            {
                Indices = new int[3];

                Indices[0] = v1;
                Indices[1] = v2;
                Indices[2] = v3;
            }
        }

        /// <summary>
        /// A class to hold a mesh
        /// </summary>
        public class Mesh
        {
            /// <summary>
            /// Gets or Sets the vertices
            /// </summary>
            public List<Vertex> Vertices { get; set; }

            /// <summary>
            /// Gets or Sets the faces
            /// </summary>
            public List<Face> Faces { get; set; }

            /// <summary>
            /// Gets or Sets the material indices
            /// </summary>
            public List<int> MaterialIndices { get; set; }

            /// <summary>
            /// Creates a new mesh
            /// </summary>
            public Mesh()
            {
                Vertices        = new List<Vertex>();
                Faces           = new List<Face>();
                MaterialIndices = new List<int>();
            }

            /// <summary>
            /// Creates a new mesh and preallocates the data counts
            /// </summary>
            /// <param name="vertexCount">Number of vertices</param>
            /// <param name="faceCount">Number of faces</param>
            public Mesh(int vertexCount, int faceCount)
            {
                Vertices        = new List<Vertex>(vertexCount);
                Faces           = new List<Face>(faceCount);
                MaterialIndices = new List<int>();
            }
        }

        /// <summary>
        /// A class to hold a material
        /// </summary>
        public class Material
        {
            /// <summary>
            /// Gets or Sets the name of the material
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or Sets the material images
            /// </summary>
            public Dictionary<string, string> Images { get; set; }

            /// <summary>
            /// Gets or Sets the material settings
            /// </summary>
            public Dictionary<string, object> Settings { get; set; }

            /// <summary>
            /// Gets or Sets the name of the diffuse map key in the images dictionary
            /// </summary>
            public string DiffuseMapName { get; set; }

            /// <summary>
            /// Gets or Sets the name of the normal map key in the images dictionary
            /// </summary>
            public string NormalMapName { get; set; }

            /// <summary>
            /// Gets or Sets the name of the specular map key in the images dictionary
            /// </summary>
            public string SpecularMapName { get; set; }

            /// <summary>
            /// Gets or Sets the name of the gloss map key in the images dictionary
            /// </summary>
            public string GlossMapName { get; set; }

            /// <summary>
            /// Creates a new material with the given name
            /// </summary>
            /// <param name="name">Material name</param>
            public Material(string name)
            {
                Name = name;
                Images = new Dictionary<string, string>();
                Settings = new Dictionary<string, object>();
                DiffuseMapName  = "DiffuseMap";
                NormalMapName   = "NormalMap";
                SpecularMapName = "SpecularMap";
                GlossMapName    = "GlossMap";
            }

            /// <summary>
            /// Gets the image of the given type
            /// </summary>
            /// <param name="key">Image Key/Type</param>
            /// <returns>Resulting Images</returns>
            public string GetImage(string key)
            {
                return Images.TryGetValue(key, out var image) ? image : "";
            }

            /// <summary>
            /// Gets the name of the material as a string representation of it
            /// </summary>
            /// <returns>Material name</returns>
            public override string ToString()
            {
                return Name;
            }
        }

        /// <summary>
        /// Gets or Sets the Model Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets the bones/joints
        /// </summary>
        public List<Bone> Bones { get; set; }

        /// <summary>
        /// Gets or Sets the meshes
        /// </summary>
        public List<Mesh> Meshes { get; set; }

        /// <summary>
        /// Gets or Sets the materials
        /// </summary>
        public List<Material> Materials { get; set; }

        /// <summary>
        /// Gets or Sets the shape keys
        /// </summary>
        public List<string> Shapes { get; set; }

        /// <summary>
        /// Creates a new Model
        /// </summary>
        public Model()
        {
            Bones     = new List<Bone>();
            Meshes    = new List<Mesh>();
            Materials = new List<Material>();
            Shapes = new List<string>();
        }

        /// <summary>
        /// Creates a new Model
        /// </summary>
        /// <param name="name">Model Name</param>
        public Model(string name)
        {
            Name      = name;
            Bones     = new List<Bone>();
            Meshes    = new List<Mesh>();
            Materials = new List<Material>();
            Shapes = new List<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void HierarchicalSort()
        {
            if (Bones.Count > 0)
            {
                // We have to again sort by bone parents so we're processing parents before children
                var sorted = new List<Bone>(Bones.Count);
                var input = Bones.OrderBy(x => x.ParentIndex).ToList();

                // Loop and find parent bones
                foreach (var bone in input)
                {
                    if (bone.ParentIndex != -1)
                        break;

                    bone.HierarchicalSort(input, Bones, sorted);
                }


                // Remap parents
                foreach (var bone in sorted)
                    if (bone.ParentIndex > -1)
                        bone.ParentIndex = sorted.IndexOf(Bones[bone.ParentIndex]);

                // Remap weights
                foreach (var mesh in Meshes)
                    foreach (var vtx in mesh.Vertices)
                        foreach (var weight in vtx.Weights)
                            weight.BoneIndex = sorted.IndexOf(Bones[weight.BoneIndex]);

                Bones = sorted;
            }
        }

        /// <summary>
        /// Generates global positions and rotations
        /// </summary>
        public void GenerateGlobalBoneData(bool requiresSort = false)
        {
            // We must sort by hierarchy to ensure parents are processed before children
            if(requiresSort)
                HierarchicalSort();

            foreach(var bone in Bones)
            {
                if (bone.ParentIndex > -1)
                {
                    // We need to compute the offset that is rotated based off the parent's data
                    // TODO: use quat rotation stuff now
                    bone.GlobalPosition = Bones[bone.ParentIndex].GlobalPosition + Bones[bone.ParentIndex].GlobalRotation.ToMatrix().TransformVector(bone.LocalPosition);
                    bone.GlobalRotation = Bones[bone.ParentIndex].GlobalRotation * bone.LocalRotation;
                }
                else
                {
                    bone.GlobalPosition = bone.LocalPosition;
                    bone.GlobalRotation = bone.LocalRotation;
                }
            }
        }

        /// <summary>
        /// Generates local positions and rotations
        /// </summary>
        public void GenerateLocalBoneData(bool requiresSort = false)
        {
            // We must sort by hierarchy to ensure parents are processed before children
            if (requiresSort)
                HierarchicalSort();

            foreach (var bone in Bones)
            {
                if (bone.ParentIndex > -1)
                {
                    // We need to compute the offset that is rotated based off the parent's data
                    // We inverse and subtract for local data since now we're relative to the parent bone
                    // TODO: use quat rotation stuff now
                    bone.LocalPosition = Bones[bone.ParentIndex].GlobalRotation.Inverse().ToMatrix().TransformVector(bone.GlobalPosition - Bones[bone.ParentIndex].GlobalPosition);
                    bone.LocalRotation = Bones[bone.ParentIndex].GlobalRotation.Inverse() * bone.GlobalRotation;
                }
                else
                {
                    bone.LocalPosition = bone.GlobalPosition;
                    bone.LocalRotation = bone.GlobalRotation;
                }
            }
        }

        /// <summary>
        /// Scales the model by the given value
        /// </summary>
        /// <param name="value">Value to scale the model by</param>
        public void Scale(float value)
        {
            if (value == 1.0f)
                return;

            foreach(var bone in Bones)
            {
                bone.LocalPosition *= value;
                bone.GlobalPosition *= value;
            }

            foreach(var mesh in Meshes)
            {
                foreach(var vertex in mesh.Vertices)
                {
                    vertex.Position *= value;
                }
            }
        }

        /// <summary>
        /// Checks if the model contains a bone
        /// </summary>
        /// <param name="bone">Bone to locate</param>
        /// <returns>True if the bone exists, otherwise false</returns>
        public bool HasBone(string bone)
        {
            return Bones.Find(x => x.Name == bone) != null;
        }

        /// <summary>
        /// Saves the file to the given format, determined by the extension
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            // Directory.CreateDirectory(Path.GetDirectoryName(path));

            var extension = Path.GetExtension(path).ToLower();

            switch(extension)
            {
                case ".obj": ToOBJ(path); return;
                case ".semodel": ToSEModel(path); return;
                case ".ascii": ToXNALaraASCII(path); return;
                //case ".ma": ToMA(path); return;
                //case ".fbx": ToFBX(path); return;
                //case ".xmodel_export": ToXME(path); return;
                //case ".xmodel_bin": ToXMB(path); return;
                case ".smd": ToSMD(path); return;
                default: throw new ArgumentException("Invalid Model Extension", "extension");
            }
        }

        /// <summary>
        /// Saves the model to an obj
        /// </summary>
        /// <param name="path">Output Path</param>
        internal void ToOBJ(string path)
        {
            using (var writer = new StreamWriter(path))
            {
                var globalVertexIndex = 1;

                foreach(var mesh in Meshes)
                {
                    writer.WriteLine("usemtl {0}", Materials[mesh.MaterialIndices[0]]);
                    writer.WriteLine("o {0}", Materials[mesh.MaterialIndices[0]]);
                    writer.WriteLine("g {0}", Materials[mesh.MaterialIndices[0]]);

                    foreach (var vertex in mesh.Vertices)
                    {
                        writer.WriteLine("v {0:0.00000} {1:0.00000} {2:0.00000}", vertex.Position.X, vertex.Position.Y, vertex.Position.Z);
                    }

                    foreach (var vertex in mesh.Vertices)
                    {
                        writer.WriteLine("vn {0:0.00000} {1:0.00000} {2:0.00000}", vertex.Normal.X, vertex.Normal.Y, vertex.Normal.Z);
                    }

                    foreach (var vertex in mesh.Vertices)
                    {
                        writer.WriteLine("vt {0:0.00000} {1:0.00000}", vertex.UVs[0].X, vertex.UVs[0].Y);
                    }

                    foreach (var face in mesh.Faces)
                    {
                        writer.Write("f ");

                        foreach(var index in face.Indices)
                        {
                            writer.Write("{0}/{0}/{0} ", globalVertexIndex + index);
                        }

                        writer.WriteLine();
                    }

                    globalVertexIndex += mesh.Vertices.Count;
                }
            }
        }

        /// <summary>
        /// Saves the model to a SEModel
        /// </summary>
        /// <param name="path">Output Path</param>
        internal void ToSEModel(string path)
        {
            var dir = Path.GetDirectoryName(path);

            if (!string.IsNullOrWhiteSpace(dir))
                Directory.CreateDirectory(dir);

            using (var writer = new BinaryWriter(File.Create(path)))
            {
                writer.Write(new byte[] { 0x53, 0x45, 0x4D, 0x6F, 0x64, 0x65, 0x6C });
                writer.Write((ushort)0x1);
                writer.Write((ushort)0x14);
                writer.Write((byte)0x7); // Data Presence
                writer.Write((byte)0x7); // Bone Data Presence
                writer.Write((byte)0xF); // Mesh Data Presence
                writer.Write(Bones.Count);
                writer.Write(Meshes.Count);
                writer.Write(Materials.Count);
                writer.Write(new byte[] { 0x0, 0x0, 0x0 });

                foreach(var bone in Bones)
                {
                    writer.WriteNullTerminatedString(bone.Name);
                }

                foreach(var bone in Bones)
                {
                    writer.Write((byte)0); // Unused flags

                    writer.Write(bone.ParentIndex);

                    writer.WriteStruct(bone.GlobalPosition);
                    writer.WriteStruct(bone.GlobalRotation);
                    writer.WriteStruct(bone.LocalPosition);
                    writer.WriteStruct(bone.LocalRotation);
                    writer.WriteStruct(bone.Scale);
                }

                foreach(var mesh in Meshes)
                {
                    writer.Write((byte)0); // Unused flags

                    var weightCount = 0;
                    var materialCount = mesh.MaterialIndices.Count;

                    // Loop to determine weights and materials
                    // We can have multiple UVs with 1 material, etc.
                    foreach (var vertex in mesh.Vertices)
                    {
                        if(vertex.Weights.Count > weightCount)
                            weightCount = vertex.Weights.Count;
                        if(vertex.UVs.Count > materialCount)
                            materialCount = vertex.UVs.Count;
                    }

                    writer.Write((byte)materialCount);
                    writer.Write((byte)weightCount);
                    writer.Write(mesh.Vertices.Count);
                    writer.Write(mesh.Faces.Count);

                    foreach(var vertex in mesh.Vertices)
                    {
                        writer.WriteStruct(vertex.Position);
                    }

                    foreach(var vertex in mesh.Vertices)
                    {
                        for(int i = 0; i < materialCount; i++)
                        {
                            writer.WriteStruct(vertex.UVs[i < vertex.UVs.Count ? i : vertex.UVs.Count - 1]);
                        }
                    }

                    foreach(var vertex in mesh.Vertices)
                    {
                        writer.WriteStruct(vertex.Normal);
                    }

                    foreach(var vertex in mesh.Vertices)
                    {
                        writer.Write(0xFFFFFFFF);
                    }

                    foreach(var vertex in mesh.Vertices)
                    {
                        var boneIndex = 0;
                        var influence = 0.0f;

                        for(int i = 0; i < weightCount; i++)
                        {
                            boneIndex = i < vertex.Weights.Count ? vertex.Weights[i].BoneIndex : 0;
                            influence = i < vertex.Weights.Count ? vertex.Weights[i].Influence : 0;

                            if (Bones.Count <= 0xFF)
                                writer.Write((byte)boneIndex);
                            else if (Bones.Count <= 0xFFFF)
                                writer.Write((ushort)boneIndex);
                            else
                                writer.Write(boneIndex);

                            writer.Write(influence);

                        }
                    }

                    foreach (var face in mesh.Faces)
                    {
                        if (mesh.Vertices.Count <= 0xFF)
                        {
                            writer.Write((byte)face.Indices[0]);
                            writer.Write((byte)face.Indices[1]);
                            writer.Write((byte)face.Indices[2]);
                        }
                        else if (mesh.Vertices.Count <= 0xFFFF)
                        {
                            writer.Write((ushort)face.Indices[0]);
                            writer.Write((ushort)face.Indices[1]);
                            writer.Write((ushort)face.Indices[2]);
                        }
                        else
                        {
                            writer.Write((uint)face.Indices[0]);
                            writer.Write((uint)face.Indices[1]);
                            writer.Write((uint)face.Indices[2]);
                        }
                    }

                    for (int i = 0; i < materialCount; i++)
                    {
                        writer.Write(i < mesh.MaterialIndices.Count ? mesh.MaterialIndices[i] : mesh.MaterialIndices[mesh.MaterialIndices.Count - 1]);
                    }
                }

                foreach (var material in Materials)
                {
                    writer.WriteNullTerminatedString(material.Name);
                    writer.Write(true);
                    writer.WriteNullTerminatedString(material.GetImage(material.DiffuseMapName));
                    writer.WriteNullTerminatedString(material.GetImage(material.NormalMapName));
                    writer.WriteNullTerminatedString(material.GetImage(material.SpecularMapName));
                }

                writer.Write(0xA646E656C424553);
                writer.Write((long)Shapes.Count);

                foreach (var shape in Shapes)
                {
                    writer.WriteNullTerminatedString(shape);
                }

                foreach (var mesh in Meshes)
                {
                    foreach (var vertex in mesh.Vertices)
                    {
                        writer.Write(vertex.Shapes.Count);

                        foreach (var shape in vertex.Shapes)
                        {
                            writer.Write(shape.ShapeIndex);
                            writer.Write(shape.Delta.X);
                            writer.Write(shape.Delta.Y);
                            writer.Write(shape.Delta.Z);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Saves the model to an SMD
        /// </summary>
        /// <param name="path">Output Path</param>
        internal void ToSMD(string path)
        {
            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine("version 1");

                writer.WriteLine("nodes");
                for(int i = 0; i < Bones.Count; i++)
                    writer.WriteLine("{0} \"{1}\" {2}", i, Bones[i].Name, Bones[i].ParentIndex);
                writer.WriteLine("end");

                writer.WriteLine("skeleton");
                writer.WriteLine("time 0");

                for (int i = 0; i < Bones.Count; i++)
                {
                    var rotation = Bones[i].LocalRotation.ToEuler();
                    writer.WriteLine("{0} {1:0.0000} {2:0.0000} {3:0.0000} {4:0.0000} {5:0.0000} {6:0.0000}",
                        i,
                        Bones[i].LocalPosition.X,
                        Bones[i].LocalPosition.Y,
                        Bones[i].LocalPosition.Z,
                        rotation.X,
                        rotation.Y,
                        rotation.Z);
                }

                writer.WriteLine("end");

                foreach(var mesh in Meshes)
                {
                    writer.WriteLine("triangles");

                    foreach(var face in mesh.Faces)
                    {
                        writer.WriteLine(Materials[mesh.MaterialIndices[0]].Name);

                        for(int i = 0; i < 3; i++)
                        {
                            var vertex = mesh.Vertices[face.Indices[i]];
                            var normal = vertex.Normal.Normalize();

                            writer.Write("0 {0:0.0000} {1:0.0000} {2:0.0000} {3:0.0000} {4:0.0000} {5:0.0000} {6:0.0000} {7:0.0000} {8} ",
                                vertex.Position.X,
                                vertex.Position.Y,
                                vertex.Position.Z,
                                normal.X,
                                normal.Y,
                                normal.Z,
                                vertex.UVs[0].X,
                                vertex.UVs[0].Y,
                                vertex.Weights.Count);

                            foreach (var weight in vertex.Weights)
                                writer.Write("{0} {1:0.0000} ", weight.BoneIndex, weight.Influence);

                            writer.WriteLine();
                        }
                    }

                    writer.WriteLine("end");
                }
            }
        }

        /// <summary>
        /// Saves the model to an XNA Lara ASCII
        /// </summary>
        /// <param name="path">Output Path</param>
        internal void ToXNALaraASCII(string path)
        {
            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine(Bones.Count);

                foreach(var bone in Bones)
                {
                    writer.WriteLine(bone.Name);
                    writer.WriteLine(bone.ParentIndex);
                    writer.WriteLine("{0} {1} {2}",
                        bone.GlobalPosition.X,
                        bone.GlobalPosition.Y,
                        bone.GlobalPosition.Z);
                }

                writer.WriteLine(Meshes.Count);

                foreach(var mesh in Meshes)
                {

                    writer.WriteLine(Materials[mesh.MaterialIndices[0]].Name);
                    writer.WriteLine(1);
                    writer.WriteLine(0);
                    writer.WriteLine(mesh.Vertices.Count);

                    foreach(var vertex in mesh.Vertices)
                    {
                        writer.WriteLine("{0} {1} {2}", 
                            vertex.Position.X,
                            vertex.Position.Y,
                            vertex.Position.Z);
                        writer.WriteLine("{0} {1} {2}",
                            vertex.Normal.X,
                            vertex.Normal.Y,
                            vertex.Normal.Z);
                        writer.WriteLine("{0} {1} {2} {3}",
                            (byte)(vertex.Color.X * 255.0f),
                            (byte)(vertex.Color.Y * 255.0f),
                            (byte)(vertex.Color.Z * 255.0f),
                            (byte)(vertex.Color.W * 255.0f));
                        writer.WriteLine("{0} {1}",
                            vertex.UVs[0].X,
                            vertex.UVs[0].Y);

                        for (int i = 0; i < 4; i++)
                            writer.Write("{0} ", i < vertex.Weights.Count ? vertex.Weights[i].BoneIndex : 0);
                        writer.WriteLine();
                        for (int i = 0; i < 4; i++)
                            writer.Write("{0} ", i < vertex.Weights.Count ? vertex.Weights[i].Influence : 0.0f);
                        writer.WriteLine();
                    }

                    writer.WriteLine(mesh.Faces.Count);

                    foreach (var face in mesh.Faces)
                    {
                        writer.WriteLine("{0} {1} {2}", face.Indices[0], face.Indices[1], face.Indices[2]);
                    }
                }

                writer.WriteLine();
            }
        }
    }
}
