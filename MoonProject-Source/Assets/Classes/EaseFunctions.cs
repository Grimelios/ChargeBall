using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Classes
{
	// Adapted from https://github.com/nicolausYes/easing-functions/blob/master/src/easing.cpp and
	// https://gist.github.com/cjddmut/d789b9eb78216998e95c. Note that I only implemented the easing functions I needed
	// for this particular project.
	public enum EaseTypes
	{
		Linear,
		QuadraticOut,
		QuadraticInOut,
		CubicOut,
		CubicIn
	}

	public static class EaseFunctions
	{
		public static float Ease(float t, EaseTypes type)
		{
			switch (type)
			{
				case EaseTypes.QuadraticOut: return QuadraticOut(t);
				case EaseTypes.QuadraticInOut: return QuadraticInOut(t);
				case EaseTypes.CubicIn: return EaseCubicIn(t);
				case EaseTypes.CubicOut: return EaseCubicOut(t);
			}

			// This is equivalent to EaseTypes.Linear.
			return t;
		}

		private static float QuadraticOut(float t)
		{
			return t * (2 - t);
		}

		private static float QuadraticInOut(float t)
		{
			return t < 0.5f ? 2 * t * t : t * (4 - 2 * t) - 1;
		}

		private static float EaseCubicIn(float t)
		{
			return t * t * t;
		}

		private static float EaseCubicOut(float t)
		{
			return 1 + --t * t * t;
		}
	}
}
