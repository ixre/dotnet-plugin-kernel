using com.mapfre.poi.entity;
using com.mapfre.poi.ILogic;
using Ops.Cms;
using Ops.Cms.DAL;
using Ops.Data.Orm;
/*
 * 用户： newmin
 * 日期: 2013/12/3
 * 时间: 22:42
 * 
 * 修改说明：
 */
using System;
using System.Data.Extensions;

namespace com.mapfre.poi.Logic
{
	/// <summary>
	/// Description of LProduct.
	/// </summary>
	public class LProduct:DALBase,IProduct
	{
		
		public void AddProduct(Product pro)
		{
			IEntityManager<entity.Product> entityManager=new EntityManager<entity.Product>(base.db);
			entityManager.Insert(pro);
		}
		
		public Product GetProduct(int id)
		{
			Product pro=null;
			db.ExecuteReader("SELECT * FROM %PREFIX%b2c_product WHERE id=%id%".Template(Settings.DB_PREFIX,id.ToString()),rd=>
			                 {
			                 	if(rd.HasRows)
			                 	{
			                 		pro= rd.ToEntity<Product>();
			                 	}
			                 });
			return pro;
		}
		
		public Product GetProduct(string alias)
		{
			Product pro=null;
			db.ExecuteReader("SELECT * FROM %PREFIX%b2c_product WHERE alias=%alias%".Template(Settings.DB_PREFIX,alias),rd=>
			                 {
			                 	if(rd.HasRows)
			                 	{
			                 		pro = rd.ToEntity<Product>();
			                 	}
			                 });
			return pro;
		}
	}
}
