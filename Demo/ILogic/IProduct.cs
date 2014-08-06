/*
 * 用户： newmin
 * 日期: 2013/12/3
 * 时间: 22:35
 * 
 * 修改说明：
 */
using System;

namespace com.mapfre.poi.ILogic
{
	/// <summary>
	/// Description of IProduct.
	/// </summary>
	public interface IProduct
	{
		void AddProduct(entity.Product pro);
		
		/// <summary>
		/// 获取产品
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		entity.Product GetProduct(int id);
		
		/// <summary>
		/// 获取产品
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		entity.Product GetProduct(string alias);
	}
}
