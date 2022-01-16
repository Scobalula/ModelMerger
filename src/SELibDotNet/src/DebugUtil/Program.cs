using SELib;
using SELib.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

///
///   Program.cs (Debugger Utility)
///   Author: DTZxPorter
///   Written for the SE Format Project
///   Follows SEAnim specification v1.1
///   https://github.com/SE2Dev/SEAnim-Docs/blob/master/spec.md
///

namespace DebugUtil
{
    class Program
    {
        private static void Usage()
        {
            // Output
            Console.WriteLine("   DebugUtil.exe <SEAnim file>.seanim");
            Console.WriteLine("   DebugUtil.exe <SEAnim file>.seanim <log>.txt");
        }

        static void Main(string[] args)
        {
            // Debugger start
            Console.Title = "SE Format Debugger Utility";
            Console.WriteLine("-- SE Format Debugger Utility --\n");
            // Check for input file
            if (args.Length <= 0)
            {
                // Usage
                Usage();
            }
            else
            {
                // Make sure the file is a seanim file
                if (args[0].EndsWith(".seanim"))
                {
                    // Load and output debug info
                    if (args.Length > 1)
                    {
                        DebugLOGSEAnim(args[0], args[1]);
                    }
                    else
                    {
                        DebugSEAnim(args[0]);
                    }
                }
                else
                {
                    // Usage
                    Usage();
                }
            }
            // Pause
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Print the log to the file
        /// </summary>
        /// <param name="File">The file to debug</param>
        /// <param name="FileOutput">The output log file</param>
        private static void DebugLOGSEAnim(string File, string FileOutput)
        {
            // Load the anim
            var anim = SEAnim.Read(File);
            // Load bones
            var bones = anim.Bones;
            // Log
            Console.WriteLine("   Writing log to file...");
            // Prepare output
            using (StreamWriter writeFile = new StreamWriter(FileOutput))
            {
                // Log
                writeFile.WriteLine("-- SEAnim Debugger Utility --");
                writeFile.WriteLine("");
                // Output initial info
                writeFile.WriteLine("   Header:");
                // Header data
                {
                    writeFile.WriteLine("   - Frame count: " + anim.FrameCount);
                    writeFile.WriteLine("   - Frame rate: " + anim.FrameRate);
                    writeFile.WriteLine("   - Animation type: " + anim.AnimType.ToString());
                    writeFile.WriteLine("   - Bone count: " + anim.BoneCount);
                    writeFile.WriteLine("   - Notetrack count: " + anim.NotificationCount);
                    writeFile.WriteLine("   - Modifiers count: " + anim.AnimationBoneModifiers.Count);
                    // Check for delta
                    if (anim.AnimType == AnimationType.Delta)
                    {
                        // Delta bone must be the first one
                        writeFile.WriteLine("   - Delta tag \"" + anim.DeltaTagName + "\"");
                    }
                }
                // Separate and prepare keys by bone
                writeFile.WriteLine("\n   Frames:");
                // Output keys
                foreach (string Bone in anim.Bones)
                {
                    // Bone header
                    writeFile.WriteLine("   - Bone \"" + Bone + "\" data:");
                    // Translation header
                    writeFile.Write("    - Translations [");
                    // Check for translation keys
                    if (anim.AnimationPositionKeys.ContainsKey(Bone))
                    {
                        // Output translation keys
                        var translationKeys = anim.AnimationPositionKeys[Bone];
                        // Count
                        writeFile.WriteLine(translationKeys.Count + "]");
                        // Output values
                        foreach (SEAnimFrame frame in translationKeys)
                        {
                            // Output the vector
                            writeFile.WriteLine("    [" + frame.Frame + "] {0} {1} {2}", ((Vector3)frame.Data).X, ((Vector3)frame.Data).Y, ((Vector3)frame.Data).Z);
                        }
                    }
                    else
                    {
                        // Had no keys
                        writeFile.WriteLine("0]");
                    }
                    // Rotation header
                    writeFile.Write("    - Rotations [");
                    // Check for rotation keys
                    if (anim.AnimationRotationKeys.ContainsKey(Bone))
                    {
                        // Output rotation keys
                        var rotKeys = anim.AnimationRotationKeys[Bone];
                        // Count
                        writeFile.WriteLine(rotKeys.Count + "]");
                        // Output values
                        foreach (SEAnimFrame frame in rotKeys)
                        {
                            // Output the quat
                            writeFile.WriteLine("    [" + frame.Frame + "] {0} {1} {2} {3}", ((Quaternion)frame.Data).X, ((Quaternion)frame.Data).Y, ((Quaternion)frame.Data).Z, ((Quaternion)frame.Data).W);
                        }
                    }
                    else
                    {
                        // Had no keys
                        writeFile.WriteLine("0]");
                    }
                    // Scales header
                    writeFile.Write("    - Scales [");
                    // Check for scale keys
                    if (anim.AnimationScaleKeys.ContainsKey(Bone))
                    {
                        // Output scale keys
                        var scaleKeys = anim.AnimationScaleKeys[Bone];
                        // Count
                        writeFile.WriteLine(scaleKeys.Count + "]");
                        // Output values
                        foreach (SEAnimFrame frame in scaleKeys)
                        {
                            // Output the vector
                            writeFile.WriteLine("    [" + frame.Frame + "] {0} {1} {2}", ((Vector3)frame.Data).X, ((Vector3)frame.Data).Y, ((Vector3)frame.Data).Z);
                        }
                    }
                    else
                    {
                        // Had no keys
                        writeFile.WriteLine("0]");
                    }
                }
                // Separate and prepare notes
                writeFile.WriteLine("\n   Notifications:");
                // Output notetracks
                foreach (KeyValuePair<string, List<SEAnimFrame>> Notetrack in anim.AnimationNotetracks)
                {
                    // Note header
                    writeFile.WriteLine("   - Note \"" + Notetrack.Key + "\" data:");
                    // Loop and output keys
                    foreach (SEAnimFrame frame in Notetrack.Value)
                    {
                        // Log it
                        writeFile.WriteLine("   [" + frame.Frame + "] plays");
                    }
                }
            }
        }

        /// <summary>
        /// Print the log to the console
        /// </summary>
        /// <param name="File">The seanim to debug</param>
        private static void DebugSEAnim(string File)
        {
            // Load the anim
            var anim = SEAnim.Read(File);
            // Load bones
            var bones = anim.Bones;
            // Output initial info
            Console.WriteLine("   Header:");
            // Header data
            {
                Console.WriteLine("   - Frame count: " + anim.FrameCount);
                Console.WriteLine("   - Frame rate: " + anim.FrameRate);
                Console.WriteLine("   - Animation type: " + anim.AnimType.ToString());
                Console.WriteLine("   - Bone count: " + anim.BoneCount);
                Console.WriteLine("   - Notetrack count: " + anim.NotificationCount);
                Console.WriteLine("   - Modifiers count: " + anim.AnimationBoneModifiers.Count);
                // Check for delta
                if (anim.AnimType == AnimationType.Delta)
                {
                    // Delta bone must be the first one
                    Console.WriteLine("   - Delta tag \"" + anim.DeltaTagName + "\"");
                }
            }
            // Separate and prepare keys by bone
            Console.WriteLine("\n   Frames:");
            // Output keys
            foreach (string Bone in anim.Bones)
            {
                // Bone header
                Console.WriteLine("   - Bone \"" + Bone + "\" data:");
                // Translation header
                Console.Write("    - Translations [");
                // Check for translation keys
                if (anim.AnimationPositionKeys.ContainsKey(Bone))
                {
                    // Output translation keys
                    var translationKeys = anim.AnimationPositionKeys[Bone];
                    // Count
                    Console.WriteLine(translationKeys.Count + "]");
                    // Output values
                    foreach (SEAnimFrame frame in translationKeys)
                    {
                        // Output the vector
                        Console.WriteLine("    [" + frame.Frame + "] {0} {1} {2}", ((Vector3)frame.Data).X, ((Vector3)frame.Data).Y, ((Vector3)frame.Data).Z);
                    }
                }
                else
                {
                    // Had no keys
                    Console.WriteLine("0]");
                }
                // Rotation header
                Console.Write("    - Rotations [");
                // Check for rotation keys
                if (anim.AnimationRotationKeys.ContainsKey(Bone))
                {
                    // Output rotation keys
                    var rotKeys = anim.AnimationRotationKeys[Bone];
                    // Count
                    Console.WriteLine(rotKeys.Count + "]");
                    // Output values
                    foreach (SEAnimFrame frame in rotKeys)
                    {
                        // Output the quat
                        Console.WriteLine("    [" + frame.Frame + "] {0} {1} {2} {3}", ((Quaternion)frame.Data).X, ((Quaternion)frame.Data).Y, ((Quaternion)frame.Data).Z, ((Quaternion)frame.Data).W);
                    }
                }
                else
                {
                    // Had no keys
                    Console.WriteLine("0]");
                }
                // Scales header
                Console.Write("    - Scales [");
                // Check for scale keys
                if (anim.AnimationScaleKeys.ContainsKey(Bone))
                {
                    // Output scale keys
                    var scaleKeys = anim.AnimationScaleKeys[Bone];
                    // Count
                    Console.WriteLine(scaleKeys.Count + "]");
                    // Output values
                    foreach (SEAnimFrame frame in scaleKeys)
                    {
                        // Output the vector
                        Console.WriteLine("    [" + frame.Frame + "] {0} {1} {2}", ((Vector3)frame.Data).X, ((Vector3)frame.Data).Y, ((Vector3)frame.Data).Z);
                    }
                }
                else
                {
                    // Had no keys
                    Console.WriteLine("0]");
                }
            }
            // Separate and prepare notes
            Console.WriteLine("\n   Notifications:");
            // Output notetracks
            foreach (KeyValuePair<string, List<SEAnimFrame>> Notetrack in anim.AnimationNotetracks)
            {
                // Note header
                Console.WriteLine("   - Note \"" + Notetrack.Key + "\" data:");
                // Loop and output keys
                foreach (SEAnimFrame frame in Notetrack.Value)
                {
                    // Log it
                    Console.WriteLine("   [" + frame.Frame + "] plays");
                }
            }
        }
    }
}
