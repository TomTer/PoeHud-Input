namespace PoeHUD.Framework
{
	public struct RectUV
	{
		public float U1;
		public float V1;
		public float U2;
		public float V2;
		public RectUV(float u1, float v1, float u2, float v2)
		{
			U1 = u1;
			V1 = v1;
			U2 = u2;
			V2 = v2;
		}
		public RectUV(float w, float h)
		{
			this = new RectUV(0, 0, w, h);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is RectUV && Equals((RectUV) obj);
		}
	
		public bool Equals(RectUV other)
		{
			return U1.Equals(other.U1) && V1.Equals(other.V1) && U2.Equals(other.U2) && V2.Equals(other.V2);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = U1.GetHashCode();
				hashCode = (hashCode*397) ^ V1.GetHashCode();
				hashCode = (hashCode*397) ^ U2.GetHashCode();
				hashCode = (hashCode*397) ^ V2.GetHashCode();
				return hashCode;
			}
		}
		public override string ToString()
		{
			return string.Concat(new object[] { "[", U1, ", ", V1, ", ", U2, ", ", V2, "]" });
		}
	}
}