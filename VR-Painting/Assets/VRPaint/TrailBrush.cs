﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;

namespace GPUBasedTrails
{
    public class TrailBrush : MonoBehaviour
    {
        #region Type Define

        public static class CSPARAM
        {
            // kernels
            public const string CALC_INPUT = "InputCalc";

            // parameters
            public const string TIME = "_Time";
            public const string UPDATE_DISTANCE_MIN = "_UpdateDistanceMin";
            public const string TRAIL_NUM = "_TrailNum";
            public const string LIFE = "_Life";
            public const string NODE_NUM_PER_TRAIL = "_NodeNumPerTrail";
            public const string TRAIL_BUFFER = "_TrailBuffer";
            public const string NODE_BUFFER = "_NodeBuffer";
            public const string INPUT_BUFFER = "_InputBuffer";
        }
    
        public enum TrailType
        {
            Trail,
            Particle
        }

        public struct Trail
        {
            public int currentNodeIdx;
            public int type;
        }

        public struct Node
        {
            public float time;
            public Vector3 pos;
        }

        public struct Input
        {
            public Vector3 pos;
        }

        #endregion

        public ComputeShader cs;

        public int trailNum = 100000;
        public int nodeNum = 4000;
        public float updateDistaceMin = 0.01f;

        public ComputeBuffer trailBuffer;
        public ComputeBuffer nodeBuffer;
        public ComputeBuffer inputBuffer;

        #region Unity

        void Start()
        {
            Assert.IsNotNull(cs);

            var totalNodeNum = trailNum * nodeNum;

            trailBuffer = new ComputeBuffer(trailNum, Marshal.SizeOf(typeof(Trail)));
            nodeBuffer = new ComputeBuffer(totalNodeNum, Marshal.SizeOf(typeof(Node)));
            inputBuffer = new ComputeBuffer(trailNum, Marshal.SizeOf(typeof(Input)));

            var initTrail = new Trail() { currentNodeIdx = -1 };
            var initNode = new Node() { time = -1 };

            trailBuffer.SetData(Enumerable.Repeat(initTrail, trailNum).ToArray());
            nodeBuffer.SetData(Enumerable.Repeat(initNode, totalNodeNum).ToArray());
        }


        void LateUpdate()
        {
            cs.SetFloat(CSPARAM.TIME, Time.time);
            cs.SetFloat(CSPARAM.UPDATE_DISTANCE_MIN, updateDistaceMin);
            cs.SetInt(CSPARAM.TRAIL_NUM, trailNum);
            cs.SetInt(CSPARAM.NODE_NUM_PER_TRAIL, nodeNum);

            var kernel = cs.FindKernel(CSPARAM.CALC_INPUT);
            cs.SetBuffer(kernel, CSPARAM.TRAIL_BUFFER, trailBuffer);
            cs.SetBuffer(kernel, CSPARAM.NODE_BUFFER, nodeBuffer);
            cs.SetBuffer(kernel, CSPARAM.INPUT_BUFFER, inputBuffer);

            ComputeShaderUtil.Dispatch(cs, kernel, new Vector3(trailNum, 1f, 1f));
        }


        private void OnDestroy()
        {
            trailBuffer.Release();
            nodeBuffer.Release();
            inputBuffer.Release();
        }

        #endregion



        public void InputPoint(List<Input> inputs)
        {
            inputBuffer.SetData(inputs);
        }
    }
}