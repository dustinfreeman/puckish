/**********************************************************************************
* Blueprint Reality Inc. CONFIDENTIAL
* 2023 Blueprint Reality Inc.
* All Rights Reserved.
*
* NOTICE:  All information contained herein is, and remains, the property of
* Blueprint Reality Inc. and its suppliers, if any.  The intellectual and
* technical concepts contained herein are proprietary to Blueprint Reality Inc.
* and its suppliers and may be covered by Patents, pending patents, and are
* protected by trade secret or copyright law.
*
* Dissemination of this information or reproduction of this material is strictly
* forbidden unless prior written permission is obtained from Blueprint Reality Inc.
***********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BlueprintReality.MixCast
{
    public enum ExpLightType
    {
        None,
        Point,
        Directional,
        Spot
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ExpLight
    {
        public ExpLightType type;

        public Vector3 color;

        public Vector3 pos;
        public Vector3 forward;

        public float range;     //for Point and Spot
        public float angle;     //for Spot
    }

    //avoiding array to avoid memory allocation (trust me)!
    [StructLayout(LayoutKind.Sequential)]
    public struct ExpLights
    {
        public ExpLight light0;
        public ExpLight light1;
        public ExpLight light2;
        public ExpLight light3;
        public ExpLight light4;
        public ExpLight light5;
        public ExpLight light6;
        public ExpLight light7;
        public ExpLight light8;
        public ExpLight light9;
        public ExpLight light10;
        public ExpLight light11;
        public ExpLight light12;
        public ExpLight light13;
        public ExpLight light14;
        public ExpLight light15;

        public ExpLight this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return light0;
                    case 1: return light1;
                    case 2: return light2;
                    case 3: return light3;
                    case 4: return light4;
                    case 5: return light5;
                    case 6: return light6;
                    case 7: return light7;
                    case 8: return light8;
                    case 9: return light9;
                    case 10: return light10;
                    case 11: return light11;
                    case 12: return light12;
                    case 13: return light13;
                    case 14: return light14;
                    case 15: return light15;
                }
                throw new ArgumentOutOfRangeException();
            }
            set
            {
                switch (index)
                {
                    case 0: light0 = value; return;
                    case 1: light1 = value; return;
                    case 2: light2 = value; return;
                    case 3: light3 = value; return;
                    case 4: light4 = value; return;
                    case 5: light5 = value; return;
                    case 6: light6 = value; return;
                    case 7: light7 = value; return;
                    case 8: light8 = value; return;
                    case 9: light9 = value; return;
                    case 10: light10 = value; return;
                    case 11: light11 = value; return;
                    case 12: light12 = value; return;
                    case 13: light13 = value; return;
                    case 14: light14 = value; return;
                    case 15: light15 = value; return;
                }
                throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum ExpCamFlagBit
    {
        FirstPerson = 1 << 0,
        Translucent = 1 << 1,
        SeparateOpaque = 1 << 2,
    }

    [StructLayout(LayoutKind.Sequential)]
    public class ExpFrame
    {
        public const int MaxLights = 16;

        //Generated by MixCast Client
        public ulong frameIndex;
        public ulong syncTime;

        public float camFramerate;
        public uint camWidth;
        public uint camHeight;

        public Vector3 camPos;
        public Quaternion camRot;
        public float camFoV;

        [MarshalAs(UnmanagedType.U1)]
        public bool renderForeground;
        [MarshalAs(UnmanagedType.U1)]
        public bool renderFull;

        public uint camFlags;
        public bool HasCamFlag(ExpCamFlagBit flag)
        {
            return (camFlags & (uint)flag) > 0;
        }

        public float occlusionApproxDepth;
        public IntPtr occlusionTex;

        //Generated by MixCast SDK
        public IntPtr layersTex;
        public ExpLights lights;

        public IntPtr opaqueLayerTex;

        public void CopyFrom(ExpFrame other)
        {
            frameIndex = other.frameIndex;
            syncTime = other.syncTime;

            camFramerate = other.camFramerate;
            camWidth = other.camWidth;
            camHeight = other.camHeight;

            camPos = other.camPos;
            camRot = other.camRot;
            camFoV = other.camFoV;

            renderForeground = other.renderForeground;
            renderFull = other.renderFull;

            camFlags = other.camFlags;

            occlusionApproxDepth = other.occlusionApproxDepth;
            occlusionTex = other.occlusionTex;

            layersTex = other.layersTex;
            opaqueLayerTex = other.opaqueLayerTex;
            lights = other.lights;
        }
    }
}
