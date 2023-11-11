using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItSoftware.Core.ID
{
	public enum ItsCreateIDOptions
	{
		LowerCase,
		UpperCase,
		LowerAndUpperCase
	}

	public static class ItsID
    {
		#region Private Static Fields
		private static Random s_rnd = new Random();
		#endregion

		/// <summary>
		/// Creates an id. Chars and Numbers.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="count">Number of chars.</param>
		/// <returns></returns>
		public static string ItsCreateID(int count, ItsCreateIDOptions options, bool includeNumbers)
		{
			if (count <= 0)
			{
				count = 16;
			}

			string dataMin = "abcdefghijklmnopqrstuvwxyz";
			string dataMaj = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			string dataMinMaj = "aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVwWxXyYzZ";
			string dataMinNum = "a0b1c2d3e4f5g6h7i8j9k0l1m2n3o4p5q6r7s8t9u0v1w2x3y4z5";
			string dataMajNum = "A0B1C2D3E4F5G6G7I8J9K0L1M2N3O4P5Q6R7S8T9U0V1W2X3Y4Z5";
			string dataMinMajNum = "aA0bB1cC2dD3eE4fF5gG6hH7iI8jJ9kK0lL1mM2nN3oO4pP5qQ6rR7sS8tT9uU0vV1wW2xX3yY4zZ5";

			string data = dataMin;
			if (options == ItsCreateIDOptions.LowerAndUpperCase && includeNumbers)
			{
				data = dataMinMajNum;
			}
			if (options == ItsCreateIDOptions.LowerAndUpperCase && !includeNumbers)
			{
				data = dataMinMaj;
			}
			else if (options == ItsCreateIDOptions.LowerCase && includeNumbers)
			{
				data = dataMinNum;
			}
			else if (options == ItsCreateIDOptions.LowerCase && !includeNumbers)
			{
				data = dataMin;
			}
			else if (options == ItsCreateIDOptions.UpperCase && includeNumbers)
			{
				data = dataMajNum;
			}
			else if (options == ItsCreateIDOptions.UpperCase && !includeNumbers)
			{
				data = dataMaj;
			}

			Random rnd = s_rnd;
			byte[] next = new byte[count];
			rnd.NextBytes(next);

			var id = new StringBuilder();
			int i = 0;
			do
			{
				int j = rnd.Next(0, data.Length - 1);
				id.Append(data[j]);

			} while (++i < count);

			return id.ToString();
		}
	}
}
