using System;
using UnityEngine;

namespace Vexe.Runtime.Helpers
{
	public static class GizHelper
	{
        static ColorBlock block = new ColorBlock();

        public static IDisposable Begin(Color c)
        {
            return block.Begin(c);
        }

		public static void DrawSphere(Vector3 pos, float radius, Color c)
		{
			using(block.Begin(c))
                Gizmos.DrawSphere(pos, radius);
		}

		public static void DrawWireSphere(Vector3 pos, float radius, Color c)
		{
			using(block.Begin(c))
                Gizmos.DrawWireSphere(pos, radius);
		}

		public static void DrawCube(Vector3 pos, Vector3 size, Color c)
		{
			using(block.Begin(c))
                Gizmos.DrawCube(pos, size);
		}

		public static void DrawWireCube(Vector3 pos, Vector3 size, Color c)
		{
			using(block.Begin(c))
                Gizmos.DrawWireCube(pos, size);
		}

		public static void DrawLine(Vector3 from, Vector3 to, Color c)
		{
			using(block.Begin(c))
                Gizmos.DrawLine(from, to);
		}

		public static void DrawRay(Vector3 from, Vector3 direction, Color c)
		{
			using(block.Begin(c))
                Gizmos.DrawRay(from, direction);
		}

		public static void DrawRay(Ray ray, Color c)
		{
			using(block.Begin(c))
                Gizmos.DrawRay(ray);
		}

		public static void DrawFrustum(Vector3 center, float fov, float maxRange, float minRange, float aspect, Color c)
		{
			using(block.Begin(c))
                Gizmos.DrawFrustum(center, fov, maxRange, minRange, aspect);
		}

		public static void DrawIcon(Vector3 center, string name, Color c)
		{
			using(block.Begin(c))
                Gizmos.DrawIcon(center, name);
		}

		public static void DrawIcon(Vector3 center, string name, bool allowScaling, Color c)
		{
			using(block.Begin(c))
                Gizmos.DrawIcon(center, name, allowScaling);
		}

		public static void DrawGUITexture(Rect screenRect, Texture texture, Color c)
		{
			using(block.Begin(c))
                Gizmos.DrawGUITexture(screenRect, texture);
		}

		public static void DrawGUITexture(Rect screenRect, Texture texture, Material mat, Color c)
		{
			using(block.Begin(c))
                Gizmos.DrawGUITexture(screenRect, texture, mat);
		}

		public static void DrawGUITexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color c)
		{
			using(block.Begin(c))
                Gizmos.DrawGUITexture(screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder);
		}

		public static void DrawGUITexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Material mat, Color c)
		{
			using(block.Begin(c))
                Gizmos.DrawGUITexture(screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder, mat);
		}

        private class ColorBlock : IDisposable
        {
            private Color _original;

            public ColorBlock Begin(Color color)
            {
                _original = Gizmos.color;
                Gizmos.color = color;
                return this;
            }

            void IDisposable.Dispose()
            {
                Gizmos.color = _original;
            }
        }
	}
}
