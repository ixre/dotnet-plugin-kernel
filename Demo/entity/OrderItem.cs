/*
 * 用户： newmin
 * 日期: 2013/12/3
 * 时间: 22:22
 * 
 * 修改说明：
 */
using System;
using Ops.Data.Orm.Mapping;

namespace com.mapfre.poi.entity
{
	/// <summary>
	/// Description of OrderItem.
	/// </summary>
	[DataTable("$PREFIX_b2c_orderItem",Format=typeof(SqlFormat))]
	public class OrderItem
	{
		/// <summary>
		/// 编号
		/// </summary>
		[Column("ID",IsPrimaryKey=true,AutoGeneried=true)]
		public int Id{get;set;}
		
		/// <summary>
		/// 订单号
		/// </summary>
		[Column("OrderId")]
		public int OrderId{get;set;}
		
		/// <summary>
		/// 产品编号
		/// </summary>
		[Column("ProductId")]
		public int ProductId{get;set;}
		
		/// <summary>
		/// 数量
		/// </summary>
		[Column("Quantity")]
		public int Quantity{get;set;}
		
		/// <summary>
		/// 金额
		/// </summary>
		[Column("Fee")]
		public float Fee{get;set;}
	}
}
