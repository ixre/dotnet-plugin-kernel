/*
 * 用户： newmin
 * 日期: 2013/12/4
 * 时间: 6:47
 * 
 * 修改说明：
 */
using System;
using Ops.Data;
using Spc;
using Ops.Cms;
using Ops.Cms.Conf;

namespace com.mapfre.poi.entity
{
	/// <summary>
	/// Description of OrmMapping.
	/// </summary>
	internal class SqlFormat:ISqlFormat
	{
		public string Format(string source,params string[] objs)
		{
			source=source.Replace("$PREFIX_",Settings.DB_PREFIX);
			if(objs.Length!=0){
				source=String.Format(source,objs);
			}
			return source;
		}
	}
}
