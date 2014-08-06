/*
 * 用户： newmin
 * 日期: 2013/12/3
 * 时间: 23:03
 * 
 * 修改说明：
 */

using System.Collections;
using com.mapfre.cir;
using com.mapfre.poi.ILogic;
using Ops.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace com.mapfre.poi.Logic
{
    /// <summary>
    /// Description of LCustomer.
    /// </summary>
    public class CaseLogic : ICaseLogic
    {
        public int InsertCase(cir.Case _case)
        {
            DataBaseAccess dba = Helper.DBA;
            int hasCount = int.Parse(dba.ExecuteScalar(
                new SqlQuery("SELECT count(0) FROM cir_case WHERE caseNo = @caseNo",
                    new object[,]{
                        {"@caseNo",_case.CaseNo}
                    })).ToString());

            if (hasCount != 0)
            {
                throw new Exception("案件号:" + _case.CaseNo + "已经存在！");
            }

            dba.ExecuteNonQuery(new SqlQuery(@"INSERT INTO [cir_case]
		([caseNo]
		,[partnerCode]
		,[contractNo]
		,[createTime]
		,[importTime]
		,[state])
	VALUES
		(@caseNo
		,@partnerCode
		,@contractNo
		,@createTime
		,@importTime
		,@state)",
                 new object[,]{
                       {"@caseNo",_case.CaseNo},
		{"@partnerCode",_case.PartnerCode},
		{"@contractNo",_case.ContractNo},
		{"@createTime",_case.CreateTime},
		{"@importTime",_case.ImportTime},
		{"@state",_case.State}
                 }));


            return int.Parse(dba.ExecuteScalar(
                 new SqlQuery("SELECT id FROM cir_case WHERE caseNo = @caseNo",
                     new object[,]{
                        {"@caseNo",_case.CaseNo}
                    })).ToString());
        }


        public Case GetCaseByPartnerCodeAndCashNo(string partnerCode, string cashNo)
        {
            Case entity = null;
            Helper.DBA.ExecuteReader(
                new SqlQuery(
                    @"SELECT [id]
                        ,[cashNo]
                        ,[cusCaseNo]
		                ,[caseNo]
		                ,[partnerCode]
		                ,[contractNo]
		                ,[createTime]
		                ,[importTime]
		                ,[state]
	                    FROM [cir_case]
	                    where cashNo=@cashNo And partnerCode=@partnerCode",

                    new object[,]
                    {
                        {"@cashNo", cashNo},
                        {"@partnerCode", partnerCode}
                    }), rd =>
                    {
                        if (rd.HasRows)
                        {
                            entity = rd.ToEntity<Case>();
                        }
                    });
            return entity;
        }


        private bool SaveCaseState(int caseId, int stateCode)
        {
            return Helper.DBA.ExecuteNonQuery(new SqlQuery(
                "UPDATE cir_case Set [state]=@state where id=@caseId",
                new object[,]
                {
                    {"@state", stateCode},
                    {"@caseId", caseId}
                }
                )) == 1;
        }

        public int SaveImagesForCase(int caseId, int imgType, string[] imgList)
        {
            SqlQuery[] querys = new SqlQuery[imgList.Length];

            const string sql = @"INSERT INTO [cir_case_images]
		([caseId]
		,[imgUrl]
        ,[imgType]
		,[createTime])
	VALUES
		(@caseId
		,@imgUrl
        ,@imgType
		,@createTime)";

            DateTime time = DateTime.Now;

            int i = 0;
            foreach (string img in imgList)
            {
                querys[i++] = new SqlQuery(sql,
                    new object[,]
                        {
                            {"@caseId", caseId},
                            {"@imgUrl", img},
                            {"@imgType", imgType},
                            {"@createTime", time}
                        });
            }

            return Helper.DBA.ExecuteNonQuery(querys);
        }


        public bool DelCase(int caseId)
        {
            return this.SaveCaseState(caseId, -2);
        }

        public bool CancelCase(int caseId)
        {
            return this.SaveCaseState(caseId, -1);
        }




        public bool SetCaseToUploadedImageState(int caseId)
        {
            return this.SaveCaseState(caseId, 1);
        }


        public bool PassCase(int caseId)
        {
            return this.SaveCaseState(caseId, 2);
        }

        public bool BackCase(int caseId)
        {
            return this.SaveCaseState(caseId, -3);
        }


        public DataTable GetGalleryOfCase(int caseId)
        {
            return Helper.DBA.GetDataSet(new SqlQuery(
                "SELECT * FROM cir_case_images where caseId=@caseId",
                new object[,]
                {
                    {"@caseId", caseId}
                }
                )).Tables[0];
        }

        public int RemoveGalleryOfCase(int caseId)
        {
            return Helper.DBA.ExecuteNonQuery(new SqlQuery(
                "DELETE FROM cir_case_images where caseId=@caseId",
                new object[,]
                {
                    {"@caseId", caseId}
                }));
        }


        public IList<Case> GetTodayCasesOfPartner(string partnerCode)
        {
            IList<Case> entityList = null;
            Helper.DBA.ExecuteReader(
                new SqlQuery(
                    @"SELECT cs.[id],cs.[cashNo]
					,cs.[caseNo]
					,img.createTime
					FROM cir_case cs
					INNER JOIN cir_case_images img ON img.caseId=cs.id
					WHERE img.id= (SELECT MAX(id) FROM cir_case_images
					WHERE caseId = cs.Id) AND partnerCode=@partnerCode
					AND img.createTime BETWEEN @startTime AND @endTime",

                    new object[,]
                    {
                        {"@partnerCode", partnerCode},
                        {"@startTime", DateTime.Now.Date},
                        {"@endTime", DateTime.Now}
                    }), rd =>
                    {
                        if (rd.HasRows)
                        {
                            entityList = rd.ToEntityList<Case>();
                        }
                    });
            return entityList;
        }


        public string GetLastPicUrlOfCaseGallay(int caseId,string picParttern)
        {
           
            object data = Helper.DBA.ExecuteScalar(new SqlQuery(

                @"SELECT imgUrl FROM cir_case_images where id IN (
                    SELECT MAX(id) FROM cir_case_images WHERE caseId=@caseId
                    AND imgUrl LIKE '%"+picParttern+@"%'
                    )",
                new object[,]
                {
                    {"@caseId", caseId},
                    {"@picParttern",picParttern}
                }
                ));
            return data == null ? null : data.ToString();
        }


        public DataTable SearchGalleryOfCase(int imgType, string caseIds)
        {
            Hashtable hash= new Hashtable();
            hash.Add("caseIds",caseIds);
            hash.Add("imgType",imgType);
            const string sql = "SELECT * FROM cir_case_images where caseId IN ({caseIds}) AND ({imgType}=0 OR imgType={imgType})";


            return Helper.DBA.GetDataSet(sql.Template(hash)).Tables[0];
        }


        public void UpgradeCaseState()
        {
            Helper.DBA.ExecuteNonQuery("UPDATE cir_case SET state = 0 WHERE state = 1 AND (SELECT COUNT(0) FROM cir_case_images c WHERE c.caseId=id)= 0");
        }
    }
}
