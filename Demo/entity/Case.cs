using com.mapfre.poi.entity;
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

namespace com.mapfre.cir
{
	/// <summary>
	/// Description of Order.
	/// </summary>
	[DataTable("$PREFIX_cir_case",Format=typeof(SqlFormat))]
	public class Case
	{
		/// <summary>
		/// 编号
		/// </summary>
		[Column("ID",IsPrimaryKey=true,AutoGeneried=true)]
		public Int32 Id{get;set;}

        /// <summary>
        /// 合同
        /// </summary>
        public string Contract { get; set; }

        /// <summary>
        /// 服务
        /// </summary>
        public string Service { get; set; }


		/// <summary>
		/// 订单号
		/// </summary>
        [Column("CaseNo")]
		public string CaseNo{get;set;}

        /// <summary>
        /// 财务编号
        /// </summary>
        [Column("CashNo")]
        public string CashNo { get; set; }

        /// <summary>
        /// 客户案件号
        /// </summary>
        [Column("CusCaseNo")]
        public string CusCaseNo { get; set; }

		/// <summary>
		/// 供应商代码
		/// </summary>
        [Column("PartnerCode")]
		public string PartnerCode{get;set;}
		
		/// <summary>
		/// 合同号
		/// </summary>
        [Column("ContractNo")]
        public string ContractNo{ get; set; }
		
		/// <summary>
		/// 案件创建时间
		/// </summary>
        [Column("CreateTime")]
		public DateTime CreateTime{get;set;}

        /// <summary>
        /// 导入时间
        /// </summary>
        [Column("ImportTime")]
        public DateTime ImportTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Column("State")]
        public int State { get; set; }

        /// <summary>
        /// 财务操作类型
        /// </summary>
        public string Cw { get; set; }

        /// <summary>
        /// 报销
        /// </summary>
        public string Bx { get; set; }

        /// <summary>
        /// 人员简称
        /// </summary>
        public string PersonName { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 用于客户端显示
        /// </summary>
        public string _Time { get; set; }
	}

    public enum CaseType
    {
        /// <summary>
        /// 等待上传照片
        /// </summary>
        WattingUpload = 0,

        /// <summary>
        /// 完成
        /// </summary>
        Complete = 1 
    }
}
