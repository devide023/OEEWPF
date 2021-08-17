using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Regions;
using LBJOEE.Views;
using Prism.Ioc;
using Prism.Navigation;
using LBJOEE.Tools;
using Newtonsoft.Json;
using LBJOEE.Services;
namespace LBJOEE.ViewModels
{
    public class DataInterfaceViewModel:BindableBase,INavigationAware
    {
        private IRegionManager regionmgr;
        private IContainerExtension container;
        private base_sbxx base_sbxx;
        private SBTJService _sbtjservice;
        private SBSJService _sbsjservice;
        private SBXXService _sbxxservice;
        private string _errormsg;
        public string ErrorMsg
        {
            get { return _errormsg; }
            set { SetProperty(ref _errormsg, value); }
        }
        private string _socketdata;
        public string socketdata
        {
            get { return _socketdata; }
            set { SetProperty(ref _socketdata, value); }
        }
        public DataInterfaceViewModel(SBXXService sbxxservice, SBTJService sbtjservice,SBSJService sbsjservice, IContainerExtension containerExtension)
        {
            _sbxxservice = sbxxservice;
            _sbtjservice = sbtjservice;
            _sbsjservice = sbsjservice;
            container = containerExtension;
            regionmgr = containerExtension.Resolve<IRegionManager>();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            base_sbxx = navigationContext.Parameters.GetValue<base_sbxx>("sbxx");
            var socketservice = navigationContext.Parameters.GetValue<SocketServer>("socketservice");
            socketservice.ReceiveAction = ReceiveData;
        }

        /// <summary>
        /// 接收数据采集软件数据
        /// </summary>
        /// <param name="data"></param>
        private void ReceiveData(string data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var receive_data = JsonConvert.DeserializeObject<JsonEntity>(data);
                    receive_data.devicedata.sbbh = base_sbxx.sbbh;
                    switch (receive_data.status)
                    {
                        case "故障":
                            DeviceError(new
                            {
                                status = receive_data.status,
                                message = receive_data.message,
                                errorcode = receive_data.errorcode,
                            });
                            break;
                        case "正常":
                            {
                                DeviceNormal(new
                                {
                                    status = receive_data.status,
                                    message = receive_data.message,
                                    errorcode = receive_data.errorcode,
                                });
                                _sbsjservice.Add(receive_data.devicedata);
                            }
                            break;
                        default:
                            break;
                    }
                    socketdata = JsonConvert.SerializeObject(receive_data);
                }
            }
            catch (Exception e)
            {
                ErrorMsg = e.Message;
            }
        }

        private void DeviceNormal(dynamic obj)
        {
            if (base_sbxx.sfgz == "Y")
            {
                base_sbxx.sbzt = obj.status;
                base_sbxx.sfgz = "N";
                DateTime now = DateTime.Now;
                _sbtjservice.Add(new sbtj()
                {
                    sbbh = base_sbxx.sbbh,
                    tjjssj = now,
                    tjkssj = base_sbxx.gzkssj,
                    tjlx = "故障",
                    tjsj = (int)(now - base_sbxx.gzkssj).TotalSeconds,
                    tjms = obj.message
                });
            }
        }
        private void DeviceError(dynamic obj)
        {
            if (base_sbxx.sfgz != "Y")
            {
                base_sbxx.sbzt = obj.status;
                base_sbxx.sfgz = "Y";
                base_sbxx.tjms = obj.message;
                base_sbxx.gzkssj = DateTime.Now;
                _sbxxservice.SetGZtj(base_sbxx);
            }
        }


    }
}
