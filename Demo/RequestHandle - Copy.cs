
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Imaging;
using com.mapfre.cir;
using com.mapfre.cir.Utils;
using com.mapfre.poi.Logic;
using Newtonsoft.Json;
using Ops.Cms;
using Ops.Cms.Web;
using Ops.Data;
using Ops.Framework.Graphic;
using Ops.Template;
using Ops.Web.UI;
/*
 * 由SharpDevelop创建。
 * 用户： newmin
 * 日期: 2013/11/19
 * 时间: 21:44
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace com.mapfre.poi
{
    /// <summary>
    /// Description of MobileActions.
    /// </summary>
    public class RequestHandle
    {
        private string workSpace;
        private int imgWidth;
        private int imgHeight;
        private string waterPath;

        internal RequestHandle()
        {
            workSpace = Config.PluginAttrs.WorkSpace;
            imgWidth = int.Parse(Config.PluginAttrs.Settings["img.width"]);
            imgHeight = int.Parse(Config.PluginAttrs.Settings["img.height"]);

            float waterMarkPercent = float.Parse(Config.PluginAttrs.Settings["img.water.percent"]);
            if (waterMarkPercent == 0F) waterMarkPercent = 1;

            waterPath = workSpace + "watermark_resize.png";
            Image srcImg = new Bitmap(workSpace + "watermark.png");
            byte[] data = GraphicsHelper.DrawBySize(srcImg,
                ImageSizeMode.SuitWidth,
                (int)(srcImg.Width * waterMarkPercent),
                (int)(srcImg.Height * waterMarkPercent),
                ImageFormat.Png,
                null);

            using (FileStream fs = new FileStream(waterPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fs.Write(data, 0, data.Length);
                fs.Flush();
                fs.Dispose();
            }

            srcImg.Dispose();
        }


        /// <summary>
        /// 首页
        /// </summary>
        /// <param name="context"></param>
        /// <param name="segments"></param>
        public string Default(HttpContext context)
        {
            return "<script>location.replace('/cirp/partner')</script>";
        }


        public string Case_List(HttpContext context)
        {
            TemplatePage page = Cms.Plugins.GetPage<Main>("admin/case_list.html");
            page.AddVariable("page", new PageVariable());
            return page.ToString();
        }

        public string Case_Gallery(HttpContext context)
        {
            int caseId = int.Parse(context.Request["caseId"]);
            TemplatePage page = Cms.Plugins.GetPage<Main>("admin/case_gallery.html");
            page.AddVariable("page", new PageVariable());
            page.AddVariable("case", new { Id = caseId });
            return page.ToString();
        }

        public string DelCase_post(HttpContext context)
        {
            int id = int.Parse(context.Request["id"]);
            bool result = IocObject.Case.DelCase(id);
            return JsonConvert.SerializeObject(new { result = result, message = result ? "删除成功" : "删除失败" });
        }

        public string CancelCase_post(HttpContext context)
        {
            int id = int.Parse(context.Request["id"]);
            bool result = IocObject.Case.CancelCase(id);
            return JsonConvert.SerializeObject(new { result = result, message = result ? "删除成功" : "删除失败" });
        }
        public string PassCase_post(HttpContext context)
        {
            int id = int.Parse(context.Request["id"]);
            bool result = IocObject.Case.PassCase(id);
            return JsonConvert.SerializeObject(new { result = result, message = result ? "通过成功" : "通过失败" });
        }

        public string BackCase_post(HttpContext context)
        {
            int id = int.Parse(context.Request["id"]);
            bool result = IocObject.Case.BackCase(id);
            return JsonConvert.SerializeObject(new { result = result, message = result ? "已经驳回" : "驳回失败" });
        }
        public void Download(HttpContext context)
        {
            string fileName;
            string url;

            url = context.Request["url"];
            string filePath = AppDomain.CurrentDomain.BaseDirectory + url;
            if (!File.Exists(filePath))
            {
                context.Response.Write("资源不存在");
                return;
            }

            fileName = Regex.Match(url, "(\\\\|/)(([^\\\\/]+)\\.(.+))$").Groups[2].Value;
            context.Response.AppendHeader("Content-Type", "");
            context.Response.AppendHeader("Content-Disposition", "attachment;filename=" + fileName);

            const int bufferSize = 100;
            byte[] buffer = new byte[bufferSize];
            int readSize = -1;

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                while (readSize != 0)
                {
                    readSize = fs.Read(buffer, 0, bufferSize);
                    context.Response.BinaryWrite(buffer);
                }
            }
        }
        public void DL_Gallery(HttpContext context)
        {
            //如果文件存在，则删除
            // FileInfo file = new FileInfo(workSpace + "dl/gallery.zip");

            int caseId = int.Parse(context.Request["caseId"]);
            int imgType = int.Parse(context.Request["imgType"]);
            string imgIds = context.Request["imgIds"];
            byte[] bytes = this._CompressGalleryToZip(caseId, imgType, imgIds);
            context.Response.BinaryWrite(bytes);
            //context.Response.AppendHeader("Content-Type","");
            context.Response.AppendHeader("Content-Disposition", "attachment;filename=case_" + caseId.ToString() + ".zip");
        }
        public void DL_Gallery2(HttpContext context)
        {
            //如果文件存在，则删除
            int imgType = int.Parse(context.Request["imgtype"]);
            string caseIds = context.Request["caseIds"];
            byte[] bytes = this._CompressGalleryToZipForCases(imgType, caseIds);
            context.Response.BinaryWrite(bytes);
            //context.Response.AppendHeader("Content-Type","");
            context.Response.AppendHeader("Content-Disposition", "attachment;filename=" + String.Format("{0:yyyyMMddHHmmss}",DateTime.Now)+ ".zip");
        }

        private byte[] _CompressGalleryToZipForCases(int imgType, string caseIds)
        {
            string dlGalleryDir = InitDlDirectory();

            return this._CopyGalleryOfCasesToDownloadDirectory(imgType, caseIds,dlGalleryDir);
        }

        private byte[] _CopyGalleryOfCasesToDownloadDirectory( int imgType, string caseIds,  string dlGalleryDir)
        {
            DataTable dt;
            dt = IocObject.Case.SearchGalleryOfCase(imgType,caseIds);
            IList<string> filePaths = new List<string>();

            foreach (DataRow dr in dt.Rows)
            {
                filePaths.Add(dr["imgUrl"].ToString());
            }

            //拷贝图片
            FileInfo file;
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            foreach (string imgUrl in filePaths)
            {
                file = new FileInfo(baseDir + imgUrl);
                File.Copy(file.FullName, dlGalleryDir + "/" + file.Name);
            }

            byte[] data = ZipHelper.Compress(dlGalleryDir);


            return data;
        }


        private string InitDlDirectory()
        {
            //初始化目录
            string dlDir = workSpace + "dl";
            string dlGalleryDir = dlDir + "/gallery";
            if (!Directory.Exists(dlDir)) Directory.CreateDirectory(dlDir);
            if (Directory.Exists(dlGalleryDir))
            {
                //删除图片资源
                Directory.Delete(dlGalleryDir, true);
            }
            Directory.CreateDirectory(dlGalleryDir);
            return dlGalleryDir;
        }

        private byte[] _CompressGalleryToZip(int caseId, int imgType, string imgIds)
        {
            string dlGalleryDir= InitDlDirectory();

            return this._CopyGalleryToDownloadDirectory(caseId, imgType, imgIds, dlGalleryDir);

        }



        /// <summary>
        /// 复制相册文件到目录下
        /// </summary>
        /// <param name="caseId"></param>
        /// <param name="imgType"></param>
        /// <param name="imgIds"></param>
        /// <param name="dlGalleryDir"></param>
        private byte[] _CopyGalleryToDownloadDirectory(int caseId, int imgType, string imgIds,string dlGalleryDir)
        {
            DataTable dt;
            bool isTypeFilter = imgType != -1;
            Array idArrary = null;

            if (!String.IsNullOrEmpty(imgIds)) idArrary = imgIds.Split(',');

            dt = IocObject.Case.GetGalleryOfCase(caseId);
            IList<string> filePaths = new List<string>();

            foreach (DataRow dr in dt.Rows)
            {
                if ((!isTypeFilter || int.Parse(dr["imgType"].ToString()) == imgType)
                    && (idArrary == null || Array.IndexOf(idArrary, dr["id"].ToString()) != -1))
                {
                    filePaths.Add(dr["imgUrl"].ToString());
                }
            }

            //拷贝图片
            FileInfo file;
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            foreach (string imgUrl in filePaths)
            {
                file = new FileInfo(baseDir + imgUrl);
                File.Copy(file.FullName, dlGalleryDir + "/" + file.Name);
            }

            byte[] data = ZipHelper.Compress(dlGalleryDir);


            return data;
        }

        #region 前段
        public string Partner(HttpContext context)
        {
            string template = Helper.IsWapPortal(context.Request)
                ? "html/partner_mobile.html"
                : "html/partner.html";

            TemplatePage page = Cms.Plugins.GetPage<Main>(template);
            page.AddVariable("page", new PageVariable());
            return page.ToString();
        }
        public string Partner_VerifyCase_post(HttpContext context)
        {
            string cashNo = context.Request["s_cashNo"];
            string partnerCode = context.Request["s_partnerCode"];
            string verifyCode = context.Request["verifyCode"];

            if (!VerifyCodeManager.Compare(verifyCode))
            {
                return JsonConvert.SerializeObject(new { result = false, message = "验证码不正确！" });
            }

            Case _case = IocObject.Case.GetCaseByPartnerCodeAndCashNo(partnerCode, cashNo);
            if (_case == null)
            {
                return JsonConvert.SerializeObject(new { result = false, message = "" });
            }
            else
            {
                context.Session["partnerCode"] = partnerCode;

                if (_case.State == -2 || _case.State == -1)
                {
                    return JsonConvert.SerializeObject(new { result = false, message = "案件不存在" });
                }
                else if (_case.State == 2)
                {
                    return JsonConvert.SerializeObject(new { result = false, message = "exists" });
                }
                else
                {
                    TemplatePage page = Cms.Plugins.GetPage<Main>("html/partner_saveCase.html");
                    string entityJson = JsonConvert.SerializeObject(_case);
                    string galleryJson = JsonConvert.SerializeObject(IocObject.Case.GetGalleryOfCase(_case.Id));
                    page.AddVariable("entity", new { json = entityJson, galleryJson = galleryJson });

                    return JsonConvert.SerializeObject(new { result = true, html = page.ToString() });
                }
            }
        }
        public string Partner_SaveCase_post(HttpContext context)
        {
            object json = null;
            string cashNo = context.Request["CashNo"];
            string partnerCode = context.Request["PartnerCode"];

            Case _case = IocObject.Case.GetCaseByPartnerCodeAndCashNo(partnerCode, cashNo);
            if (_case == null || _case.State == -1 || _case.State == -2)
            {
                json = new { result = false, message = "案件不存在!" };
            }
            else if (_case.State == 2)
            {
                json = new { result = false, message = "案件已经录入!" };
            }
            else
            {
                string imgs = context.Request["imgs"];
                string imgs2 = context.Request["imgs2"];
                if (imgs.Length == 0 && imgs2.Length == 0)
                {
                    throw new Exception("没有上传图片");
                }
                else
                {
                    //以最大的编号来编号
                    int maxId = 0;
                    Regex maxIdRegex = new Regex("_pic_(\\d+)\\.");
                    string maxPicUrl = IocObject.Case.GetLastPicUrlOfCaseGallay(_case.Id, "_pic_");

                    if (maxPicUrl != null && maxIdRegex.IsMatch(maxPicUrl))
                    {
                        maxId = int.Parse(maxIdRegex.Match(maxPicUrl).Groups[1].Value);
                    }

                    //删除数据
                    if (_case.State == -3 || _case.State == 1)
                    {
                        int num = IocObject.Case.RemoveGalleryOfCase(_case.Id);
                    }

                    if (imgs != "")
                    {
                        string[] imgsArr = this.GetRenamedImages(0,imgs, _case.CaseNo + "_pli_");
                        IocObject.Case.SaveImagesForCase(_case.Id, 1, imgsArr);
                    }

                    if (imgs2 != "")
                    {
                        string[] imgsArr = this.GetRenamedImages(maxId,imgs2, _case.CaseNo + "_pic_");
                        IocObject.Case.SaveImagesForCase(_case.Id, 2, imgsArr);
                    }

                    IocObject.Case.SetCaseToUploadedImageState(_case.Id);
                }

                json = new { result = true, message = "录入成功!" };
            }
            return JsonConvert.SerializeObject(json);
        }

        private string[] GetRenamedImages(int picIndex,string imgs, string prefix)
        {
            string[] reImgArr;
            string[] imgArr;
            Regex regex = new Regex("(([^\\\\/]+)\\.(.+))$");
            imgs = Regex.Replace(imgs, "http://(.+?)/", "/");
            imgArr = imgs.Split(',');
            reImgArr = new string[imgArr.Length];

            int index = 0;
            string img2;
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;

            //以最大的编号来编号
            Regex maxIdRegex=new Regex(prefix+"(\\d+)\\.");

            foreach (string img in imgArr)
            {
                if (!maxIdRegex.IsMatch(img))
                {
                    picIndex++;
                    img2 = String.Concat(
                        regex.Replace(img, prefix),
                        picIndex > 10 ? index.ToString() : "0" + picIndex.ToString(),
                        ".jpg");

                    if (File.Exists(rootPath + img2))
                    {
                        File.Delete(rootPath + img2);
                    }
                    File.Move(rootPath + img, rootPath + img2);
                    reImgArr[index++] = img2;
                }
                else
                {
                    reImgArr[index++] = img;
                }
            }

            return reImgArr;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string Upload_post(HttpContext context)
        {
            string uploadfor = context.Request["for"];
            string id = context.Request["upload.id"];
            DateTime dt = DateTime.Now;
            string dir = string.Format("/images/{0:yyyyMMdd}/", dt);
            string name = String.Format("{0}{1:HHss}{2}",
                String.IsNullOrEmpty(uploadfor) ? "" : uploadfor + "_",
                dt, String.Empty.RandomLetters(4));

            string file = new FileUpload(dir, name).Upload(false);
            if (uploadfor == "image")
            {
                string rootPath = Cms.PyhicPath;


                Bitmap img = new Bitmap(rootPath + file);
                int width, height;
                if (img.Width > img.Height)
                {
                    width = imgWidth;
                    height = imgHeight;
                }
                else
                {
                    width = imgHeight;
                    height = imgWidth;
                }

                byte[] data = GraphicsHelper.DrawBySize(img, ImageSizeMode.CustomSize, width, height, ImageFormat.Png, null);
                img.Dispose();
                MemoryStream ms1 = new MemoryStream(data);
                img = new Bitmap(ms1);

                Image water = new Bitmap(waterPath);

                data = GraphicsHelper.MakeWatermarkImage(
                    img,
                    water,
                     WatermarkPosition.Middle
                     );

                ms1.Dispose();
                img.Dispose();

                FileStream fs = File.OpenWrite(rootPath + file);
                BinaryWriter w = new BinaryWriter(fs);
                w.Write(data);
                w.Flush();
                fs.Dispose();
            }

            return String.Format("<script> window.parent.{0}.onUploadComplete('{1}')</script>", id, file);
        }

        /// <summary>
        /// 验证码
        /// </summary>
        /// <param name="context"></param>
        public void VerifyCode(HttpContext context)
        {
            string word = null;
            VerifyCode v = new VerifyCode();
            var font = v.GetDefaultFont();
            try
            {
                font = new System.Drawing.Font(font.FontFamily, 16);
                v.AllowRepeat = false;
                context.Response.BinaryWrite(v.GraphicDrawImage(4,
                    VerifyWordOptions.Number,
                    !true,
                    font,
                    30,
                    out word));
            }
            catch
            {
                if (font != null)
                {
                    font.Dispose();
                }
            }
            context.Response.ContentType = "Image/Jpeg";
            VerifyCodeManager.AddWord(word);
        }

        /// <summary>
        /// 获取今日上传的案件
        /// </summary>
        /// <param name="context"></param>
        public string GetTodayUploadedCases_POST(HttpContext context)
        {
            object partnerCode = context.Session["partnerCode"];
            IList<Case> cases = null;
            if (partnerCode != null)
            {
                cases = IocObject.Case.GetTodayCasesOfPartner(partnerCode.ToString());
            }
            if (cases != null)
            {

                foreach (Case c in cases)
                {
                    c._Time = String.Format("{0:HH:mm}", c.CreateTime);
                }
            }
            return JsonConvert.SerializeObject(new
            {
                rows = cases
            });
        }

        #endregion



        public string Export_Setup(HttpContext context)
        {
            TemplatePage page = Cms.Plugins.GetPage<Main>("admin/export_setup.html");
            page.AddVariable("page", new PageVariable());
            page.AddVariable("export", new { setup = ExportHandle.Setup(context.Request["portal"]) });
            return page.ToString();
        }

        public string Export_GetExportData_Post(HttpContext context)
        {
            return ExportHandle.GetExportData(context);
        }

        public void Export_ProcessExport(HttpContext context)
        {
            ExportHandle.ProcessExport(context);
        }

        public string Export_Import(HttpContext context)
        {
            TemplatePage page = Cms.Plugins.GetPage<Main>("admin/export_import.html");
            page.AddVariable("page", new PageVariable());
            page.AddVariable("case", new { json = new object() });
            return page.ToString();
        }

        public string Export_Import_post(HttpContext context)
        {
            // try
            // {
            FileInfo file = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + context.Request["file"]);
            DataTable dt = NPOIHelper.ImportFromExcel(file.FullName).Tables[0];
            //DataView dv = dt.DefaultView;
            //dv.Sort = "财务编号 ASC";
            SqlQuery[] querys = new SqlQuery[dt.Rows.Count];
            int i = 0;
            DateTime importTime = DateTime.Now;
            foreach (DataRow dr in dt.Rows)
            {
                const string insertSql = @"INSERT INTO [cir_case]
		([CashNo]
        ,[caseNo]
        ,[cusCaseNo]
		,[partnerCode]
		,[contractNo]
		,[createTime]
		,[importTime]
		,[state]
        ,[province]
		,[city]
		,[service]
		,[cw]
		,[bx]
		,[personName]
		,[contract])
	VALUES
		SELECT
        @cashNo
        ,@caseNo
        ,@cusCaseNo
		,@partnerCode
		,@contractNo
		,@createTime
		,@importTime
		,0
        ,@province
		,@city
		,@service
		,@cw
		,@bx
		,@personName
		,@contract
    WHERE NOT EXISTS (SELECT cashNo FROM cir_case WHERE cashNo=@cashNo)
   ";
                querys[i++] = new SqlQuery(insertSql,
                    new object[,]
                        {
                            {"@contractNo", dr["合同编号"]},
                            {"@caseNo", dr["案件编号"]},
                            {"@cusCaseNo", dr["外方档案号"]},
                            {"@createTime", Convert.ToDateTime(dr["发生日期"])},
                            {"@cashNo", dr["财务操作号"]},
                            {"@partnerCode", dr["供应商编码"]},
                            {"@importTime", importTime},
                            {"@province", dr["省份"]},
                            {"@city", dr["城市"]},
                            {"@service", dr["服务"]},
                            {"@cw", dr["财务操作类型"]},
                            {"@bx", dr["报销"]},
                            {"@personName", dr["人员简称"]},
                            {"@contract", dr["合同"]}
                        }
                    );

            }

            int rows = Helper.DBA.ExecuteNonQuery(querys);

            return "{result:true,message:'导入完成,共导入" + rows.ToString() + "条！'}";
            /* }
             catch (Exception exc)
             {
                 return "{result:false,message:'"+exc.Message+"！'}";
             }*/
        }
    }

}
