using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Classes
{
	public class SpringValue
	{
		private const float K = 0.4f;
		private const float Damping = 0.7f;

		private float velocity;

		public SpringValue(float initialValue)
		{
			value = initialValue;
			targetValue = initialValue;
		}

		public float value;
		public float targetValue;

		public void Update()
		{
			velocity += (targetValue - value) * K;
			velocity *= Damping;
			value += velocity;
		}
	}
}
