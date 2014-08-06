using com.mapfre.poi.entity;
using Ops.Data.Orm.Mapping;
/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2013/10/30
 * 时间: 17:54
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;

namespace com.mapfre.poi
{
	/// <summary>
	/// Description of Person.
	/// </summary>
	[DataTable("$PREFIX_b2c_Guest",Format=typeof(SqlFormat))]
	public class Guest
	{
		/// <summary>
		/// 匿名用户编号
		/// </summary>
		[Column("ID",IsPrimaryKey=true,AutoGeneried=true)]
		public int Id{get;set;}
		
		/// <summary>
		/// 身份证编号
		/// </summary>
		[Column("IdNum")]
		public String IdNum{get;set;}
		
		
		/// <summary>
		/// 姓名
		/// </summary>
		[Column("Name")]
		public string Name{get;set;}
		
		/// <summary>
		/// 手机
		/// </summary>
		[Column("Mobile")]
		public string Mobile{get;set;}
		
		/// <summary>
		/// 电话
		/// </summary>
		[Column("Tel")]
		public string Tel{get;set;}
		
		/// <summary>
		/// 邮箱
		/// </summary>
		[Column("Email")]
		public string Email{get;set;}
		
		/// <summary>
		/// 地址
		/// </summary>
		[Column("Address")]
		public string Address{get;set;}
		
		/// <summary>
		/// IP地址
		/// </summary>
		[Column("Ip")]
		public string Ip{get;set;}
		
		[Column("CreateTime")]
		public DateTime CreateTime{get;set;}
	}
}
