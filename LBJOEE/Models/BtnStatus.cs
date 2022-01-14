using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
namespace LBJOEE
{
    public class BtnStatus:BindableBase
    {
        public string name { get; set; }
        public string tjlx { get; set; }
        public string tjms { get; set; }
        public string tjtxt { get; set; }
        public string normaltxt { get; set; }
        private string _btntxt;
        public string btntxt
        {
            get { return _btntxt; }
            set { SetProperty(ref _btntxt, value); }
        }

        private bool _btnenable=true;
        public bool btnenable
        {
            get { return _btnenable; }
            set { SetProperty(ref _btnenable, value); }
        }

        private int _tjsj=0;
        public int tjsj
        {
            get { return _tjsj; }
            set { SetProperty(ref _tjsj, value); }
        }

        private bool _sfgz = false;
        /// <summary>
        /// 手动按钮设置的故障
        /// </summary>
        public bool sfgz
        {
            get { return _sfgz; }
            set { SetProperty(ref _sfgz, value); }
        }
        private bool _sfjx = false;
        public bool sfjx
        {
            get { return _sfjx; }
            set { SetProperty(ref _sfjx, value); }
        }

        private bool _sfql =false;
        public bool sfql
        {
            get { return _sfql; }
            set { SetProperty(ref _sfql, value); }
        }
        private bool _sfhm=false;
        public bool sfhm
        {
            get { return _sfhm; }
            set { SetProperty(ref _sfhm, value); }
        }

        private bool _sfqt=false;
        public bool sfqt
        {
            get { return _sfqt; }
            set { SetProperty(ref _sfqt, value); }
        }

        private int _flag;
        /// <summary>
        /// 停机按钮颜色标志,1表示
        /// </summary>
        public int flag
        {
            get { return _flag; }
            set { SetProperty(ref _flag, value); }
        }
        /// <summary>
        /// 停机时间是否可见
        /// </summary>
        private string _tjsjvisibale = "Collapsed";
        public string tjsjvisible
        {
            get { return _tjsjvisibale; }
            set { SetProperty(ref _tjsjvisibale, value); }
        }
        /// <summary>
        /// 是否调试
        /// </summary>
        public bool sfts { get; set; } 
        /// <summary>
        /// 是否修模
        /// </summary>
        public bool sfxm { get; set; }
        /// <summary>
        /// 是否保养
        /// </summary>
        public bool sfby { get; set; }
        /// <summary>
        /// 是否离岗停机
        /// </summary>
        public bool sflgtj { get; set; }
    }
}
