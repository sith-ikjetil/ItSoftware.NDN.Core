using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItSoftware.Core.Exception;
using System.Globalization;
using ItSoftware.Core.Crypto;
using ItSoftware.Core.Azure;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Collections;
using System.Diagnostics;

namespace ItSoftware.Core.Extensions
{
	/// <summary>
	/// enum: ItsWidthExpandDirection
	/// </summary>
	public enum ItsWidthExpandDirection
	{
		Left,
		Middle,
		Right
	}

	

	public class ItsApplyTagTemplateResult : List<Dictionary<string, List<string>>>
	{

	}

	/// <summary>
	/// class: ItsExtensions
	/// Extensions methods.
	/// </summary>
	public static class ItsExtensions
	{
		#region Private Static Fields
		private static Random s_rnd = new Random();
		#endregion

		#region ItsRenderException
		/// <summary>
		/// Formats an exception to string
		/// </summary>
		/// <param name="exception"></param>
		/// <returns></returns>
		public static string ItsRenderException(this System.Exception exception)
		{
			StringBuilder output = new StringBuilder();
			output.AppendLine();
			output.AppendLine("#####################################");
			output.AppendLine(string.Format("## Timestamp: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff").Replace('T', ' ')));
			output.AppendLine();

			output.AppendLine("#####################################");
			output.AppendLine("## Environment");
			output.AppendLine("##");
			output.AppendLine(string.Format("Machine Name: {0}", Environment.MachineName));
			output.AppendLine(string.Format("Current Directory: {0}", Environment.CurrentDirectory));
			output.AppendLine(string.Format("Is 64 Bit Operating System: {0}", Environment.Is64BitOperatingSystem));
			output.AppendLine(string.Format("Is 64 Bit Process: {0}", Environment.Is64BitProcess));
			output.AppendLine(string.Format("OS Version: {0}", Environment.OSVersion));
			output.AppendLine(string.Format("Processor Count: {0}", Environment.ProcessorCount));
			output.AppendLine(string.Format("CLR Version: {0}", Environment.Version));
			output.AppendLine();

			output.AppendLine("#####################################");
			output.AppendLine("## Summary");
			output.AppendLine("##");
			output.Append(ItsExtensions.ItsRenderExceptionShort(exception));
			output.AppendLine();

			output.Append(ItsExceptionRenderExtension.Render(exception));

			ItsExtensions.RenderExceptionHelper(output, exception, false);

			return output.ToString();
		}

		/// <summary>
		/// Helper Method.
		/// </summary>
		/// <param name="output"></param>
		/// <param name="exception"></param>
		private static void RenderExceptionHelper(StringBuilder output, System.Exception exception, bool inner)
		{
			output.AppendLine("#####################################");
			output.AppendLine((inner) ? "## Inner Exception" : "## Exception");
			output.AppendLine("##");
			output.AppendLine(string.Format("Full Name: {0}", exception.GetType().FullName));
			output.AppendLine(string.Format("Message: {0}", exception.Message ?? "<NULL>"));
			output.AppendLine(string.Format("Source: {0}", exception.Source ?? "<NULL>"));
			output.AppendLine(string.Format("InnerException: {0}", exception.InnerException?.GetType().FullName ?? "<NULL>"));
			output.AppendLine(string.Format("Stack Trace: {0}", exception.StackTrace ?? "<NULL>"));
			if (exception.TargetSite != null)
			{
				output.AppendLine(string.Format("Target Site: {0}", exception.TargetSite.Name ?? "<NULL>"));
			}
			output.AppendLine(string.Format("Help Link: {0}", exception.HelpLink ?? "<NULL>"));
			if (exception.Data != null)
			{
				if ( exception.Data.Keys != null )
				{
					foreach ( object key in exception.Data.Keys )
					{
						output.AppendLine( string.Format( "{0}: {1}", key.ToString(), exception.Data[key]?.ToString() ?? "<NULL>" ) );
					}
				}
			}
			output.AppendLine();
			
			if (exception.InnerException != null)
			{
				RenderExceptionHelper(output, exception.InnerException, true);
			}
		}
		#endregion

		#region ItsRenderExceptionShort
		public static string ItsRenderExceptionShort(this System.Exception x)
		{
			if (x == null)
			{
				return string.Empty;
			}

			var msg = new StringBuilder();

			System.Exception y = x;
			do
			{
				msg.AppendLine(y.GetType().FullName);
				msg.AppendLine(y.Message);
				msg.AppendLine();

				y = y.InnerException;
			} while (y != null);

			return msg.ToString();
		}
		#endregion

		#region ItsStringBetweenStrings
		/// <summary>
		/// Finds the first string that lies in source between substr1 and substr2 or between
		/// substr2 and substr1.
		/// </summary>
		/// <param name="source">String to search in.</param>
		/// <param name="substr1">String to find between.</param>
		/// <param name="substr2">String to find between.</param>
		/// <returns></returns>
		public static string ItsStringBetweenStrings(this string source, string substr1, string substr2)
		{
			if (string.IsNullOrEmpty(source))
			{
				throw new ArgumentNullException("source");
			}
			if (string.IsNullOrEmpty(substr1))
			{
				throw new ArgumentNullException("substr1");
			}
			if (string.IsNullOrEmpty(substr2))
			{
				throw new ArgumentNullException("substr2");
			}

			int index1 = source.IndexOf(substr1);
			if (index1 == -1)
			{
				throw new ItsException<ItsStringExceptionArgs>("String.ItsStringBetweenStrings failed. Cannot find substring 1.");
			}

			int index2 = source.IndexOf(substr2, index1 + substr1.Length);
			if (index2 == -1)
			{
				throw new ItsException<ItsStringExceptionArgs>("String.ItsStringBetweenStrings failed. Cannot find substring 2.");
			}

			return source.Substring(index1 + substr1.Length, (index2 - index1 - substr1.Length));
		}
		#endregion

		#region ItsApplyTagTemplate
		/// <summary>
		/// Applies a tag-template on a source string. The template defines tags that specify
		/// what information to retrieve. The tag template must begin and end with known 
		/// characters. Known characters must also be in-between all tags. Example of a
		/// tag pattern: knownchar(s){{heading}}knownchar(s){{text}}knownchar(s){{url}}knownchar(s)
		/// The function returns an List of NameValueCollection. Each NameValueCollection consist of the
		/// the tags with their respective found values. If more than one pattern is found
		/// then the arraylist consist of several NameValueCollection each with their respective
		/// found values.
		/// </summary>
		/// <param name="source">Source string to search through</param>
		/// <param name="tagTemplate">Tag template string.</param>
		/// <param name="preTag">Tag start string.</param>
		/// <param name="postTag">Tag end string.</param>
		/// <returns></returns>
		public static ItsApplyTagTemplateResult ItsApplyTagTemplate(this string source, string tagTemplate, string preTag, string postTag)
		{
			if (string.IsNullOrEmpty(source) || string.IsNullOrWhiteSpace(source))
			{
				throw new ArgumentException("source");
			}
			if (string.IsNullOrEmpty(tagTemplate) || string.IsNullOrWhiteSpace(tagTemplate))
			{
				throw new ArgumentException("tagTemplate");
			}
			if (string.IsNullOrEmpty(preTag) || string.IsNullOrWhiteSpace(preTag))
			{
				throw new ArgumentException("preTag");
			}
			if (string.IsNullOrEmpty(postTag) || string.IsNullOrWhiteSpace(postTag))
			{
				throw new ArgumentException("postTag");
			}

			List<string> listTags = new List<string>(); // {{tag}}
			List<string> listtagTemplate = new List<string>();
			int indexPre = ItsFind(tagTemplate, preTag);
			if (indexPre == -1)//|| indexPre == 0) // Cannot be 0 because we need a value before the first tag
				return new ItsApplyTagTemplateResult();

			if (indexPre != 0)
			{
				listtagTemplate.Add(tagTemplate.Substring(0, indexPre));//+1
			}
			else
			{
				listtagTemplate.Add(string.Empty);
			}

			int indexPost = 0;
			while (indexPre != -1)
			{
				indexPost = ItsFind(tagTemplate, postTag, indexPre + preTag.Length);
				if (indexPost == -1)
					return new ItsApplyTagTemplateResult( );

				listTags.Add(tagTemplate.Substring(indexPre + preTag.Length, indexPost - indexPre - preTag.Length));
				indexPre = ItsFind(tagTemplate, preTag, indexPost + postTag.Length);
				if (indexPre != -1)
				{
					// We must have som values between the tags
					if (indexPre == indexPost + postTag.Length)
						return new ItsApplyTagTemplateResult( );

					listtagTemplate.Add(tagTemplate.Substring(indexPost + postTag.Length, indexPre - indexPost - postTag.Length));
				}
			}

			if (tagTemplate.Length - indexPost - postTag.Length != 0)
			{
				listtagTemplate.Add(tagTemplate.Substring(indexPost + postTag.Length, tagTemplate.Length - indexPost - postTag.Length));
			}
			else
			{
				listtagTemplate.Add(string.Empty);
			}

			if (listtagTemplate.Count == 0 && listTags.Count > 1)
			{
				throw new ArgumentException("Invalid tag template");
			}

			if (listtagTemplate.Count == 2 && listTags.Count == 1 && listtagTemplate[0].Length == 0 && listtagTemplate[1].Length == 0)
			{
				var retVal = new ItsApplyTagTemplateResult( );

				List<string> listRetVal = new List<string>();
				listRetVal.Add(source);

				var dictRetVal = new Dictionary<string, List<string>>();
				dictRetVal.Add(listTags[0], listRetVal);

				retVal.Add(dictRetVal);

				return retVal;
			}

			var listNameValueCollection = new ItsApplyTagTemplateResult( );
			int startIndex = 0;
			do
			{
				bool IsAllFound = false;

				int[] indextagTemplate = new int[listtagTemplate.Count];
				
				indextagTemplate[0] = source.IndexOf(((string)listtagTemplate[0]), startIndex);
				if (indextagTemplate[0] == -1)
					break;

				for (int i = 1; i < listtagTemplate.Count; i++)
				{
					int index = indextagTemplate[i - 1] + ((string)listtagTemplate[i - 1]).Length;
					if (index >= source.Length)
					{
						// We are at end of source string so lets quit
						IsAllFound = true;  // Make sure we break out of it
						break;
					}
				
					// Check empty string (end of source then)
					if (listtagTemplate[i].Length == 0)
					{
						indextagTemplate[i] = source.Length;
					}
					else
					{
						indextagTemplate[i] = source.IndexOf(((string)listtagTemplate[i]), index);
						if (indextagTemplate[i] == -1)
						{
							IsAllFound = true;
							break;
						}
					}
				}
				if (IsAllFound)
					break;

				startIndex = indextagTemplate[listtagTemplate.Count - 1] + ((string)listtagTemplate[listtagTemplate.Count - 1]).Length;

				// Now extract values				
				var nvc = new Dictionary<string, List<string>>();
				for (int j = 0; j < listTags.Count; j++)
				{

					int length = length = indextagTemplate[j + 1] - indextagTemplate[j] - ((string)listtagTemplate[j]).Length;
					int index = indextagTemplate[j] + ((string)listtagTemplate[j]).Length;
					if (length < 0)
						break;

					string tagValue = source.Substring(index, length);
					if (nvc.ContainsKey(listTags[j].ToString()))
					{
						var list = nvc[listTags[j]];
						list.Add(tagValue);
					}
					else
					{
						nvc.Add(listTags[j].ToString(), new List<string>() { tagValue });
					}

					//string tagValue = source.Substring(startIndex, source.Length-startIndex );
					//if (nvc.ContainsKey(listTags[j].ToString()))
					//{
					//    var list = nvc[listTags[j]];
					//    list.Add(tagValue);
					//}
					//else
					//{
					//    nvc.Add(listTags[j].ToString(), new List<string>() { tagValue });
					//}

					//startIndex = source.Length;

				}
				listNameValueCollection.Add(nvc);

			} while (startIndex < source.Length);

			return listNameValueCollection;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		/// <param name="tagTemplate"></param>
		/// <param name="preTag"></param>
		/// <param name="postTag"></param>
		/// <returns></returns>
		public static ItsApplyTagTemplateResult ItsApplyTagTemplate(this string source, IEnumerable<string> tagTemplate, string preTag, string postTag)
		{
			if (tagTemplate == null)
			{
				throw new ArgumentNullException("tagTemplate");
			}

			var retVal = new ItsApplyTagTemplateResult( );

			foreach (var tt in tagTemplate)
			{
				var result = source.ItsApplyTagTemplate(tt, preTag, postTag);
				foreach (var obj in result)
				{
					retVal.Add(obj);
				}
			}

			return retVal;
		}
		#endregion

		#region String Methods
		/// <summary>
		/// Finds first occurence of substr in source.
		/// </summary>
		/// <param name="source">String to search in.</param>
		/// <param name="substr">String to find.</param>
		/// <returns></returns>
		public static int ItsFind(this string source, string substr)
		{
			return source.IndexOf(substr);
		}

		/// <summary>
		/// Finds the first occurence of substr in source starting at
		/// startIndex postition.
		/// </summary>
		/// <param name="source">String to search in.</param>
		/// <param name="substr">String to find.</param>
		/// <param name="startIndex">Index in source to start searching from.</param>
		/// <returns></returns>
		public static int ItsFind(this string source, string substr, int startIndex)
		{
			return source.IndexOf(substr, startIndex);
		}

		/// <summary>
		/// Finds the next occurence of substr in source starting at position
		/// indexLastFind.
		/// </summary>
		/// <param name="source">String to search in.</param>
		/// <param name="substr">String to find.</param>
		/// <param name="indexLastFind">Location to start search from.</param>
		/// <returns>The index of the nest occurence of the string substr 
		/// in source, or -1 if not found.</returns>
		public static int ItsFindNext(this string source, string substr, int indexLastFind)
		{
			if ((indexLastFind + substr.Length) >= source.Length)
				return -1;

			return source.IndexOf(substr, indexLastFind + substr.Length);
		}

		/// <summary>
		/// Finds the amount of occurences substr is found in source.
		/// </summary>
		/// <param name="source">String to search in.</param>
		/// <param name="substr">String to find.</param>
		/// <returns>The number of times the string is found.</returns>
		public static int ItsFindCount(this string source, string substr)
		{
			int count = 0;
			int index = ItsFind(source, substr);
			while (index != -1)
			{
				count++;
				index = ItsFindNext(source, substr, index);
			}
			return count;
		}
		/// <summary>
		/// Removes all occurences of substr in source.
		/// </summary>
		/// <param name="source">Source string.</param>
		/// <param name="substr">String to remove.</param>
		/// <returns></returns>
		public static string ItsRemove(this string source, string substr)
		{
			int index = source.IndexOf(substr);
			if (index == -1)
				return source;  // does no exist then ok, nothing to remove

			return ItsRemove(source.Remove(index, substr.Length), substr);
		}
		
		/// <summary>
		/// Splits the string source using the string token and
		/// returnes an array of strings
		/// </summary>
		/// <param name="source">String to split.</param>
		/// <param name="token">String to act like a split token.</param>
		/// <returns>Array of strings.</returns>
		public static string[] ItsSplit(this string source, string token)
		{
			if (source == null || source.Length == 0)
				return new string[0];

			if (token == null || token.Length == 0)
				return new string[0];

			int count = ItsFindCount(source, token);
			if (count == 0)
				return new string[0];

			string[] splits = new string[count + 1];    //abc;abc;abc;abc
			int index = ItsFind(source, token);
			int indexLast = 0;
			int countIndex = 0;
			while (index != -1 && countIndex < count)
			{
				splits[countIndex++] = source.Substring(indexLast, index - indexLast);
				indexLast = index + token.Length;
				index = ItsFindNext(source, token, index);
			}
			splits[count] = source.Substring(indexLast, source.Length - indexLast);
			return splits;
		}
		#endregion

		#region ItsTo/FromBase64
		public static string ItsToBase64(this string source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			if (source.Length == 0)
			{
				return string.Empty;
			}

			byte[] arrayb64 = Encoding.Unicode.GetBytes(source);
			return Convert.ToBase64String(arrayb64);
		}
		public static string ItsToBase64( this byte[] source )
		{
			if ( source == null )
			{
				throw new ArgumentNullException( "source" );
			}

			if ( source.Length == 0 )
			{
				return string.Empty;
			}
			
			return Convert.ToBase64String( source );
		}

		public static string ItsFromBase64ToString(this string source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			if (source.Length == 0)
			{
				return string.Empty;
			}
			byte[] normalBytes = Convert.FromBase64String(source);
			return Encoding.Unicode.GetString(normalBytes);
		}
		public static byte[] ItsFromBase64ToByteArray( this string source )
		{
			if ( source == null )
			{
				throw new ArgumentNullException( "source" );
			}

			if ( source.Length == 0 )
			{
				return new byte[0];
			}

			return Convert.FromBase64String( source );			
		}
		#endregion

		#region ItsWidthExpand
		public static string ItsWidthExpand(this string source, int width, char fill, ItsWidthExpandDirection direction)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			if (source.Length >= width)
			{
				return source.Substring(0, width);
			}

			var result = new StringBuilder();

			if (direction == ItsWidthExpandDirection.Left)
			{
				for (int i = 0; i < (width - source.Length); i++)
				{
					result.Append(fill);
				}
				result.Append(source);
			}
			else if (direction == ItsWidthExpandDirection.Middle)
			{
				for (int i = 0; i < ((width - source.Length) / 2); i++)
				{
					result.Append(fill);
				}

				result.Append(source);

				for (int i = result.Length; i < width; i++)
				{
					result.Append(fill);
				}
			}
			else if (direction == ItsWidthExpandDirection.Right)
			{
				result.Append(source);

				for (int i = 0; i < (width - source.Length); i++)
				{
					result.Append(fill);
				}
			}

			return result.ToString();
		}
		#endregion

		#region ItsToDataSizeString
		public static string ItsToDataSizeString(this int size, int digits)
		{
			return ItsExtensions.ItsToDataSizeStringHelper(Convert.ToDecimal(size), digits);
		}

		public static string ItsToDataSizeString(this int size, int digits, CultureInfo ci)
		{
			return ItsExtensions.ItsToDataSizeStringHelper(Convert.ToDecimal(size), digits, ci);
		}

		public static string ItsToDataSizeString(this uint size, int digits)
		{
			return ItsExtensions.ItsToDataSizeStringHelper(Convert.ToDecimal(size), digits);
		}

		public static string ItsToDataSizeString(this uint size, int digits, CultureInfo ci)
		{
			return ItsExtensions.ItsToDataSizeStringHelper(Convert.ToDecimal(size), digits, ci);
		}

		public static string ItsToDataSizeString(this long size, int digits)
		{
			return ItsExtensions.ItsToDataSizeStringHelper(Convert.ToDecimal(size), digits);
		}

		public static string ItsToDataSizeString(this long size, int digits, CultureInfo ci)
		{
			return ItsExtensions.ItsToDataSizeStringHelper(Convert.ToDecimal(size), digits, ci);
		}

		public static string ItsToDataSizeString(this ulong size, int digits)
		{
			return ItsExtensions.ItsToDataSizeStringHelper(Convert.ToDecimal(size), digits);
		}

		public static string ItsToDataSizeString(this ulong size, int digits, CultureInfo ci)
		{
			return ItsExtensions.ItsToDataSizeStringHelper(Convert.ToDecimal(size), digits, ci);
		}

		public static string ItsToDataSizeString(this decimal dSize, int digits)
		{
			return ItsExtensions.ItsToDataSizeStringHelper(dSize, digits);
		}

		public static string ItsToDataSizeString(this decimal dSize, int digits, CultureInfo ci)
		{
			return ItsExtensions.ItsToDataSizeStringHelper(dSize, digits, ci);
		}

		private static string ItsToDataSizeStringHelper(decimal dSize, int digits)
		{
			if (digits < 0)
			{
				digits = 0;
			}
			else if (digits > 3)
			{
				digits = 3;
			}

			var text = new StringBuilder();

			int index = 0;			
			while (dSize >= 1024)
			{
				dSize /= 1024;
				index++;
			}

			string[] szSize = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB", "BB", "GP" };

			text.Append(dSize.ToString($"F{digits}"));
			text.Append(" ");
			text.Append(((index > (szSize.Length-1) || index < 0) ? "?" : szSize[index]));
			return text.ToString();
		}

		private static string ItsToDataSizeStringHelper(decimal dSize, int digits, CultureInfo ci)
		{
			if (digits < 0)
			{
				digits = 0;
			}
			else if (digits > 3)
			{
				digits = 3;
			}

			var text = new StringBuilder();

			int index = 0;			
			while (dSize >= 1024)
			{
				dSize /= 1024;
				index++;
			}

			string[] szSize = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB", "BB", "GP" };

			text.Append(dSize.ToString($"F{digits}", ci));
			text.Append(" ");
			text.Append(((index > (szSize.Length-1) || index < 0) ? "?" : szSize[index]));
			return text.ToString();
		}
		#endregion

		#region ItsForEach
		public static void ItsForEach<T>(this IEnumerable<T> items, Action<T> action)
		{
			foreach (var item in items)
			{
				action(item);
			}
		}
		#endregion

		#region ItsApplyTagTemplate LINQ Extensions Enumerable
		public static IEnumerable<Dictionary<string, List<string>>> ItsApplyTagTemplate(this IEnumerable<string> items, string tagTemplate, string preTag, string postTag)
		{
			var result = new List<Dictionary<string, List<string>>>();
			foreach (var item in items)
			{
				result.AddRange(item.ItsApplyTagTemplate(tagTemplate, preTag, postTag));
			}
			return result;
		}
		public static IEnumerable<Dictionary<string, List<string>>> ItsApplyTagTemplate(this IEnumerable<string> items, IEnumerable<string> tagTemplate, string preTag, string postTag)
		{
			var result = new List<Dictionary<string, List<string>>>();
			foreach (var item in items)
			{
				result.AddRange(item.ItsApplyTagTemplate(tagTemplate, preTag, postTag));
			}
			return result;
		}
		#endregion

		#region ItsRenderTimeSpan
		public static string ItsRenderTimeSpan(this TimeSpan ts, bool includeMilliseconds)
		{
			StringBuilder time = new StringBuilder( );

			if ( ts.Days > 0 )
			{
				long years = ts.Days / 365;
				long days = ts.Days - (years * 365);

				if ( years > 0 )
				{
					if ( years == 1 )
					{
						time.Append( $"{years} year " );
					}
					else
					{
						time.Append( $"{years} years " );
					}
				}

				if ( days > 0 )
				{
					if ( days == 1 )
					{
						time.Append( $"{days} day " );
					}
					else
					{
						time.Append( $"{days} days " );
					}
				}
			}

			if ( ts.Hours > 0 )
			{
				if ( ts.Hours == 1 )
				{
					time.Append( $"{ts.Hours} hour " );
				}
				else
				{
					time.Append( $"{ts.Hours} hours " );
				}
			}

			time.Append( $"{ts.Minutes.ToString("D2")}:{ts.Seconds.ToString("D2")}" );
			if ( includeMilliseconds )
			{
				time.Append( $".{ts.Milliseconds.ToString( "D3" )}" );
			}

			return time.ToString( );
		}
		#endregion

		#region ItsHashXxx
		/// <summary>
		/// Hash to MD5.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ItsHashMD5( this string str, Encoding enc )
		{
			return ItsHash.HashMD5( str, enc );
		}
		/// <summary>
		/// Hash To SHA1
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ItsHashSHA1( this string str, Encoding enc )
		{
			return ItsHash.HashSHA1( str, enc );
		}
		/// <summary>
		/// Hash to SHA256
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ItsHashSHA256(this string str, Encoding enc)
		{
			return ItsHash.HashSHA256( str, enc );
		}
		/// <summary>
		/// Hash to SHA384
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ItsHashSHA384( this string str, Encoding enc )
		{
			return ItsHash.HashSHA384( str, enc );
		}
		/// <summary>
		/// Hash to SHA512
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ItsHashSHA512( this string str, Encoding enc )
		{
			return ItsHash.HashSHA512( str, enc );
		}
		#endregion

		#region ItsToHttpXxxResponseMessage
		public static HttpResponseMessage ItsToHttpHtmlResponseMessage(this string payload)
		{
			var result = new AzureFunctionResult(payload);
			return result.AsHtml();
		}
		public static HttpResponseMessage ItsToHttpXmlResponseMessage(this string payload)
		{
			var result = new AzureFunctionResult(payload);
			return result.AsXml();
		}
		public static HttpResponseMessage ItsToHttpTextResponseMessage(this string payload)
		{
			var result = new AzureFunctionResult(payload);
			return result.AsText();
		}
		public static HttpResponseMessage ItsToHttpJsonResponseMessage(this string payload)
		{
			var result = new AzureFunctionResult(payload);
			return result.AsJson();
		}
		#endregion

		#region ItsRegExMatches
		public static Match ItsRegExPatternMatch(this string input, string pattern)
		{
			return Regex.Match(input, pattern);
		}
		public static string[] ItsRegExPatternMatchAsArray(this string input, string pattern)
		{
			var match = Regex.Match(input, pattern);

			var result = new List<string>();
			foreach (Group g in match.Groups)
			{
				result.Add(g.Value);
			}

			return result.ToArray();
		}
		public static MatchCollection ItsRegExPatternMatches(this string input, string pattern)
		{
			return Regex.Matches(input, pattern);
		}
		public static string[] ItsRegExPatternMatchesAsArray(this string input, string pattern)
		{
			var matches = Regex.Matches(input, pattern);

			var result = new List<string>();
			foreach (Match m in matches)
			{
				foreach (Group g in m.Groups)
				{
					result.Add(g.Value);
				}
			}

			return result.ToArray();
		}
		public static string[] ItsRegExPatternMatchesGroupAsArray(this string input, string groupId, string pattern)
		{
			var match = Regex.Matches(input, pattern);

			var result = new List<string>();
			foreach (Match m in match)
			{
				string tmp = m.Groups[groupId]?.Value ?? null;
				if (!string.IsNullOrEmpty(tmp))
				{
					result.Add(tmp);
				}
			}

			return result.ToArray();
		}
		public static Match ItsRegExInputMatch(this string pattern, string input)
		{
			return Regex.Match(input, pattern);
		}
		public static string[] ItsRegExInputMatchAsArray(this string pattern, string input)
		{
			var match = Regex.Match(input, pattern);

			var result = new List<string>();
			foreach (Group g in match.Groups)
			{
				result.Add(g.Value);
			}

			return result.ToArray();
		}
		public static MatchCollection ItsRegExInputMatches(this string pattern, string input)
		{
			return Regex.Matches(input, pattern);
		}
		public static string[] ItsRegExInputMatchesAsArray(this string pattern, string input)
		{
			var matches = Regex.Matches(input, pattern);

			var result = new List<string>();
			foreach (Match m in matches)
			{
				foreach (Group g in m.Groups)
				{
					result.Add(g.Value);
				}
			}

			return result.ToArray();
		}
		public static string[] ItsRegExInputMatchesGroupAsArray(this string pattern, string groupId, string input)
		{
			var match = Regex.Matches(input, pattern);

			var result = new List<string>();
			foreach (Match m in match)
			{
				string tmp = m.Groups[groupId]?.Value ?? null;
				if (!string.IsNullOrEmpty(tmp))
				{
					result.Add(tmp);
				}
			}

			return result.ToArray();
		}
		#endregion

		#region ItsGetStopwatchXxxSeconds
		public static long ItsGetStopWatchNanoSeconds(this Stopwatch s)
		{
			long frequency = Stopwatch.Frequency;
			long nanosecPerTick = (1000L * 1000L * 1000L) / frequency;
			return s.ElapsedTicks * nanosecPerTick;
		}
		public static long ItsGetStopWatchMicroSeconds(this Stopwatch s)
		{
			long frequency = Stopwatch.Frequency;
			long nanosecPerTick = (1000L * 1000L * 1000L) / frequency;
			return s.ElapsedTicks * nanosecPerTick / 1_000L;
		}
		public static long ItsGetStopWatchMilliSeconds(this Stopwatch s)
		{
			long frequency = Stopwatch.Frequency;
			long nanosecPerTick = (1000L * 1000L * 1000L) / frequency;
			return s.ElapsedTicks * nanosecPerTick / 1_000_000L;
		}
		#endregion

		#region ItsRandom
		public static T ItsRandom<T>(this ICollection<T> list)
        {
			if ( list == null )
            {
				return default(T);
            }

			if ( list.Count == 1 )
            {
				return list.ElementAt(0);
            }
			
			return list.ElementAt(s_rnd.Next(0, list.Count));
        }
		#endregion
	}// class
}// namespace
