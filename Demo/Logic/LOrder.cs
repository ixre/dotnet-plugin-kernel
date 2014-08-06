using com.mapfre.poi.entity;
using com.mapfre.poi.ILogic;
using Ops.Cms;
using Ops.Cms.DAL;
using Ops.Data;
using Ops.Data.Orm;
/*
 * 用户： newmin
 * 日期: 2013/12/3
 * 时间: 23:04
 * 
 * 修改说明：
 */
using System;
using System.Collections.Generic;
using System.Data;

namespace com.mapfre.poi.Logic
{
	/// <summary>
	/// Description of LOrder.
	/// </summary>
	public class LOrder:DALBase,IOrder
	{
		
		public int CreateOrder(Order order, IList<OrderItem> items)
		{
			int orderID;
			
			IEntityManager<Order> _order=new EntityManager<Order>(base.db);
			_order.Insert(order);
			
			//返回订单号
			orderID=int.Parse(db.ExecuteScalar("SELECT MAX(id) FROM  %PREFIX%b2c_order".Template(Settings.DB_PREFIX)).ToString());
			
			SqlQuery[] querys=new SqlQuery[items.Count];
			int i=0;
			foreach(OrderItem item in items)
			{
				querys[i++]=new SqlQuery(
					"INSERT INTO %PREFIX%b2c_orderItem (orderId,productId,quantity,fee) VALUES(@orderId,@productId,@quantity,@fee)".Template(Settings.DB_PREFIX),
					new object[,]{
						{"@orderId",orderID},
						{"@productId",item.ProductId},
						{"@quantity",item.Quantity},
						{"@fee",item.Fee}
					});
			}
			
			
			base.db.ExecuteNonQuery(querys);
			
			return orderID;
		}
		
		public Order GetOrder(string orderNo)
		{
			Order order=null;
			db.ExecuteReader("SELECT * FROM %PREFIX%b2c_order WHERE orderNo='%orderNo%'".Template(Settings.DB_PREFIX,orderNo),rd=>
			                 {
			                 	if(rd.HasRows)
			                 	{
			                 		order= rd.ToEntity<Order>();
			                 	}
			                 });
			return order;
		}
		
		public IList<OrderItem> GetOrderItems(string orderNo)
		{
			throw new NotImplementedException();
		}
		
		public DataTable PagerOrderList(OrderState state, int pageSize, int currentPageIndex, out int recordCount, out int pageCount, string @orderby)
		{
			int _state=(int)state;
			pageCount=0;
			recordCount=0;
			
			const string ms_sql=@"SELECT * FROM (SELECT
									orderNo,
									o.id,
									totalFee,
									isGuestBuy,
									buynerId,
									idnum as buynerIdNum,
									email as buynerEmail,
									buynerName,
									buynerPhone,
									buynerAddress,
									submitTime,
									orderState,
									sendTime,
									confirmTime,
									payTime,
									email,
									ROW_NUMBER()OVER(ORDER BY {3}) as rowNum
									FROM
									$PREFIX_b2c_order o
									LEFT JOIN $PREFIX_b2c_guest g ON o.isGuestBuy=1 AND o.buynerId=g.id
									WHERE ({1}=-1 OR orderState={1})) t WHERE rowNum BETWEEN {2} AND ({2}+{0})";
			
			const string my_sql=@"SELECT
									orderNo,
									o.id,
									totalFee,
									isGuestBuy,
									buynerId,
									idnum as buynerIdNum,
									email as buynerEmail,
									buynerName,
									buynerPhone,
									buynerAddress,
									submitTime,
									orderState,
									sendTime,
									confirmTime,
									payTime,
									email
									FROM
									$PREFIX_b2c_order o
									LEFT JOIN $PREFIX_b2c_guest g ON o.isGuestBuy AND o.buynerId=g.id
									WHERE ({1}=-1 OR orderState={1}) ORDER BY {3} LIMIT {2},{0}
									";
			
			string condition=state==OrderState.Unkown?"":" OrderState="+_state;

			entity.SqlFormat format=new entity.SqlFormat();
			
			
			//排序规则
			if (String.IsNullOrEmpty(orderby)) orderby = String.Intern("orderNo DESC");
			
			//记录数
			recordCount = int.Parse(base.db.ExecuteScalar(
				new SqlQuery(format.Format("SELECT COUNT(0) FROM $PREFIX_b2c_Order {0}",condition.Length==0?"":"WHERE "+condition)
				            )).ToString());

			//页数
			pageCount = recordCount / pageSize;
			if (recordCount % pageSize != 0) pageCount++;

			//对当前页数进行验证
			if (currentPageIndex > pageCount&&currentPageIndex!=1)currentPageIndex= pageCount;
			if (currentPageIndex < 1) currentPageIndex = 1;

			//跳过记录数
			int skipCount = pageSize * (currentPageIndex - 1);

			//如果调过记录为0条，且为OLEDB时候，则用sql1
			string sql =base.db.DbType==DataBaseType.MySQL || base.db.DbType==DataBaseType.SQLite?my_sql:ms_sql;
			
			sql=format.Format(sql,pageSize.ToString(), _state.ToString(),skipCount.ToString(),orderby);
			
			return base.GetDataSet(new SqlQuery(sql)).Tables[0];
		}
		
		public void SetCompletePayment(string orderNo)
		{
			SqlQuery sql= new SqlQuery(
				"UPDATE %PREFIX%b2c_order SET paytime=@paytime,orderState=@state where orderNo=@orderNo".Template(Settings.DB_PREFIX),
				new object[,]{
					{"@paytime",DateTime.Now},
					{"@state",OrderState.Payed},
					{"@orderNo",orderNo}
				});
			
			base.db.ExecuteNonQuery(sql);
		}
		
		public void ConfirmOrder(string orderNo)
		{
			
		}
		
		public void SendForOrder(string orderNo)
		{
			SqlQuery sql= new SqlQuery(
				"UPDATE %PREFIX%b2c_order SET sendtime=@sendtime,orderState=@state where orderNo=@orderNo".Template(Settings.DB_PREFIX),
				new object[,]{
					{"@sendtime",DateTime.Now},
					{"@state",OrderState.Sended},
					{"@orderNo",orderNo}
				});
			
			base.db.ExecuteNonQuery(sql);
		}
		
		public void CompleteOrder(string orderNo)
		{
			SqlQuery sql= new SqlQuery(
				"UPDATE %PREFIX%b2c_order SET orderState=@state where orderNo=@orderNo".Template(Settings.DB_PREFIX),
				new object[,]{
					{"@state",OrderState.Over},
					{"@orderNo",orderNo}
				});
			
			base.db.ExecuteNonQuery(sql);
		}
		
		public void CheckOrderState()
		{
			DateTime dt=DateTime.Now.AddHours(-48);
			DateTime dt2=DateTime.Now.AddDays(-21);
			
			
			
			
			SqlQuery sql= new SqlQuery(
				"UPDATE %PREFIX%b2c_order SET orderState=@state where SubmitTime<@dt AND orderState=0".Template(Settings.DB_PREFIX),
				new object[,]{
					{"@state",OrderState.Failed},
					{"@dt",dt}
				});
			
			SqlQuery sql2= new SqlQuery(
				"UPDATE %PREFIX%b2c_order SET orderState=@state where SendTime>SubmitTime AND SendTime<@dt".Template(Settings.DB_PREFIX),
				new object[,]{
					{"@state",OrderState.Expired},
					{"@dt",dt2}
				});
			
			base.db.ExecuteNonQuery(sql,sql2);
		}
		
		public void DelOrder(int orderId)
		{
			SqlQuery sql= new SqlQuery(
				"UPDATE %PREFIX%b2c_order SET orderState=@state where id=@id".Template(Settings.DB_PREFIX),
				new object[,]{
					{"@state",OrderState.Deleted},
					{"@id",orderId}
				});
			base.db.ExecuteNonQuery(sql);
		}
	}
}
