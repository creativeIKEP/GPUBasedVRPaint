using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;


namespace TrailBase
{
    public class TrailData
    {
        public ComputeBuffer trailBuffer;
        public ComputeBuffer nodeBuffer;
        public TrailType type;
        public Trail[] trails;
        public Node[] nodes;
        public int currentTrailId;
        public int currentNodeIdx;

        public TrailData(int trailNum, int nodeNum, TrailType trailType)
        {
            currentTrailId = -1;
            currentNodeIdx = -1;
            type = trailType;
            var totalNodeNum = trailNum * nodeNum;

            trailBuffer = new ComputeBuffer(trailNum, Marshal.SizeOf(typeof(Trail)));
            nodeBuffer = new ComputeBuffer(totalNodeNum, Marshal.SizeOf(typeof(Node)));

            var initTrail = new Trail();
            var initNode = new Node() { time = -1 };

            trails = Enumerable.Repeat(initTrail, trailNum).ToArray();
            nodes = Enumerable.Repeat(initNode, totalNodeNum).ToArray();

            trailBuffer.SetData(trails);
            nodeBuffer.SetData(nodes);
        }
    }


    public enum TrailType
    {
        Trail,
        Particle
    }

    public struct Trail
    {
        public int type;
    }

    public struct Node
    {
        public float time;
        public Vector3 pos;
        public int trailId;
        public Color color;
    }

    public struct Input
    {
        public Vector3 pos;
    }


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

        #endregion

        public int trailNum = 100000;
        public float updateDistaceMin = 0.01f;
        public TrailType currentTrailType;
        public float width;

        public TrailData[] trailDatas;

        //変更するときは、ParticleBrush.computeの定数宣言も書き換えるように
        public int nodeNum { get { return 4000; } }

        #region Unity

        void Start()
        {
            trailDatas = new TrailData[Enum.GetValues(typeof(TrailType)).Length];
            for(int i=0; i<trailDatas.Length; i++)
            {
                trailDatas[i] = new TrailData(trailNum, nodeNum, (TrailType)i);
            }
        }


        public void InputPoint(Input input, bool isNewTrail)
        {
            TrailData trailData = trailDatas[(int)currentTrailType];
            if (isNewTrail)
            {
                trailData.currentTrailId++;
            }

            if (trailData.currentTrailId < trailNum)
            {
                Trail trail = trailData.trails[trailData.currentTrailId];
                if (isNewTrail)
                {
                    trail.type = (int)currentTrailType;
                }

                float dist = 0;
                if (trailData.currentNodeIdx >= 0)
                {
                    dist = Vector3.Distance(input.pos, trailData.nodes[trailData.currentNodeIdx].pos);
                }

                while(dist > updateDistaceMin && !isNewTrail)
                {
                    Node node;
                    node.time = Time.time;
                    node.pos = trailData.nodes[trailData.currentNodeIdx].pos + updateDistaceMin * (input.pos - trailData.nodes[trailData.currentNodeIdx].pos).normalized;
                    node.trailId = trailData.currentTrailId;
                    node.color = new Color(1, 0, 0, 1);

                    trailData.currentNodeIdx++;

                    if(trailData.currentNodeIdx >= nodeNum)
                    {
                        trailData.currentNodeIdx = 0;
                    }

                    // update trail
                    trailData.trails[trailData.currentTrailId] = trail;

                    // write new node
                    trailData.nodes[trailData.currentNodeIdx] = node;

                    trailData.trailBuffer.SetData(trailData.trails, trailData.currentTrailId, trailData.currentTrailId, 1);
                    trailData.nodeBuffer.SetData(trailData.nodes, trailData.currentNodeIdx, trailData.currentNodeIdx, 1);

                    dist = Vector3.Distance(input.pos, node.pos);
                }

                if (isNewTrail)
                {
                    Node mouseNode;
                    mouseNode.time = Time.time;
                    mouseNode.pos = input.pos;
                    mouseNode.trailId = trailData.currentTrailId;
                    mouseNode.color = new Color(1, 1, 1, 1);

                    trailData.currentNodeIdx++;

                    if (trailData.currentNodeIdx >= nodeNum)
                    {
                        trailData.currentNodeIdx = 0;
                    }

                    // update trail
                    trailData.trails[trailData.currentTrailId] = trail;

                    // write new node
                    trailData.nodes[trailData.currentNodeIdx] = mouseNode;

                    trailData.trailBuffer.SetData(trailData.trails, trailData.currentTrailId, trailData.currentTrailId, 1);
                    trailData.nodeBuffer.SetData(trailData.nodes, trailData.currentNodeIdx, trailData.currentNodeIdx, 1);
                }
                
            }
        }



        private void OnDestroy()
        {
            for (int i = 0; i < trailDatas.Length; i++)
            {
                trailDatas[i].trailBuffer.Release();
                trailDatas[i].nodeBuffer.Release();
            }
        }

        #endregion
    }
}