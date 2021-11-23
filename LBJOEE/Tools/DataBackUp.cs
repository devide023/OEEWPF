﻿using System;
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
        private static string path1 = @"d:\backup\yssj";
        /// <summary>
        /// 保存数据到本地磁盘
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static void SaveDataToLocal(sjcj entity)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filepath = path + "\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".json";
                using (StreamWriter sw = new StreamWriter(filepath))
                {
                    entity.cjsj = DateTime.Now;
                    sw.WriteLine(JsonConvert.SerializeObject(entity));
                }
            }
            catch (Exception)
            {
                //Environment.Exit(0);
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
                string filepath = path1 + "\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".json";
                using (StreamWriter sw = new StreamWriter(filepath))
                {
                    yssj.rq = DateTime.Now;
                    sw.WriteLine(JsonConvert.SerializeObject(yssj));
                }
            }
            catch (Exception)
            {

                throw;
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
                SBSJService service = new SBSJService();
                sjcj entity = new sjcj();
                DirectoryInfo diinfo = new DirectoryInfo(path);
                FileInfo[] finfos = diinfo.GetFiles();
                foreach (FileInfo item in finfos.Where(t=>t.Extension==".json"))
                {
                    string filepath = item.FullName;
                    StreamReader sr = new StreamReader(filepath);
                    string json = sr.ReadToEnd();
                    entity = JsonConvert.DeserializeObject<sjcj>(json);
                    if (Tool.IsPing())
                    {
                        dynamic ret = service.Add(entity);
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
                //var log = LogManager.GetLogger(typeof(DataBackUp));
                //log.Error(e.Message);
                //Environment.Exit(0);
            }
        }
    }
}
