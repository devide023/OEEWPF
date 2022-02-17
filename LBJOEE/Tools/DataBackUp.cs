using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using LBJOEE.Services;
using LBJOEE.Models;
using log4net;
namespace LBJOEE.Tools
{
    public static class DataBackUp
    {
        private static string path = @"d:\backup\cjsj";
        private static string tjpath = @"d:\backup\tjcjsj";
        private static string path1 = @"d:\backup\yssj";
        private static ILog log = LogManager.GetLogger("LBJOEE.Tools.DataBackUp");
        /// <summary>
        /// 运行状态，保存数据到本地磁盘
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static void SaveDataToLocal(sjcjnew entity)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filepath = path + "\\" + Guid.NewGuid().ToString() + ".json";
                using (StreamWriter sw = new StreamWriter(filepath))
                {
                    entity.cjsj = DateTime.Now;
                    sw.WriteLine(JsonConvert.SerializeObject(entity));
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }
        /// <summary>
        /// 停机状态接收到的数据本地存储
        /// </summary>
        /// <param name="entity"></param>
        public static void SaveTJDataToLocal(sjcjnew entity)
        {
            try
            {
                if (!Directory.Exists(tjpath))
                {
                    Directory.CreateDirectory(tjpath);
                }
                string filepath = tjpath + "\\" + Guid.NewGuid().ToString() + ".json";
                using (StreamWriter sw = new StreamWriter(filepath))
                {
                    entity.cjsj = DateTime.Now;
                    sw.WriteLine(JsonConvert.SerializeObject(entity));
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }
        public static void SaveOrginalDataToLocal(originaldata yssj)
        {
            try
            {
                if (!Directory.Exists(path1))
                {
                    Directory.CreateDirectory(path1);
                }
                string filepath = path1 + "\\" + Guid.NewGuid().ToString() + ".json";
                using (StreamWriter sw = new StreamWriter(filepath))
                {
                    yssj.rq = DateTime.Now;
                    sw.WriteLine(JsonConvert.SerializeObject(yssj));
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }
        /// <summary>
        /// 从硬盘读取文件数据
        /// </summary>
        /// <returns></returns>
        public static void ReadDataFromLocal()
        {
            try
            {
                SBSJService service = SBSJService.Instance;
                SBZTGXService _sbztbhservice = new SBZTGXService();
                sjcjnew entity = new sjcjnew();
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                DirectoryInfo diinfo = new DirectoryInfo(path);
                FileInfo[] finfos = diinfo.GetFiles();
                foreach (FileInfo item in finfos.Where(t=>t.Extension==".json"))
                {
                    string filepath = item.FullName;
                    StreamReader sr = new StreamReader(filepath);
                    string json = sr.ReadToEnd();
                    entity = JsonConvert.DeserializeObject<sjcjnew>(json);
                    if (Tool.IsPing())
                    {
                        dynamic ret = service.AddByDate(entity);
                        _sbztbhservice.AddByDate(new sbztbhb()
                        {
                            sj = entity.cjsj,
                            sbbh=entity.sbbh
                        });
                        if (!string.IsNullOrEmpty(ret.ToString()))
                        {
                            sr.Close();
                            item.Delete();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }

        public static void ReadTJDataFromLocal()
        {
            try
            {
                SBSJService service = SBSJService.Instance;
                sjcjnew entity = new sjcjnew();
                if (!Directory.Exists(tjpath))
                {
                    Directory.CreateDirectory(tjpath);
                }
                DirectoryInfo diinfo = new DirectoryInfo(tjpath);
                FileInfo[] finfos = diinfo.GetFiles();
                foreach (FileInfo item in finfos.Where(t => t.Extension == ".json"))
                {
                    string filepath = item.FullName;
                    StreamReader sr = new StreamReader(filepath);
                    string json = sr.ReadToEnd();
                    entity = JsonConvert.DeserializeObject<sjcjnew>(json);
                    if (Tool.IsPing())
                    {
                        var ret = service.TJSJCJ(entity);
                        if (ret>0)
                        {
                            sr.Close();
                            item.Delete();
                        }
                    }
                }
            }
            catch (Exception e)
            {                  
                log.Error(e.Message);
            }
        }
    }
}
