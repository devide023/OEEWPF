using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using DapperExtensions.Mapper;
namespace LBJOEE
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public class base_sbxx: BindableBase
    {
        private string _sbbh;
        /// <summary>
        /// 设备编号
        /// </summary>
        public string sbbh
        {
            get { return _sbbh; }
            set { SetProperty(ref _sbbh, value); }
        }
        private string _sbmc;
        /// <summary>
        /// 设备名称
        /// </summary>
        public string sbmc
        {
            get { return _sbmc; }
            set { SetProperty(ref _sbmc, value); }
        }
        private string _sbxh;
        /// <summary>
        /// 设备型号
        /// </summary>
        public string sbxh
        {
            get { return _sbxh; }
            set { SetProperty(ref _sbxh, value); }
        }
        private string _sbpp;
        /// <summary>
        /// 设备品牌
        /// </summary>
        public string sbpp
        {
            get { return _sbpp; }
            set { SetProperty(ref _sbpp, value); }
        }

        private string _sbzt;
        /// <summary>
        /// 设备状态
        /// </summary>
        public string sbzt
        {
            get { return _sbzt; }
            set { SetProperty(ref _sbzt, value); }
        }
        private string _ip;
        /// <summary>
        /// ip地址
        /// </summary>
        public string ip
        {
            get { return _ip; }
            set { SetProperty(ref _ip, value); }
        }

        private string _sbqy;
        /// <summary>
        /// 设备区域
        /// </summary>
        public string sbqy
        {
            get { return _sbqy; }
            set { SetProperty(ref _sbqy, value); }
        }

        private string _llr;
        /// <summary>
        /// 录入人
        /// </summary>
        public string lrr
        {
            get { return _llr; }
            set { SetProperty(ref _llr, value); }
        }
        private DateTime _lrsj;
        /// <summary>
        /// 录入时间
        /// </summary>
        public DateTime lrsj
        {
            get { return _lrsj; }
            set { SetProperty(ref _lrsj, value); }
        }
        private string _sfjx="N";
        /// <summary>
        /// 是否检修
        /// </summary>
        public string sfjx
        {
            get { return _sfjx; }
            set {  SetProperty(ref _sfjx, value); }
        }
        private DateTime _jxkssj;
        /// <summary>
        /// 检修开始时间
        /// </summary>
        public DateTime jxkssj
        {
            get { return _jxkssj; }
            set { SetProperty(ref _jxkssj, value); }
        }

        private string _sfhm="N";
        /// <summary>
        /// 是否换模
        /// </summary>
        public string sfhm
        {
            get { return _sfhm; }
            set { SetProperty(ref _sfhm, value); }
        }
        private DateTime _hmkssj;
        /// <summary>
        /// 换模开始时间
        /// </summary>
        public DateTime hmkssj
        {
            get { return _hmkssj; }
            set { SetProperty(ref _hmkssj, value); }
        }

        private string _sfgz="N";
        /// <summary>
        /// 是否故障
        /// </summary>
        public string sfgz
        {
            get { return _sfgz; }
            set { SetProperty(ref _sfgz, value); }
        }

        private DateTime _gzkssj;
        /// <summary>
        /// 故障开始时间
        /// </summary>
        public DateTime gzkssj
        {
            get { return _gzkssj; }
            set { SetProperty(ref _gzkssj, value); }
        }

        private string _sfql="N";
        /// <summary>
        /// 是否缺料
        /// </summary>
        public string sfql
        {
            get { return _sfql; }
            set { SetProperty(ref _sfql, value); }
        }
        private DateTime _qlkssj;
        /// <summary>
        /// 缺料开始时间
        /// </summary>
        public DateTime qlkssj
        {
            get { return _qlkssj; }
            set { SetProperty(ref _qlkssj, value); }
        }

        private string _sfqt="N";
        /// <summary>
        /// 是否其他停机
        /// </summary>
        public string sfqttj
        {
            get { return _sfqt; }
            set { SetProperty(ref _sfqt, value); }
        }

        private DateTime _qttjkssj;
        /// <summary>
        /// 其他停机开始时间
        /// </summary>
        public DateTime qttjkssj
        {
            get { return _qttjkssj; }
            set { SetProperty(ref _qttjkssj, value); }
        }
        /// <summary>
        /// 停机描述
        /// </summary>
        private string _tjms;

        public string tjms
        {
            get { return _tjms; }
            set { SetProperty(ref _tjms, value); }
        }
        /// <summary>
        /// socket服务端端口号
        /// </summary>
        private int _port;
        public int port
        {
            get { return _port; }
            set { SetProperty(ref _port, value); }
        }


    }

    public class base_sbxx_mapper : ClassMapper<base_sbxx>
    {
        public base_sbxx_mapper()
        {
            Map(t => t.sbbh).Key(KeyType.Assigned);
            AutoMap();
        }
    }
}
