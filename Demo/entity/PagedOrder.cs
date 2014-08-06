using Ops.Data.Orm.Mapping;
/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2013/10/30
 * 时间: 17:53
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;

namespace com.mapfre.poi
{
	/// <summary>
	/// Description of Order.
	/// </summary>
	public class PagedOrder
	{
		/// <summary>
		/// 编号
		/// </summary>
		public Int32 Id{get;set;}
		
		/// <summary>
		/// 订单号
		/// </summary>
		public string OrderNo{get;set;}
		
		/// <summary>
		/// 订单金额
		/// </summary>
		[Column("TotalFee")]
		public float TotalFee{get;set;}
		
		/// <summary>
		/// 是否游客购买
		/// </summary>
		[Column("IsGuestBuy")]
		public bool IsGuestBuy{get;set;}
		
		/// <summary>
		/// 购买者编号
		/// </summary>
		[Column("BuynerId")]
		public int BuynerId{get;set;}
		
		/// <summary>
		/// 购买者姓名
		/// </summary>
		[Column("BuynerName")]
		public String BuynerName{get;set;}
		
		/// <summary>
		/// 收货地址
		/// </summary>
		[Column("BuynerAddress")]
		public String BuynerAddress{get;set;}
		
		/// <summary>
		/// 购买者手机
		/// </summary>
		[Column("BuynerPhone")]
		public String BuynerPhone{get;set;}
		
		/// <summary>
		/// 购买者邮箱
		/// </summary>
		public String BuynerEmail{get;set;}
		
		/// <summary>
		/// 身份证号码
		/// </summary>
		public string BuynerIdNum{get;set;}
		
		/// <summary>
		/// 提交时间
		/// </summary>
		[Column("SubmitTime")]
		public DateTime SubmitTime{get;set;}
		
		/// <summary>
		/// 支付时间
		/// </summary>
		[Column("PayTime")]
		public DateTime PayTime{get;set;}
		
		/// <summary>
		/// 申请时间
		/// </summary>
		[Column("ConfirmTime")]
		public DateTime ConfirmTime{get;set;}
		
		/// <summary>
		/// 发货时间
		/// </summary>
		[Column("SendTime")]
		public DateTime SendTime{get;set;}
		
		/// <summary>
		/// 订单状态
		/// </summary>
		[Column("OrderState")]
		public OrderState OrderState{get;set;}
	}
}
