using com.mapfre.poi.Logic;
using Ops.Toolkit.Data.Export;
using Ops.Toolkit.DataExport;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace com.mapfre.poi
{
    public class ExportItem:BaseDataExportPortal
    {
        public ExportItem(string queryName, DataColumnMapping[] columns):base(columns)
        {
            this.PortalKey = queryName;
        }

        public override DataTable GetShemalAndData( Hashtable hash,out int totalCount)
        {
            return IocObject.Data.GetQueryView(
                this.PortalKey,
                hash,
                hash.ContainsKey("pageSize") ? int.Parse(hash["pageSize"].ToString()) : 100000,
                hash.ContainsKey("pageIndex") ? int.Parse(hash["pageIndex"].ToString()) : 1, out totalCount);
        }

        public override DataRow GetTotalView(Hashtable hash)
        {
            return IocObject.Data.GetTotalView(
               this.PortalKey,
                hash);
        }


        public override string PortalKey { get; set; }
    }


    public class ExportItemManager
    {
        private static IDictionary<string, IDataExportPortal> exportPortals;

        static ExportItemManager()
        {
            exportPortals = new Dictionary<string, IDataExportPortal>();
        }

        public static IDataExportPortal GetPortal(string queryName)
        {
            if (!exportPortals.Keys.Contains(queryName))
            {
                DataColumnMapping[] columns =
                    ExportUtil.GetColumnMappings(IocObject.Data.GetColumnMappingString(queryName));

                exportPortals.Add(queryName,new ExportItem(queryName,columns));
            }
            return exportPortals[queryName];
        }
    }
}