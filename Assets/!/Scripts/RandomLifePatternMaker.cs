using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace GameControlling
{
	public class RandomLifePatternMaker
	{
		public static Vector2 MinMaxAmount;
		public static Vector2 Boundaries;

		public static string GenerateRandomSeed(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			char[] seed = new char[length];
			System.Random random = new System.Random();
			for (int i = 0; i < length; i++)
				seed[i] = chars[random.Next(chars.Length)];
			return new string(seed);
		}
		public async static Task<Vector2Int[]> GenerateHashSetFromSeed(string seed)
		{
			System.Random random = new System.Random(seed.GetHashCode());
			int numberOfPoints = random.Next((int)MinMaxAmount.x, (int)MinMaxAmount.y);
			Vector2Int[] points = new Vector2Int[numberOfPoints];
			List<int> xPositions = new List<int>();
			List<int> yPositions = new List<int>();
			int xBound = GetBound(random);
			int yBound = GetBound(random);
			await Awaitable.BackgroundThreadAsync();
			xPositions.AddRange(Enumerable.Range(0, xBound));
			yPositions.AddRange(Enumerable.Range(0, yBound));
			Shuffle(xPositions, random);
			Shuffle(yPositions, random);
			int iterations = numberOfPoints >= xBound * yBound ? Mathf.Max(xBound, yBound) : numberOfPoints;
			Parallel.For(0, iterations, (i =>
			{
				int xPosition = xPositions[i % xBound];
				int yPosition = yPositions[i % yBound];
				points[i] = new Vector2Int(xPosition, yPosition);
			}));
			return points;
		}
		private static void Shuffle<T>(List<T> list, System.Random random)
		{
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = random.Next(0, n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
		private static int GetBound(System.Random random) =>
			random.Next(0, random.Next((int)Boundaries.x, (int)Boundaries.y)) + 1;
	}
}