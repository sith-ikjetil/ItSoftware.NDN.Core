﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ItSoftware.Core.Crypto
{
	public static class ItsHash
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		public static string HashSHA512( string password, Encoding enc )
		{			
			byte[] message = enc.GetBytes( password );

			SHA512Managed hashString = new SHA512Managed( );
			byte[] hashValue = hashString.ComputeHash( message );

			StringBuilder sBuilder = new StringBuilder( );
			for ( int i = 0; i < hashValue.Length; i++ )
			{
				sBuilder.Append( hashValue[i].ToString( "x2" ) );
			}

			return sBuilder.ToString( );
		}// HashSHA512
		/// <summary>
		/// 
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		public static string HashSHA256( string password, Encoding enc)
		{			
			byte[] message = enc.GetBytes( password );

			SHA256Managed hashString = new SHA256Managed( );
			byte[] hashValue = hashString.ComputeHash( message );

			StringBuilder sBuilder = new StringBuilder( );
			for ( int i = 0; i < hashValue.Length; i++ )
			{
				sBuilder.Append( hashValue[i].ToString( "x2" ) );
			}

			return sBuilder.ToString( );
		}// HashSHA256
		/// <summary>
		/// 
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		public static string HashSHA384( string password, Encoding enc )
		{			
			byte[] message = enc.GetBytes( password );

			SHA384Managed hashString = new SHA384Managed( );
			byte[] hashValue = hashString.ComputeHash( message );

			StringBuilder sBuilder = new StringBuilder( );
			for ( int i = 0; i < hashValue.Length; i++ )
			{
				sBuilder.Append( hashValue[i].ToString( "x2" ) );
			}

			return sBuilder.ToString( );
		}// HashSHA384
		/// <summary>
		/// 
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		public static string HashSHA1( string password, Encoding enc )
		{			
			byte[] message = enc.GetBytes( password );

			SHA1Managed hashString = new SHA1Managed( );
			byte[] hashValue = hashString.ComputeHash( message );

			StringBuilder sBuilder = new StringBuilder( );			
			for ( int i = 0; i < hashValue.Length; i++ )
			{
				sBuilder.Append( hashValue[i].ToString( "x2" ) );
			}

			return sBuilder.ToString( );
		}// HashSHA1
		/// <summary>
		/// 
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		public static string HashMD5( string password, Encoding enc)
		{			
			byte[] message = enc.GetBytes( password );

			using ( MD5 md5Hash = MD5.Create( ) )
			{			
				byte[] hashValue = md5Hash.ComputeHash( Encoding.UTF8.GetBytes( password ) );

				StringBuilder sBuilder = new StringBuilder( );
				for ( int i = 0; i < hashValue.Length; i++ )
				{
					sBuilder.Append( hashValue[i].ToString( "x2" ) );
				}
				
				return sBuilder.ToString( );
			}
		}// HashMD5
	}// class
}
