using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LBJOEE.DesignMode
{
    public class XJState : State
    {
        public override void Handle(StateContext context)
        {
            try
            {
                if (context.SBXX.sfql == "Y")
                {
                    context.State = new DLState();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
