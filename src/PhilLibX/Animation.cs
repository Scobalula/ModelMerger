using PhilLibX.IO;
using PhilLibX.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhilLibX
{
    /// <summary>
    /// A class to hold a 3-D Animation
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// Animation/Bone Data Types
        /// </summary>
        public enum DataType
        {
            /// <summary>
            /// Animation Data is same as other bones (for bones that will match the other bones)
            /// </summary>
            None,

            /// <summary>
            /// Animation Data is relative to zero
            /// </summary>
            Absolute,

            /// <summary>
            /// Animation Data is relative to parent bind pose
            /// </summary>
            Relative,

            /// <summary>
            /// Animation Data is applied to existing animation data in the scene
            /// </summary>
            Additive,
        }

        /// <summary>
        /// A class to hold an Animation Bone
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
            /// Gets or Sets the data type for this bone
            /// </summary>
            public DataType Type { get; set; }

            /// <summary>
            /// Gets or Sets the translation keys
            /// </summary>
            public SortedDictionary<int, Vector3> Translations { get; set; }

            /// <summary>
            /// Gets or Sets the rotation keys
            /// </summary>
            public SortedDictionary<int, Quaternion> Rotations { get; set; }

            /// <summary>
            /// Gets or Sets the rotation keys
            /// </summary>
            public SortedDictionary<int, Vector3> Scales { get; set; }

            /// <summary>
            /// Creates a new animation bone
            /// </summary>
            /// <param name="name">Name of the bone</param>
            public Bone(string name)
            {
                Name        = name;
                ParentIndex = -2; // < -1 to indicate no parent data to halt formats that need it
                Type        = DataType.None;

                Translations = new SortedDictionary<int, Vector3>();
                Rotations    = new SortedDictionary<int, Quaternion>();
                Scales       = new SortedDictionary<int, Vector3>();
            }

            /// <summary>
            /// Creates a new animation bone
            /// </summary>
            /// <param name="name">Name of the bone</param>
            /// <param name="parentIndex">Parent index</param>
            public Bone(string name, int parentIndex)
            {
                Name        = name;
                ParentIndex = parentIndex;
                Type        = DataType.None;

                Translations = new SortedDictionary<int, Vector3>();
                Rotations    = new SortedDictionary<int, Quaternion>();
                Scales       = new SortedDictionary<int, Vector3>();
            }

            /// <summary>
            /// Creates a new animation bone
            /// </summary>
            /// <param name="name">Name of the bone</param>
            /// <param name="type">Bone data type</param>
            public Bone(string name, DataType type)
            {
                Name        = name;
                ParentIndex = -2; // < -1 to indicate no parent data to halt formats that need it
                Type        = type;

                Translations = new SortedDictionary<int, Vector3>();
                Rotations    = new SortedDictionary<int, Quaternion>();
                Scales       = new SortedDictionary<int, Vector3>();
            }

            /// <summary>
            /// Creates a new animation bone
            /// </summary>
            /// <param name="name">Name of the bone</param>
            /// <param name="parentIndex">Parent index</param>
            /// <param name="type">Bone data type</param>
            public Bone(string name, int parentIndex, DataType type)
            {
                Name        = name;
                ParentIndex = parentIndex;
                Type        = type;

                Translations = new SortedDictionary<int, Vector3>();
                Rotations    = new SortedDictionary<int, Quaternion>();
                Scales       = new SortedDictionary<int, Vector3>();
            }

            /// <summary>
            /// Gets the Translation at the frame
            /// </summary>
            /// <param name="frame"></param>
            /// <returns></returns>
            public Vector3 GetTranslation(int frame)
            {
                // Check if we have translations
                if(Translations.Count > 0)
                {
                    if (Translations.TryGetValue(frame, out var v))
                        return new Vector3(v.X, v.Y, v.Z);

                    // Search for the best match
                    var start = Translations.LastOrDefault(Translations.First(), x => x.Key < frame);
                    var end = Translations.FirstOrDefault(Translations.Last(), x => x.Key > frame);

                    // Return interpolated result
                    return start.Value.Lerp(end.Value, MathUtilities.Clamp((float)(frame - start.Key) / (end.Key - start.Key), 1.0f, 0.0f));
                }

                return new Vector3();
            }

            /// <summary>
            /// Gets the Rotation at the frame
            /// </summary>
            /// <param name="frame"></param>
            /// <returns></returns>
            public Quaternion GetRotation(int frame)
            {
                // Check if we have translations
                if (Rotations.Count > 0)
                {
                    if (Rotations.TryGetValue(frame, out var v))
                        return new Quaternion(v.X, v.Y, v.Z, v.W);

                    // Search for the best match
                    var start = Rotations.LastOrDefault(Rotations.First(), x => x.Key < frame);
                    var end = Rotations.FirstOrDefault(Rotations.Last(), x => x.Key > frame);

                    // Return interpolated result
                    return start.Value.Lerp(end.Value, MathUtilities.Clamp((float)(frame - start.Key) / (end.Key - start.Key), 1.0f, 0.0f));
                }

                return new Quaternion();
            }

            /// <summary>
            /// Gets the name of the bone as a string representation of it
            /// </summary>
            /// <returns>Name</returns>
            public override string ToString() => Name;
        }

        /// <summary>
        /// A class to hold an Animation Note
        /// </summary>
        public class Note
        {
            /// <summary>
            /// Gets or Sets the note name
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or Sets the frames the note occurs on
            /// </summary>
            public List<int> Frames { get; set; }

            /// <summary>
            /// Creates a new animation notetrack
            /// </summary>
            /// <param name="name">note name</param>
            public Note(string name)
            {
                Name = name;
                Frames = new List<int>();
            }
        }

        /// <summary>
        /// Gets or Sets the animation bones
        /// </summary>
        public Dictionary<string, Bone> Bones { get; set; }

        /// <summary>
        /// Gets or Sets the animation notes
        /// </summary>
        public Dictionary<string, Note> Notes { get; set; }

        /// <summary>
        /// Gets or Sets the data type for this animation
        /// </summary>
        public DataType Type { get; set; }

        /// <summary>
        /// Gets or Sets the framerate
        /// </summary>
        public float Framerate { get; set; }

        /// <summary>
        /// Gets whether the animation contains translation keys in any of the bones
        /// </summary>
        public bool ContainsTranslationKeys
        {
            get
            {
                foreach (var bone in Bones)
                    if (bone.Value.Translations.Count > 0)
                        return true;

                return false;
            }
        }

        /// <summary>
        /// Gets whether the animation contains rotation keys in any of the bones
        /// </summary>
        public bool ContainsRotationKeys
        {
            get
            {
                foreach (var bone in Bones)
                    if (bone.Value.Rotations.Count > 0)
                        return true;

                return false;
            }
        }

        /// <summary>
        /// Gets whether the animation contains scale keys in any of the bones
        /// </summary>
        public bool ContainsScaleKeys
        {
            get
            {
                foreach (var bone in Bones)
                    if (bone.Value.Scales.Count > 0)
                        return true;

                return false;
            }
        }

        /// <summary>
        /// Gets the number of frames (highest frame across bones/notes)
        /// </summary>
        public int FrameCount
        {
            get
            {
                int result = 0;

                foreach(var bone in Bones)
                {
                    if (bone.Value.Translations.Count > 0)
                    {
                        var lastFrame = bone.Value.Translations.LastOrDefault().Key;

                        if (lastFrame > result)
                            result = lastFrame;
                    }

                    if (bone.Value.Rotations.Count > 0)
                    {
                        var lastFrame = bone.Value.Rotations.LastOrDefault().Key;

                        if (lastFrame > result)
                            result = lastFrame;
                    }

                    if (bone.Value.Scales.Count > 0)
                    {
                        var lastFrame = bone.Value.Scales.LastOrDefault().Key;

                        if (lastFrame > result)
                            result = lastFrame;
                    }
                }

                foreach (var note in Notes)
                    foreach (var frame in note.Value.Frames)
                        if (frame > result)
                        result = frame;

                return result + 1;
            }
        }

        /// <summary>
        /// Gets whether the animation contains notes
        /// </summary>
        public bool ContainsNotes
        {
            get => Notes.Count > 0;
        }

        /// <summary>
        /// Creates a new animation
        /// </summary>
        public Animation()
        {
            Type = DataType.Absolute;
            Framerate = 30.0f;
            Bones = new Dictionary<string, Bone>();
            Notes = new Dictionary<string, Note>();
        }

        /// <summary>
        /// Creates a new animation
        /// </summary>
        /// <param name="type">Animation data type</param>
        public Animation(DataType type)
        {
            Type = type;
            Framerate = 30.0f;
            Bones = new Dictionary<string, Bone>();
            Notes = new Dictionary<string, Note>();
        }

        /// <summary>
        /// Clears all loaded data
        /// </summary>
        public void Clear()
        {
            Type = DataType.Absolute;
            Framerate = 30.0f;
            Bones.Clear();
            Notes.Clear();
        }

        /// <summary>
        /// Adds a translation frame for the given bone at the given frame
        /// </summary>
        /// <param name="bone">Bone to add the keyframe too, if the bone doesn't exist, it will be created and added to the animation</param>
        /// <param name="frame">Frame to set the data on</param>
        /// <param name="data">Translation data to add at the frame</param>
        public void SetDataType(string bone, int frame, DataType data)
        {
            if (Bones.TryGetValue(bone, out var val))
            {
                val.Type = data;
            }
            else
            {
                var nBone = new Bone(bone, -1, data);
                Bones[bone] = nBone;
            }
        }

        /// <summary>
        /// Adds a translation frame for the given bone at the given frame
        /// </summary>
        /// <param name="bone">Bone to add the keyframe too, if the bone doesn't exist, it will be created and added to the animation</param>
        /// <param name="frame">Frame to set the data on</param>
        /// <param name="data">Translation data to add at the frame</param>
        public void AddTranslation(string bone, int frame, Vector3 data)
        {
            if(Bones.TryGetValue(bone, out var val))
            {
                val.Translations[frame] = data * 2.54f;
            }
            else
            {
                var nBone = new Bone(bone, -1, Type);
                nBone.Translations[frame] = data * 2.54f;
                Bones[bone] = nBone;
            }
        }

        /// <summary>
        /// Adds a translation frame for the given bone at the given frame
        /// </summary>
        /// <param name="bone">Bone to add the keyframe too, if the bone doesn't exist, it will be created and added to the animation</param>
        /// <param name="frame">Frame to set the data on</param>
        /// <param name="data">Translation data to add at the frame</param>
        public void AddScale(string bone, int frame, Vector3 data)
        {
            if (Bones.TryGetValue(bone, out var val))
            {
                val.Scales[frame] = data;
            }
            else
            {
                var nBone = new Bone(bone, -1, Type);
                nBone.Scales[frame] = data;
                Bones[bone] = nBone;
            }
        }

        /// <summary>
        /// Adds a rotation frame for the given bone at the given frame
        /// </summary>
        /// <param name="bone">Bone to add the keyframe too, if the bone doesn't exist, it will be created and added to the animation</param>
        /// <param name="frame">Frame to set the data on</param>
        /// <param name="data">Rotation data to add at the frame</param>
        public void AddRotation(string bone, int frame, Quaternion data)
        {
            if (Bones.TryGetValue(bone, out var val))
            {
                val.Rotations[frame] = data;
            }
            else
            {
                var nBone = new Bone(bone, -1, Type);
                nBone.Rotations[frame] = data;
                Bones[bone] = nBone;
            }
        }

        /// <summary>
        /// Scales the animation by the given value
        /// </summary>
        /// <param name="value">Value to scale the animation by</param>
        public void Scale(float value)
        {
            if (value == 1.0f)
                return;

            foreach (var bone in Bones)
            {
                var translationKeys = bone.Value.Translations.Keys.ToArray();

                foreach (var key in translationKeys)
                {
                    bone.Value.Translations[key] *= value;
                }
            }
        }

        /// <summary>
        /// Saves the animation to a SEAnim
        /// </summary>
        /// <param name="path">Output Path</param>
        public void ToSEAnim(string path)
        {
            // Determine bones with different types
            var boneModifiers = new Dictionary<int, byte>();

            var bones = Bones.Values.ToArray();

            for (int i = 0; i < Bones.Count; i++)
            {
                if (bones[i].Type != DataType.None && bones[i].Type != Type)
                {
                    // Convert to SEAnim Type
                    switch (bones[i].Type)
                    {
                        case DataType.Absolute: boneModifiers[i] = 0; break;
                        case DataType.Additive: boneModifiers[i] = 1; break;
                        case DataType.Relative: boneModifiers[i] = 2; break;
                    }
                }
            }

            // Build notes
            var notetracks = new Dictionary<int, string>();

            foreach (var note in Notes)
                foreach (var frame in note.Value.Frames)
                    notetracks[frame] = note.Value.Name;

            var frameCount = FrameCount;

            using (var writer = new BinaryWriter(File.Create(path)))
            {
                writer.Write(new char[] { 'S', 'E', 'A', 'n', 'i', 'm' });
                writer.Write((short)0x1);
                writer.Write((short)0x1C);

                // Convert to SEAnim Type
                switch (Type)
                {
                    case DataType.Absolute: writer.Write((byte)0); break;
                    case DataType.Additive: writer.Write((byte)1); break;
                    case DataType.Relative: writer.Write((byte)2); break;
                }

                writer.Write((byte)0);

                byte flags = 0;

                if (ContainsTranslationKeys)
                    flags |= 1;
                if (ContainsRotationKeys)
                    flags |= 2;
                if (ContainsScaleKeys)
                    flags |= 4;
                if (ContainsNotes)
                    flags |= 64;

                writer.Write(flags);
                writer.Write((byte)0);
                writer.Write((ushort)0);
                writer.Write(Framerate);
                writer.Write(frameCount);
                writer.Write(Bones.Count);
                writer.Write((byte)boneModifiers.Count);
                writer.Write((byte)0);
                writer.Write((ushort)0);
                writer.Write(notetracks.Count);

                foreach (var bone in Bones)
                    writer.WriteNullTerminatedString(bone.Value.Name);

                foreach(var modifier in boneModifiers)
                {
                    if (Bones.Count <= 0xFF)
                        writer.Write((byte)modifier.Key);
                    else if (Bones.Count <= 0xFFFF)
                        writer.Write((ushort)modifier.Key);
                    else
                        writer.Write(modifier.Key);

                    writer.Write(modifier.Value);
                }

                foreach(var bone in Bones)
                {
                    writer.Write((byte)0);

                    // Translations
                    if((flags & 1) != 0)
                    {
                        if (frameCount <= 0xFF)
                            writer.Write((byte)bone.Value.Translations.Count);
                        else if (frameCount <= 0xFFFF)
                            writer.Write((ushort)bone.Value.Translations.Count);
                        else
                            writer.Write(bone.Value.Translations.Count);

                        foreach (var data in bone.Value.Translations)
                        {
                            if (frameCount <= 0xFF)
                                writer.Write((byte)data.Key);
                            else if (frameCount <= 0xFFFF)
                                writer.Write((ushort)data.Key);
                            else
                                writer.Write(data.Key);

                            writer.WriteStruct(data.Value);
                        }

                    }

                    // Rotations
                    if ((flags & 2) != 0)
                    {
                        if (frameCount <= 0xFF)
                            writer.Write((byte)bone.Value.Rotations.Count);
                        else if (frameCount <= 0xFFFF)
                            writer.Write((ushort)bone.Value.Rotations.Count);
                        else
                            writer.Write(bone.Value.Rotations.Count);

                        foreach (var data in bone.Value.Rotations)
                        {
                            if (frameCount <= 0xFF)
                                writer.Write((byte)data.Key);
                            else if (frameCount <= 0xFFFF)
                                writer.Write((ushort)data.Key);
                            else
                                writer.Write(data.Key);

                            writer.WriteStruct(data.Value);
                        }
                    }

                    // Scales
                    if ((flags & 4) != 0)
                    {
                        if (frameCount <= 0xFF)
                            writer.Write((byte)bone.Value.Scales.Count);
                        else if (frameCount <= 0xFFFF)
                            writer.Write((ushort)bone.Value.Scales.Count);
                        else
                            writer.Write(bone.Value.Scales.Count);

                        foreach (var data in bone.Value.Scales)
                        {
                            if (frameCount <= 0xFF)
                                writer.Write((byte)data.Key);
                            else if (frameCount <= 0xFFFF)
                                writer.Write((ushort)data.Key);
                            else
                                writer.Write(data.Key);

                            writer.WriteStruct(data.Value);
                        }
                    }
                }

                foreach(var notetrack in notetracks)
                {
                    if (frameCount <= 0xFF)
                        writer.Write((byte)notetrack.Key);
                    else if (frameCount <= 0xFFFF)
                        writer.Write((ushort)notetrack.Key);
                    else
                        writer.Write(notetrack.Key);

                    writer.WriteNullTerminatedString(notetrack.Value);
                }
            }
        }
    }
}
