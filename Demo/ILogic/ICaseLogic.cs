/*
 * 用户： newmin
 * 日期: 2013/12/3
 * 时间: 23:00
 * 
 * 修改说明：
 */

using System.Collections.Generic;
using System.Data;
using com.mapfre.cir;
namespace com.mapfre.poi.ILogic
{
	/// <summary>
	/// Description of ICustomer.
	/// </summary>
	public interface ICaseLogic
	{
        int InsertCase(Case _case);

        Case GetCaseByPartnerCodeAndCashNo( string partnerCode,string cashNo);

       // bool SaveCaseState(int caseId, int stateCode);

        int SaveImagesForCase(int caseId, int imgType,string[] imgList);

        bool DelCase(int id);

        bool CancelCase(int id);

        bool SetCaseToUploadedImageState(int caseId);

        bool PassCase(int caseId);

        bool BackCase(int caseId);

        DataTable GetGalleryOfCase(int caseId);

        int RemoveGalleryOfCase(int caseId);

        IList<Case> GetTodayCasesOfPartner(string partnerCode);

        string GetLastPicUrlOfCaseGallay(int caseId,string picParttern);

        /// <summary>
        /// 搜索案件的图片列表
        /// </summary>
        /// <param name="imgType"></param>
        /// <param name="caseIds"></param>
        /// <returns></returns>
        DataTable SearchGalleryOfCase(int imgType, string caseIds);

        void UpgradeCaseState();
    }
}
