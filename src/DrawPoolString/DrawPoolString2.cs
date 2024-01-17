using System;
using System.Collections.Generic;

namespace DrawPoolStringSystem
{
    // 1[(1,1,1),(2,2,2),(3,1,1)~(3,1,2)];...;*（1～10）
    public class DrawPoolString2
    {
        /// <summary>
        /// 抽取次数
        /// </summary>
        public int DrawCount { get; private set; }
        
        public class Entry
        {
            public int Power;
            public VecList List;
        }
        
        /// <summary>
        /// 存放向量表示组的列表
        /// </summary>
        public class VecList
        {
            /// <summary>
            /// 纬度
            /// 
            /// </summary>
            public int Rank;
            /// <summary>
            /// 数据定义
            /// 如果是单个向量，则向量存放为[x,y,z]
            /// 如果是多个向量，则向量存放为[xMin,yMin,zMin,xMax,yMax,zMax]
            /// </summary>
            public List<int[]> Range;

            /// <summary>
            /// 等概率抽取一个元素
            /// </summary>
            public int[] DrawAnVector(int randNum)
            {
                throw new NotImplementedException();
            }
        }
    }
}