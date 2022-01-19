// ------------------------------------------------------------------------
// Model Merger - Simple Tool to Merge Models
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
// File: Program.cs
// Author: Philip/Scobalula
// Description: Main App Logic
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SELib;
using PhilLibX;
using PhilLibX.Mathematics;
using System.Reflection;

namespace ModelMerger
{
    class Program
    {
        #region Model Loaders
        private static Model LoadSEModel(string filePath)
        {
            var model = new Model(Path.GetFileNameWithoutExtension(filePath));
            var input = SEModel.Read(filePath);

            Printer.WriteLine("LOADER", string.Format("Loading {0}", model.Name));

            foreach (var shape in input.Shapes)
            {
                model.Shapes.Add(shape);
            }

            foreach (var bone in input.Bones)
            { 
                model.Bones.Add(new Model.Bone(
                    bone.BoneName,
                    bone.BoneParent,
                    new Vector3(
                        (float)bone.LocalPosition.X,
                        (float)bone.LocalPosition.Y,
                        (float)bone.LocalPosition.Z),
                    new Quaternion(
                        (float)bone.LocalRotation.X,
                        (float)bone.LocalRotation.Y,
                        (float)bone.LocalRotation.Z,
                        (float)bone.LocalRotation.W),
                    new Vector3(
                        (float)bone.GlobalPosition.X,
                        (float)bone.GlobalPosition.Y,
                        (float)bone.GlobalPosition.Z),
                    new Quaternion(
                        (float)bone.GlobalRotation.X,
                        (float)bone.GlobalRotation.Y,
                        (float)bone.GlobalRotation.Z,
                        (float)bone.GlobalRotation.W)));
            }

            foreach (var semesh in input.Meshes)
            {
                var mesh = new Model.Mesh((int)semesh.VertexCount, (int)semesh.FaceCount);

                foreach (var mtl in semesh.MaterialReferenceIndicies)
                    mesh.MaterialIndices.Add(mtl);

                foreach (var severtex in semesh.Verticies)
                {
                    var vertex = new Model.Vertex(
                        new Vector3((float)severtex.Position.X, (float)severtex.Position.Y, (float)severtex.Position.Z),
                        new Vector3((float)severtex.VertexNormal.X, (float)severtex.VertexNormal.Y, (float)severtex.VertexNormal.Z));

                    foreach (var uv in severtex.UVSets)
                        vertex.UVs.Add(new Vector2((float)uv.X, (float)uv.Y));

                    foreach (var weight in severtex.Weights)
                        vertex.Weights.Add(new Model.Vertex.Weight((int)weight.BoneIndex, weight.BoneWeight));

                    foreach (var shape in severtex.Shapes)
                        vertex.Shapes.Add(new Model.Vertex.Shape(
                            (int)shape.ShapeIndex,
                            new Vector3(
                                (float)shape.Delta.X,
                                (float)shape.Delta.Y,
                                (float)shape.Delta.Z)));

                    vertex.Color = new Vector4(
                        severtex.VertexColor.R / 255.0f,
                        severtex.VertexColor.G / 255.0f,
                        severtex.VertexColor.B / 255.0f,
                        severtex.VertexColor.A / 255.0f);

                    mesh.Vertices.Add(vertex);
                }

                foreach (var face in semesh.Faces)
                {
                    mesh.Faces.Add(new Model.Face((int)face.FaceIndex1, (int)face.FaceIndex2, (int)face.FaceIndex3));
                }

                model.Meshes.Add(mesh);
            }

            foreach (var material in input.Materials)
            {
                model.Materials.Add(new Model.Material(material.Name));
            }

            Printer.WriteLine("LOADER", string.Format("Loaded {0}", model.Name));

            return model;
        }

        static Model LoadModel(string filePath)
        {
            switch(Path.GetExtension(filePath).ToLower())
            {
                case ".semodel":
                    return LoadSEModel(filePath);
                default:
                    return null;
            }
        }

        static List<Model> LoadModels(string[] args)
        {
            var fileNames = args.ToList();
            var models = new List<Model>(args.Length);

            fileNames.Sort();

            foreach(var fileName in fileNames)
            {
                switch(Path.GetExtension(fileName).ToLower())
                {
                    case ".semodel":
                        models.Add(LoadSEModel(fileName));
                        break;
                    default:
                        Printer.WriteLine("ERROR", string.Format("Invalid file: ", Path.GetFileNameWithoutExtension(fileName)), ConsoleColor.Red);
                        break;
                }
            }

            return models;
        }
        #endregion

        static bool CanBeConnected(Model input, List<Model> models)
        {
            var root = input.Bones[0].Name;

            foreach(var model in models)
            {
                if(model != input)
                {
                    if (model.HasBone(root))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        static Model GetRootModel(List<Model> models)
        {
            foreach(var a in models)
            {
                if(a.Bones.Count > 0)
                {
                    if(!CanBeConnected(a, models))
                    {
                        return a;
                    }
                }
            }

            return models[0];
        }

        static void Main(string[] args)
        {
            Printer.WriteLine("INIT", "---------------------------");
            Printer.WriteLine("INIT", "ModelMerger by Scobalula");
            Printer.WriteLine("INIT", "Merges SEModels into 1");
            Printer.WriteLine("INIT", string.Format("Version {0}", Assembly.GetExecutingAssembly().GetName().Version));
            Printer.WriteLine("INIT", "---------------------------");

            try
            {
                var models = LoadModels(args);

                if (models.Count == 0)
                {
                    Printer.WriteLine("USAGE", "Simply drag and drop supported model files onto the exe");
                }
                else
                {
                    var rootModel = GetRootModel(models);

                    if (rootModel == null)
                        throw new Exception("Failed to obtain root model");

                    Printer.WriteLine("MERGER", string.Format("Using {0} as root model", rootModel.Name));

                    var merged = new List<Model>(models.Count)
                    {
                        rootModel
                    };

                    var outputFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Merged Models");

                    // Keep looping until we've resolved all models
                    // We do this because some models connect to other
                    // so we need to wait until we've processed that model
                    while (merged.Count < models.Count)
                    {
                        foreach (var model in models)
                        {
                            // Check if we've processed, also considers root as it's added to merged
                            if (merged.Contains(model))
                                continue;
                            // If we have a model that doesn't exist, and can be connected, we must wait 
                            // for it's parent model to be connected
                            if (!rootModel.HasBone(model.Bones[0].Name) && CanBeConnected(model, models))
                                continue;
                            // Add to the group
                            merged.Add(model);

                            Printer.WriteLine("MERGER", string.Format("Merging {0}", model.Name));

                            foreach (var bone in model.Bones)
                            {
                                if (!rootModel.HasBone(bone.Name))
                                {
                                    var nBone = new Model.Bone(bone.Name, bone.ParentIndex, bone.LocalPosition, bone.LocalRotation);

                                    if (bone.ParentIndex > -1)
                                    {
                                        nBone.ParentIndex = rootModel.Bones.FindIndex(x => x.Name == model.Bones[nBone.ParentIndex].Name);
                                    }

                                    rootModel.Bones.Add(nBone);
                                }
                            }

                            foreach (var shape in model.Shapes)
                            {
                                if(!rootModel.Shapes.Contains(shape))
                                {
                                    rootModel.Shapes.Add(shape);
                                }
                            }

                            // Compute global positions (we need them for offsetting)
                            rootModel.GenerateGlobalBoneData();
                            model.GenerateGlobalBoneData();

                            // Get root and the new root, to compute offsets
                            var root = model.Bones[0];
                            var nRoot = rootModel.Bones.Find(x => x.Name == root.Name);

                            // TODO: compute this for each bone and utilize weights
                            // but as an option, as it may cause severe deformations 
                            // if bones have moved
                            var translation = nRoot.GlobalPosition - root.GlobalPosition;
                            var rotation = (nRoot.GlobalRotation * root.GlobalRotation.Inverse()).ToMatrix();

                            foreach (var material in model.Materials)
                            {
                                if (rootModel.Materials.Find(x => x.Name == material.Name) == null)
                                {
                                    rootModel.Materials.Add(material);
                                }
                            }

                            foreach (var mesh in model.Meshes)
                            {
                                var nMesh = new Model.Mesh(mesh.Vertices.Count, 0)
                                {
                                    Faces = new List<Model.Face>(mesh.Faces)
                                };

                                foreach (var material in mesh.MaterialIndices)
                                {
                                    nMesh.MaterialIndices.Add(rootModel.Materials.FindIndex(x => x.Name == model.Materials[material].Name));
                                }

                                foreach (var vertex in mesh.Vertices)
                                {
                                    var nVertex = new Model.Vertex(vertex.Position, vertex.Normal, vertex.Tangent)
                                    {
                                        Color = vertex.Color,
                                        Weights = new List<Model.Vertex.Weight>(vertex.Weights.Count),
                                        UVs = new List<Vector2>(vertex.UVs)
                                    };

                                    foreach (var weight in vertex.Weights)
                                    {
                                        nVertex.Weights.Add(new Model.Vertex.Weight(rootModel.Bones.FindIndex(x => x.Name == model.Bones[weight.BoneIndex].Name), weight.Influence));
                                    }

                                    foreach (var shape in vertex.Shapes)
                                    {
                                        nVertex.Shapes.Add(new Model.Vertex.Shape(
                                            rootModel.Shapes.FindIndex(x => x == model.Shapes[shape.ShapeIndex]),
                                            rotation.TransformVector(shape.Delta)));
                                    }

                                    // Now move it to the new position
                                    nVertex.Position = rotation.TransformVector(vertex.Position);
                                    nVertex.Normal = rotation.TransformVector(vertex.Normal);
                                    nVertex.Position += translation;

                                    nMesh.Vertices.Add(nVertex);
                                }

                                rootModel.Meshes.Add(nMesh);
                            }

                            Printer.WriteLine("MERGER", string.Format("Merged {0}", model.Name));
                        }
                    }

                    Printer.WriteLine("MERGER", string.Format("Saving {0}", rootModel.Name));
                    Directory.CreateDirectory(outputFolder);
                    rootModel.Save(Path.Combine(outputFolder, rootModel.Name + ".semodel"));
                    Printer.WriteLine("MERGER", string.Format("Saved {0}", rootModel.Name));
                }
            }
            catch(Exception e)
            {
                Printer.WriteLine("ERROR", "An unhandled exception has occured:", ConsoleColor.DarkRed);
                Console.WriteLine(e);
            }


            Printer.WriteLine("DONE", "Execution complete, press Enter to exit");
            Console.ReadLine();
        }
    }
}
