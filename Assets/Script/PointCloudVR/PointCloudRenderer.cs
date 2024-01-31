using UnityEngine;

namespace PointCloudVR
{
    class PointCloudRenderer
    {
        private Vector3[] points;
        private int[] colors;

        private int totalPoints = 0;

        private Matrix4x4 localToWorldMatrix;
        private Material pointMaterial;

        private MeshTopology topology;

        private ComputeBuffer pointsBuffer;
        private ComputeBuffer colorsBuffer;
        public PointCloudRenderer()
        {
            topology = MeshTopology.Points;
            pointMaterial = new Material(Shader.Find("Custom/MyDefaultPoint"));
        }

        private void PrepareMaterial()
        {
            pointMaterial.SetMatrix("_Transform", localToWorldMatrix);
            pointMaterial.SetBuffer("_Positions", pointsBuffer);
            pointMaterial.SetBuffer("_Colors", colorsBuffer);
            pointMaterial.SetPass(0);
        }

        public void SetWorldMatrix(Matrix4x4 localToWorldMatrix)
        {
            this.localToWorldMatrix = localToWorldMatrix;
        }
        public void SetData(Vector3[] points, int[] colors)
        {

            if (points.Length == 0 || colors.Length == 0) return;

            if (pointsBuffer != null && colorsBuffer != null)
            {
                pointsBuffer.Release();
                colorsBuffer.Release();
            }

            pointsBuffer = new ComputeBuffer(points.Length, 3 * sizeof(float));
            colorsBuffer = new ComputeBuffer(colors.Length, sizeof(int));

            pointsBuffer.SetData(points);
            colorsBuffer.SetData(colors);

            totalPoints = points.Length;

            //PrepareMaterial();
        }
        public void SetTopology(RenderMode renderMode)
        {
            switch (renderMode)
            {
                case RenderMode.POINT:
                    topology = MeshTopology.Points;
                    break;
                case RenderMode.CUBE:
                    topology = MeshTopology.Quads;
                    break;
                default:
                    break;
            }
        }
        //public int GetNPoints()
        //{
        //    return (points != null) ? points.Length : 0;
        //}

        public void Render()
        {
            if (pointsBuffer == null || colorsBuffer == null || totalPoints == 0)
            {
                Debug.Log("Esco dalla funzione Render()");
                return;
            }


            //ComputeBuffer positionsBuffer = new ComputeBuffer(points.Length, 3 * sizeof(float));
            //ComputeBuffer colorsBuffer = new ComputeBuffer(colors.Length, sizeof(int));
            //ComputeBuffer normalsBuffer = new ComputeBuffer(points.Length, 3 * sizeof(float));
            //positionsBuffer.SetData(points);
            //colorsBuffer.SetData(colors);

            //Vector3[] normals = new Vector3[points.Length];
            //for (int i = 0; i < normals.Length; i++ )
            //{
            //    normals[i] = Vector3.up;
            //}

            Debug.Log($"Punti da Renderizzare: {pointsBuffer.count}");
            

            //pointsBuffer.SetData(points);
            //colorsBuffer.SetData(colors);

            pointMaterial.SetMatrix("_Transform", localToWorldMatrix);
            pointMaterial.SetBuffer("_Positions", pointsBuffer);
            pointMaterial.SetBuffer("_Colors", colorsBuffer);
            pointMaterial.SetPass(0);
            Graphics.DrawProceduralNow(topology, totalPoints, 1);


        }
    }

}
