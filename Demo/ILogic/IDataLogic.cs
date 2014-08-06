/*
 * 用户： newmin
 * 日期: 2013/12/3
 * 时间: 23:00
 * 
 * 修改说明：
 */

using com.mapfre.cir;
using Ops.Toolkit.DataExport;
using System.Collections;
using System.Data;
namespace com.mapfre.poi.ILogic
{
    /// <summary>
    /// Description of ICustomer.
    /// </summary>
    public interface IDataLogic
    {
        DataTable GetQueryView(string queryName, Hashtable hash, int pageSize, int currentPageIndex, out int totalCount);

        DataRow GetTotalView(string queryName, Hashtable hash);

        string GetColumnMappingString(string queryName);

    }
}
