using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LBJOEE.DesignMode
{
    public class DLState : State
    {
        public override void Handle(StateContext context)
        {
            try
            {
                if(context.SBXX.sfgz == "Y")
                {
                    context.State = new XJState();
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
