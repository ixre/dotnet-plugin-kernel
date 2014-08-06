/*
 * 用户： newmin
 * 日期: 2013/12/3
 * 时间: 22:12
 * 
 * 修改说明：
 */
using System;
using Ops.Data.Orm.Mapping;

namespace com.mapfre.poi.entity
{
	/// <summary>
	/// Description of Product.
	/// </summary>
	[DataTable("$PREFIX_b2c_Product",Format=typeof(SqlFormat))]
	public class Product
	{
		/// <summary>
		/// 商品编号
		/// </summary>
		[Column("ID",IsPrimaryKey=true,AutoGeneried=true)]
		public int Id{get;set;}
		
		/// <summary>
		/// 商品名称
		/// </summary>
		[Column("Name")]
		public string Name{get;set;}
		
		/// <summary>
		/// 别名
		/// </summary>
		[Column("Alias")]
		public string Alias{get;set;}
		
		/// <summary>
		/// 市场价
		/// </summary>
		[Column("MarketPrice")]
		public string MarketPrice{get;set;}
		
		/// <summary>
		/// 价格
		/// </summary>
		[Column("Price")]
		public float Price{get;set;}
		
		/// <summary>
		/// 信息关联编号，这通常是一个文档编号
		/// </summary>
		[Column("RelationId")]
		public int RelationId{get;set;}
	}
}
