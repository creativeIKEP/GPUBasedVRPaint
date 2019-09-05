using System.Collections.Generic;
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
            public int trailId;
        }

        public struct Input
        {
            public Vector3 pos;
        }

        #endregion

        public int trailNum = 100000;
        public int nodeNum = 4000;
        public float updateDistaceMin = 0.01f;

        public ComputeBuffer trailBuffer;
        public ComputeBuffer nodeBuffer;

        Trail[] trails;
        Node[] nodes;
        int currentTrailId;

        #region Unity

        void Start()
        {
            currentTrailId = -1;

            var totalNodeNum = trailNum * nodeNum;

            trailBuffer = new ComputeBuffer(trailNum, Marshal.SizeOf(typeof(Trail)));
            nodeBuffer = new ComputeBuffer(totalNodeNum, Marshal.SizeOf(typeof(Node)));

            var initTrail = new Trail() { currentNodeIdx = -1 };
            var initNode = new Node() { time = -1 };

            trails = Enumerable.Repeat(initTrail, trailNum).ToArray();
            nodes = Enumerable.Repeat(initNode, totalNodeNum).ToArray();

            trailBuffer.SetData(trails);
            nodeBuffer.SetData(nodes);
        }


        public void InputPoint(Input input, bool isNewTrail)
        {
            if (isNewTrail)
            {
                currentTrailId++;
            }

            if (currentTrailId < trailNum)
            {
                Trail trail = trails[currentTrailId];
                int currentNodeIdx = trail.currentNodeIdx + currentTrailId * nodeNum;

                bool update = true;
                if (trail.currentNodeIdx >= 0)
                {
                    Node node = nodes[trail.currentNodeIdx];
                    float dist = Vector3.Distance(input.pos, node.pos);
                    update = dist > updateDistaceMin;
                }

                if (update)
                {
                    Node node;
                    node.time = Time.time;
                    node.pos = input.pos;
                    node.trailId = currentTrailId;

                    currentNodeIdx++;

                    // update trail
                    trail.currentNodeIdx = currentNodeIdx % nodeNum;
                    trails[currentTrailId] = trail;

                    // write new node
                    currentNodeIdx = trail.currentNodeIdx + currentTrailId * nodeNum;
                    nodes[currentNodeIdx] = node;

                    trailBuffer.SetData(trails);
                    nodeBuffer.SetData(nodes);
                }
            }
        }



        private void OnDestroy()
        {
            trailBuffer.Release();
            nodeBuffer.Release();
        }

        #endregion
    }
}