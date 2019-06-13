using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGW
{
    public class QuadTreeLevelInfo
    {
        public float planetSpawnProbability {get; set;}
        public int spetialModePlanetsCount {get; set;}
        public int minQuadTreeCellSize {get; set;}
        public int maxDepth {get; set;}
        public int randomAppendix {get; set;}
        public float distributionOfPlanetsDelta {get; set;}
        public int maxRating {get; set;}
        public int minRating {get; set;}
        public class QuadTreeInfo
        {
            public Vector2Int topLeftCorner;
            public int depth;
            public int planetsCount;
            public int size;
            public List<QuadTreeInfo> nodes;
            public System.Random random;
            public List<int> planetsValuesInRange;
            public List<Vector2Int> planetsPositionsInRange;
        }



        private int m_rootCellSize;
        private int m_rootPlanetsCount;
        private Vector2Int m_rootTopLeftCorner;
        private QuadTreeInfo m_rootInfo;

        public int currentMaxDepth = 0;

        public QuadTreeLevelInfo(float planetSpawnProbability, int minQuadTreeCellSize, int maxDepth, int randomAppendix, int spetialModePlanetsCount, int minRating, int maxRating)
        {
            distributionOfPlanetsDelta = 0.01f;
            this.planetSpawnProbability = planetSpawnProbability;
            this.minQuadTreeCellSize = minQuadTreeCellSize;
            this.maxDepth = maxDepth;
            this.randomAppendix = randomAppendix;
            this.spetialModePlanetsCount = spetialModePlanetsCount;
            this.minRating = minRating;
            this.maxRating = maxRating;
            m_rootCellSize = minQuadTreeCellSize * (int)Mathf.Pow(2, maxDepth);
            m_rootPlanetsCount = Mathf.RoundToInt(m_rootCellSize * m_rootCellSize * planetSpawnProbability / 100.0f);
            var corner = m_rootCellSize / 2;
            m_rootTopLeftCorner = new Vector2Int(-corner, corner);
            m_rootInfo = new QuadTreeInfo();
            m_rootInfo.topLeftCorner = m_rootTopLeftCorner;
            m_rootInfo.planetsCount = m_rootPlanetsCount;
            m_rootInfo.depth = 0;
            m_rootInfo.size = m_rootCellSize;
        }

        public QuadTreeInfo GetQuadTreeInfoForRectInt(RectInt visibleRectInt, int maxDepthByVisual, int shipValue)
        {
            m_rootInfo.nodes = null;
            currentMaxDepth = 0;
            SplitQuadTreeUntilReachMaxDepth(0, m_rootPlanetsCount, m_rootTopLeftCorner, m_rootCellSize, m_rootInfo, visibleRectInt, maxDepthByVisual, shipValue, new List<int>(), 0);
            return m_rootInfo;
        }

        void SplitQuadTreeUntilReachMaxDepth(int depth, int planetsCount, Vector2Int topLeftCorner, int size,  QuadTreeInfo quadTreeCellInfo, RectInt visibleRectInt, int maxDepthByVisual, int shipValue, List<int> previouslyGeneratedPValues, int previousRestriction)
        {
            if (currentMaxDepth < depth)
            {
                currentMaxDepth = depth;
            }

            // Utils.DebugDrawColoredRectangle(visibleRectInt, Color.green);
            RectInt cellRectInt = new RectInt(topLeftCorner.x, topLeftCorner.y - size, size, size);
            // Utils.DebugDrawColoredRectangle(cellRectInt, Color.red);
            bool isIntersectWithVisibleRectInt = cellRectInt.IntersectOrContains(visibleRectInt);
            quadTreeCellInfo.random = new System.Random(topLeftCorner.x + topLeftCorner.y + (int)cellRectInt.center.magnitude + cellRectInt.y + size + randomAppendix);
            var planetsValuesInRange = new List<int>();
            var planetsPositionsInRange = new List<Vector2Int>();
            int offset = maxDepth + 1 - depth;
            planetsValuesInRange.AddRange(previouslyGeneratedPValues);
            int currentRestriction = shipValue / offset;
            int planetsCountInRange = (depth != maxDepth ? spetialModePlanetsCount : planetsCount);
            List<Vector2Int> tepmPositionsList = null;
            Vector2Int posOffset = new Vector2Int(cellRectInt.xMin, cellRectInt.yMax);
            tepmPositionsList = new List<Vector2Int>();
            float multiplicator = size / 7;
            int tepmPositionsInListCount = 49;
            int minQuadTreeCellSize_ = 7;

            if (depth == maxDepth)
            {
                multiplicator = 1;
                tepmPositionsInListCount  = minQuadTreeCellSize * minQuadTreeCellSize;
                minQuadTreeCellSize_ = minQuadTreeCellSize;
            }

            for (int i = 0; i < tepmPositionsInListCount; i++)
            {
                var index = Utils.XYFromIndex(i, minQuadTreeCellSize);
                tepmPositionsList.Add(posOffset + new Vector2Int((int)(index.x * multiplicator), -(int)(index.y * multiplicator)));
            }

            tepmPositionsList.Shuffle(quadTreeCellInfo.random);

            for (int i = previouslyGeneratedPValues.Count; i < planetsCountInRange; i++)
            {
                var ratingOffset = quadTreeCellInfo.random.Next(previousRestriction, currentRestriction);
                var ratingValue =  shipValue + (ratingOffset * (quadTreeCellInfo.random.Next(0, 1) == 1 ? 1 : -1));
                planetsValuesInRange.Add(Mathf.Clamp(ratingValue, minRating, maxRating));
            }

            for (int i = 0; i < planetsCountInRange; i++)
            {
                int index = quadTreeCellInfo.random.Next(0, tepmPositionsList.Count - 1);
                var randomPosition = tepmPositionsList[index];
                tepmPositionsList.RemoveAt(index);
                planetsPositionsInRange.Add(randomPosition);
            }

            planetsValuesInRange.Shuffle(quadTreeCellInfo.random);
            planetsPositionsInRange.Shuffle(quadTreeCellInfo.random);
            quadTreeCellInfo.planetsValuesInRange = planetsValuesInRange;
            quadTreeCellInfo.planetsPositionsInRange = planetsPositionsInRange;

            // _ _
            //|A B|
            //|C D|
            // ¯ ¯
            if (isIntersectWithVisibleRectInt && depth < maxDepthByVisual && depth < maxDepth)
            {
                int generatedPlanetsPerRange = spetialModePlanetsCount / 4;
                int quadAGeneratedPlanetsCount = spetialModePlanetsCount % 4 + generatedPlanetsPerRange;
                quadTreeCellInfo.nodes = new List<QuadTreeInfo>();
                var newSize = size / 2;
                Vector2Int topLeftCornerA = topLeftCorner;
                Vector2Int topLeftCornerB = topLeftCorner + new Vector2Int(newSize, 0);
                Vector2Int topLeftCornerC = topLeftCorner + new Vector2Int(0, -newSize);
                Vector2Int topLeftCornerD = topLeftCorner + new Vector2Int(newSize, -newSize);
                int newDepth = depth + 1;
                int count = quadTreeCellInfo.planetsCount;
                var cellPlanetsCount = quadTreeCellInfo.planetsCount / 4;
                //A
                QuadTreeInfo quadTreeCellInfoA = new QuadTreeInfo();
                quadTreeCellInfoA.topLeftCorner = topLeftCornerA;
                int cellPlanetsCountA = cellPlanetsCount ;//+ (int)(quadTreeCellInfo.planetsCount * quadTreeCellInfo.random.GetRandomNumberInRange(-1, 1) * distributionOfPlanetsDelta);
                count -= cellPlanetsCountA;
                quadTreeCellInfoA.planetsCount = cellPlanetsCountA;
                quadTreeCellInfoA.depth = newDepth;
                quadTreeCellInfoA.size = newSize;
                quadTreeCellInfo.nodes.Add(quadTreeCellInfoA);
                //B
                QuadTreeInfo quadTreeCellInfoB = new QuadTreeInfo();
                quadTreeCellInfoB.topLeftCorner = topLeftCornerB;
                int cellPlanetsCountB = cellPlanetsCount ;//+ (int)(quadTreeCellInfo.planetsCount * quadTreeCellInfo.random.GetRandomNumberInRange(-1, 1) * distributionOfPlanetsDelta);
                count -= cellPlanetsCountB;
                quadTreeCellInfoB.planetsCount = cellPlanetsCountB;
                quadTreeCellInfoB.depth = newDepth;
                quadTreeCellInfoB.size = newSize;
                quadTreeCellInfo.nodes.Add(quadTreeCellInfoB);
                //C
                QuadTreeInfo quadTreeCellInfoC = new QuadTreeInfo();
                quadTreeCellInfoC.topLeftCorner = topLeftCornerC;
                int cellPlanetsCountC = cellPlanetsCount ;//+ (int)(quadTreeCellInfo.planetsCount * quadTreeCellInfo.random.GetRandomNumberInRange(-1, 1) * distributionOfPlanetsDelta);
                count -= cellPlanetsCountC;
                quadTreeCellInfoC.planetsCount = cellPlanetsCountC;
                quadTreeCellInfoC.depth = newDepth;
                quadTreeCellInfoC.size = newSize;
                quadTreeCellInfo.nodes.Add(quadTreeCellInfoC);
                //D
                QuadTreeInfo quadTreeCellInfoD = new QuadTreeInfo();
                quadTreeCellInfoD.topLeftCorner = topLeftCornerD;
                quadTreeCellInfoD.planetsCount = count;
                quadTreeCellInfoD.depth = newDepth;
                quadTreeCellInfoD.size = newSize;
                quadTreeCellInfo.nodes.Add(quadTreeCellInfoD);
                //split
                int lastIndex = 0;
                SplitQuadTreeUntilReachMaxDepth(newDepth, quadTreeCellInfoA.planetsCount, quadTreeCellInfoA.topLeftCorner, quadTreeCellInfoA.size, quadTreeCellInfoA,
                                                visibleRectInt, maxDepthByVisual, shipValue,
                                                planetsValuesInRange.GetRange(lastIndex, quadAGeneratedPlanetsCount), currentRestriction);
                lastIndex = quadAGeneratedPlanetsCount;
                SplitQuadTreeUntilReachMaxDepth(newDepth, quadTreeCellInfoB.planetsCount, quadTreeCellInfoB.topLeftCorner, quadTreeCellInfoB.size, quadTreeCellInfoB,
                                                visibleRectInt, maxDepthByVisual, shipValue,
                                                planetsValuesInRange.GetRange(lastIndex, generatedPlanetsPerRange), currentRestriction);
                lastIndex += quadAGeneratedPlanetsCount;
                SplitQuadTreeUntilReachMaxDepth(newDepth, quadTreeCellInfoC.planetsCount, quadTreeCellInfoC.topLeftCorner, quadTreeCellInfoC.size, quadTreeCellInfoC,
                                                visibleRectInt, maxDepthByVisual, shipValue,
                                                planetsValuesInRange.GetRange(lastIndex, generatedPlanetsPerRange), currentRestriction);
                lastIndex += quadAGeneratedPlanetsCount;
                SplitQuadTreeUntilReachMaxDepth(newDepth, quadTreeCellInfoD.planetsCount, quadTreeCellInfoD.topLeftCorner, quadTreeCellInfoD.size, quadTreeCellInfoD,
                                                visibleRectInt, maxDepthByVisual, shipValue,
                                                planetsValuesInRange.GetRange(lastIndex, generatedPlanetsPerRange), currentRestriction);;
            }
        }
    }
}