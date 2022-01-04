using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LBJOEE.DesignMode
{
    public class StateContext
    {
        private State _state;
        private base_sbxx _base_sbxx;
        private List<BtnStatus> _btnlist = new List<BtnStatus>();
        public StateContext(State state)
        {
            this._state = state;
        }
        public List<BtnStatus> SetBtnList
        {
            set
            {
                _btnlist = value;
            }
        }
        public base_sbxx SBXX
        {
            get
            {
                return this._base_sbxx;
            }
            set
            {
                this._base_sbxx = value;
            }
        }
        public State State
        {
            set
            {
                this._state = value;
            }
            get
            {
                return this._state;
            }
        }
    }
}
