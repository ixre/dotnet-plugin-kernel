using com.mapfre.poi;
using com.mapfre.poi.ILogic;
using Ops.Data;
using Ops.Toolkit.Data.Export;
using Ops.Toolkit.DataExport;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Config = com.mapfre.poi.Config;

namespace com.mapfre.cir.Logic
{
    internal class DataLogic : IDataLogic
    {
        //private  SettingFile _sqlSettings;
        private IDictionary<string, ExportItemConfig> configDict;
        private readonly string baseDirectory;

        public DataLogic()
        {
            //this._sqlSettings = new SettingFile(AppDomain.CurrentDomain.BaseDirectory+"bin/config/query.config");
            this.configDict = new Dictionary<string, ExportItemConfig>();
            baseDirectory = Config.PluginAttrs.WorkSpace;
        }

        private ExportItemConfig GetConfigByQueryName(string queryName)
        {
            if (!this.configDict.ContainsKey(queryName))
            {
                string filePath = baseDirectory + "query/" + queryName + ".config";

                if (!File.Exists(filePath))
                {
                    throw new Exception("不包含查询:" + queryName);
                }
                ExportItemConfig cfg = ExportUtil.GetExportItemFormXml(
                    File.ReadAllText(filePath)
                    , null);

                this.configDict.Add(queryName, cfg);
            }
            return this.configDict[queryName];
        }

        public DataTable GetQueryView(string queryName, Hashtable hash, int pageSize, int currentPageIndex,
            out int totalCount)
        {
            DataBaseAccess db = Helper.DBA;

            string query = this.GetConfigByQueryName(queryName).Query;
            string queryTotal = this.GetConfigByQueryName(queryName).Total;

            //添加分页参数
            if (hash != null)
            {
                hash.Add("page_start", currentPageIndex<=0?0:(currentPageIndex - 1) * pageSize);
                hash.Add("page_end", (currentPageIndex) * pageSize);
                hash.Add("page_size", pageSize);


                //格式化
                query = query.Template(hash);
               // throw new Exception(query + "/" + currentPageIndex+"/"+pageSize);
                if (!String.IsNullOrEmpty(queryTotal))
                {
                    queryTotal = queryTotal.Template(hash);
                }
            }


            //获取分页结果
            DataTable dataTable = db.GetDataSet(query, hash).Tables[0];

            //获取统计数据
            if (!String.IsNullOrEmpty(queryTotal))
            {
                object data = db.ExecuteScalar(queryTotal, hash);
                int.TryParse(data.ToString(), out totalCount);
            }
            else
            {
                totalCount = dataTable.Rows.Count;
            }

            return dataTable;
        }

        public string GetColumnMappingString(string queryName)
        {
            return this.GetConfigByQueryName(queryName).ColumnMappingString;
        }

        public DataRow GetTotalView(string queryName, Hashtable hash)
        {
            throw new NotImplementedException();
        }
    }
}
