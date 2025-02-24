//using System.Collections;
//using System.Collections.Generic;
//using System;
//using System.Windows;
//using UnityEngine;
//using System;
//public class Avatar : MonoBehaviour
//{

//    //public Animator targetAvatar;
//    public List<Bvh> motions = new List<Bvh>();
//    public int Motion=0,frames=0,nodeframe=0;
//    public float frametime = 0,startime;
//    public List<List<Single3>> offset_vectors = new List<List<Single3>>(); // vectorX, vectorY, distance
//    public float frameRate;
//    GameObject super;
//    private Dictionary<string, string> renamingMap;
//    private Dictionary<string, Transform> nameMap;
//    private string prefix;

//    public AnimationClip clip;
//    public bool doingAction=false;
//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        float times = (float)motions[Motion].FrameTime.TotalSeconds ;
//        if (this.transform.childCount != 0 && motions.Count != 0)
//        { InvokeRepeating("Do_Motions", times, times);}
//        if (doingAction)
//            frameRate = 1f / motions[Motion].FrameTime.Milliseconds;
        
//    }

//    class point2D
//    {
//        public float x;
//        public float y;
//        public point2D(float x, float y)
//        {
//            this.x = x;
//            this.y = y;
//        }
//    }
    
//    public void Add_Motions(List<Bvh> newMotion)
//    {
//        foreach (Bvh motion in newMotion)
//            ChordLength(motion);
//        motions.AddRange(newMotion);
//        CurveFitting(motions[0]);
//        List<List<Single3>> newOffset = new List<List<Single3>>(newMotion.Count);
        
//        /*offset_vectors.AddRange(newOffset);
//        offset_vectors[0] = new List<Single3>(motions[0].FrameCount);*/
//        //Cal_Origin_Path(motions[0]);
//        Debug.Log(newMotion.Count);
//    }
//    /*
//    public void Do_Motions()
//    {
//        doingAction = true;
//        nodeframe = 0;
        
//        //prefix = GetPathBetween(this.transform.GetChild(0), this.transform.GetChild(0).GetChild(0), true, true);
//        //getCurves("", motions[Motion].Root, this.transform,true);
        
//        foreach (Transform child in this.transform)
//        {
//            if (child.gameObject.name == "link")
//                Destroy(child.gameObject);

//        }
//        super = null;

//        if (frame >= motions[Motion].FrameCount)
//            frame = 0;

//        foreach(Transform child in this.transform)
//        {
//            if(child.gameObject.name!= "link")
//                ChildMotion(child, motions[Motion].Root);

//        }
//        nodeframe = 0;
//        frames++;
//        Debug.Log("endmotions");
//    }*/

//    /*void ChildMotion(Transform child,BvhNode bvh)
//    {        
//            Debug.Log("child + " + child.gameObject.name+ "   childrenNode+"+bvh.Name);
//        int channels=bvh.Channels.Length;
//        if (channels == 6)
//        {
//            float posx=0.0f;
//            if (nodeframe == 0)
//            { posx = child.position.x + motions[Motion].Channels[frames].Keys[nodeframe];}
//            else if (nodeframe!=0)
//            { posx = child.position.x + motions[Motion].Channels[++frames].Keys[nodeframe];}
//            float posy = child.position.y + motions[Motion].Channels[++frames].Keys[nodeframe];
//            float posz = child.position.z + motions[Motion].Channels[++frames].Keys[nodeframe];
//            float rotz = child.rotation.z + motions[Motion].Channels[++frames].Keys[nodeframe];
//            float rotx = child.rotation.x + motions[Motion].Channels[++frames].Keys[nodeframe];
//            float roty = child.rotation.y + motions[Motion].Channels[++frames].Keys[nodeframe];
//            child.position =new Vector3 (posx,posy, posz );
//            //child.Rotate(new Vector3(rotx,roty ,rotz ));
//            Quaternion childrot= fromEulerZXY(new Vector3( rotz, rotx, roty));
//            child.rotation = childrot* child.rotation;
//        }
//        else if (channels == 3)
//        {
//            float rotz = child.rotation.z + motions[Motion].Channels[++frames].Keys[nodeframe];
//            float rotx = child.rotation.x + motions[Motion].Channels[++frames].Keys[nodeframe];
//            float roty = child.rotation.y + motions[Motion].Channels[++frames].Keys[nodeframe];
//            Quaternion childrot = fromEulerZXY(new Vector3(rotz, rotx, roty));
//            child.rotation = childrot * child.rotation;
//        }
//        if (super!=null)
//            DrawLS(super, child.gameObject);
//        if (child.childCount != 0)
//            for (var i = 0; i < child.childCount; i++)
//            {
//                if (child.GetChild(i).name != "End")
//                {
//                    super = child.gameObject;
//                    ChildMotion(child.GetChild(i), bvh.Children[i]);
//                }
//                else if(child.GetChild(i).name != "End")
//                {
//                    DrawLS(child.gameObject, child.GetChild(0).gameObject);
//                }
//            }

//    }*/

//    //建立骨架
//    public void Add_Bone() { ROOT(); }
//    //GameObject super;
//    public void ROOT()
//    {
//        GameObject root = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//        root.transform.position = new Vector3(motions[0].Root.Offset.x, motions[0].Root.Offset.y, motions[0].Root.Offset.z);
//        root.gameObject.transform.parent = this.transform;
//        root.name = motions[0].Root.Name;
//        super = root;
//        foreach (BvhNode descentant in motions[0].Root.Children)
//        {
//            super = root;
//            bones(descentant);
//        }

//        Debug.Log("Bvh.Root.Offset" + motions[0].Root.Offset);
//        //Debug.Log("Bvh.Root.Channel" + Bvh.Root.Channels);
//    }

//    public void bones(BvhNode child)
//    {
//        GameObject bone = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//        bone.transform.parent = super.transform;
//        bone.transform.position = new Vector3(child.Offset.x, child.Offset.y, child.Offset.z);
//        bone.name = child.Name;
//        DrawLS(super, bone);
//        super = bone;
//        if (child.Children != null)
//            foreach (BvhNode descentant in child.Children)
//            {
//                super = bone;
//                bones(descentant);
//            }
//        else if (child.Children == null)
//            super = null;
//    }
//    //建立骨架連結
//    void DrawLS(GameObject startP, GameObject finalP)
//    {
//        Vector3 rightPosition = (startP.transform.position + finalP.transform.position) / 2;
//        Vector3 rightRotation = finalP.transform.position - startP.transform.position;
//        float HalfLength = Vector3.Distance(startP.transform.position, finalP.transform.position) / 2;
//        float LThickness = 0.1f;//粗细

//        //創建圆柱體
//        GameObject MyLine = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
//        MyLine.gameObject.transform.parent = this.transform;
//        MyLine.transform.position = rightPosition;
//        MyLine.transform.rotation = Quaternion.FromToRotation(Vector3.up, rightRotation);
//        MyLine.transform.localScale = new Vector3(LThickness, HalfLength, LThickness);
//        MyLine.name = "link";
//        //設置材質
//        //MyLine.GetComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
//    }

//    void Cal_Origin_Path(Bvh bvh)
//    {
//        // a, c取值範圍
//        const double MIN_a = -2768.0;
//        const double MAX_a = 2768.0;
//        const double MIN_c = -2768.0;
//        const double MAX_c = 2768.0;
//        // 遞增值
//        const double INC = 0.01;

//        //z = a * x + c
//        double x_pos = bvh.Root.Offset.x;
//        double z_pos = bvh.Root.Offset.z;

//        int frameCount = bvh.FrameCount;
//        double[] x_pts = new double[frameCount];
//        double[] z_pts = new double[frameCount];

//        double m_a = 0, m_c = 0;

//        for (int i = 0; i < frameCount; i++)
//        {
//            x_pts[i] = bvh.Channels[0].Keys[i];
//            z_pts[i] = bvh.Channels[2].Keys[i];

//            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//            sphere.transform.position = new Vector3(Convert.ToSingle(x_pts[i]), 0, Convert.ToSingle(z_pts[i]));
//        }

//        double minCost = CaculateCost(m_a, m_c, x_pts, z_pts, frameCount);
//        double curCost = 0.0;

//        // 先計算最佳的a
//        for (double a = MIN_a; a <= MAX_a; a += INC)
//        {
//            curCost = CaculateCost(a, m_c, x_pts, z_pts, frameCount);
//            if (curCost < minCost)
//            {
//                m_a = a;
//                minCost = curCost;
//            }
//        }

//        // 再計算最佳的c
//        for (double c = MIN_c; c <= MAX_c; c += INC)
//        {
//            curCost = CaculateCost(m_a, c, x_pts, z_pts, frameCount);
//            if (curCost < minCost)
//            {
//                m_c = c;
//                minCost = curCost;
//            }
//        }

//        //L = a * x + b * z + c = 0;
//        double m_b = -1;
//        double MAX_X = Double.MinValue, MAX_Z = Double.MinValue;
//        double MIN_X = Double.MaxValue, MIN_Z = Double.MaxValue;
//        double[] T_xs = new double[frameCount];
//        double[] T_zs = new double[frameCount];
//        for (int i = 0; i < frameCount; i++)
//        {
//            //T = (x0 + at, z0 + bt)
//            double t = -(m_a * x_pts[i] + m_b * z_pts[i] + m_c) / (m_a * m_a + m_b * m_b);
//            double T_x = x_pts[i] + m_a * t;
//            double T_z = z_pts[i] + m_b * t;

//            T_xs[i] = T_x;
//            T_zs[i] = T_z;

//            MAX_X = MAX_X < T_x ? T_x : MAX_X;
//            MIN_X = MIN_X > T_x ? T_x : MIN_X;
//            MAX_Z = MAX_Z < T_z ? T_z : MAX_Z;
//            MIN_Z = MIN_Z > T_z ? T_z : MIN_Z;

//            double vector_x = m_a * t;
//            double vector_z = m_b * t;
//            double dis = Math.Sqrt(vector_x * vector_x + vector_z * vector_z);
//            Offset_Vector newOffset = new Offset_Vector();
//            newOffset.x = vector_x / dis;
//            newOffset.z = vector_z / dis;
//            newOffset.length = dis;
//            //bvh.Offset_Vectors.Add(newOffset);
//        }

//        if(m_a >= 0)
//        {
//            Pt2D start, end;
//            start.x = MIN_X;
//            start.z = MIN_Z;
//            end.x = MAX_X;
//            end.z = MAX_Z;
//            bvh.Origin_Start = start;
//            bvh.Origin_End = end;
//        }
//        else
//        {
//            Pt2D start, end;
//            start.x = MIN_X;
//            start.z = MAX_Z;
//            end.x = MAX_X;
//            end.z = MIN_Z;
//            bvh.Origin_Start = start;
//            bvh.Origin_End = end;
//        }
//        Debug.DrawLine(new Vector3(Convert.ToSingle(bvh.Origin_Start.x), 5.0f, Convert.ToSingle(bvh.Origin_Start.z)), new Vector3(Convert.ToSingle(bvh.Origin_End.x), 0.0f, Convert.ToSingle(bvh.Origin_End.z)), Color.green, 214748364);
        
//        double x_intercept = bvh.Origin_Start.x - bvh.Origin_End.x;
//        double z_intercept = bvh.Origin_Start.z - bvh.Origin_End.z;
//        double pathLen = Math.Sqrt(x_intercept * x_intercept + z_intercept * z_intercept);
//        /*for(int i = 0; i < frameCount; i++)
//        {
//            Offset_Vector newOffset = bvh.Offset_Vectors[i];
//            x_intercept = T_xs[i] - bvh.Origin_Start.x;
//            z_intercept = T_zs[i] - bvh.Origin_Start.z;
//            double len = Math.Sqrt(x_intercept * x_intercept + z_intercept * z_intercept);
//            newOffset.t = len / pathLen;
//            bvh.Offset_Vectors[i] = newOffset;
//        }*/
//    }

//    double CaculateCost(double a, double b, double[] x_pts, double[] y_pts, int size)
//    {
//        double cost = 0.0;
//        double xReal = 0.0;
//        double yReal = 0.0;
//        double yPredict = 0.0;
//        double yDef = 0.0;
//        for (int i = 0; i < size; ++i)
//        {
//            // x實際值
//            xReal = x_pts[i];
//            // y實際值
//            yReal = y_pts[i];
//            // y預測值
//            yPredict = a * xReal + b;

//            yDef = yPredict - yReal;
//            // 累加方差
//            cost += (yDef * yDef);
//        }
//        return cost;
//    }

//    void ChordLength(Bvh bvh)
//    {
//        int frameCount = bvh.FrameCount;
//        float[] prmtrs = new float[frameCount];

//        double sum = 0;
//        double[] accumulate = new double[frameCount];

//        accumulate[0] = 0;
//        for (int i = 1; i < frameCount; i++)
//        {
//            sum += CalDistance(bvh.Channels[0].Keys[i - 1], bvh.Channels[2].Keys[i - 1], bvh.Channels[0].Keys[i], bvh.Channels[2].Keys[i]);
//            accumulate[i] = sum;
//        }

//        prmtrs[0] = 0;
//        prmtrs[frameCount - 1] = 1;
//        for (int i = 1; i < frameCount - 1; i++)
//        {
//            prmtrs[i] = Convert.ToSingle(accumulate[i] / sum);
//        }

//        bvh.Params = prmtrs;
//    }

//    double CalDistance(float x1, float y1, float x2, float y2)
//    {
//        float _x = x1 - x2, _y = y1 - y2;
//        return Math.Sqrt(_x * _x + _y * _y);
//    }

//    void CurveFitting(Bvh bvh)
//    {
//        int frameCount = bvh.FrameCount;
//        float[][] Q = MatrixCreate(frameCount, 3);
//        float[] x_pts = new float[frameCount];
//        float[] y_pts = new float[frameCount];
//        float[] z_pts = new float[frameCount];

//        for (int i = 0; i < frameCount; i++)
//        {
//            Q[i][0] = bvh.Channels[0].Keys[i];
//            Q[i][1] = bvh.Channels[1].Keys[i];
//            Q[i][2] = bvh.Channels[2].Keys[i];
//        }
//        float B3_0(float t) => 0.16667f * (1 - t) * (1 - t) * (1 - t);
//        float B3_1(float t) => 0.16667f * (3 * t * t * t - 6 * t * t + 4);
//        float B3_2(float t) => 0.16667f * (-3 * t * t * t + 3 * t * t + 3 * t + 1);
//        float B3_3(float t) => 0.16667f * t * t * t;

//        float[][] A = MatrixCreate(4, 4);
//        float[][] p = MatrixCreate(4, 3);

//        for(int i = 0; i < frameCount; i++)
//        {
//            float B0 = B3_0(bvh.Params[i]);
//            float B1 = B3_1(bvh.Params[i]);
//            float B2 = B3_2(bvh.Params[i]);
//            float B3 = B3_3(bvh.Params[i]);

//            A[0][0] += B0 * B0;
//            A[0][1] += B0 * B1;
//            A[0][2] += B0 * B2;
//            A[0][3] += B0 * B3;
//            A[1][1] += B1 * B1;
//            A[1][2] += B1 * B2;
//            A[1][3] += B1 * B3;

//            A[2][2] += B2 * B2;
//            A[2][3] += B2 * B3;

//            A[3][3] += B3 * B3;
//        }

//        A[1][0] = A[0][1];

//        A[2][0] = A[0][2];
//        A[2][1] = A[1][2];

//        A[3][0] = A[0][3];
//        A[3][1] = A[1][3];
//        A[3][2] = A[2][3];

//        for(int dimension = 0; dimension < 3; dimension++)
//        {
//            float[][] b = MatrixCreate(4, 1);
//            for(int i = 0; i < frameCount; i++)
//            {
//                b[0][0] += B3_0(bvh.Params[i]) * Q[i][dimension];
//                b[1][0] += B3_1(bvh.Params[i]) * Q[i][dimension];
//                b[2][0] += B3_2(bvh.Params[i]) * Q[i][dimension];
//                b[3][0] += B3_3(bvh.Params[i]) * Q[i][dimension];
//            }
//            float[][] x = MatrixProduct(MatrixInverse(A), b);
//            p[0][dimension] = x[0][0];
//            p[1][dimension] = x[1][0];
//            p[2][dimension] = x[2][0];
//            p[3][dimension] = x[3][0];
//        }

//        for(int i = 0; i < 4; i++)
//        {
//            bvh.ControlPoints[i].x = p[i][0];
//            bvh.ControlPoints[i].y = p[i][1];
//            bvh.ControlPoints[i].z = p[i][2];
//        }

//        for (int i = 0; i < frameCount; i++)
//        {
//            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//            float t = bvh.Params[i];
//            Vector3 pos = new Vector3();
//            pos.x = B3_0(t) * p[0][0] + B3_1(t) * p[1][0] + B3_2(t) * p[2][0] + B3_3(t) * p[3][0];
//            pos.y = B3_0(t) * p[0][1] + B3_1(t) * p[1][1] + B3_2(t) * p[2][1] + B3_3(t) * p[3][1];
//            pos.z = B3_0(t) * p[0][2] + B3_1(t) * p[1][2] + B3_2(t) * p[2][2] + B3_3(t) * p[3][2];
//            sphere.transform.position = pos;
//        }
//        return;
//    }

//    float DeBoorValue(int index/*i*/, int degree/*p*/, float u, float[] knots)
//    {
//        if (degree == 0)
//        {
//            if (u >= knots[index] && u < knots[index + 1])
//                return 1;
//            else
//                return 0;
//        }
//        float first = 0;
//        float second = 0;
//        float first_DeBoor = DeBoorValue(index, degree - 1, u, knots);
//        float second_DeBoor = DeBoorValue(index + 1, degree - 1, u, knots);
//        float first_coef = (u - knots[index]) / (knots[index + degree] - knots[index]);
//        float second_coef = (knots[index + degree + 1] - u) / (knots[index + degree + 1] - knots[index + 1]);

//        if (first_DeBoor != 0)
//            first = first_coef * first_DeBoor;
//        if (second_DeBoor != 0)
//            second = second_coef * second_DeBoor;

//        float ans = first + second;
//        return ans;
//    }

//    float[] ComputeCoef(int n, int p, int m, float u, float[] knots)
//    {
//        float[] N = new float[n + 1];
//        for (int i = 0; i <= n; i++)
//            N[i] = 0;
//        if (u == knots[0])
//        {
//            N[0] = 0;
//            return N;
//        }
//        else if (u == knots[m])
//        {
//            N[n] = 1;
//            return N;
//        }

//        int k = 0;
//        for (; k < m; k++)
//        {
//            if (u < knots[k])
//            {
//                k--;
//                break;
//            }
//        }
//        N[k] = 1.0f;
//        for (int d = 1; d <= p; d++)
//        {
//            N[k - d] = (knots[k + 1] - u) / (knots[k + 1] - knots[k - d + 1]) * N[k - d + 1];
//            for (int i = k - d + 1; i <= k - 1; i++)
//                N[i] = (u - knots[i]) / (knots[i + d] - knots[i]) * N[i] + (knots[i + d + 1] - u) / (knots[i + d + 1] - knots[i + 1]) * N[i + 1];
//            N[k] = (u - knots[k]) / (knots[k + d] - knots[k]) * N[k];
//        }

//        return N;
//    }

//    static float[][] MatrixCreate(int rows, int cols)
//    {
//        float[][] result = new float[rows][];
//        for (int i = 0; i < rows; ++i)
//            result[i] = new float[cols];
//        return result;
//    }

//    static float[][] MatrixIdentity(int n)
//    {
//        // return an n x n Identity matrix
//        float[][] result = MatrixCreate(n, n);
//        for (int i = 0; i < n; ++i)
//            result[i][i] = 1.0f;

//        return result;
//    }

//    static float[][] MatrixProduct(float[][] matrixA, float[][] matrixB)
//    {
//        int aRows = matrixA.Length; int aCols = matrixA[0].Length;
//        int bRows = matrixB.Length; int bCols = matrixB[0].Length;
//        if (aCols != bRows)
//            throw new Exception("Non-conformable matrices in MatrixProduct");

//        float[][] result = MatrixCreate(aRows, bCols);

//        for (int i = 0; i < aRows; ++i) // each row of A
//            for (int j = 0; j < bCols; ++j) // each col of B
//                for (int k = 0; k < aCols; ++k) // could use k less-than bRows
//                    result[i][j] += matrixA[i][k] * matrixB[k][j];

//        return result;
//    }

//    static float[][] MatrixInverse(float[][] matrix)
//    {
//        int n = matrix.Length;
//        float[][] result = MatrixDuplicate(matrix);

//        int[] perm;
//        int toggle;
//        float[][] lum = MatrixDecompose(matrix, out perm,
//          out toggle);
//        if (lum == null)
//            throw new Exception("Unable to compute inverse");

//        float[] b = new float[n];
//        for (int i = 0; i < n; ++i)
//        {
//            for (int j = 0; j < n; ++j)
//            {
//                if (i == perm[j])
//                    b[j] = 1.0f;
//                else
//                    b[j] = 0.0f;
//            }

//            float[] x = HelperSolve(lum, b);

//            for (int j = 0; j < n; ++j)
//                result[j][i] = x[j];
//        }
//        return result;
//    }

//    static float[][] MatrixDuplicate(float[][] matrix)
//    {
//        // allocates/creates a duplicate of a matrix.
//        float[][] result = MatrixCreate(matrix.Length, matrix[0].Length);
//        for (int i = 0; i < matrix.Length; ++i) // copy the values
//            for (int j = 0; j < matrix[i].Length; ++j)
//                result[i][j] = matrix[i][j];
//        return result;
//    }

//    static float[] HelperSolve(float[][] luMatrix, float[] b)
//    {
//        // before calling this helper, permute b using the perm array
//        // from MatrixDecompose that generated luMatrix
//        int n = luMatrix.Length;
//        float[] x = new float[n];
//        b.CopyTo(x, 0);

//        for (int i = 1; i < n; ++i)
//        {
//            float sum = x[i];
//            for (int j = 0; j < i; ++j)
//                sum -= luMatrix[i][j] * x[j];
//            x[i] = sum;
//        }

//        x[n - 1] /= luMatrix[n - 1][n - 1];
//        for (int i = n - 2; i >= 0; --i)
//        {
//            float sum = x[i];
//            for (int j = i + 1; j < n; ++j)
//                sum -= luMatrix[i][j] * x[j];
//            x[i] = sum / luMatrix[i][i];
//        }

//        return x;
//    }

//    static float[][] MatrixDecompose(float[][] matrix, out int[] perm, out int toggle)
//    {
//        // Doolittle LUP decomposition with partial pivoting.
//        // rerturns: result is L (with 1s on diagonal) and U;
//        // perm holds row permutations; toggle is +1 or -1 (even or odd)
//        int rows = matrix.Length;
//        int cols = matrix[0].Length; // assume square
//        if (rows != cols)
//            throw new Exception("Attempt to decompose a non-square m");

//        int n = rows; // convenience

//        float[][] result = MatrixDuplicate(matrix);

//        perm = new int[n]; // set up row permutation result
//        for (int i = 0; i < n; ++i) { perm[i] = i; }

//        toggle = 1; // toggle tracks row swaps.
//                    // +1 -greater-than even, -1 -greater-than odd. used by MatrixDeterminant

//        for (int j = 0; j < n - 1; ++j) // each column
//        {
//            float colMax = Math.Abs(result[j][j]); // find largest val in col
//            int pRow = j;
//            //for (int i = j + 1; i less-than n; ++i)
//            //{
//            //  if (result[i][j] greater-than colMax)
//            //  {
//            //    colMax = result[i][j];
//            //    pRow = i;
//            //  }
//            //}

//            // reader Matt V needed this:
//            for (int i = j + 1; i < n; ++i)
//            {
//                if (Math.Abs(result[i][j]) > colMax)
//                {
//                    colMax = Math.Abs(result[i][j]);
//                    pRow = i;
//                }
//            }
//            // Not sure if this approach is needed always, or not.

//            if (pRow != j) // if largest value not on pivot, swap rows
//            {
//                float[] rowPtr = result[pRow];
//                result[pRow] = result[j];
//                result[j] = rowPtr;

//                int tmp = perm[pRow]; // and swap perm info
//                perm[pRow] = perm[j];
//                perm[j] = tmp;

//                toggle = -toggle; // adjust the row-swap toggle
//            }

//            // --------------------------------------------------
//            // This part added later (not in original)
//            // and replaces the 'return null' below.
//            // if there is a 0 on the diagonal, find a good row
//            // from i = j+1 down that doesn't have
//            // a 0 in column j, and swap that good row with row j
//            // --------------------------------------------------

//            if (result[j][j] == 0.0)
//            {
//                // find a good row to swap
//                int goodRow = -1;
//                for (int row = j + 1; row < n; ++row)
//                {
//                    if (result[row][j] != 0.0)
//                        goodRow = row;
//                }

//                if (goodRow == -1)
//                    throw new Exception("Cannot use Doolittle's method");

//                // swap rows so 0.0 no longer on diagonal
//                float[] rowPtr = result[goodRow];
//                result[goodRow] = result[j];
//                result[j] = rowPtr;

//                int tmp = perm[goodRow]; // and swap perm info
//                perm[goodRow] = perm[j];
//                perm[j] = tmp;

//                toggle = -toggle; // adjust the row-swap toggle
//            }
//            // --------------------------------------------------
//            // if diagonal after swap is zero . .
//            //if (Math.Abs(result[j][j]) less-than 1.0E-20) 
//            //  return null; // consider a throw

//            for (int i = j + 1; i < n; ++i)
//            {
//                result[i][j] /= result[j][j];
//                for (int k = j + 1; k < n; ++k)
//                {
//                    result[i][k] -= result[i][j] * result[j][k];
//                }
//            }


//        } // main j column loop

//        return result;
//    }

//    static float[][] MatrixTranspose(float[][] matrix)
//    {
//        float[][] result = MatrixCreate(matrix[0].Length, matrix.Length);
//        for (int i = 0; i < matrix.Length; ++i) // copy the values
//            for (int j = 0; j < matrix[i].Length; ++j)
//                result[j][i] = matrix[i][j];
//        return result;
//    }
//    // BVH to Unity
//    //private Quaternion fromEulerZXY(Vector3 euler)
//    //{
//    //    return Quaternion.AngleAxis(euler.z, Vector3.forward) * Quaternion.AngleAxis(euler.x, Vector3.right) * Quaternion.AngleAxis(euler.y, Vector3.up);
//    //}
//    //private float wrapAngle(float a)
//    //{
//    //    if (a > 180f)
//    //    {
//    //        return a - 360f;
//    //    }
//    //    if (a < -180f)
//    //    {
//    //        return 360f + a;
//    //    }
//    //    return a;
//    //}
//}
